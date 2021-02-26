using DotNetCore.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

namespace Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDaprCleanArchitecture(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // DotNetCore.AspNetCore
            app.UseCorsAllowAny();

            // CloudEvents
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapDaprCleanArchitecture(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }
    }
}
