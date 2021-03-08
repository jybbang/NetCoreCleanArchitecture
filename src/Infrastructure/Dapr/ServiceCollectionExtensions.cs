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
        public static IServiceCollection AddNetCleanInfrastructure(this IServiceCollection services)
        {
            services.AddTransient(typeof(IStateStore<>), typeof(DaprStateStore<>));

            // DataTimeCaches
            services.AddTransient<IDateTimeCache, DaprDateTimeCache>();

            // EventStore
            services.AddTransient<IEventBus, DaprEventBus>();

            return services;
        }

        public static IMvcBuilder AddNetCleanDapr(this IMvcBuilder builder)
        {
            // Controller with custom validator
            var mvcBuilder = builder.AddDapr();

            return mvcBuilder;
        }
    }
}
