using DotNnetcoreCleanArchitecture.Domain.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DotNnetcoreCleanArchitecture.Domain.Abstracts
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

        protected void PropertyChanged<T>(ref T oldState, T newState, [CallerMemberName] string propertyName = default)
        {
            if (oldState.Equals(newState)) return;

            Interlocked.Increment(ref _version);

            DomainEvents.TryAdd(new PropertyChangedEvent<T>(this, Version, oldState, newState, propertyName));

            oldState = newState;
        }

        protected void Commit(DomainEvent domainEvent)
        {
            Interlocked.Increment(ref _version);

            DomainEvents.TryAdd(domainEvent);
        }
    }
}
