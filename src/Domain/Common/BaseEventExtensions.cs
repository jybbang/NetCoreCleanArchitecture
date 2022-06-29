using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public static class BaseEventExtensions
    {
        public static T WithCreatedEvent<T>(this T entitiy, object identifier, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}CreatedEvent" : topic;

            var e = new EntityCreatedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id,
                Identifier = identifier,
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithUpdatedEvent<T>(this T entitiy, object originalIdentifier, object identifier, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}UpdatedEvent" : topic;

            var e = new EntityUpdatedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id,
                OriginalIdentifier = originalIdentifier,
                Identifier = identifier,
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithDeletedEvent<T>(this T entitiy, object identifier, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}DeletedEvent" : topic;

            var e = new EntityDeletedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id,
                Identifier = identifier,
            };

            entitiy.Commit(e);

            return entitiy;
        }
    }

    public class EntityCreatedEvent : BaseEvent
    {
        public EntityCreatedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }

        public object Identifier { get; set; } = null!;
    }

    public class EntityUpdatedEvent : BaseEvent
    {
        public EntityUpdatedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }

        public object OriginalIdentifier { get; set; } = null!;

        public object Identifier { get; set; } = null!;
    }

    public class EntityDeletedEvent : BaseEvent
    {
        public EntityDeletedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }

        public object Identifier { get; set; } = null!;
    }
}
