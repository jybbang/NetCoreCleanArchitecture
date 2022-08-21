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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.MongoDb.Common;

namespace NetCoreCleanArchitecture.Infrastructure.MongoDb.Repositories
{
    public class MongoQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoQueryRepository(MongoContext context)
        {
            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);

            Queryable = _collection.AsQueryable();
        }

        public IQueryable<TEntity> Queryable { get; }

        public bool Any()
            => Queryable.Any();

        public bool Any(Expression<Func<TEntity, bool>> where)
            => Queryable.Where(where).Any();

        public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken)
            => await _collection.Find(NotId(Guid.Empty)).AnyAsync(cancellationToken);

        public async ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _collection.Find(where).AnyAsync(cancellationToken);

        public long Count()
            => Queryable.LongCount();

        public long Count(Expression<Func<TEntity, bool>> where)
            => Queryable.Where(where).LongCount();

        public async ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => await _collection.Find(NotId(Guid.Empty)).CountDocumentsAsync(cancellationToken);

        public async ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _collection.CountDocumentsAsync(where, cancellationToken: cancellationToken);

        public TEntity? Find(Guid key)
            => _collection.Find(item => item.Id == key).SingleOrDefault();

        public TEntity? Find(Expression<Func<TEntity, bool>> where)
            => Queryable.Where(where).SingleOrDefault();

        public async ValueTask<TEntity?> FindAsync(Guid key, CancellationToken cancellationToken)
            => await _collection.Find(item => item.Id == key).SingleOrDefaultAsync(cancellationToken);

        public async ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _collection.Find(where).SingleOrDefaultAsync(cancellationToken);

        public List<TEntity> FindMany()
            => Queryable.ToList();

        public List<TEntity> FindMany(Expression<Func<TEntity, bool>> where)
            => Queryable.Where(where).ToList();

        public async ValueTask<List<TEntity>> FindManyAsync(CancellationToken cancellationToken)
            => await _collection.Find(NotId(Guid.Empty)).ToListAsync(cancellationToken);

        public async ValueTask<List<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _collection.Find(where).ToListAsync(cancellationToken);

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
