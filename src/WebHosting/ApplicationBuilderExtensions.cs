using DotNetCore.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNetCoreCleanArchitecture(this IApplicationBuilder app)
        {
            // DotNetCore.AspNetCore
            app.UseCorsAllowAny();

            // CloudEvents
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCoreCleanArchitecture(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }
    }
}
