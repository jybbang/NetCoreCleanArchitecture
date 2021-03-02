using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class ApplicationEventSource : IApplicationEventSource
    {
        private const string APP_NAME_SECTION = "AppName";

        private readonly ILoggerFactory _logFactory;
        private readonly IEventBus _eventBus;
        private readonly IPublisher _mediator;
        private readonly string appName;

        private long _appPublished;
        private long _infraPublished;

        public ApplicationEventSource(
            ILoggerFactory logFactory,
            IEventBus eventBus,
            IPublisher mediator,
            IConfiguration configuration)
        {
            _logFactory = logFactory;
            _eventBus = eventBus;
            _mediator = mediator;

            appName = configuration.GetValue<string>(APP_NAME_SECTION);
        }


        public long ApplicationPublished => _appPublished;
        public long InfrastructurePublished => _infraPublished;

        public async Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : DomainEvent
        {
            var logger = _logFactory.CreateLogger<TDomainEvent>();

            // logging
            var domainEventName = typeof(TDomainEvent).Name;

            logger.LogDebug("Publishing Event: {Name} - {@Event}", domainEventName, domainEvent);

            await PublishWithPerformance(domainEvent, logger, cancellationToken);
        }

        private async Task PublishWithPerformance<TDomainEvent>(TDomainEvent domainEvent, ILogger logger, CancellationToken cancellationToken) where TDomainEvent : DomainEvent
        {
            var timer = new Stopwatch();

            try
            {
                timer.Start();

                await PublishEventNotification(domainEvent, cancellationToken);

                if (domainEvent.CanPublishWithEventBus)
                {
                    var topic = string.IsNullOrEmpty(appName) ? domainEvent.Topic : $"{appName}/{domainEvent.Topic}";

                    await _eventBus.PublishEvent(topic, domainEvent, cancellationToken);

                    Interlocked.Increment(ref _infraPublished);
                }

                Interlocked.Increment(ref _appPublished);
            }
            finally
            {
                timer.Stop();

                var elapsedMilliseconds = timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 500)
                {
                    var eventName = typeof(TDomainEvent).Name;

                    logger.LogWarning("Publishing Long Running Event: {Name} ({ElapsedMilliseconds} milliseconds) - {@Event}",
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
