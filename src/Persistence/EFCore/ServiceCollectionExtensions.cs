using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using System;

namespace NetCoreCleanArchitecture.Persistence.EFCore
{
    public enum MigrationOptions
    {
        None,
        EnsureDeleteAndCreated,
        EnsureCreated,
        Migrate
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddNetCleanDbContext<T>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options,
            MigrationOptions migration = MigrationOptions.Migrate) where T : DbContext
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddDbContextPool<T>(options);

            services.AddScoped<DbContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

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

        public static void AddNetCleanDbContextMemory<T>(this IServiceCollection services) where T : DbContext
        {
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddDbContextPool<T>(options => options.UseInMemoryDatabase(typeof(T).Name));

            services.AddScoped<DbContext>(provider => provider.GetRequiredService<T>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<T>() as IUnitOfWork);

            services.BuildServiceProvider().GetRequiredService<T>().Database.EnsureCreated();
        }

        public static IHealthChecksBuilder AddNetCleanDbContextCheck(this IHealthChecksBuilder builder)
        {
            builder.AddDbContextCheck<DbContext>();

            return builder;
        }
    }
}
