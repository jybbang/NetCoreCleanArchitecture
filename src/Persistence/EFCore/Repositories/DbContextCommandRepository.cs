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
    public class DbContextCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : Entity
    {
        private readonly DbContext _context;

        public DbContextCommandRepository(DbContext context)
        {
            _context = context;
        }

        private DbSet<TEntity> Set => _context.Set<TEntity>();

        public void Add(TEntity item) => Set.Add(item);

        public Task AddAsync(TEntity item, CancellationToken cancellationToken = default) => Set.AddAsync(item, cancellationToken).AsTask();

        public void AddRange(IEnumerable<TEntity> items) => Set.AddRange(items);

        public Task AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default) => Set.AddRangeAsync(items, cancellationToken);

        public void Delete(Guid key)
        {
            var entity = Set.Find(key);

            if (entity is null) return;

            Set.Remove(entity);
        }

        public void Delete(Expression<Func<TEntity, bool>> where)
        {
            var entity = Set.Where(where);

            if (!entity.Any()) return;

            Set.RemoveRange(entity);
        }

        public async Task DeleteAsync(Guid key, CancellationToken cancellationToken = default)
        {
            var entity = await Set.FindAsync(new object[] { key }, cancellationToken);

            Set.Remove(entity);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default)
        {
            var entity = Set.Where(where);

            if (!(await entity.AnyAsync(cancellationToken))) return;

            Set.RemoveRange(entity);
        }

        public void Update(Guid key, TEntity item) => Set.Update(item);

        public Task UpdateAsync(Guid key, TEntity item, CancellationToken cancellationToken = default) => Task.FromResult(Set.Update(item));

        public void UpdatePartial(Guid key, object item)
        {
            var entity = Set.Find(key);

            _context.Entry(entity).CurrentValues.SetValues(item);
        }

        public async Task UpdatePartialAsync(Guid key, object item, CancellationToken cancellationToken = default)
        {
            var entity = await Set.FindAsync(new object[] { key }, cancellationToken);

            _context.Entry(entity).CurrentValues.SetValues(item);
        }

        public void UpdateRange(IEnumerable<TEntity> items) => Set.UpdateRange(items);

        public Task UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default)
        {
            Set.UpdateRange(items);

            return Task.CompletedTask;
        }
    }
}
