using Microsoft.Extensions.Caching.Memory;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.InMemory.StateStores
{
    public class MemoryCacheStateStore<T> : IStateStore<T> where T : class
    {
        private readonly IMemoryCache _client;

        public MemoryCacheStateStore(IMemoryCache client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<T>?> GetBulkAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var result = new List<T>();

            foreach (var key in keys)
            {
                var state = await GetAsync(key, cancellationToken);

                if (!(state is null)) result.Add(state);
            }

            return result;
        }

        public async Task<T?> GetAsync(string key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            var item = _client.Get<T>(key);

            return await Task.FromResult(item);
        }

        public Task<T> GetOrCreateAsync(string key, Func<Task<T>> factory, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            return _client.GetOrCreateAsync<T>(key, entry =>
            {
                if (ttlSeconds > 0)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(ttlSeconds);
                }

                return factory();
            });
        }

        public Task AddAsync(string key, T item, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            if (ttlSeconds > 0)
            {
                _client.Set<T>(key, item, TimeSpan.FromSeconds(ttlSeconds));
            }
            else
            {
                _client.Set<T>(key, item);
            }

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            _client.Remove(key);

            return Task.CompletedTask;
        }
    }
}
