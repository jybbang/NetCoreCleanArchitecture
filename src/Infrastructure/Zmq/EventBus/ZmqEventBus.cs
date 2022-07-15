using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;

using NetMQ;
using NetMQ.Sockets;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Zmq.EventBus
{
    public class ZmqEventBus : IEventBus
    {
        private readonly IOptions<ZmqOptions> _options;
        private readonly PublisherSocket _pubSocket;

        public ZmqEventBus(IOptions<ZmqOptions> options)
        {
            _options = options;

            _pubSocket = new PublisherSocket();

            _pubSocket.Options.SendHighWatermark = _options.Value.SendHighWatermark;

            _pubSocket.Bind($"tcp://*:{_options.Value.Port}");
        }

        public Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            var payload = JsonSerializer.SerializeToUtf8Bytes(message);

            _pubSocket.SendMoreFrame(topic).SendFrame(payload);

            return Task.CompletedTask;
        }
    }
}
