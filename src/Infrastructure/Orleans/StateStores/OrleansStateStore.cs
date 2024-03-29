﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.StateStores;
using NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts;
using Orleans;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.StateStores
{
    public class OrleansStateStore<T> : IStateStore<T> where T : class
    {
        private readonly IClusterClient _clusterClient;

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
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

        public async ValueTask<T?> GetAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (!_clusterClient.IsInitialized) return default(T);

            var handler = _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            var response = await handler.GetStateAsync(typeof(T).Name, key, etag?.ToString());

            return JsonSerializer.Deserialize<T>(response, _serializerOptions);
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

        public ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (ttlSeconds > 0) throw new NotSupportedException($"OrleansStateStore is not supporting ttl action");

            return GetOrCreateAsync(key, factory, etag, cancellationToken);
        }

        public async ValueTask<T?> GetOrCreateAsync(string key, Func<ValueTask<T>> factory, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (!_clusterClient.IsInitialized) return default(T);

            var item = await factory.Invoke();

            var payload = JsonSerializer.SerializeToUtf8Bytes(item, _serializerOptions);

            if (payload is null) return default(T);

            var handler = _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            var response = await handler.GetOrCreateStateAsync(typeof(T).Name, key, etag?.ToString(), payload);

            return JsonSerializer.Deserialize<T>(response, _serializerOptions);
        }

        public ValueTask RemoveAsync(string key, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            var handler = _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            return handler.RemoveStateAsync(typeof(T).Name, key, etag?.ToString());
        }

        public ValueTask SetAsync(string key, T item, int ttlSeconds, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (ttlSeconds > 0) throw new NotSupportedException($"OrleansStateStore is not supporting ttl action");

            return SetAsync(key, item, etag, cancellationToken);
        }

        public ValueTask SetAsync(string key, T item, object? etag = default, CancellationToken cancellationToken = default)
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            var payload = JsonSerializer.SerializeToUtf8Bytes(item, _serializerOptions);

            if (payload is null) return new ValueTask();

            var handler = _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            return handler.SetStateAsync(typeof(T).Name, key, etag?.ToString(), payload);
        }
    }
}
