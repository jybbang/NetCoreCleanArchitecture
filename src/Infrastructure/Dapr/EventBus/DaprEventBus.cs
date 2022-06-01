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
        private readonly InfrastructureDaprOptions _opt;
        private readonly DaprClient _client;

        public DaprEventBus(IOptions<InfrastructureDaprOptions> opt, DaprClient client)
        {
            _opt = opt.Value;
            _client = client;
        }

        public async Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken)
        {
            await _client.PublishEventAsync(_opt.PubSubName, topic, message, cancellationToken);
        }
    }
}
