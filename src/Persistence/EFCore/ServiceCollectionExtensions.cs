using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.EFCore.Repositories;

namespace NetCoreCleanArchitecture.Persistence.EFCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreCleanArchitectureInMemory(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EFCoreDbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(NetCoreCleanArchitecture));
            });

            services.AddEFCore();

            return services;
        }

        public static IServiceCollection AddNetCoreCleanArchitectureSqlite(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EFCoreDbContext>(options =>
            {
                options.UseSqlite(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName));
                EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(options);
            });

            services.AddEFCore();

            return services;
        }

        internal static IServiceCollection AddEFCore(this IServiceCollection services)
        {
            services.AddScoped<DbContext>(provider => provider.GetService<EFCoreDbContext>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetService<EFCoreDbContext>());

            return services;
        }
    }
}
