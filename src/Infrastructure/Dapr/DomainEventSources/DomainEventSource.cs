using Dapr.Client;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources
{
    public class DomainEventSource : IDomainEventSource
    {
        private readonly DaprClient _client;

        public DomainEventSource(DaprClient client)
        {
            _client = client;
        }

        public Task PublishEvent<T>(T message, CancellationToken cancellationToken = default) where T : DomainEvent
             => _client.PublishEventAsync(nameof(DomainEventSource), message.Topic, message, cancellationToken);
    }
}
