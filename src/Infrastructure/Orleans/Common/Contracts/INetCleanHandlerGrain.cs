using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts
{
    public interface INetCleanHandlerGrain : IGrainWithGuidKey
    {
        ValueTask HandleEventAsync(string topic, byte[] payload, DateTimeOffset timestamp);

        ValueTask RemoveStateAsync(string typeName, string key, string? etag);

        ValueTask SetStateAsync(string typeName, string key, string? etag, byte[] item);

        ValueTask<byte[]> GetOrCreateStateAsync(string typeName, string key, string? etag, byte[] item);

        [AlwaysInterleave]
        ValueTask<byte[]> GetStateAsync(string typeName, string key, string? etag);
    }
}
