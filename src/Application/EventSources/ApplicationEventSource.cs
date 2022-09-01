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

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.EventSources
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

        public async ValueTask PublishAsync<TDomainEvent>(TDomainEvent domainEvent, DateTimeOffset timestamp, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            var eventName = domainEvent.GetType().Name;

            var logger = _logFactory.CreateLogger(eventName);

            var timer = new Stopwatch();

            try
            {
                timer.Start();

                domainEvent.Publising(timestamp);

                await PublishEventNotification(domainEvent, cancellationToken);

                await PublishToEventbus(domainEvent, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogTrace(ex, "Publish event invalid exception: {Name}", eventName);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogTrace(ex, "Publish event canceled exception: {Name}", eventName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Publish event unhandled exception: {Name}", eventName);
            }
            finally
            {
                timer.Stop();

                var elapsedMilliseconds = timer.ElapsedMilliseconds;

                if (elapsedMilliseconds > 2000)
                {
                    logger.LogDebug("Publish event long running: {Name} ({ElapsedMilliseconds} milliseconds)",
                        eventName, elapsedMilliseconds);
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

        private ValueTask PublishToEventbus<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            if (_eventBus is null) return new ValueTask();

            if (domainEvent is BufferedEvent bufferedEvent && bufferedEvent.BufferCount > 0)
            {
                _bulkEvent.BufferPublish(domainEvent.Topic, bufferedEvent);

                return new ValueTask();
            }

            return _eventBus.PublishAsync(domainEvent.Topic, domainEvent, cancellationToken);
        }
    }
}
