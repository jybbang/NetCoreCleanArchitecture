using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Zmq.Common.Zmqs;
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
        private readonly ZmqPublisher _pubsub;

        public ZmqEventBus(ZmqPublisher pubsub)
        {
            _pubsub = pubsub;
        }

        public Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            return _pubsub.PublishAsync(topic, message, cancellationToken);
        }
    }
}
