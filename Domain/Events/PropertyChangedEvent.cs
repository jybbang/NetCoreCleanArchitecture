using DaprCleanArchitecture.Domain.Common;
using System.Runtime.CompilerServices;

namespace DaprCleanArchitecture.Domain.Events
{
    public sealed class PropertyChangedEvent<T> : DomainEvent
    {
        public PropertyChangedEvent(Entity sender, long version, T oldState, T newState, [CallerMemberName] string propertyName = default)
            : base(sender, version)
        {
            Topic = $"{Topic}/{propertyName}ChangedEvent";
            OldState = oldState;
            NewState = newState;
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public T OldState { get; }

        public T NewState { get; }
    }
}
