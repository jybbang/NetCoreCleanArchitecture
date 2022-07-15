using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;
using NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanZmqEventBus(this IServiceCollection services)
        {
            services.AddOptions<ZmqOptions>("Api:Zmq");

            services.AddSingleton<ZmqPublisher>();

            // EventBus
            services.AddScoped<IEventBus, ZmqEventBus>();

            return services;
        }
    }
}
