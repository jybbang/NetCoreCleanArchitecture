using System;

namespace NetCoreCleanArchitecture.Application.Common.Interfaces
{
    public interface IDateTimeCache
    {
        DateTime Now { get; }
    }
}
