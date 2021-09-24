using System;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public interface IHasId
    {
        Guid Id { get; }
    }
}
