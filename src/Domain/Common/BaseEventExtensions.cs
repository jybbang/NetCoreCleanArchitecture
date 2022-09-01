using System;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public static class BaseEventExtensions
    {
        public static T WithCreatedEvent<T>(this T entitiy, string identifier, string? etag = null, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}CreatedEvent" : topic;

            var e = new EntityCreatedEvent()
            {
                Topic = topic,
                EntityName = entitiyName,
                Id = entitiy.Id,
                Identifier = identifier,
                Etag = etag,
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithUpdatedEvent<T>(this T entitiy, string originalIdentifier, string identifier, string? etag = null, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}UpdatedEvent" : topic;

            var e = new EntityUpdatedEvent()
            {
                Topic = topic,
                EntityName = entitiyName,
                Id = entitiy.Id,
                Identifier = identifier,
                OriginalIdentifier = originalIdentifier,
                Etag = etag,
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithDeletedEvent<T>(this T entitiy, string identifier, string? etag = null, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}DeletedEvent" : topic;

            var e = new EntityDeletedEvent()
            {
                Topic = topic,
                EntityName = entitiyName,
                Id = entitiy.Id,
                Identifier = identifier,
                Etag = etag,
            };

            entitiy.Commit(e);

            return entitiy;
        }
    }

    public class EntityCreatedEvent : BaseEvent
    {
        public Guid Id { get; set; }

        public string EntityName { get; set; } = null!;

        public string Identifier { get; set; } = null!;

        public string? Etag { get; set; }
    }

    public class EntityUpdatedEvent : BaseEvent
    {
        public Guid Id { get; set; }

        public string EntityName { get; set; } = null!;

        public string Identifier { get; set; } = null!;

        public string OriginalIdentifier { get; set; } = null!;

        public string? Etag { get; set; }
    }

    public class EntityDeletedEvent : BaseEvent
    {
        public Guid Id { get; set; }

        public string EntityName { get; set; } = null!;

        public string Identifier { get; set; } = null!;

        public string? Etag { get; set; }
    }
}
