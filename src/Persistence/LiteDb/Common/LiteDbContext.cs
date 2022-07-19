using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Persistence.LiteDb.Common
{
    public abstract class LiteDbContext
    {
        private readonly ConcurrentDictionary<Guid, BaseEntity> _changeTracker = new ConcurrentDictionary<Guid, BaseEntity>();
        private readonly ConcurrentDictionary<Guid, Func<Guid, BaseEntity, CancellationToken, Task>> _updateHandlers = new ConcurrentDictionary<Guid, Func<Guid, BaseEntity, CancellationToken, Task>>();

        public ILiteDatabase Database { get; }

        protected LiteDbContext(ILiteDatabase database)
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

        public IReadOnlyList<BaseEntity> ChangeTracking()
        {
            return _changeTracker.Values.ToList();
        }

        public void DropCollections(CancellationToken cancellationToken = default)
        {
            var collections = Database.GetCollectionNames();

            if (collections is null) return;

            foreach (var collection in collections)
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                Database.DropCollection(collection);
            }
        }

        internal void AddTracking(BaseEntity entity, Func<Guid, BaseEntity, CancellationToken, Task> handler)
        {
            _changeTracker.AddOrUpdate(entity.Id, entity, (k, v) => entity);

            _updateHandlers.AddOrUpdate(entity.Id, handler, (k, v) => handler);
        }

        internal void AddTrackingRange(in IReadOnlyList<BaseEntity> entities, in Func<Guid, BaseEntity, CancellationToken, Task> handler, CancellationToken cancellationToken = default)
        {
            foreach (var entity in entities)
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                AddTracking(entity, handler);
            }
        }
    }
}
