using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Infrastructure.Orleans.EventBus;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            // EventBus
            services.AddScoped<IEventBus, OrleansEventBus>();

            return services;
        }
    }
}
