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

using MongoDB.Driver;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Persistence.MongoDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Persistence.MongoDb.Repositories
{
    public class MongoCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : Entity
    {
        private readonly MongoContext _context;
        private readonly IMongoCollection<TEntity> _collection;

        public MongoCommandRepository(MongoContext context)
        {
            _context = context;

            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public void Add(TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.InsertOne(item);
        }

        public Task AddAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            return _collection.InsertOneAsync(item, cancellationToken: cancellationToken);
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            _collection.InsertMany(items);
        }

        public Task AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            return _collection.InsertManyAsync(items, cancellationToken: cancellationToken);
        }

        public void Remove(Guid key)
        {
            var result = _collection.FindOneAndDelete(Id(key));
            
            _context.AddTracking(result, null);
        }

        public async Task RemoveAsync(Guid key, CancellationToken cancellationToken = default)
        {
            var result = await _collection.FindOneAndDeleteAsync(Id(key), cancellationToken: cancellationToken);

            _context.AddTracking(result, null);
        }

        public void RemoveRange(Expression<Func<TEntity, bool>> where)
        {
            var items = _collection.AsQueryable().Where(where);

            _context.AddTrackingRange(items, null);

            _collection.DeleteMany(where);
        }

        public Task RemoveRangeAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            var items = _collection.AsQueryable().Where(where);

            _context.AddTrackingRange(items, null);

            return _collection.DeleteManyAsync(where, cancellationToken: cancellationToken);
        }

        public void Update(Guid key, TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.ReplaceOne(Id(key), item);
        }

        public Task UpdateAsync(Guid key, TEntity item, CancellationToken cancellationToken = default)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            return _collection.ReplaceOneAsync(Id(key), item, cancellationToken: cancellationToken);
        }

        public void UpdatePartial(Guid key, object item)
        {
            _collection.ReplaceOne(Id(key), item as TEntity);
        }

        public Task UpdatePartialAsync(Guid key, object item, CancellationToken cancellationToken = default)
        {
            return _collection.ReplaceOneAsync(Id(key), item as TEntity, cancellationToken: cancellationToken);
        }

        public void UpdateRange(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            _collection.BulkWrite(CreateUpdates(items));
        }

        public Task UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            _context.AddTrackingRange(items, UpdatePartialAsync);

            return _collection.BulkWriteAsync(CreateUpdates(items), cancellationToken: cancellationToken);
        }

        private IEnumerable<WriteModel<TEntity>> CreateUpdates(IEnumerable<TEntity> items)
        {
            var updates = new List<WriteModel<TEntity>>();

            foreach (var item in items)
            {
                if (item.Id == Guid.Empty) continue;

                updates.Add(new ReplaceOneModel<TEntity>(Id(item.Id), item));
            }

            return updates;
        }

        private FilterDefinition<TEntity> Id(Guid value)
        {
            return Builders<TEntity>.Filter.Eq(nameof(Id), value);
        }
    }
}
