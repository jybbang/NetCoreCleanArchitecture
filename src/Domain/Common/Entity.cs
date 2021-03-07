using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class Entity : Base<Entity>
    {
        protected Entity(Guid id = default)
        {
            Id = id == Guid.Empty ? Guid.NewGuid() : id;
        }

        public Guid Id { get; init; }

        protected sealed override IEnumerable<object> Equals()
        {
            yield return Id;
        }
    }
}
