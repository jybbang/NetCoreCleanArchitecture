using NetCoreCleanArchitecture.Domain.Common;
using System.Runtime.CompilerServices;

namespace NetCoreCleanArchitecture.Domain.Events
{
    public sealed class PropertyChangedEvent<TSource, TProperty> : DomainEvent where TSource : Entity
    {
        public PropertyChangedEvent(Entity source, long version, TProperty oldState, TProperty newState, [CallerMemberName] string propertyName = default)
            : base(source, version, $"{typeof(TSource).Name}{propertyName}PropertyChangedEvent")
        {
            PropertyName = propertyName;
            OldState = oldState;
            NewState = newState;
        }

        public string PropertyName { get; }

        public TProperty OldState { get; }

        public TProperty NewState { get; }
    }
}
