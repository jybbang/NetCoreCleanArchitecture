using DaprCleanArchitecture.Domain.Common;
using System.Runtime.CompilerServices;

namespace DaprCleanArchitecture.Domain.Events
{
    public sealed class PropertyChangedEvent<T> : DomainEvent
    {
        public PropertyChangedEvent(Entity source, long version, T oldState, T newState, [CallerMemberName] string propertyName = default)
            : base(source, version, $"{propertyName}PropertyChangedEvent")
        {
            PropertyName = propertyName;
            OldState = oldState;
            NewState = newState;
        }

        public string PropertyName { get; }

        public T OldState { get; }

        public T NewState { get; }
    }
}
