using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Dapr.EventBus;
using NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IApplicationBuilder UseNetCleanDapr(this IApplicationBuilder app)
        {
            // CloudEventsx
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCleanDapr(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }

        public static IMvcBuilder AddNetCleanDapr(this IMvcBuilder builder)
        {
            // Controller with custom validator
            var mvcBuilder = builder.AddDapr();

            return mvcBuilder;
        }

        public static IServiceCollection AddNetCleanEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DaprOptions>(configuration.GetSection("Api:Dapr"));

            // EventBus
            services.AddScoped<IEventBus, DaprEventBus>();

            return services;
        }

        public static IServiceCollection AddNetCleanStateStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DaprOptions>(configuration.GetSection("Api:Dapr"));

            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(DaprStateStore<>));

            return services;
        }
    }
}
