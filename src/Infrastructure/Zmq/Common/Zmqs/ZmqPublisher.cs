using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
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
        private readonly Channel<(string topic, object message)> _c = Channel.CreateUnbounded<(string topic, object message)>();
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

            Task.Run(ConsumingEvent).ConfigureAwait(false);
        }

        public async ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (string.IsNullOrEmpty(topic) || message is null) return;

            await _c.Writer.WriteAsync((topic, message), cancellationToken);
        }

        private async Task ConsumingEvent()
        {
            var options = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode,
            };

            while (await _c.Reader.WaitToReadAsync())
            {
                try
                {
                    var (topic, message) = await _c.Reader.ReadAsync();

                    var payload = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), options);

                    _pubSocket.SendMoreFrame(topic).SendFrame(payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception occurred: {Exception}", ex.Message);
                }
            }
        }
    }
}
