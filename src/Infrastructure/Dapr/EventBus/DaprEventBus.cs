using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken)
        {
            await _client.PublishEventAsync(_options.PubSubName, topic, message, cancellationToken);
        }
    }
}
