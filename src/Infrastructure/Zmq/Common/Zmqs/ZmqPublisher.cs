using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetMQ;
using NetMQ.Sockets;

namespace NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs
{
    public class ZmqPublisher
    {
        private readonly PublisherSocket _pubSocket;
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

        public ZmqPublisher(IOptions<ZmqOptions> options)
        {
            _pubSocket = new PublisherSocket();

            _pubSocket.Options.SendHighWatermark = options.Value.SendHighWatermark;

            var uri = $"tcp://*:{options.Value.Port}";

            _pubSocket.Bind(uri);
        }

        public async ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (!message.AtLeastOnce && _sync.CurrentCount <= 0) return;

            await _sync.WaitAsync(cancellationToken);

            try
            {
                var payload = JsonSerializer.SerializeToUtf8Bytes(message);

                _pubSocket.SendMoreFrame(topic).SendFrame(payload);
            }
            finally
            {
                _sync.Release();
            }
        }
    }
}
