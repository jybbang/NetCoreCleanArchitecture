using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.EFCore.Repositories;

namespace NetCoreCleanArchitecture.Persistence.Npgsql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreCleanArchitectureNpgsql(this IServiceCollection services, IConfiguration configuration)
        {
            // Repositories
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<EFCoreDbContext>(options =>
                {
                    options.UseInMemoryDatabase(nameof(NetCoreCleanArchitecture));
                });
            }
            else if (configuration.GetValue<bool>("UseFileDatabase"))
            {
                services.AddDbContext<EFCoreDbContext>(options =>
                {
                    options.UseSqlite(
                        configuration.GetConnectionString("File"),
                        b => b.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName));
                    EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(options);
                });
            }
            else
            {
                services.AddDbContext<EFCoreDbContext>(options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString("Default"),
                        b => b.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName));
                    EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions.UseExceptionProcessor(options);
                });
            }

            services.AddScoped<DbContext>(provider => provider.GetService<EFCoreDbContext>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetService<EFCoreDbContext>());

            return services;
        }
    }
}
