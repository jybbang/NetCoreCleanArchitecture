using NetCoreCleanArchitecture.Application.Common.EventSources;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Repositories
{
    public class ApplicationDbContext : IApplicationDbContext
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApplicationEventSource _eventSource;
        private readonly ICurrentUserService _currentUser;

        public ApplicationDbContext(
            IUnitOfWork unitOfWork,
            IApplicationEventSource eventSource,
            ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _eventSource = eventSource;
            _currentUser = currentUser;
        }

        public ICommandRepository<TEntity> CommandSet<TEntity>() where TEntity : Entity
            => _unitOfWork.CommandSet<TEntity>();

        public IQueryRepository<TEntity> QuerySet<TEntity>() where TEntity : Entity
            => _unitOfWork.QuerySet<TEntity>();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var timestamp = DateTimeOffset.UtcNow;

            var changedEntities = _unitOfWork.ChangeTracking();

            UpdateAuditable(changedEntities, timestamp, cancellationToken);

            var eventsToDispatch = GetEventsToDispatch(changedEntities, cancellationToken);

            var result = await _unitOfWork.SaveChangesAsync(cancellationToken);

            await DispatchEvents(eventsToDispatch, timestamp, cancellationToken);

            return result;
        }

        private void UpdateAuditable(IEnumerable<Entity> changedEntities, DateTimeOffset timestamp, CancellationToken cancellationToken)
        {
            foreach (var entity in changedEntities)
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

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

        private IProducerConsumerCollection<DomainEvent> GetEventsToDispatch(IEnumerable<Entity> changedEntities, CancellationToken cancellationToken)
        {
            var eventsToDispatch = new ConcurrentQueue<DomainEvent>();

            var domainEventsList = changedEntities
                .Where(e => e is EntityWithDomainEvent entity && entity.DomainEvents.Any())
                .Select(e => e as EntityWithDomainEvent)
                .Select(e => e.DomainEvents);

            foreach (var domainevents in domainEventsList)
            {
                while (domainevents.TryTake(out var domainEvent))
                {
                    if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                    eventsToDispatch.Enqueue(domainEvent);
                }
            }

            return eventsToDispatch;
        }

        private async Task DispatchEvents(IProducerConsumerCollection<DomainEvent> events, DateTimeOffset timestamp, CancellationToken cancellationToken)
        {
            while (events.TryTake(out var domainEvent))
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                await _eventSource.PublishAsync(domainEvent, timestamp, cancellationToken);
            }
        }
    }
}
