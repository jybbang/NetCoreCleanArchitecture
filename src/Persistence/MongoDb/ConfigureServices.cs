using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.MongoDb.Common;
using System;

namespace NetCoreCleanArchitecture.Persistence
{
    public enum MigrationOptions
    {
        None,
        EnsureDeleteAndCreated,
        EnsureCreated,
    }

    public static class ConfigureServices
    {
        public static void AddNetCleanDbContext<T>(
            this IServiceCollection services,
            string connectionString,
            MigrationOptions migration = MigrationOptions.EnsureCreated) where T : MongoContext
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' is required.", nameof(connectionString));
            }

            var databaseName = new MongoUrl(connectionString).DatabaseName;

            var database = new MongoClient(connectionString).GetDatabase(databaseName);

            var context = Activator.CreateInstance(typeof(T), database) as T;

            if (context is null) throw new NullReferenceException("Could not resolve MongoDatabase");

            services.AddSingleton<T>(context);

            services.AddScoped<MongoContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<T>());

            switch (migration)
            {
                case MigrationOptions.EnsureDeleteAndCreated:
                    services.BuildServiceProvider().GetRequiredService<T>().DropCollections();
                    break;
                default:
                    break;
            }
        }

        public static void AddNetCleanDbContextInMemory<T>(this IServiceCollection services) where T : MongoContext
        {
            var _runner = MongoDbRunner.Start();

            var database = new MongoClient(_runner.ConnectionString).GetDatabase(typeof(T).Name);

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            var context = Activator.CreateInstance(typeof(T), database) as T;

            if (context is null) throw new NullReferenceException("Could not resolve MongoDatabase");

            services.AddSingleton<T>(context);

            services.AddScoped<MongoContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<T>());
        }
    }
}
