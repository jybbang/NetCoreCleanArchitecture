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
using MongoDB.Driver.Linq;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Persistence.MongoDb.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Persistence.MongoDb.Repositories
{
    public class MongoQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : Entity
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoQueryRepository(MongoContext context)
        {
            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);

            Queryable = _collection.AsQueryable();
        }

        public IQueryable<TEntity> Queryable { get; }

        public bool Any() => Queryable.Any();

        public bool Any(Expression<Func<TEntity, bool>> where) => Queryable.Where(where).Any();

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Task.FromResult(Any());

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Task.FromResult(Any(where));

        public long Count() => Queryable.LongCount();

        public long Count(Expression<Func<TEntity, bool>> where) => Queryable.Where(where).LongCount();

        public Task<long> CountAsync(CancellationToken cancellationToken = default) => Task.FromResult(Count());

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Task.FromResult(Count(where));

        public TEntity Get(Guid key) => _collection.Find(item => item.Id == key).SingleOrDefault();

        public Task<TEntity> GetAsync(Guid key, CancellationToken cancellationToken = default) => _collection.Find(item => item.Id == key).SingleOrDefaultAsync(cancellationToken);

        public List<TEntity> List() => Queryable.ToList();

        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default) => Task.FromResult(List());

        public List<TEntity> List(Expression<Func<TEntity, bool>> where) => Queryable.Where(where).ToList();

        public Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Task.FromResult(List(where));
    }
}
