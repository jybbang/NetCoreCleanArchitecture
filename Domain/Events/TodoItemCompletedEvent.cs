using DotNnetcoreCleanArchitecture.Domain.Abstracts;
using DotNnetcoreCleanArchitecture.Domain.Entities;
using System;

namespace DotNnetcoreCleanArchitecture.Domain.Events
{
    public sealed class TodoItemCompletedEvent : DomainEvent
    {
        public TodoItemCompletedEvent(Entity sender, long version, TodoItem todoItem)
            : base(sender, version)
        {
            Topic = $"{Topic}/{nameof(TodoItemCompletedEvent)}";
            TodoItem = todoItem;
        }

        public TodoItem TodoItem { get; }
    }
}
