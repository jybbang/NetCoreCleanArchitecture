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

        public string Topic { get; }

        public Guid EventId { get; set; }

        public long SourceVersion { get; set; }

        public bool IsPublished { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public DomainEvent Publising(DateTimeOffset timestamp = default)
        {
            EventId = Guid.NewGuid();

            Timestamp = timestamp == default ? DateTimeOffset.UtcNow : timestamp;

            IsPublished = true;

            return this;
        }

        protected sealed override IEnumerable<object> Equals()
        {
            yield return EventId;
        }
    }
}
