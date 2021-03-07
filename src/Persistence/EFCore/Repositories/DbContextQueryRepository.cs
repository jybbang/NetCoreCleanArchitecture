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

using Microsoft.EntityFrameworkCore;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Persistence.EFCore.Repositories
{
    public class DbContextQueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : Entity
    {
        private readonly DbContext _context;

        public DbContextQueryRepository(DbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Queryable => _context.Set<TEntity>();

        public bool Any() => Queryable.Any();

        public bool Any(Expression<Func<TEntity, bool>> where) => Queryable.Any(where);

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Queryable.AnyAsync(cancellationToken);

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Queryable.AnyAsync(where, cancellationToken);

        public long Count() => Queryable.Count();

        public long Count(Expression<Func<TEntity, bool>> where) => Queryable.Count(where);

        public Task<long> CountAsync(CancellationToken cancellationToken = default) => Queryable.LongCountAsync(cancellationToken);

        public Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Queryable.LongCountAsync(where, cancellationToken);

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> where) => Queryable.Where(where).ToList();

        public Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Queryable.Where(where).ToListAsync(cancellationToken);

        public TEntity Find(Expression<Func<TEntity, bool>> where) => Queryable.Where(where).FirstOrDefault();

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default) => Queryable.Where(where).FirstOrDefaultAsync(cancellationToken);

        public TEntity Get(Guid key) => _context.Set<TEntity>().Find(key);

        public Task<TEntity> GetAsync(Guid key, CancellationToken cancellationToken = default) => _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).AsTask();

        public IEnumerable<TEntity> List() => Queryable.ToList();

        public Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default) => Queryable.ToListAsync(cancellationToken);
    }
}
