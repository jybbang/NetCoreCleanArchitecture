
using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores
{
    public class DaprStateStore<T> : IStateStore<T> where T : class
    {
        private readonly DaprOptions _options;
        private readonly DaprClient _client;

        public DaprStateStore(IOptions<DaprOptions> options, DaprClient client)
        {
            _options = options.Value;
            _client = client;
        }

        public async Task<T> GetOrCreateAsync(string key, Func<Task<T>> factory, CancellationToken cancellationToken = default)
        {
            var result = await GetAsync(key, cancellationToken);

            if (result is null)
            {
                var item = await factory();

                await AddAsync(key, item, cancellationToken);

                result = item;
            }

            return result;
        }

        public async Task<T?> GetAsync(string key, CancellationToken cancellationToken = default)
            => await _client.GetStateAsync<T>(_options.StoreName, key, cancellationToken: cancellationToken);

        public Task AddAsync(string key, T item, CancellationToken cancellationToken = default)
            => _client.SaveStateAsync(_options.StoreName, key, item, cancellationToken: cancellationToken);

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
            => _client.DeleteStateAsync(_options.StoreName, key, cancellationToken: cancellationToken);
    }
}
