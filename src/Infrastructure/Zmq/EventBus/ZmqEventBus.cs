using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
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

        public Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            return _pubsub.PublishAsync(topic, message, cancellationToken);
        }
    }
}
