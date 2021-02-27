using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class DomainEvent : Base<DomainEvent>
    {
        protected DomainEvent(Entity source, string subject, long version)
        {
            Type = source.GetType().Name;
            Source = source.Id;
            SourceVersion = version;
            Subject = subject;
            Topic = $"{Type}/{Subject}";
        }

        public Guid Id { get; } = Guid.NewGuid();

        public string Topic { get; }

        public string Type { get; }

        public Guid Source { get; }

        public long SourceVersion { get; }

        public string Subject { get; }

        public bool CanPublishWithSaveChanges { get; init; } = true;

        public bool IsPublished { get; private set; }

        public DateTimeOffset Time { get; private set; }

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
