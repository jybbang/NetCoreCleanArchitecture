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

        private uint _appPublished;
        private uint _eventbusPublished;

        public ApplicationEventSource(
            ILoggerFactory logFactory,
            IEventBus eventBus,
            IPublisher mediator)
        {
            _logFactory = logFactory;
            _eventBus = eventBus;
            _mediator = mediator;
        }

        public uint ApplicationPublished => _appPublished;
        public uint EventbusPublished => _eventbusPublished;

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

                    logger.LogWarning("Publishing Long Running Event: {Name} ({ElapsedMilliseconds} milliseconds) - {@Event}",
                        eventName, elapsedMilliseconds, domainEvent);
                }
            }
        }

        private async Task PublishToEventbus(DomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
        {
            try
            {
                if (domainEvent.CanPublishToEventBus)
                {
                    await _eventBus.PublishEvent(domainEvent.Topic, domainEvent, cancellationToken);

                    Interlocked.Increment(ref _eventbusPublished);
                }

                Interlocked.Increment(ref _appPublished);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception.");
            }
        }

        private Task PublishEventNotification(DomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken)
        {
            var notification = (INotification)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);

            //var notification = new DomainEventNotification<TDomainEvent>(domainEvent);

            return _mediator.Publish(notification, cancellationToken);
        }
    }
}
