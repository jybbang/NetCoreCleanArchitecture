using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;
using NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus;

namespace NetCoreCleanArchitecture.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddNetCleanZmqEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ZmqOptions>(configuration.GetSection("Api:Zmq"));

            services.AddSingleton<ZmqPublisher>();

            // EventBus
            services.AddScoped<IEventBus, ZmqEventBus>();

            return services;
        }
    }
}
