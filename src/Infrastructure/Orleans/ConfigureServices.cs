using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Orleans.EventBus;
using NetCoreCleanArchitecture.Infrastructure.Orleans.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            // EventBus
            services.AddScoped<IEventBus, OrleansEventBus>();

            return services;
        }

        public static IServiceCollection AddNetCleanStateStore(this IServiceCollection services, IConfiguration configuration)
        {
            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(OrleansStateStore<>));

            return services;
        }
    }
}
