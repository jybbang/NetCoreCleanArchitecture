using Dapr.Client;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources
{
    public class EventStore : IEventStore
    {
        private readonly DaprClient _client;

        public EventStore(DaprClient client)
        {
            _client = client;
        }

        public Task PublishEvent<T>(T message, CancellationToken cancellationToken = default) where T : DomainEvent
             => _client.PublishEventAsync(nameof(EventStore), message.Topic, message, cancellationToken);
    }
}
