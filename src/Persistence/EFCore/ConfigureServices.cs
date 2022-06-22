using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using System;

namespace NetCoreCleanArchitecture.Persistence
{
    public enum MigrationOptions
    {
        None,
        EnsureDeleteAndCreated,
        EnsureCreated,
        Migrate
    }

    public static class ConfigureServices
    {
        public static void AddNetCleanUnitOfWork<T>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options,
            MigrationOptions migration = MigrationOptions.Migrate) where T : DbContext
        {
            services.AddDbContextPool<T>(options);

            services.AddScoped<DbContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<T>());

            switch (migration)
            {
                case MigrationOptions.EnsureDeleteAndCreated:
                    {
                        services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureDeleted();
                        services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureCreated();
                    }
                    break;
                case MigrationOptions.EnsureCreated:
                    services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureCreated();
                    break;
                case MigrationOptions.Migrate:
                    services.BuildServiceProvider().GetRequiredService<T>().Database.Migrate();
                    break;
                default:
                    break;
            }
        }

        public static void AddNetCleanUnitOfWorkInMemory<T>(this IServiceCollection services) where T : DbContext
        {
            services.AddDbContextPool<T>(options => options.UseInMemoryDatabase(typeof(T).Name));

            services.AddScoped<DbContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => (IUnitOfWork)provider.GetRequiredService<T>());

            services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureCreated();
        }
    }
}
