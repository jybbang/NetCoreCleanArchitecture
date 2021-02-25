using System;

namespace DaprCleanArchitecture.Domain.Common
{
    public abstract class DomainEvent
    {
        protected DomainEvent(Entity sender, long version)
        {
            Topic = $"{AppName}/{EntityName}";
            EntityName = sender.GetType().Name;
            EntityId = sender.Id;
            Version = version;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

        public string AppName { get; } = nameof(DaprCleanArchitecture);

        public string Topic { get; init; }

        public Guid EntityId { get; }

        public long Version { get; }

        public string EntityName { get; }

        public bool CanPublishWithSaveChanges { get; init; } = true;

        public bool IsPublished { get; private set; }

        public DateTimeOffset PublishedAt { get; private set; }

        public DomainEvent Publising(DateTimeOffset timestamp = default)
        {
            IsPublished = true;
            PublishedAt = timestamp == default ? DateTimeOffset.UtcNow : timestamp;

            return this;
        }
    }
}
