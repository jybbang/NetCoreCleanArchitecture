using DaprCleanArchitecture.Domain.Common;
using DaprCleanArchitecture.Domain.Entities;

namespace DaprCleanArchitecture.Domain.Events
{
    public sealed class TodoItemCompletedEvent : DomainEvent
    {
        public TodoItemCompletedEvent(Entity source, long version, TodoItem todoItem)
            : base(source, version, nameof(TodoItemCompletedEvent))
        {
            TodoItem = todoItem;
        }

        public TodoItem TodoItem { get; }
    }
}
