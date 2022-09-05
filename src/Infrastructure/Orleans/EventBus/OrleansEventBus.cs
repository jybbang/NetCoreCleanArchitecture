using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts;
using Orleans;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.EventBus
{
    public class OrleansEventBus : IEventBus
    {
        private readonly IClusterClient _clusterClient;

        private static JsonSerializerOptions options = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode,
        };

        public OrleansEventBus(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public ValueTask PublishAsync<TDomainEvent>(TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            var payload = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), options);

            if (payload is null) return new ValueTask();

            var handler = _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            return handler.HandleEventAsync(message.Topic, payload);
        }
    }
}
