using DotNetCore.AspNetCore;
using Microsoft.AspNetCore.Builder;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNetCoreCleanArchitectureWebHosting(this IApplicationBuilder app)
        {
            // DotNetCore.AspNetCore
            app.UseCorsAllowAny();

            return app;
        }
    }
}
