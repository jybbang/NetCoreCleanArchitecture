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

        public ApplicationEventSource(
            ILoggerFactory logFactory,
            IEventBus eventBus,
            IPublisher mediator)
        {
            _logFactory = logFactory;
            _eventBus = eventBus;
            _mediator = mediator;
        }

        public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // logging
            var eventName = domainEvent.GetType().Name;

            var logger = _logFactory.CreateLogger(eventName);

            logger.LogDebug("Publishing Event: {Name} - {@Event}", eventName, domainEvent);

            await PublishWithPerformance(domainEvent, logger, cancellationToken);
        }

        private async Task PublishWithPerformance(DomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var timer = new Stopwatch();

            try
            {
                timer.Start();

                await PublishEventNotification(domainEvent, logger, cancellationToken);

                await PublishToEventbus(domainEvent, logger, cancellationToken);
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

        private Task PublishEventNotification(DomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var notification = (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);

            return _mediator.Publish(notification, cancellationToken);
        }

        private async Task PublishToEventbus(DomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
        {
            try
            {
                if (domainEvent.CanPublishToEventBus)
                {
                    await _eventBus.PublishAsync(domainEvent.Topic, domainEvent, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                var eventName = domainEvent.GetType().Name;

                logger.LogError(ex, "Publishing Event: PublishToEventbus Unhandled Exception {Name} - {@Event}", eventName, domainEvent);
            }
        }
    }
}
