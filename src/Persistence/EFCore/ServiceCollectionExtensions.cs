using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.EFCore.Repositories;
using System;
using System.Reflection;

namespace NetCoreCleanArchitecture.Persistence.EFCore
{
    public enum MigrationOptions
    {
        None,
        EnsureCreated,
        Migrate
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddNetCoreCleanArchitectureDbContext<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> options, MigrationOptions migration = MigrationOptions.Migrate) where T : DbContext
        {
            services.AddDbContext<T>(options);

            services.AddScoped<DbContext>(provider => provider.GetService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

            switch (migration)
            {
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

        public static void AddNetCoreCleanArchitectureDbContextMemory<T>(this IServiceCollection services) where T : DbContext
        {
            services.AddDbContextPool<T>(options => options.UseInMemoryDatabase(typeof(T).Name));

            services.AddScoped<DbContext>(provider => provider.GetService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

            services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureCreated();
        }
    }
}
