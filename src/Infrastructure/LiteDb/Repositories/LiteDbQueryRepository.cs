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
using LiteDB;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using NetCoreCleanArchitecture.Infrastructure.LiteDb.Common;

namespace NetCoreCleanArchitecture.Infrastructure.LiteDb.Repositories
{
    public class LiteDbQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ILiteCollection<TEntity> _collection;

        public LiteDbQueryRepository(LiteDbContext context)
        {
            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public IQueryable<TEntity> Queryable => throw new NotSupportedException();

        public bool Any()
            => _collection.Exists(x => x.Id != default);

        public bool Any(Expression<Func<TEntity, bool>> where)
            => _collection.Exists(where);

        public ValueTask<bool> AnyAsync(CancellationToken cancellationToken)
            => new ValueTask<bool>(Any());

        public ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => new ValueTask<bool>(Any(where));

        public long Count()
            => _collection.LongCount();

        public long Count(Expression<Func<TEntity, bool>> where)
            => _collection.LongCount(where);

        public ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => new ValueTask<long>(Count());

        public ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => new ValueTask<long>(Count(where));

        public TEntity? Find(Guid key)
            => _collection.FindById(key);

        public TEntity? Find(Expression<Func<TEntity, bool>> where)
            => _collection.Find(where).SingleOrDefault();

        public ValueTask<TEntity?> FindAsync(Guid key, CancellationToken cancellationToken)
            => new ValueTask<TEntity?>(Find(key));

        public ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => new ValueTask<TEntity?>(Find(where));

        public List<TEntity> FindMany()
            => _collection.FindAll().ToList();

        public List<TEntity> FindMany(Expression<Func<TEntity, bool>> where)
            => _collection.Find(where).ToList();

        public ValueTask<List<TEntity>> FindManyAsync(CancellationToken cancellationToken)
            => new ValueTask<List<TEntity>>(FindMany());

        public ValueTask<List<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => new ValueTask<List<TEntity>>(FindMany(where));
    }
}
