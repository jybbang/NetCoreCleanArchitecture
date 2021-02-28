using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.Infrastructure.Dapr.DateTimeCaches;
using NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources;
using NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreCleanArchitectureInfrastructure(this IServiceCollection services)
        {
            // Dapr
            //services.AddDaprClient();

            services.AddTransient(typeof(IStateStore<>), typeof(StateStore<>));

            // DataTimeCaches
            services.AddTransient<IDateTimeCache, DateTimeCache>();

            // EventStore
            services.AddTransient<IEventStore, EventStore>();

            return services;
        }
    }
}
