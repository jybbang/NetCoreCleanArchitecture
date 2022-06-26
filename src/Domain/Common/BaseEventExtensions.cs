using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public static class BaseEventExtensions
    {
        public static T WithCreatedEvent<T>(this T entitiy, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}CreatedEvent" : topic;

            var e = new EntitiyCreatedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithUpdatedEvent<T>(this T entitiy, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}UpdatedEvent" : topic;

            var e = new EntitiyUpdatedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id
            };

            entitiy.Commit(e);

            return entitiy;
        }

        public static T WithDeletedEvent<T>(this T entitiy, string topic = "") where T : BaseEntity
        {
            var entitiyName = entitiy.GetType().Name;

            topic = string.IsNullOrEmpty(topic) ? $"{entitiyName}DeletedEvent" : topic;

            var e = new EntitiyDeletedEvent(topic)
            {
                EntityName = entitiyName,
                Id = entitiy.Id
            };

            entitiy.Commit(e);

            return entitiy;
        }
    }

    public class EntitiyCreatedEvent : BaseEvent
    {
        public EntitiyCreatedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }
    }

    public class EntitiyUpdatedEvent : BaseEvent
    {
        public EntitiyUpdatedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }
    }

    public class EntitiyDeletedEvent : BaseEvent
    {
        public EntitiyDeletedEvent(string topic) : base(topic)
        {
        }

        public string EntityName { get; set; } = null!;

        public Guid Id { get; set; }
    }
}
