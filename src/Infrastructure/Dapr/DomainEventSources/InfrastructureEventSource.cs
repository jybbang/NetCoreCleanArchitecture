using Dapr.Client;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources
{
    public class InfrastructureEventSource : IInfrastructureEventSource
    {
        private readonly DaprClient _client;

        public InfrastructureEventSource(DaprClient client)
        {
            _client = client;
        }

        public Task PublishEvent<T>(string topic, T message, CancellationToken cancellationToken = default) where T : DomainEvent
             => _client.PublishEventAsync("eventsource", topic, message, cancellationToken);
    }
}
