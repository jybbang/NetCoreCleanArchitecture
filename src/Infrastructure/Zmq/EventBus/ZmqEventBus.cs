using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;

namespace NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus
{
    public class ZmqEventBus : IEventBus
    {
        private readonly ILogger<ZmqEventBus> _logger;
        private readonly IOptions<ZmqOptions> _options;
        private readonly ZmqPublisher _pubsub;

        private readonly BlockingCollection<(string topic, object message)> _bc = new BlockingCollection<(string topic, object message)>();

        public ZmqEventBus(
            ILogger<ZmqEventBus> logger,
            IOptions<ZmqOptions> options,
            ZmqPublisher pubsub)
        {
            _logger = logger;
            _options = options;
            _pubsub = pubsub;
        }

        private async Task RunBlockingCollection()
        {
            while (true)
            {
                try
                {
                    var item = _bc.Take();

                    using var cts = new CancellationTokenSource(_options.Value.RequestTimeout);

                    await _pubsub.PublishAsync(item.topic, (BaseEvent)item.message, cts.Token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred: {Exception}", ex.Message);
                }
            }
        }

        public async ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            await _pubsub.PublishAsync(topic, message, cancellationToken);
        }
    }
}
