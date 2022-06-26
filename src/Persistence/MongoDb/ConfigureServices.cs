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
        EnsureDeleteAndCreated,
        EnsureCreated,
    }

    public static class ConfigureServices
    {
        public static void AddNetCleanUnitOfWork<T>(
            this IServiceCollection services,
            string connectionString,
            MigrationOptions migration = MigrationOptions.EnsureCreated) where T : MongoContext
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), $"'{nameof(connectionString)}' is required.");
            }

            var databaseName = new MongoUrl(connectionString).DatabaseName;

            var database = new MongoClient(connectionString).GetDatabase(databaseName);

            if (!(Activator.CreateInstance(typeof(T), database) is T context))
                throw new NullReferenceException("Could not resolve MongoContext");

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

        public static void AddNetCleanUnitOfWorkInMemory<T>(this IServiceCollection services) where T : MongoContext
        {
            var _runner = MongoDbRunner.Start();

            var database = new MongoClient(_runner.ConnectionString).GetDatabase(typeof(T).Name);

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            if (!(Activator.CreateInstance(typeof(T), database) is T context))
                throw new NullReferenceException("Could not resolve MongoContext");

            services.AddSingleton<T>(context);

            services.AddScoped<MongoContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<T>());
        }
    }
}
