using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Interfaces
{
    public interface IDateTimeCache
    {
        Task<DateTime> Now(CancellationToken cancellationToken = default);
    }
}
