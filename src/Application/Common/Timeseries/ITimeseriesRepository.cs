using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Timeseries
{
    public interface ITimeseriesRepository
    {
        ValueTask<IReadOnlyList<(DateTimeOffset, double)>> GetAsync(Guid id, DateTimeOffset from, DateTimeOffset to = default, CancellationToken cancellationToken = default);

        ValueTask AddAsync(Guid id, DateTimeOffset timestamp, double value, CancellationToken cancellationToken);

        ValueTask RemoveAsync(Guid id, DateTimeOffset from = default, DateTimeOffset to = default, CancellationToken cancellationToken = default);
    }
}
