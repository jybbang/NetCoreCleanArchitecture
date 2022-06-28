using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Rpcs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Grpc.EventBus
{
    public class GrpcEventBus : EventBusRpc.EventBusRpcBase, IEventBus
    {
        private readonly GrpcEventBusRpc _server;

        public GrpcEventBus(GrpcEventBusRpc server)
        {
            _server = server;
        }

        public async Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (_server.TryGetSubscribes(topic, out var subscribes))
            {
                var eventBytes = JsonSerializer.SerializeToUtf8Bytes(message);

                var response = new SubscribeResponse
                {
                    Topic = topic,
                    DomainEvent = Google.Protobuf.ByteString.CopyFrom(eventBytes)
                };

                foreach (var subscribe in subscribes!.Values.ToList())
                {
#if NETCOREAPP
                    await subscribe.WriteAsync(response, cancellationToken);
#else
                    if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                    await subscribe.WriteAsync(response);
#endif
                }
            }
        }
    }
}
