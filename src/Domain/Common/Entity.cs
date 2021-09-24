using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class Entity : Base<Entity>, IHasId
    {
        [JsonInclude]
        public Guid Id { get; private set; } = Guid.NewGuid();

        protected sealed override IEnumerable<object> Equals()
        {
            yield return Id;
        }
    }
}
