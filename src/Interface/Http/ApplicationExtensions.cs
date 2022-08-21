using System;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Identities;
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

        public static IApplicationBuilder UseNetCleanHttp(this IApplicationBuilder app)
        {
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

            return endpoints;
        }

        public static IServiceCollection AddNetCleanHttp(this IServiceCollection services, IConfiguration configuration)
        {
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
                .AddFluentValidation();

            // Customise default Api behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            // ConfigureFormOptionsMaxLengthLimit
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
            });

            if (Uri.TryCreate(configuration.GetConnectionString("Zipkin"), UriKind.Absolute, out var uri))
            {
                var appName = configuration.GetValue<string>("ApplicationName");

                appName = string.IsNullOrEmpty(appName) ? Guid.NewGuid().ToString() : appName;

                services.AddOpenTelemetryTracing(configure =>
                {
                    configure
                    .AddSource(appName)
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(appName))
                    .AddAspNetCoreInstrumentation()
                    .AddZipkinExporter(configure => configure.Endpoint = uri);
                });
            }

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
