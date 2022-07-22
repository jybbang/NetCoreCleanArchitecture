using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class BulkEventService
    {
        private readonly ILogger<BulkEventService> _logger;
        private readonly IServiceProvider _services;

        private readonly ConcurrentDictionary<string, Subject<object>> _buffers = new ConcurrentDictionary<string, Subject<object>>();

        public BulkEventService(
            ILogger<BulkEventService> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;
        }

        public void BufferPublish<TDomainEvent>(string topic, TDomainEvent domainEvent) where TDomainEvent : BufferedEvent
        {
            if (_buffers.TryGetValue(topic, out var buffer))
            {
                buffer.OnNext(domainEvent);
            }
            else
            {
                buffer = new Subject<object>();

                if (_buffers.TryAdd(topic, buffer))
                {
                    var observer = buffer
                            .Buffer(domainEvent.BufferCount);

                    if (domainEvent.BufferTime > TimeSpan.Zero)
                    {
                        observer = observer
                            .Merge(buffer
                            .Buffer(domainEvent.BufferTime));
                    }

                    observer
                        .Where(events => events.Any())
                        .Subscribe(async events =>
                        {
                            try
                            {
                                using var cts = new CancellationTokenSource(domainEvent.PublishTimeout);

                                using var scope = _services.CreateScope();

                                var e = new BulkEvent(topic)
                                {
                                    DomainEvents = events
                                };

                                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                                await eventBus.PublishAsync(topic, e, cts.Token);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "BulkEventService unhandled exception: {Topic}", topic);
                            }
                        });

                    buffer.OnNext(domainEvent);
                }
            }
        }
    }
}
