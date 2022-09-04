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
            _context.AddTracking(item);

            _collection.Insert(item);
        }

        public ValueTask AddAsync(TEntity item, CancellationToken cancellationToken)
        {
            Add(item);

            return new ValueTask();
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items);

            _collection.InsertBulk(items);
        }

        public ValueTask AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            AddRange(items);

            return new ValueTask();
        }

        public void Remove(TEntity item, Guid key)
        {
            _context.AddTracking(item);

            _collection.Delete(key);
        }

        public ValueTask RemoveAsync(TEntity item, Guid key, CancellationToken cancellationToken)
        {
            Remove(item, key);

            return new ValueTask();
        }

        public void RemoveMany(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items);

            _collection.DeleteAll();
        }

        public ValueTask RemoveManyAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            RemoveMany(items);

            return new ValueTask();
        }

        public void RemoveAll()
        {
            _context.Database.DropCollection(_collection.Name);
        }

        public void Update(TEntity item)
        {
            _context.AddTracking(item);

            _collection.Update(item.Id, item);
        }

        public ValueTask UpdateAsync(TEntity item, CancellationToken cancellationToken)
        {
            Update(item);

            return new ValueTask();
        }

        public void UpdateRange(IEnumerable<TEntity> items)
        {
            foreach (var item in items)
            {
                Update(item);
            }
        }

        public ValueTask UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            UpdateRange(items);

            return new ValueTask();
        }
    }
}
