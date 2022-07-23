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
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Common.Repositories
{
    public interface IQueryRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> Queryable { get; }

        bool Any();

        bool Any(Expression<Func<TEntity, bool>> where);

        ValueTask<bool> AnyAsync(CancellationToken cancellationToken);

        ValueTask<bool> AnyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken);

        long Count();

        long Count(Expression<Func<TEntity, bool>> where);

        ValueTask<long> CountAsync(CancellationToken cancellationToken);

        ValueTask<long> CountAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken);

        TEntity? Find(Guid key);

        TEntity? Find(Expression<Func<TEntity, bool>> where);

        ValueTask<TEntity?> FindAsync(Guid key, CancellationToken cancellationToken);

        ValueTask<TEntity?> FindAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken);

        List<TEntity> FindMany();

        List<TEntity> FindMany(Expression<Func<TEntity, bool>> where);

        ValueTask<List<TEntity>> FindManyAsync(CancellationToken cancellationToken);

        ValueTask<List<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken);
    }
}
