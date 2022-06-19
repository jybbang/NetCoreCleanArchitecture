using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Dapr.EventBus;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IApplicationBuilder UseNetCleanDapr(this IApplicationBuilder app)
        {
            // CloudEvents
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

        public static IServiceCollection AddNetCleanDapr(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<InfrastructureDaprOptions>("Api:Dapr");

            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(DaprStateStore<>));

            // EventBus
            services.AddScoped<IEventBus, DaprEventBus>();

            return services;
        }
    }
}
