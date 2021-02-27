using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Persistence.EFCore;
using NetCoreCleanArchitecture.Persistence.EFCore.Repositories;

namespace NetCoreCleanArchitecture.Persistence.Npgsql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetCoreCleanArchitectureNpgsql(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EFCoreDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(EFCoreDbContext).Assembly.FullName));
                EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions.UseExceptionProcessor(options);
            });

            services.AddEFCore();

            return services;
        }
    }
}
