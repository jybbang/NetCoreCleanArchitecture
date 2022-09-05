using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreCleanArchitecture.Application.Identities;
using NetCoreCleanArchitecture.Interface.Http.Behaviours;
using NetCoreCleanArchitecture.Interface.Http.Filters;
using NetCoreCleanArchitecture.Interface.Http.Identity;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Results.Fluent;

namespace NetCoreCleanArchitecture.Interface
{
    public static class ApplicationExtensions
    {
        public static ActionResult ToActionResult(this Result result)
        {
            ActionResult? actionResult;

            if (result.IsFailed)
            {
                actionResult = new UnprocessableEntityObjectResult(result.Errors);
            }
            else
            {
                actionResult = new NoContentResult();
            }

            return actionResult;
        }

        public static ActionResult<TContainer> ToActionResult<TContainer>(this Result<TContainer> result) where TContainer : class
        {
            ActionResult? actionResult;

            if (result.IsFailed)
            {
                actionResult = new UnprocessableEntityObjectResult(result.Errors);
            }
            else if (result.IsSucceeded)
            {
                actionResult = new OkObjectResult(result.Container);
            }
            else
            {
                actionResult = new NoContentResult();
            }

            return new ActionResult<TContainer>(actionResult);
        }

        public static IApplicationBuilder UseNetCleanHttp(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            // UseCorsAllowAny
            app.UseCors("AllowAny");

            app.UseAuthorization();

            // ASP.NET Core HTTP request metrics
            app.UseHttpMetrics();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCleanHttp(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();

            // ASP.NET Core exporter middleware
            endpoints.MapMetrics("/metrics");

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            return endpoints;
        }

        public static IServiceCollection AddNetCleanHttp(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // Identity
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddHttpContextAccessor();

            // AddCorsAllowAny
            services.AddCors(options => options.AddPolicy("AllowAny",
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            // Controller with custom validator
            var builder = services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddJsonOptions(options =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    options.JsonSerializerOptions.Converters.Add(enumConverter);
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                })
                .AddFluentValidation(x => x.AutomaticValidationEnabled = false);

            // Customise default Api behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            // ConfigureFormOptionsMaxLengthLimit
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
            });

            return services;
        }

        public static IServiceCollection AddNetCleanHttpTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var appName = configuration.GetValue<string>("ApplicationName");
            var appVersion = configuration.GetValue<string>("ApplicationVersion");
            var appId = configuration.GetValue<string>("ApplicationInstanceId");
            var smapling = configuration.GetValue<int>("TracingSampling");
            smapling = smapling <= 0 ? 1 : 0;

            if (Uri.TryCreate(configuration.GetConnectionString("Zipkin"), UriKind.Absolute, out var zipkinEndpoint))
            {
                services.AddOpenTelemetryTracing(configure =>
                {
                    configure
                    .AddSource(appName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                        .AddService(appName, serviceVersion: appVersion, serviceInstanceId: appId))
                    .SetSampler(new TraceIdRatioBasedSampler(smapling))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddZipkinExporter(configure => configure.Endpoint = zipkinEndpoint);
                });

                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityExceptionBehaviour<,>));
            }
            else if (Uri.TryCreate(configuration.GetConnectionString("Jaeger"), UriKind.Absolute, out var jaegerEndpoint))
            {
                services.AddOpenTelemetryTracing(configure =>
                {
                    var builder = configure
                    .AddSource(appName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                        .AddService(appName, serviceVersion: appVersion, serviceInstanceId: appId))
                    .SetSampler(new TraceIdRatioBasedSampler(smapling))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                    if (string.Equals(jaegerEndpoint.Scheme, "udp", StringComparison.CurrentCultureIgnoreCase))
                    {
                        builder
                        .AddJaegerExporter(configure =>
                        {
                            configure.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.UdpCompactThrift;
                            configure.AgentHost = jaegerEndpoint.Host;
                            configure.AgentPort = jaegerEndpoint.Port;
                        });
                    }
                    else
                    {
                        builder
                        .AddJaegerExporter(configure =>
                        {
                            configure.Protocol = OpenTelemetry.Exporter.JaegerExportProtocol.HttpBinaryThrift;
                            configure.Endpoint = jaegerEndpoint;
                        });
                    }
                });

                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ActivityExceptionBehaviour<,>));
            }

            services.AddSingleton(new ActivitySource(appName, appVersion));

            return services;
        }

        public static IHealthChecksBuilder AddNetCleanHttpHealthChecks(this IHealthChecksBuilder builder)
        {
            // ASP.NET Core health check status metrics
            builder.ForwardToPrometheus();

            return builder;
        }
    }
}
