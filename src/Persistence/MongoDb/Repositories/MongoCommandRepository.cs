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

        public void Remove(TEntity item)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            _collection.DeleteOne(Id(item.Id));
        }

        public Task RemoveAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            _context.AddTracking(item, UpdatePartialAsync);

            return _collection.DeleteOneAsync(Id(item.Id), cancellationToken: cancellationToken);
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

        private FilterDefinition<TEntity> NotId(Guid value)
        {
            return Builders<TEntity>.Filter.Not(Id(value));
        }
    }
}
