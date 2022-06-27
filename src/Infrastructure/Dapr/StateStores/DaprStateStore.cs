
using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Common.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IReadOnlyList<T>?> GetBulkAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var stream = await _client.GetBulkStateAsync(_options.StoreName, keys, null, cancellationToken: cancellationToken);

            if (stream.Count == 0) return null;

            return stream.OfType<T>().ToList();
        }

        public async Task<T?> GetAsync(string key, CancellationToken cancellationToken)
            => await _client.GetStateAsync<T>(_options.StoreName, key, cancellationToken: cancellationToken);

        public async Task<T> GetOrCreateAsync(string key, Func<Task<T>> factory, int ttlSeconds, CancellationToken cancellationToken)
        {
            var result = await GetAsync(key, cancellationToken);

            if (result is null)
            {
                var item = await factory();

                await AddAsync(key, item, ttlSeconds, cancellationToken);

                result = item;
            }

            return result;
        }

        public Task AddAsync(string key, T item, int ttlSeconds, CancellationToken cancellationToken)
        {
            var metadata = new Dictionary<string, string>
            {
                {"ttlInSeconds",ttlSeconds.ToString() },
            };

            return _client.SaveStateAsync(_options.StoreName, key, item, metadata: metadata, cancellationToken: cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken)
            => _client.DeleteStateAsync(_options.StoreName, key, cancellationToken: cancellationToken);
    }
}
