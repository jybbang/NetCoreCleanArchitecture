using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class DomainEvent : Base<DomainEvent>
    {
        protected DomainEvent(string topic)
        {
            Topic = topic;
        }

        public Guid EventId { get; } = Guid.NewGuid();

        public string Topic { get; }

        public bool CanPublishToEventBus { get; set; } = true;

        public bool CanSaveToEventStore { get; set; } = true;

        public bool CanBuffered { get; set; } = false;

        public string BufferKey { get; set; }

        public long SourceVersion { get; private set; }

        public bool IsPublished { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public DomainEvent SetVersion(long version)
        {
            SourceVersion = version;

            return this;
        }

        public DomainEvent Publising(DateTimeOffset timestamp = default)
        {
            IsPublished = true;

            Timestamp = timestamp == default ? DateTimeOffset.UtcNow : timestamp;

            return this;
        }

        protected sealed override IEnumerable<object> Equals()
        {
            yield return EventId;
        }
    }
}
