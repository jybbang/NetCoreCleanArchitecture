using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.Infrastructure.Dapr.EventBus;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseNetCleanInfrastructure(this IApplicationBuilder app)
        {
            // CloudEvents
            app.UseCloudEvents();

            return app;
        }

        public static IEndpointRouteBuilder MapNetCleanInfrastructure(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapSubscribeHandler();

            return endpoints;
        }

        public static IMvcBuilder AddNetCleanInfrastructure(this IMvcBuilder builder)
        {
            // Controller with custom validator
            var mvcBuilder = builder.AddDapr();

            return mvcBuilder;
        }

        public static IServiceCollection AddNetCleanInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<InfrastructureDaprOptions>();

            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(DaprStateStore<>));

            // EventBus
            services.AddScoped<IEventBus, DaprEventBus>();

            return services;
        }
    }
}
