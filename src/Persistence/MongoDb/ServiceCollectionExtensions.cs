using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Persistence.MongoDb.Common;
using System;
using HealthChecks.MongoDb;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using Mongo2Go;
using System.Reflection;

namespace NetCoreCleanArchitecture.Persistence.MongoDb
{
    public enum MigrationOptions
    {
        None,
        EnsureDeleteAndCreated,
        EnsureCreated
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddNetCleanMongoContext<T>(
            this IServiceCollection services,
            string connectionString,
            MongoContextOptions options,
            MigrationOptions migration = MigrationOptions.EnsureCreated) where T : MongoContext
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException($"'{nameof(connectionString)}' is required.", nameof(connectionString));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddScoped<IApplicationContext, ApplicationContext>();

            var context = (T)Activator.CreateInstance(typeof(T), new object[] { connectionString, options });

            services.AddSingleton<T>(context);

            services.AddScoped<MongoContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

            switch (migration)
            {
                case MigrationOptions.EnsureDeleteAndCreated:
                    {
                        services.BuildServiceProvider().GetRequiredService<T>().DropCollection();
                        services.BuildServiceProvider().GetRequiredService<T>().CreateCollection();
                    }
                    break;
                case MigrationOptions.EnsureCreated:
                    services.BuildServiceProvider().GetRequiredService<T>().CreateCollection();
                    break;
                default:
                    break;
            }
        }

        public static void AddNetCleanMongoContextMemory<T>(this IServiceCollection services) where T : MongoContext
        {
            services.AddScoped<IApplicationContext, ApplicationContext>();

            var options = new MongoContextOptions { UseInMemory = true };

            var context = (T)Activator.CreateInstance(typeof(T), new object[] { typeof(T).Name, options });

            services.AddSingleton<T>(context);

            services.AddScoped<MongoContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

            services.BuildServiceProvider().GetRequiredService<T>().CreateCollection();
        }

        public static IHealthChecksBuilder AddNetCleanMongoContextCheck(this IHealthChecksBuilder builder, string connectionString)
        {
            builder.AddMongoDb(connectionString);

            return builder;
        }
    }
}
