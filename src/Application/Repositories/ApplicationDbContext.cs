﻿// 
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Application.Identities;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Repositories
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

        public ICommandRepository CommandSet(Type entityType)
            => _unitOfWork.CommandSet(entityType);

        public IQueryRepository QuerySet(Type entityType)
            => _unitOfWork.QuerySet(entityType);

        public ICommandRepository<TEntity> CommandSet<TEntity>() where TEntity : BaseEntity
            => _unitOfWork.CommandSet<TEntity>();

        public IQueryRepository<TEntity> QuerySet<TEntity>() where TEntity : BaseEntity
            => _unitOfWork.QuerySet<TEntity>();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var timestamp = DateTimeOffset.UtcNow;

            var changedEntities = _unitOfWork.ChangeTracking();

            UpdateAuditable(changedEntities, timestamp, cancellationToken);

            var changes = await _unitOfWork.SaveChangesAsync(cancellationToken);

            await DispatchEvents(changedEntities, timestamp, cancellationToken);

            return changes;
        }

        private void UpdateAuditable(in IEnumerable<BaseEntity> changedEntities, DateTimeOffset timestamp, CancellationToken cancellationToken = default)
        {
            foreach (var entity in changedEntities)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (entity is BaseAuditableEntity auditableEntity)
                {
                    if (auditableEntity.CreatedAt == default)
                    {
                        auditableEntity.CreatedBy = _currentUser.UserId;
                        auditableEntity.CreatedAt = timestamp;
                    }

                    auditableEntity.LastModifiedBy = _currentUser.UserId;
                    auditableEntity.LastModifiedAt = timestamp;
                }
            }
        }

        private async ValueTask DispatchEvents(IEnumerable<BaseEntity> changedEntities, DateTimeOffset timestamp, CancellationToken cancellationToken)
        {
            var domainEventsList = changedEntities
                .Select(e => e.GetDomainEvents());

            foreach (var domainEvents in domainEventsList)
            {
                while (domainEvents.TryTake(out var domainEvent))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await _eventSource.PublishAsync(domainEvent, timestamp, cancellationToken);
                }
            }
        }
    }
}
