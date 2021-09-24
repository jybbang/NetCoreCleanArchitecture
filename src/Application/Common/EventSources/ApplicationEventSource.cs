using MediatR;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class ApplicationEventSource : IApplicationEventSource
    {
        private readonly ILoggerFactory _logFactory;
        private readonly IEventBus _eventBus;
        private readonly IPublisher _mediator;
        private readonly EventBufferService _eventBuffer;

        public ApplicationEventSource(
            ILoggerFactory logFactory,
            IEventBus eventBus,
            IPublisher mediator,
            EventBufferService eventBuffer)
        {
            _logFactory = logFactory;
            _eventBus = eventBus;
            _mediator = mediator;
            _eventBuffer = eventBuffer;
        }

        public async Task Publish<T>(T domainEvent, CancellationToken cancellationToken = default) where T : DomainEvent
        {
            // logging
            var eventName = domainEvent.GetType().Name;

            var logger = _logFactory.CreateLogger(eventName);

            logger.LogDebug("Publishing Event: {Name} - {@Event}", eventName, domainEvent);

            await PublishWithPerformance(domainEvent, logger, cancellationToken);
        }

        private async Task PublishWithPerformance<T>(T domainEvent, ILogger logger, CancellationToken cancellationToken) where T : DomainEvent
        {
            var timer = new Stopwatch();

            try
            {
                timer.Start();

                await PublishEventNotification(domainEvent, logger, cancellationToken);

                await PublishToEventbus(domainEvent, logger, cancellationToken);
            }
            catch (Exception ex)
            {
                var eventName = domainEvent.GetType().Name;

                logger.LogError(ex, "Publishing Event: Unhandled Exception {Name} - {@Event}", eventName, domainEvent);
            }
            finally
            {
                timer.Stop();

                var elapsedMilliseconds = timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500)
                {
                    var eventName = domainEvent.GetType().Name;

                    logger.LogWarning("Publishing Event: Long Running {Name} ({ElapsedMilliseconds} milliseconds) - {@Event}",
                        eventName, elapsedMilliseconds, domainEvent);
                }
            }
        }

        private Task PublishEventNotification<T>(T domainEvent, ILogger logger, CancellationToken cancellationToken) where T : DomainEvent
        {
            var notification = (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);

            return _mediator.Publish(notification, cancellationToken);
        }

        private async Task PublishToEventbus<T>(T domainEvent, ILogger logger, CancellationToken cancellationToken) where T : DomainEvent
        {
            if (!domainEvent.CanPublishToEventBus) return;

            var notification = Convert.ChangeType(domainEvent, domainEvent.GetType());

            if (domainEvent.CanBuffered)
            {
                _eventBuffer.BufferPublish(notification);
                return;
            }

            await _eventBus.PublishAsync(domainEvent.Topic, notification, cancellationToken);
        }
    }
}
