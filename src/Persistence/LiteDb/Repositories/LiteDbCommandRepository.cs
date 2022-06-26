// 
// Copyright (c) Rafael Garcia <https://github.com/rafaelfgx>
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

using LiteDB;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Persistence.LiteDb.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Persistence.LiteDbDb.Repositories
{
    public class LiteDbCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly LiteDbContext _context;
        private readonly ILiteCollection<TEntity> _collection;

        public LiteDbCommandRepository(LiteDbContext context)
        {
            _context = context;

            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public void Add(TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.Insert(item);
        }

        public Task AddAsync(TEntity item, CancellationToken cancellationToken)
        {
            Add(item);

            return Task.CompletedTask;
        }

        public void AddRange(IReadOnlyList<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            _collection.InsertBulk(items);
        }

        public Task AddRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken)
        {
            AddRange(items);

            return Task.CompletedTask;
        }

        public void Remove(Guid key)
        {
            var item = _collection.FindById(key);

            if (item is null) return;

            _context.AddTracking(item, UpdatePartialAsync);

            _collection.Delete(key);
        }

        public Task RemoveAsync(Guid key, CancellationToken cancellationToken)
        {
            Remove(key);

            return Task.CompletedTask;
        }

        public void Update(TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.Update(item.Id, item);
        }

        public Task UpdateAsync(TEntity item, CancellationToken cancellationToken)
        {
            Update(item);

            return Task.CompletedTask;
        }

        public void UpdateRange(IReadOnlyList<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            foreach (var item in items)
            {
                Update(item);
            }
        }

        public Task UpdateRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken)
        {
            UpdateRange(items);

            return Task.CompletedTask;
        }

        private void UpdatePartial(Guid key, object item)
        {
            _collection.Upsert(key, (TEntity)item);
        }

        private Task UpdatePartialAsync(Guid key, object item, CancellationToken cancellationToken)
        {
            UpdatePartial(key, item);

            return Task.CompletedTask;
        }
    }
}
