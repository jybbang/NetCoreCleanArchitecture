using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts
{
    public interface IEventHandlerGrain : IGrainWithGuidKey
    {
        [OneWay]
        ValueTask HandleAsync(string topic, byte[] payload, DateTimeOffset timestamp);
    }
}
