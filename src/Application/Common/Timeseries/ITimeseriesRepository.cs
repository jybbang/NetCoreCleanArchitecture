using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Timeseries
{
    public interface ITimeseriesRepository
    {
        Task<IReadOnlyList<(DateTimeOffset, double)>> GetAsync(Guid id, DateTimeOffset from, DateTimeOffset to = default, CancellationToken cancellationToken = default);

        Task AddAsync(Guid id, DateTimeOffset timestamp, double value, CancellationToken cancellationToken);

        Task RemoveAsync(Guid id, DateTimeOffset from = default, DateTimeOffset to = default, CancellationToken cancellationToken = default);
    }
}
