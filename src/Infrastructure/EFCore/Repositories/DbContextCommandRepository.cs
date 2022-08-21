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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetCoreCleanArchitecture.Application.Repositories;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Infrastructure.EFCore.Repositories
{
    public class DbContextCommandRepository<TEntity> : ICommandRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _context;

        public DbContextCommandRepository(DbContext context)
        {
            _context = context;
        }

        private DbSet<TEntity> Set => _context.Set<TEntity>();

        public void Add(TEntity item) => Set.Add(item);

        public async ValueTask AddAsync(TEntity item, CancellationToken cancellationToken) => await Set.AddAsync(item, cancellationToken);

        public void AddRange(IReadOnlyList<TEntity> items) => Set.AddRange(items);

        public async ValueTask AddRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken) => await Set.AddRangeAsync(items, cancellationToken);

        public void Remove(Guid key)
        {
            var entity = Set.Find(key);

            if (entity is null) return;

            Set.Remove(entity);
        }

        public async ValueTask RemoveAsync(Guid key, CancellationToken cancellationToken)
        {
            var entity = await Set.FindAsync(new object[] { key }, cancellationToken);

            if (entity is null) return;

            Set.Remove(entity);
        }

        public void RemoveAll()
        {
            var entities = Set.AsNoTracking().ToList();

            Set.RemoveRange(entities);
        }

        public async ValueTask RemoveAllAsync(CancellationToken cancellationToken)
        {
            var entities = await Set.AsNoTracking().ToListAsync(cancellationToken);

            Set.RemoveRange(entities);
        }

        public void Update(TEntity item) => Set.Update(item);

        public ValueTask UpdateAsync(TEntity item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            Set.Update(item);

            return new ValueTask();
        }

        public void UpdateRange(IReadOnlyList<TEntity> items) => Set.UpdateRange(items);

        public ValueTask UpdateRangeAsync(IReadOnlyList<TEntity> items, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

            Set.UpdateRange(items);

            return new ValueTask();
        }
    }
}
