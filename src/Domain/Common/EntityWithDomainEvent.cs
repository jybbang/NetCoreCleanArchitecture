using NetCoreCleanArchitecture.Domain.Events;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class EntityWithDomainEvent : Entity
    {
        protected long _version;

        public long Version { get => _version; set => Interlocked.Exchange(ref _version, value); }

        internal IProducerConsumerCollection<DomainEvent> DomainEvents { get; } = new ConcurrentQueue<DomainEvent>();

        public void Commit(DomainEvent domainEvent)
        {
            Interlocked.Increment(ref _version);

            DomainEvents.TryAdd(domainEvent.SetVersion(_version));
        }

        protected void PropertyChanged<TSource, TProperty>(ref TProperty oldState, TProperty newState, bool canPublishToEventStore = true, string subject = default, [CallerMemberName] string propertyName = default) where TSource : EntityWithDomainEvent
        {
            if (oldState.Equals(newState)) return;

            Commit(new PropertyChangedEvent<TSource, TProperty>(this, oldState, newState, subject, propertyName)
            {
                CanPublishToInfrastructure = canPublishToEventStore
            });

            oldState = newState;
        }
    }
}
