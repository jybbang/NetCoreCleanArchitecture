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
using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Repositories
{
    public interface ICommandRepository<TEntity> where TEntity : BaseEntity
    {
        void Add(TEntity item);

        ValueTask AddAsync(TEntity item, CancellationToken cancellationToken);

        void AddRange(IEnumerable<TEntity> items);

        ValueTask AddRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken);

        void Remove(TEntity item, Guid key);

        ValueTask RemoveAsync(TEntity item, Guid key, CancellationToken cancellationToken);

        void RemoveMany(IEnumerable<TEntity> items);

        ValueTask RemoveManyAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken);

        void RemoveAll();

        void Update(TEntity item);

        ValueTask UpdateAsync(TEntity item, CancellationToken cancellationToken);

        void UpdateRange(IEnumerable<TEntity> items);

        ValueTask UpdateRangeAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken);
    }
}
