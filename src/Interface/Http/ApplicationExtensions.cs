using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Identities;
using NetCoreCleanArchitecture.Interface.Http.Filters;
using NetCoreCleanArchitecture.Interface.Http.Identity;
using NetCoreCleanArchitecture.Interface.Http.Options;
using Prometheus;

namespace NetCoreCleanArchitecture.Interface
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseNetCleanHttp(this IApplicationBuilder app)
        {
            app.UseStaticFiles();

            app.UseRouting();

            // UseCorsAllowAny
            app.UseCors("AllowAny");

            app.UseAuthorization();

            app.UseSwaggerUi3(settings =>
            {
                settings.Path = "/api";
                settings.DocumentPath = "/api/specification.json";
            });

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

        public static IServiceCollection AddNetCleanHttp(this IServiceCollection services)
        {
            // Identity
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddOptions<ApiOptions>("Api");

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

            services.AddOpenApiDocument();

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
