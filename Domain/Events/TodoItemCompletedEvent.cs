using DaprCleanArchitecture.Domain.Common;
using DaprCleanArchitecture.Domain.Entities;

namespace DaprCleanArchitecture.Domain.Events
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
