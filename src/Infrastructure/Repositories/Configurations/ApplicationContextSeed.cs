using DaprCleanArchitecture.Application.Common.Repositories;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Infrastructure.Repositories.Configurations
{
    public static class ApplicationContextSeed
    {
        public static Task CreateSeedData(this IApplicationContext context)
        {
            // Create Seed
            return Task.CompletedTask;
        }
    }
}
