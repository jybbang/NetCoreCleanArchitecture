
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IEnumerable<string> keys, CancellationToken cancellationToken = default)
        {
            var stream = await _client.GetBulkStateAsync(_options.StoreName, keys.ToList(), null, cancellationToken: cancellationToken);

            if (stream.Count == 0) return null;

            return stream.OfType<T>().ToList();
        }

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IEnumerable<(string key, object? etag)> keys, CancellationToken cancellationToken = default)
        {
            var stream = await _client.GetBulkStateAsync(_options.StoreName, keys.Select(x => x.key).ToList(), null, cancellationToken: cancellationToken);

            if (stream.Count == 0) return null;

            return stream.OfType<T>().ToList();
        }

        public async ValueTask<T?> GetAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
            => await _client.GetStateAsync<T>(_options.StoreName, key, cancellationToken: cancellationToken);

        public async ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            var result = await GetAsync(key, etag, cancellationToken);

            if (result is null)
            {
                var item = await factory();

                await SetAsync(key, item, ttlSeconds, etag, cancellationToken);

                result = item;
            }

            return result;
        }

        public ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, object? etag = default, CancellationToken cancellationToken = default)
        {
            return GetOrCreateAsync(key, factory, 0, etag, cancellationToken);
        }

        public async ValueTask SetAsync(string key, T item, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            var metadata = ttlSeconds > 0
                ? new Dictionary<string, string>
                {
                    { "ttlInSeconds", ttlSeconds.ToString() },
                    { "etag", etag?.ToString() ?? string.Empty },
                }
                : null;

            await _client.SaveStateAsync(_options.StoreName, key, item, metadata: metadata, cancellationToken: cancellationToken);
        }

        public ValueTask SetAsync(string key, T item, object? etag = default, CancellationToken cancellationToken = default)
        {
            return SetAsync(key, item, 0, etag, cancellationToken);
        }

        public async ValueTask RemoveAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
            => await _client.DeleteStateAsync(_options.StoreName, key, cancellationToken: cancellationToken);
    }
}
