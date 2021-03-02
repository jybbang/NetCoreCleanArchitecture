using Dapr.Client;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources
{
    public class DaprEventBus : IEventBus
    {
        private const string DAPR_PUBSUB_NAME = "pubsub";

        private readonly ILogger<DaprEventBus> _logger;
        private readonly DaprClient _client;

        public DaprEventBus(ILogger<DaprEventBus> logger, DaprClient client)
        {
            _logger = logger;
            _client = client;
        }

        public Task PublishEvent<T>(string topic, T message, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            _logger.LogDebug("DaprEventBus PublishEvent: {Topic} - {@Message}", topic, message);

            return _client.PublishEventAsync(DAPR_PUBSUB_NAME, topic, message, cancellationToken);
        }
    }
}
