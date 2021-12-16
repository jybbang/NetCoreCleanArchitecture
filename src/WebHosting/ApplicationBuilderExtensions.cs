using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Prometheus;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNetCleanWebHosting(this IApplicationBuilder app)
        {
            // UseCorsAllowAny
            app.UseCors("AllowAny");

            // ASP.NET Core HTTP request metrics
            app.UseHttpMetrics();

            app.UseHttpsRedirection();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCleanWebHosting(this IEndpointRouteBuilder endpoints)
        {
            // ASP.NET Core exporter middleware
            endpoints.MapMetrics();

            endpoints.MapHealthChecks("/health");

            return endpoints;
        }
    }
}
