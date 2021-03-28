using Dapr.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DomainEventSources
{
    public class DaprEventBus : IEventBus
    {
        private readonly ILogger<DaprEventBus> _logger;
        private readonly InfrastructureDaprOptions _opt;
        private readonly DaprClient _client;

        public DaprEventBus(ILogger<DaprEventBus> logger, IOptions<InfrastructureDaprOptions> opt, DaprClient client)
        {
            _logger = logger;
            _opt = opt.Value;
            _client = client;
        }

        public Task PublishEvent<T>(string topic, T message, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("DaprEventBus PublishEvent: {Topic} - {@Message}", topic, message);

            return _client.PublishEventAsync(_opt.PubSubName, topic, message, cancellationToken);
        }
    }
}
