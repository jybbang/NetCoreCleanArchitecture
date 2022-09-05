using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.MemoryCache.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure.MemoryCache
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanStateStore(this IServiceCollection services, IConfiguration configuration)
        {
            // StateStore
            services.AddScoped(typeof(IStateStore<>), typeof(MemoryCacheStateStore<>));

            return services;
        }
    }
}
