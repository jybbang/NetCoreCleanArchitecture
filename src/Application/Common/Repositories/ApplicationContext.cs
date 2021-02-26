using DaprCleanArchitecture.Application.Common.EventSources;
using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Domain.Common;
using DaprCleanArchitecture.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.Repositories
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationEventSource _eventSource;
        private readonly ICurrentUserService _currentUser;
        private readonly IDateTimeCache _dateTimeCache;

        public ApplicationContext(
            IUnitOfWork unitOfWork,
            IApplicationEventSource eventSource,
            ICurrentUserService currentUser,
            IDateTimeCache dateTimeCache,
            ICommandRepository<TodoItem> todoItemCommands,
            IQueryRepository<TodoItem> todoItemQueries)
        {
            _unitOfWork = unitOfWork;
            _eventSource = eventSource;
            _currentUser = currentUser;
            _dateTimeCache = dateTimeCache;
            TodoItemCommands = todoItemCommands;
            TodoItemQueries = todoItemQueries;
        }

        public ICommandRepository<TodoItem> TodoItemCommands { get; }

        public IQueryRepository<TodoItem> TodoItemQueries { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var timestamp = _dateTimeCache.Now;

            var changedEntities = _unitOfWork.ChangeTracking();

            UpdateAuditable(changedEntities, timestamp);

            var eventsToDispatch = GetEventsToDispatch(changedEntities, timestamp);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            await DispatchEvents(eventsToDispatch, cancellationToken);

            return result;
        }

        private void UpdateAuditable(IEnumerable<Entity> changedEntities, DateTimeOffset timestamp)
        {
            foreach (var entity in changedEntities)
            {
                if (entity is not IAuditableEntity auditableEntity) return;

                if (auditableEntity.CreatedAt == default)
                {
                    auditableEntity.CreateUserId = _currentUser.UserId;
                    auditableEntity.CreatedAt = timestamp;
                }

                auditableEntity.UpdateUserId = _currentUser.UserId;
                auditableEntity.UpdatedAt = timestamp;

                break;
            }
        }

        private IProducerConsumerCollection<DomainEvent> GetEventsToDispatch(IEnumerable<Entity> changedEntities, DateTimeOffset timestamp)
        {
            var eventsToDispatch = new ConcurrentQueue<DomainEvent>();

            var domainEventsList = changedEntities
                .Where(e => e is EntityHasDomainEvent ehde && ehde.DomainEvents.Any())
                .Select(e => e as EntityHasDomainEvent)
                .Select(e => e.DomainEvents);

            foreach (var domainevents in domainEventsList)
            {
                while (domainevents.Count > 0)
                {
                    if (domainevents.TryTake(out var domainEvent))
                    {
                        domainEvent.Publising(timestamp);

                        eventsToDispatch.Enqueue(domainEvent);
                    }
                }
            }

            return eventsToDispatch;
        }

        private async Task DispatchEvents(IProducerConsumerCollection<DomainEvent> events, CancellationToken cancellationToken = default)
        {
            while (events.Count > 0)
            {
                if (events.TryTake(out var domainEvent))
                {
                    await _eventSource.Publish(domainEvent, cancellationToken);
                }
            }
        }
    }
}
