﻿using System;
using System.Collections.Generic;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class Entity : Base<Entity>
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        protected sealed override IEnumerable<object> Equals()
        {
            yield return Id;
        }
    }
}
