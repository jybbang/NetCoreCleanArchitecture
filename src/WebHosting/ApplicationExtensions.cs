using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.WebHosting.Filters;
using NetCoreCleanArchitecture.WebHosting.Identity;
using NetCoreCleanArchitecture.WebHosting.Options;
using Prometheus;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseNetCleanWebHosting(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

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

        public static IEndpointRouteBuilder MapNetCleanWebHosting(this IEndpointRouteBuilder endpoints)
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

        public static IServiceCollection AddNetCleanWebHosting(this IServiceCollection services, IConfiguration configuration)
        {
            // Identity
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            services.AddOptions<NetCoreCleanArchitectureOptions>("NetCoreCleanArchitecture");

            services.AddHttpContextAccessor();

            // AddCorsAllowAny
            services.AddCors(options => options.AddPolicy("AllowAny",
                policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            // Controller with custom validator
            var builder = services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>())
                .AddJsonOptions(opt => opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)
                .AddFluentValidation();

            // Customise default API behaviour
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

        public static IHealthChecksBuilder AddNetCleanHealthChecks(this IHealthChecksBuilder builder)
        {
            // ASP.NET Core health check status metrics
            builder
                .ForwardToPrometheus();

            return builder;
        }
    }
}
