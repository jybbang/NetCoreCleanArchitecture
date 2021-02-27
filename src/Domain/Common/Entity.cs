using NetCoreCleanArchitecture.Domain.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class Entity : Base<Entity>
    {
        protected Entity(Guid id = default)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
        }

        public Guid Id { get; }

        protected sealed override IEnumerable<object> Equals()
        {
            yield return Id;
        }
    }

    public abstract class EntityHasDomainEvent : Entity
    {
        protected long _version;

        public long Version { get => _version; set => Interlocked.Exchange(ref _version, value); }

        public IProducerConsumerCollection<DomainEvent> DomainEvents { get; } = new ConcurrentQueue<DomainEvent>();

        protected void PropertyChanged<TSource, TProperty>(ref TProperty oldState, TProperty newState, [CallerMemberName] string propertyName = default) where TSource : EntityHasDomainEvent
        {
            if(oldState.Equals(newState)) return;

            Commit(new PropertyChangedEvent<TSource, TProperty>(this, Version, oldState, newState, propertyName));

            oldState = newState;
        }

        protected void Commit(DomainEvent domainEvent)
        {
            Interlocked.Increment(ref _version);

            DomainEvents.TryAdd(domainEvent);
        }
    }
}
