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

using DaprCleanArchitecture.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.Repositories
{
    public interface ICommandRepository<TEntity> where TEntity : Entity
    {
        void Add(TEntity item);

        Task AddAsync(TEntity item, CancellationToken cancellationToken = default);

        void AddRange(IEnumerable<TEntity> items);

        Task AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);

        void Delete(Guid key);

        void Delete(Expression<Func<TEntity, bool>> where);

        Task DeleteAsync(Guid key, CancellationToken cancellationToken = default);

        Task DeleteAsync(Expression<Func<TEntity, bool>> where, CancellationToken cancellationToken = default);

        void Update(Guid key, TEntity item);

        Task UpdateAsync(Guid key, TEntity item, CancellationToken cancellationToken = default);

        void UpdatePartial(Guid key, object item);

        Task UpdatePartialAsync(Guid key, object item, CancellationToken cancellationToken = default);

        void UpdateRange(IEnumerable<TEntity> items);

        Task UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);
    }
}
