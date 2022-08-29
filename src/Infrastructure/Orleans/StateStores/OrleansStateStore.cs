using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts;
using Orleans;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.StateStores
{
    public class OrleansStateStore<T> : IStateStore<T> where T : class
    {
        private readonly IClusterClient _clusterClient;

        private INetCleanHandlerGrain? _handler;

        private JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode,
        };

        public OrleansStateStore(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public async ValueTask<T?> GetAsync(string key, object? etag, CancellationToken cancellationToken)
        {
            if (!_clusterClient.IsInitialized) return default(T);

            _handler ??= _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            try
            {
                var response = await _handler.GetStateAsync(typeof(T).Name, key, etag?.ToString());

                return JsonSerializer.Deserialize<T>(response, _options);
            }
            catch (Exception)
            {
                _handler = null;

                throw;
            }
        }

        public async ValueTask<IReadOnlyList<T>?> GetBulkAsync(IEnumerable<(string key, object? etag)> keys, CancellationToken cancellationToken)
        {
            var result = new List<T>();

            foreach (var key in keys)
            {
                var state = await GetAsync(key.key, key.etag, cancellationToken);

                if (!(state is null)) result.Add(state);
            }

            return result;
        }

        public ValueTask<T?> GetOrCreateAsync(string key, object? etag, Func<ValueTask<T>> factory, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (ttlSeconds > 0) throw new NotSupportedException($"OrleansStateStore is not support ttl");

            return GetOrCreateAsync(key, etag, factory, cancellationToken);
        }

        public async ValueTask<T?> GetOrCreateAsync(string key, object? etag, Func<ValueTask<T>> factory, CancellationToken cancellationToken)
        {
            if (!_clusterClient.IsInitialized) return default(T);

            _handler ??= _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            try
            {
                var item = await factory.Invoke();

                var payload = JsonSerializer.SerializeToUtf8Bytes(item, _options);

                var response = await _handler.GetOrCreateStateAsync(typeof(T).Name, key, etag?.ToString(), payload);

                return JsonSerializer.Deserialize<T>(response, _options);
            }
            catch (Exception)
            {
                _handler = null;

                throw;
            }
        }

        public ValueTask RemoveAsync(string key, object? etag, CancellationToken cancellationToken)
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            _handler ??= _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            try
            {
                return _handler.RemoveStateAsync(typeof(T).Name, key, etag?.ToString());
            }
            catch (Exception)
            {
                _handler = null;

                throw;
            }
        }

        public ValueTask SetAsync(string key, object? etag, T item, int ttlSeconds, CancellationToken cancellationToken)
        {
            if (ttlSeconds > 0) throw new NotSupportedException($"OrleansStateStore is not support ttl");

            return SetAsync(key, etag, item, cancellationToken);
        }

        public ValueTask SetAsync(string key, object? etag, T item, CancellationToken cancellationToken)
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            _handler ??= _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            try
            {
                var payload = JsonSerializer.SerializeToUtf8Bytes(item, _options);

                return _handler.SetStateAsync(typeof(T).Name, key, etag?.ToString(), payload);
            }
            catch (Exception)
            {
                _handler = null;

                throw;
            }
        }
    }
}
