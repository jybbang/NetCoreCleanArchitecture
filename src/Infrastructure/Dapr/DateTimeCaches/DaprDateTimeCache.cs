using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DateTimeCaches
{
    public class DaprDateTimeCache : IDateTimeCache
    {
        private readonly ILogger<DaprDateTimeCache> _logger;
        private readonly IStateStore<DateTime> _stateStore;
        private readonly InfrastructureDaprOptions _opt;

        public DaprDateTimeCache(
            ILogger<DaprDateTimeCache> logger,
            IStateStore<DateTime> stateStore,
            IOptions<InfrastructureDaprOptions> opt)
        {
            _logger = logger;
            _stateStore = stateStore;
            _opt = opt.Value;
        }

        public async Task<DateTime> Now(CancellationToken cancellationToken = default)
        {
            var result = default(DateTime);

            try
            {
                result = _opt.UseDatetimeCache
                    ? await _stateStore.GetAsync(_opt.DatetimeKey, new CancellationTokenSource(200).Token)
                    : DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DaprDateTimeCache Unhandled Exception");
            }

            return result == default ? DateTime.UtcNow : result;
        }
    }
}
