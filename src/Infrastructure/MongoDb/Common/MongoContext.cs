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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Infrastructure.MongoDb.Common
{
    public abstract class MongoContext
    {
        private readonly ConcurrentDictionary<Guid, BaseEntity> _changeTracker = new ConcurrentDictionary<Guid, BaseEntity>();
        private readonly ConcurrentDictionary<Guid, Func<Guid, BaseEntity, CancellationToken, ValueTask>> _updateHandlers = new ConcurrentDictionary<Guid, Func<Guid, BaseEntity, CancellationToken, ValueTask>>();

        public IMongoDatabase Database { get; }

        protected MongoContext(IMongoDatabase database)
        {
            Database = database;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var result = _changeTracker.Count;

            foreach (var track in _changeTracker)
            {
                if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

                if (_updateHandlers.TryGetValue(track.Key, out var handler))
                {
                    if (handler is null) continue;

                    await handler.Invoke(track.Key, track.Value, cancellationToken);
                }
            }

            _changeTracker.Clear();

            _updateHandlers.Clear();

            return result;
        }

        public IEnumerable<BaseEntity> ChangeTracking()
        {
            return _changeTracker.Values;
        }

        public void DropCollections(CancellationToken cancellationToken = default)
        {
            var collections = Database.ListCollectionNames(cancellationToken: cancellationToken);

            collections.MoveNext(cancellationToken);

            foreach (var collection in collections.Current)
            {
                Database.DropCollection(collection, cancellationToken: cancellationToken);
            }
        }

        internal void AddTracking(BaseEntity entity, Func<Guid, BaseEntity, CancellationToken, ValueTask> handler)
        {
            _changeTracker.AddOrUpdate(entity.Id, entity, (k, v) => entity);

            _updateHandlers.AddOrUpdate(entity.Id, handler, (k, v) => handler);
        }

        internal void AddTrackingRange(in IReadOnlyList<BaseEntity> entities, Func<Guid, BaseEntity, CancellationToken, ValueTask> handler, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                AddTracking(entity, handler);
            }
        }
    }
}
