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

using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Repositories
{
    public interface IQueryRepository<TEntity> where TEntity : Entity
    {
        IQueryable<TEntity> Queryable { get; }

        bool Any();

        bool Any(Expression<Func<TEntity, bool>> where);

        Task<bool> AnyAsync(CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);

        long Count();

        long Count(Expression<Func<TEntity, bool>> where);

        Task<long> CountAsync(CancellationToken cancellationToken = default);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);

        TEntity Get(Guid key);

        Task<TEntity> GetAsync(Guid key, CancellationToken cancellationToken = default);

        List<TEntity> List();

        List<TEntity> List(Expression<Func<TEntity, bool>> where);

        Task<List<TEntity>> ListAsync(CancellationToken cancellationToken = default);

        Task<List<TEntity>> ListAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);
    }
}
