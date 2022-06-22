using Microsoft.Extensions.Caching.Memory;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using System;
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

        public async Task<T> GetOrCreateAsync(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        {
            return await _client.GetOrCreateAsync<T>(key, entry => factory());
        }

        public async Task<T?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var item = _client.Get<T>(key);

            return await Task.FromResult(item);
        }

        public Task AddAsync(string key, T item, CancellationToken cancellationToken = default)
        {
            _client.Set<T>(key, item);

            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            _client.Remove(key);

            return Task.CompletedTask;
        }
    }
}
