using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.LiteDb.Common;
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
            MigrationOptions migration = MigrationOptions.EnsureCreated) where T : LiteDbContext
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), $"'{nameof(connectionString)}' is required.");
            }

            var database = new LiteDatabase(connectionString);

            if (!(Activator.CreateInstance(typeof(T), database) is T context)) 
                throw new NullReferenceException("Could not resolve LiteDbContext");

            services.AddSingleton<T>(context);

            services.AddScoped<LiteDbContext>(provider => provider.GetRequiredService<T>());

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
    }
}
