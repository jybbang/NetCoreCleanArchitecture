using NetCoreCleanArchitecture.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DateTimeCaches
{
    public class DaprDateTimeCache : IDateTimeCache
    {
        private const string DATETIME_CACHE_KEY = "now";

        private readonly IStateStore<DateTime> _stateStore;

        public DaprDateTimeCache(IStateStore<DateTime> stateStore)
        {
            _stateStore = stateStore;
        }

        public async Task<DateTime> Now(CancellationToken cancellationToken = default)
        {
            var result = DateTime.MinValue;

            try
            {
                result = await _stateStore.GetAsync(DATETIME_CACHE_KEY, cancellationToken);
            }
            catch (Exception)
            {
                result = DateTime.UtcNow;
            }

            return result;
        }
    }
}
