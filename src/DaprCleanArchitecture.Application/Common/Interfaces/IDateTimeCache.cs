using System;

namespace DaprCleanArchitecture.Application.Common.Interfaces
{
    public interface IDateTimeCache
    {
        DateTime Now { get; }
    }
}
