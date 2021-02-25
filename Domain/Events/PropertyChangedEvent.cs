using DotNnetcoreCleanArchitecture.Domain.Abstracts;
using System;
using System.Runtime.CompilerServices;

namespace DotNnetcoreCleanArchitecture.Domain.Events
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
