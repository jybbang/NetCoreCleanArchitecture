using DotNetCore.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

namespace Hosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDarpCleanArchitecture(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            // DotNetCore.AspNetCore
            app.UseCorsAllowAny();

            // CloudEvents
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapDarpCleanArchitecture(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }
    }
}
