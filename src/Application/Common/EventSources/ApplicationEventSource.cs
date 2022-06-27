// 
// Copyright (c) 2019 Jason Taylor <https://github.com/jasontaylordev>
// 
// All rights reserved.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

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
        private readonly BulkEventService _bulkEvent;

        public ApplicationEventSource(
            ILoggerFactory logFactory,
            IEventBus eventBus,
            IPublisher mediator,
            BulkEventService eventBuffer)
        {
            _logFactory = logFactory;
            _eventBus = eventBus;
            _mediator = mediator;
            _bulkEvent = eventBuffer;
        }

        public async Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, DateTimeOffset timestamp, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            var eventName = domainEvent.GetType().Name;

            var logger = _logFactory.CreateLogger(eventName);

            var timer = new Stopwatch();

            try
            {
                timer.Start();

                logger.LogTrace("Publish event: {Name} - {@Event}", eventName, domainEvent);

                domainEvent.Publising(timestamp);

                await PublishEventNotification(domainEvent, cancellationToken);

                await PublishToEventbus(domainEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Publish event unhandled exception: {Name} - {@Event}", eventName, domainEvent);
            }
            finally
            {
                timer.Stop();

                var elapsedMilliseconds = timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 1000)
                {
                    logger.LogWarning("Publish event long running: {Name} ({ElapsedMilliseconds} milliseconds) - {@Event}",
                        eventName, elapsedMilliseconds, domainEvent);
                }
            }
        }

        private Task PublishEventNotification<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            var notification = (INotification?)Activator.CreateInstance(
                typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);

            return notification is null
                ? Task.CompletedTask
                : _mediator.Publish(notification, cancellationToken);
        }

        private async Task PublishToEventbus<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (_eventBus is null) return;

            if (domainEvent is BufferedEvent bufferedEvent && bufferedEvent.BufferCount > 0)
            {
                _bulkEvent.BufferPublish(domainEvent.Topic, bufferedEvent);

                return;
            }

            await _eventBus.PublishAsync(domainEvent.Topic, domainEvent, cancellationToken);
        }
    }
}
