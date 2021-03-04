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

        private bool _isCircuitBroken;
        private int _fails = 0;

        public DaprDateTimeCache(IStateStore<DateTime> stateStore)
        {
            _stateStore = stateStore;
        }

        public async Task<DateTime> Now(CancellationToken cancellationToken = default)
        {
            var result = DateTime.MinValue;

            try
            {
                if (_isCircuitBroken) return DateTime.UtcNow;

                result = await _stateStore.GetAsync(DATETIME_CACHE_KEY, cancellationToken);
            }
            catch (Exception)
            {
                _isCircuitBroken = true;

                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, ++_fails)), cancellationToken);
            }
            finally
            {
                _isCircuitBroken = false;
            }

            return result;
        }
    }
}
