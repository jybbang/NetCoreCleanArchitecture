using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Options;
using NetCoreCleanArchitecture.Infrastructure.Grpc.EventBus;

namespace NetCoreCleanArchitecture.Infrastructure.Grpc.Common.Rpcs
{
    public class GrpcEventBusRpc : EventBusRpc.EventBusRpcBase, IDisposable
    {
        private readonly Server _server;
        private bool _isDisposed;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>> _subscribes
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>>();

        private readonly ConcurrentDictionary<string, ManualResetEventSlim> _unsubscribes
            = new ConcurrentDictionary<string, ManualResetEventSlim>();

        public GrpcEventBusRpc(IOptions<GrpcOptions> options)
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
            if (!_isDisposed)
            {
                _subscribes.Clear();

                _unsubscribes.Values.ToList().ForEach(x => x.Dispose());

                _unsubscribes.Clear();

                _server.ShutdownAsync().ConfigureAwait(false);

                GC.SuppressFinalize(this);

                _isDisposed = true;
            }
        }

        public override Task Subscribe(SubscribeRequest request, IServerStreamWriter<SubscribeResponse> responseStream, ServerCallContext context)
        {
            var entryKey = $"{context.Host}/{request.Topic}";

            Unsubscribing(request.Topic, entryKey);

            var mrs = new ManualResetEventSlim();

            if (_unsubscribes.TryAdd(entryKey, mrs))
            {
                try
                {
                    var subscribes = _subscribes.GetOrAdd(request.Topic, _ => new ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>());

                    subscribes.TryAdd(entryKey, responseStream);

                    mrs.Wait(context.CancellationToken);
                }
                finally
                {
                    Unsubscribing(request.Topic, entryKey);
                }
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

        internal bool TryGetSubscribes(string topic, out ConcurrentDictionary<string, IServerStreamWriter<SubscribeResponse>>? subscribes)
        {
            return _subscribes.TryGetValue(topic, out subscribes);
        }

        private bool Unsubscribing(string topic, in string key)
        {
            if (_unsubscribes.TryRemove(key, out var mrs))
            {
                try
                {
                    if (_subscribes.TryGetValue(topic, out var subscribe))
                    {
                        subscribe.TryRemove(key, out _);

                        if (subscribe.IsEmpty) _subscribes.TryRemove(topic, out _);
                    }

                    mrs.Set();

                    return true;
                }
                finally
                {
                    mrs.Dispose();
                }
            }

            return false;
        }
    }
}
