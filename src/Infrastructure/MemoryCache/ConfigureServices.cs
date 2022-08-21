using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.InMemory.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanMemoryCacheStateStore(this IServiceCollection services)
        {
            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(MemoryCacheStateStore<>));

            return services;
        }
    }
}
