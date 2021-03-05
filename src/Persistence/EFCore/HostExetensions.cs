using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace NetCoreCleanArchitecture.Persistence.EFCore
{
    public static class HostExetensions
    {
        public enum MigrationOptions
        {
            None,
            EnsureCreated,
            Migrate
        }

        public static void UseMigration(this IHost host, MigrationOptions options = MigrationOptions.Migrate)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<DbContext>();

                if (dbContext.Database.IsInMemory()) return;

                switch (options)
                {
                    case MigrationOptions.EnsureCreated:
                        dbContext.Database.EnsureCreated();
                        break;
                    case MigrationOptions.Migrate:
                        dbContext.Database.Migrate();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbContext>>();

                logger.LogError(ex, "An error occurred while migrating the database.");

                throw;
            }
        }
    }
}
