using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNetCoreCleanArchitectureInfrastructure(this IApplicationBuilder app)
        {
            // CloudEvents
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCoreCleanArchitectureInfrastructure(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }
    }
}
