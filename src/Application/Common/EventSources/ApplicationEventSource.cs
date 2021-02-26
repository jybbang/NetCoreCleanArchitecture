using MediatR;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class ApplicationEventSource : IApplicationEventSource
    {
        private readonly ILoggerFactory _logFactory;
        private readonly IDomainEventSource _eventSource;
        private readonly IPublisher _mediator;
        private long _published;

        public long Published => _published;

        public ApplicationEventSource(ILoggerFactory logFactory,
            IDomainEventSource eventSource,
            IPublisher mediator)
        {
            _logFactory = logFactory;
            _eventSource = eventSource;
            _mediator = mediator;
        }

        public async Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : DomainEvent
        {
            var logger = _logFactory.CreateLogger<TDomainEvent>();

            // logging
            var domainEventName = typeof(TDomainEvent).Name;

            logger.LogDebug("Publishing Domain Event: {Name} - {@Event}", domainEventName, domainEvent);

            await PublishWithPerformance(domainEvent, logger, cancellationToken);

            Interlocked.Increment(ref _published);
        }

        private async Task PublishWithPerformance<TDomainEvent>(TDomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken) where TDomainEvent : DomainEvent
        {
            var timer = new Stopwatch();

            try
            {
                timer.Start();

                await PublishEventNotification(domainEvent, cancellationToken);

                await _eventSource.PublishEvent(domainEvent, cancellationToken);
            }
            finally
            {
                timer.Stop();

                var elapsedMilliseconds = timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500)
                {
                    var eventName = typeof(TDomainEvent).Name;

                    logger.LogWarning("Publishing Long Running Domain Event: {Name} ({ElapsedMilliseconds} milliseconds) - {@Event}",
                        eventName, elapsedMilliseconds, domainEvent);
                }
            }
        }

        private Task PublishEventNotification<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : DomainEvent
        {
            var notification = new DomainEventNotification<TDomainEvent>(domainEvent);

            return _mediator.Publish(notification, cancellationToken);
        }
    }
}
