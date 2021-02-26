using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace DaprCleanArchitecture.Infrastructure
{
    public static class HostExetensions
    {
        public static void UseMigration(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<DbContext>();

                if (dbContext.Database.IsInMemory()) return;

                dbContext.Database.Migrate();
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
