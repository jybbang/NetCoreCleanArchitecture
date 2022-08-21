using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NetCoreCleanArchitecture.Application.StateStores;

namespace NetCoreCleanArchitecture.Infrastructure.InMemory.StateStores
{
    public class MemoryCacheStateStore<T> : IStateStore<T> where T : class
    {
        private readonly IMemoryCache _client;

        public MemoryCacheStateStore(IMemoryCache client)
        {
            _client = client;
        }

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var result = new List<T>();

            foreach (var key in keys)
            {
                var state = await GetAsync(key, cancellationToken);

                if (!(state is null)) result.Add(state);
            }

            return result;
        }

        public async ValueTask<T?> GetAsync(string key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

            var item = _client.Get<T>(key);

            return await Task.FromResult(item);
        }

        public async ValueTask<T> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

            return await _client.GetOrCreateAsync<T>(key, async entry =>
            {
                if (ttlSeconds > 0)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds);
                }

                return await factory();
            });
        }

        public ValueTask AddAsync(string key, T item, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

            if (ttlSeconds > 0)
            {
                _client.Set<T>(key, item, TimeSpan.FromSeconds(ttlSeconds));
            }
            else
            {
                _client.Set<T>(key, item);
            }

            return new ValueTask();
        }

        public ValueTask RemoveAsync(string key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

            _client.Remove(key);

            return new ValueTask();
        }
    }
}
