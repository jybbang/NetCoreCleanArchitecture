using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.InMemory.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure
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
