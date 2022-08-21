
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Common.Options;

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

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var stream = await _client.GetBulkStateAsync(_options.StoreName, keys, null, cancellationToken: cancellationToken);

            if (stream.Count == 0) return null;

            return stream.OfType<T>().ToList();
        }

        public async ValueTask<T?> GetAsync(string key, CancellationToken cancellationToken)
            => await _client.GetStateAsync<T>(_options.StoreName, key, cancellationToken: cancellationToken);

        public async ValueTask<T> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, int ttlSeconds, CancellationToken cancellationToken)
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

        public async ValueTask AddAsync(string key, T item, int ttlSeconds, CancellationToken cancellationToken)
        {
            var metadata = new Dictionary<string, string>
            {
                {"ttlInSeconds",ttlSeconds.ToString() },
            };

            await _client.SaveStateAsync(_options.StoreName, key, item, metadata: metadata, cancellationToken: cancellationToken);
        }

        public async ValueTask RemoveAsync(string key, CancellationToken cancellationToken)
            => await _client.DeleteStateAsync(_options.StoreName, key, cancellationToken: cancellationToken);
    }
}
