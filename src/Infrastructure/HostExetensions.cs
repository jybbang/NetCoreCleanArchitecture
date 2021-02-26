using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Infrastructure.Repositories;
using DaprCleanArchitecture.Infrastructure.Repositories.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Infrastructure
{
    public static class HostExetensions
    {
        public static async Task UseMigration(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                Migrate(services);

                await Seed(services);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<EntityFrameworkDbContext>>();

                logger.LogError(ex, "An error occurred while migrating or seeding the database.");

                throw;
            }
        }

        private static void Migrate(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<DbContext>();

            if (dbContext.Database.IsInMemory()) return;

            dbContext.Database.Migrate();
        }

        private static Task Seed(IServiceProvider services)
        {
            var context = services.GetRequiredService<IApplicationContext>();

            return ApplicationContextSeed.CreateSeedData(context);
        }
    }
}
