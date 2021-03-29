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

using Mongo2Go;
using MongoDB.Driver;
using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Persistence.MongoDb.Common
{
    public abstract class MongoContext
    {
        private readonly string _connectionString;
        private readonly MongoContextOptions _options;

        protected MongoContext(string connectionString, MongoContextOptions options)
        {
            _connectionString = connectionString;
            _options = options;

            if (_options.UseInMemory)
            {
                _runner = MongoDbRunner.Start();

                DatabaseName = connectionString;

                Database = new MongoClient(_runner.ConnectionString).GetDatabase(DatabaseName);
            }
            else
            {
                DatabaseName = new MongoUrl(_connectionString).DatabaseName;

                Database = new MongoClient(_connectionString).GetDatabase(DatabaseName);
            }
        }

        private readonly MongoDbRunner _runner;

        private readonly ConcurrentDictionary<Guid, Entity> _changeTracker = new();

        public string DatabaseName { get; }

        public IMongoDatabase Database { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = _changeTracker.Count;

            _changeTracker.Clear();

            return Task.FromResult(result);
        }

        public IEnumerable<Entity> ChangeTracking()
        {
            return _changeTracker.Values;
        }

        internal void DropCollections()
        {
            var collections = Database.ListCollectionNames();

            foreach (var collection in collections.Current)
            {
                Database.DropCollection(collection);
            }
        }

        internal void AddTracking(Entity entity)
        {
            _changeTracker.AddOrUpdate(entity.Id, entity, (k, v) => entity);
        }

        internal void AddTrackingRange(IEnumerable<Entity> entities)
        {
            foreach (var id in entities)
            {
                AddTracking(id);
            }
        }
    }

    public record MongoContextOptions
    {
        public bool UseInMemory { get; init; } = false;
    }
}
