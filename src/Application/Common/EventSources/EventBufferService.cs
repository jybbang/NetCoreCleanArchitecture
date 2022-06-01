using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class EventBufferService
    {
        private readonly ILogger<EventBufferService> _logger;
        private readonly IServiceProvider _services;

        private readonly ConcurrentDictionary<string, Subject<object>> _buffers = new();

        public EventBufferService(
            ILogger<EventBufferService> logger,
            IServiceProvider services)
        {
            _logger = logger;
            _services = services;

            _logger.LogInformation("{Class} Initialized", nameof(EventBufferService));
        }

        public void BufferPublish<TDomainEvent>(string topic, TDomainEvent domainEvent) where TDomainEvent : BufferedDomainEvent
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

                    observer.Subscribe(async events =>
                    {
                        if (!events.Any()) return;

                        try
                        {
                            using var scope = _services.CreateScope();

                            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                            var cts = new CancellationTokenSource(domainEvent.PublishTimeout);

                            await eventBus.PublishAsync(topic, events, cts.Token);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "EventBufferService Unhandled Exception - {@Topic}", topic);
                        }
                    });

                    buffer.OnNext(domainEvent);
                }
            }
        }
    }
}
