using System.Collections.Concurrent;
using System.Threading;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class EntityWithDomainEvent : Entity
    {
        private long _version;

        public long Version { get => _version; private set => Interlocked.Exchange(ref _version, value); }

        public IProducerConsumerCollection<DomainEvent> DomainEvents { get; } = new ConcurrentQueue<DomainEvent>();

        public void Commit(DomainEvent domainEvent)
        {
            Interlocked.Increment(ref _version);

            domainEvent.SourceVersion = _version;

            DomainEvents.TryAdd(domainEvent);
        }
    }
}
