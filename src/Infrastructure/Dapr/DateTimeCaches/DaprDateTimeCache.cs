using NetCoreCleanArchitecture.Application.Common.Interfaces;
using System;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DateTimeCaches
{
    public class DaprDateTimeCache : IDateTimeCache
    {
        private const string DAPR_DATETIME_CACHE_KEY = "now";

        private readonly IStateStore<DateTime> _stateStore;

        public DaprDateTimeCache(IStateStore<DateTime> stateStore)
        {
            _stateStore = stateStore;
        }

        public DateTime Now => _stateStore.GetAsync(DAPR_DATETIME_CACHE_KEY).Result;
    }
}
