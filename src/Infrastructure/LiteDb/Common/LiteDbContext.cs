using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Infrastructure.LiteDb.Common
{
    public abstract class LiteDbContext
    {
        private readonly ConcurrentDictionary<Guid, BaseEntity> _changeTracker = new ConcurrentDictionary<Guid, BaseEntity>();

        public ILiteDatabase Database { get; }

        protected LiteDbContext(ILiteDatabase database)
        {
            Database = database;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = _changeTracker.Count;

            _changeTracker.Clear();

            return Task.FromResult(result);
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
                cancellationToken.ThrowIfCancellationRequested();

                Database.DropCollection(collection);
            }
        }

        internal void AddTracking<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            _changeTracker.AddOrUpdate(entity.Id, entity, (k, v) => entity);
        }

        internal void AddTrackingRange<TEntity>(in IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity
        {
            foreach (var entity in entities)
            {
                cancellationToken.ThrowIfCancellationRequested();

                AddTracking(entity);
            }
        }
    }
}
