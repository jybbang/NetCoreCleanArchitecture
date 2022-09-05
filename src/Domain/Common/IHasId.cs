using System;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public interface IHasId
    {
        public Guid Id { get; set; }
    }
}
