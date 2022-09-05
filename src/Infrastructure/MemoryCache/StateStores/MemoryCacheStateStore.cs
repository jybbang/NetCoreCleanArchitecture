using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetCoreCleanArchitecture.Application.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure.MemoryCache.StateStores
{
    public class MemoryCacheStateStore<T> : IStateStore<T> where T : class
    {
        private readonly IMemoryCache _client;

        public MemoryCacheStateStore(IMemoryCache client)
        {
            _client = client;
        }

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();

            foreach (var key in keys)
            {
                var state = await GetAsync(key, cancellationToken: cancellationToken);

                if (!(state is null)) result.Add(state);
            }

            return result;
        }

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IEnumerable<(string key, object? etag)> keys, CancellationToken cancellationToken = default)
        {
            var result = new List<T>();

            foreach (var key in keys)
            {
                var state = await GetAsync(key.key, key.etag, cancellationToken);

                if (!(state is null)) result.Add(state);
            }

            return result;
        }

        public async ValueTask<T?> GetAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
        {
            var item = _client.Get<T>(key);

            return await Task.FromResult(item);
        }

        public async ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            return await _client.GetOrCreateAsync(key, async entry =>
            {
                if (ttlSeconds > 0)
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds);

                return await factory();
            });
        }

        public ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, object? etag = default, CancellationToken cancellationToken = default)
        {
            return GetOrCreateAsync(key, factory, 0, etag, cancellationToken);
        }

        public ValueTask SetAsync(string key, T item, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (ttlSeconds > 0)
                _client.Set(key, item, TimeSpan.FromSeconds(ttlSeconds));
            else
            {
                _client.Set(key, item);
            }

            return new ValueTask();
        }

        public ValueTask SetAsync(string key, T item, object? etag = default, CancellationToken cancellationToken = default)
        {
            return SetAsync(key, item, 0, etag, cancellationToken);
        }

        public ValueTask RemoveAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
        {
            _client.Remove(key);

            return new ValueTask();
        }
    }
}
