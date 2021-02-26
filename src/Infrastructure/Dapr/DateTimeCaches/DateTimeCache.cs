using NetCoreCleanArchitecture.Application.Common.Interfaces;
using System;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.DateTimeCaches
{
    public class DateTimeCache : IDateTimeCache
    {
        private readonly IStateStore<DateTime> _stateStore;

        public DateTimeCache(IStateStore<DateTime> stateStore)
        {
            _stateStore = stateStore;
        }

        public DateTime Now => _stateStore.GetAsync(nameof(Now)).Result;
    }
}
