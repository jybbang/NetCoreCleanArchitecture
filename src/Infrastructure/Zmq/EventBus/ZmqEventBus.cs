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
        private readonly ZmqPublisher _pubsub;

        public ZmqEventBus(ZmqPublisher pubsub)
        {
            _pubsub = pubsub;
        }

        public ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            return _pubsub.PublishAsync(topic, message, cancellationToken);
        }
    }
}
