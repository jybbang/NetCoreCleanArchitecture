using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Runtime.CompilerServices;

namespace NetCoreCleanArchitecture.Domain.Events
{
    public sealed class PropertyChangedEvent<TSource, TProperty> : DomainEvent where TSource : Entity
    {
        public PropertyChangedEvent(Entity source, TProperty oldState, TProperty newState,
            string topic = default, [CallerMemberName] string propertyName = default)
            : base(topic == default ? $"{source.GetType().Name}{propertyName}ChangedEvent" : topic)
        {
            SourceId = source.Id;
            PropertyName = propertyName;
            OldState = oldState;
            NewState = newState;
        }

        public Guid SourceId { get; }

        public string PropertyName { get; }

        public TProperty OldState { get; }

        public TProperty NewState { get; }
    }
}
