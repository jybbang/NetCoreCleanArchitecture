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
using MongoDB.Driver;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.MongoDb.Common;

namespace NetCoreCleanArchitecture.Infrastructure.MongoDb.Repositories
{
    public class MongoCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : BaseEntity
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
            _context.AddTracking(item);

            _collection.InsertOne(item);
        }

        public async ValueTask AddAsync(TEntity item, CancellationToken cancellationToken)
        {
            _context.AddTracking(item);

            await _collection.InsertOneAsync(item, cancellationToken: cancellationToken);
        }

        public void AddRange(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items);

            _collection.InsertMany(items);
        }

        public async ValueTask AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            _context.AddTrackingRange(items, cancellationToken);

            await _collection.InsertManyAsync(items, cancellationToken: cancellationToken);
        }

        public void Remove(TEntity item, Guid key)
        {
            _context.AddTracking(item);

            _collection.DeleteOne(Id(key));
        }

        public async ValueTask RemoveAsync(TEntity item, Guid key, CancellationToken cancellationToken)
        {
            _context.AddTracking(item);

            await _collection.DeleteOneAsync(Id(key), cancellationToken: cancellationToken);
        }

        public void RemoveMany(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items);

            _collection.BulkWrite(CreateDeletes(items));
        }

        public async ValueTask RemoveManyAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            _context.AddTrackingRange(items);

            await _collection.BulkWriteAsync(CreateDeletes(items, cancellationToken), cancellationToken: cancellationToken);
        }

        public void RemoveAll()
        {
            _context.Database.DropCollection(typeof(TEntity).Name);
        }

        public void Update(TEntity item)
        {
            _context.AddTracking(item);

            _collection.ReplaceOne(Id(item.Id), item);
        }

        public async ValueTask UpdateAsync(TEntity item, CancellationToken cancellationToken)
        {
            _context.AddTracking(item);

            await _collection.ReplaceOneAsync(Id(item.Id), item, cancellationToken: cancellationToken);
        }

        public void UpdateRange(IEnumerable<TEntity> items)
        {
            _context.AddTrackingRange(items);

            _collection.BulkWrite(CreateUpdates(items));
        }

        public async ValueTask UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken)
        {
            _context.AddTrackingRange(items, cancellationToken);

            await _collection.BulkWriteAsync(CreateUpdates(items, cancellationToken), cancellationToken: cancellationToken);
        }

        private IEnumerable<WriteModel<TEntity>> CreateUpdates(in IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            var updates = new List<WriteModel<TEntity>>();

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (item.Id == Guid.Empty) continue;

                updates.Add(new ReplaceOneModel<TEntity>(Id(item.Id), item));
            }

            return updates;
        }

        private IEnumerable<WriteModel<TEntity>> CreateDeletes(in IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            var deletes = new List<WriteModel<TEntity>>();

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (item.Id == Guid.Empty) continue;

                deletes.Add(new DeleteOneModel<TEntity>(Id(item.Id)));
            }

            return deletes;
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
