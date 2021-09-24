using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class DomainEvent : Base<DomainEvent>
    {
        protected DomainEvent(Guid id, string topic)
        {
            Id = id;

            Topic = topic;
        }

        public Guid EventId { get; } = Guid.NewGuid();

        public string Topic { get; }

        public Guid Id { get; }

        public bool CanPublishToEventBus { get; init; } = true;

        public bool CanSaveToEventStore { get; init; } = true;

        public bool CanBuffered { get; init; } = false;

        public long SourceVersion { get; private set; }

        public bool IsPublished { get; private set; }

        public DateTimeOffset Time { get; private set; }

        public DomainEvent SetVersion(long version)
        {
            SourceVersion = version;

            return this;
        }

        public DomainEvent Publising(DateTimeOffset timestamp = default)
        {
            IsPublished = true;

            Time = timestamp == default ? DateTimeOffset.UtcNow : timestamp;

            return this;
        }

        protected sealed override IEnumerable<object> Equals()
        {
            yield return Id;
        }
    }
}
