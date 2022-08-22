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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.LiteDb.Common;

namespace NetCoreCleanArchitecture.Infrastructure.LiteDb.Repositories
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

        public ValueTask AddAsync(TEntity item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            Add(item);

            return new ValueTask();
        }

        public void AddRange(IReadOnlyList<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            _collection.InsertBulk(items);
        }

        public ValueTask AddRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            AddRange(items);

            return new ValueTask();
        }

        public void Remove(Guid key)
        {
            var item = _collection.FindById(key);

            if (item is null) return;

            _collection.Delete(key);
        }

        public ValueTask RemoveAsync(Guid key, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            Remove(key);

            return new ValueTask();
        }

        public void RemoveAll()
        {
            _collection.DeleteAll();
        }

        public ValueTask RemoveAllAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            RemoveAll();

            return new ValueTask();
        }

        public void Update(TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.Update(item.Id, item);
        }

        public ValueTask UpdateAsync(TEntity item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            Update(item);

            return new ValueTask();
        }

        public void UpdateRange(IReadOnlyList<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            foreach (var item in items)
            {
                Update(item);
            }
        }

        public ValueTask UpdateRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            UpdateRange(items);

            return new ValueTask();
        }

        private void UpdatePartial(Guid key, object item)
        {
            _collection.Upsert(key, (TEntity)item);
        }

        private ValueTask UpdatePartialAsync(Guid key, object item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            UpdatePartial(key, item);

            return new ValueTask();
        }
    }
}
