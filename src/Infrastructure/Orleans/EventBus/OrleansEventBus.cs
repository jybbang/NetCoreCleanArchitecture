using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Domain;
using NetCoreCleanArchitecture.Infrastructure.Orleans.Common.Contracts;
using Orleans;
using System.Text.Json;

namespace NetCoreCleanArchitecture.Infrastructure.Orleans.EventBus
{
    public class OrleansEventBus : IEventBus
    {
        private readonly IClusterClient _clusterClient;

        private INetCleanHandlerGrain? _handler;

        public OrleansEventBus(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        public ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (!_clusterClient.IsInitialized) return new ValueTask();

            _handler ??= _clusterClient.GetGrain<INetCleanHandlerGrain>(Guid.Empty);

            try
            {
                var options = new JsonSerializerOptions()
                {
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                    UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode,
                };

                var payload = JsonSerializer.SerializeToUtf8Bytes(message, message.GetType(), options);

                if(payload is null) return new ValueTask();

                return _handler.HandleEventAsync(topic, payload, message.Timestamp);
            }
            catch (Exception)
            {
                _handler = null;

                throw;
            }
        }
    }
}
