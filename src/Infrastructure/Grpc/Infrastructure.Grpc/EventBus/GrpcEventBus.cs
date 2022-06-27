using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Options;
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
    public class GrpcEventBus : EventBusRpc.EventBusRpcBase, IEventBus, IDisposable
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>> _subscribes
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>>();

        private readonly ConcurrentDictionary<string, ManualResetEventSlim> _unsubscribes
            = new ConcurrentDictionary<string, ManualResetEventSlim>();

        private readonly Server _server;

        public GrpcEventBus(IOptions<GrpcOptions> options)
        {
            _server = new Server
            {
                Services = { EventBusRpc.BindService(this) },
                Ports = { new ServerPort(options.Value.Host, options.Value.Port, ServerCredentials.Insecure) }
            };

            _server.Start();
        }

        public void Dispose()
        {
            _subscribes.Clear();

            _unsubscribes.Values.ToList().ForEach(x => x.Dispose());

            _unsubscribes.Clear();
        }

        public Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (_subscribes.TryGetValue(topic, out var subscribes))
            {
                var eventBytes = JsonSerializer.SerializeToUtf8Bytes(message);

                var response = new SubscribeResponse
                {
                    Topic = topic,
                    DomainEvent = Google.Protobuf.ByteString.CopyFrom(eventBytes)
                };

                subscribes.Values.ToList()
                    .ForEach(async subscribe => await subscribe.WriteAsync(response));
            }

            return Task.CompletedTask;
        }

        public override Task Subscribe(SubscribeRequest request, IServerStreamWriter<SubscribeResponse> responseStream, ServerCallContext context)
        {
            var entryKey = $"{context.Host}/{request.Topic}";

            Unsubscribing(request.Topic, entryKey);

            var mrs = new ManualResetEventSlim();

            if (_unsubscribes.TryAdd(entryKey, mrs))
            {
                var subscribes = _subscribes.GetOrAdd(request.Topic, _ => new ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>());

                subscribes.TryAdd(entryKey, responseStream);

                mrs.Wait();
            }

            return Task.CompletedTask;
        }

        public override Task<UnsubscribeResponse> Unsubscribe(UnsubscribeRequest request, ServerCallContext context)
        {
            var entryKey = $"{context.Host}/{request.Topic}";

            var isSuccess = Unsubscribing(request.Topic, entryKey);

            return Task.FromResult(new UnsubscribeResponse
            {
                Topic = request.Topic,
                IsSuccess = isSuccess
            });
        }

        private bool Unsubscribing(string topic, string key)
        {
            if (_unsubscribes.TryRemove(key, out var mrs))
            {
                if (_subscribes.TryGetValue(topic, out var subscribe))
                {
                    subscribe.TryRemove(key, out _);
                }

                if (subscribe.IsEmpty) _subscribes.TryRemove(topic, out _);

                mrs.Dispose();

                return true;
            }

            return false;
        }
    }
}
