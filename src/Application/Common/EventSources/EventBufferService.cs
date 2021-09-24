using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Application.Common.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Domain.Common;

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
                        using var scope = _services.CreateScope();

                        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                        await eventBus.PublishAsync(_opt.Topic, buffer);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "EventBufferService Unhandled Exception - {@Buffer}", buffer);
                    }
                });

            _logger.LogInformation("{Class} Initialized - {@Optiopns}",
                nameof(EventBufferService), _opt);
        }

        private readonly Subject<object> _buffer = new();

        public void BufferPublish(object notification)
        {
            _buffer.OnNext(notification);
        }
    }
}
