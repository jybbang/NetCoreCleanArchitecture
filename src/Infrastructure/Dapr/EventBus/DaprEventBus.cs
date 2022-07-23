using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Common.Options;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.EventBus
{
    public class DaprEventBus : IEventBus
    {
        private readonly DaprOptions _options;
        private readonly DaprClient _client;

        public DaprEventBus(IOptions<DaprOptions> options, DaprClient client)
        {
            _options = options.Value;
            _client = client;
        }

        public async ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            await _client.PublishEventAsync(_options.PubSubName, topic, message, cancellationToken);
        }
    }
}
