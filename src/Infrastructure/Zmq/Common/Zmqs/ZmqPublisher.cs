using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus;
using NetMQ;
using NetMQ.Sockets;

namespace NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs
{
    public class ZmqPublisher
    {
        private readonly ILogger<ZmqEventBus> _logger;
        private readonly IOptions<ZmqOptions> _options;
        private readonly BlockingCollection<(string topic, BaseEvent message)> _bc = new BlockingCollection<(string topic, BaseEvent message)>();
        private readonly PublisherSocket _pubSocket;

        public ZmqPublisher(
            ILogger<ZmqEventBus> logger,
            IOptions<ZmqOptions> options)
        {
            _logger = logger;
            _options = options;
            _pubSocket = new PublisherSocket();

            _pubSocket.Options.SendHighWatermark = options.Value.SendHighWatermark;

            var uri = $"tcp://*:{options.Value.Port}";

            _pubSocket.Bind(uri);

            Task.Factory.StartNew(RunBlockingCollectionAsync).Unwrap();
        }

        public ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (string.IsNullOrEmpty(topic) || message is null) return new ValueTask();

            if (message.AtLeastOnce)
            {
                _bc.Add((topic, message), cancellationToken);
            }
            else
            {
                _bc.TryAdd((topic, message), _options.Value.RequestTimeout, cancellationToken);
            }

            return new ValueTask();
        }

        private Task RunBlockingCollectionAsync()
        {
            while (true)
            {
                try
                {
                    var (topic, message) = _bc.Take();

                    var payload = JsonSerializer.SerializeToUtf8Bytes(message);

                    _pubSocket.SendMoreFrame(topic).SendFrame(payload);
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
    }
}
