using DaprCleanArchitecture.Application.Common.Interfaces;
using System;

namespace DaprCleanArchitecture.Infrastructure.DateTimeCaches
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
