using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class EventBufferService
    {
        private readonly ILogger<EventBufferService> _logger;
        private readonly EventBufferServiceOptions _opt;
        private readonly IServiceProvider _services;

        public EventBufferService(
            ILogger<EventBufferService> logger,
            IOptions<EventBufferServiceOptions> opt,
            IServiceProvider services)
        {
            _logger = logger;
            _opt = opt.Value;
            _services = services;

            _buffer
            .Buffer(_opt.BufferTime, _opt.BufferCount)
                .Subscribe(async buffer =>
                {
                    if (!buffer.Any()) return;

                    try
                    {
                        var send = buffer.ToDictionary(b => b.Item1, b => b.Item2);

                        using var scope = _services.CreateScope();

                        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                        await eventBus.PublishAsync(_opt.Topic, send.Values);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "EventBufferService Unhandled Exception - {@Buffer}", buffer);
                    }
                });

            _logger.LogInformation("{Class} Initialized - {@Optiopns}",
                nameof(EventBufferService), _opt);
        }

        private readonly Subject<Tuple<string, object>> _buffer = new();

        public void BufferPublish(string bufferKey, object notification)
        {
            _buffer.OnNext(Tuple.Create(bufferKey, notification));
        }
    }
}
