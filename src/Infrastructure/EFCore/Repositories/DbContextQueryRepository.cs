﻿// 
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Infrastructure.EFCore.Repositories
{
    public class DbContextQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _set;
        private readonly IQueryable<TEntity> _queryableAsNoTracking;

        public DbContextQueryRepository(DbContext context)
        {
            _context = context;

            _set = _context.Set<TEntity>();

            _queryableAsNoTracking = _set.AsNoTracking();

            Queryable = _set;
        }

        public IQueryable<TEntity> Queryable { get; }

        public bool Any()
            => _queryableAsNoTracking.Any();

        public bool Any(Expression<Func<TEntity, bool>> where)
            => _queryableAsNoTracking.Any(where);

        public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken)
            => await _queryableAsNoTracking.AnyAsync(cancellationToken);

        public async ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _queryableAsNoTracking.AnyAsync(where, cancellationToken);

        public long Count()
            => _queryableAsNoTracking.Count();

        public long Count(Expression<Func<TEntity, bool>> where)
            => _queryableAsNoTracking.Count(where);

        public async ValueTask<long> CountAsync(CancellationToken cancellationToken)
            => await _queryableAsNoTracking.LongCountAsync(cancellationToken);

        public async ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _queryableAsNoTracking.LongCountAsync(where, cancellationToken);

        public TEntity? Find(Guid key)
            => _set.Find(key);

        public async ValueTask<TEntity?> FindAsync(Guid key, CancellationToken cancellationToken)
            => await _set.FindAsync(new object[] { key }, cancellationToken);

        public TEntity? Find(Expression<Func<TEntity, bool>> where)
            => _queryableAsNoTracking.Where(where).SingleOrDefault();

        public async ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _queryableAsNoTracking.Where(where).SingleOrDefaultAsync(cancellationToken);

        public IReadOnlyList<TEntity> FindMany(Expression<Func<TEntity, bool>> where)
            => _queryableAsNoTracking.Where(where).ToList();

        public async ValueTask<IReadOnlyList<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken)
            => await _queryableAsNoTracking.Where(where).ToListAsync(cancellationToken);

        public IReadOnlyList<TEntity> FindMany()
            => _queryableAsNoTracking.ToList();

        public async ValueTask<IReadOnlyList<TEntity>> FindManyAsync(CancellationToken cancellationToken)
            => await _queryableAsNoTracking.ToListAsync(cancellationToken);

        public T? Find<T>(Guid key) where T : BaseEntity
            => (T)(BaseEntity)_set.Find(key);

        public async ValueTask<T?> FindAsync<T>(Guid key, CancellationToken cancellationToken) where T : BaseEntity
            => (T)(BaseEntity)(await _set.FindAsync(new object[] { key }, cancellationToken));

        public IReadOnlyList<T> FindMany<T>() where T : BaseEntity
            => _queryableAsNoTracking.OfType<T>().ToList();

        public async ValueTask<IReadOnlyList<T>> FindManyAsync<T>(CancellationToken cancellationToken) where T : BaseEntity
            => await _queryableAsNoTracking.OfType<T>().ToListAsync(cancellationToken);
    }
}
