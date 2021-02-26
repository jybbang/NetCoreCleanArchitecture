using DaprCleanArchitecture.Application.Common.EventSources;
using DaprCleanArchitecture.Application.Common.Interfaces;
using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Infrastructure.DateTimeCaches;
using DaprCleanArchitecture.Infrastructure.DomainEventSources;
using DaprCleanArchitecture.Infrastructure.Mappers;
using DaprCleanArchitecture.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DaprCleanArchitecture.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Dapr
            //services.AddDaprClient();

            services.AddTransient(typeof(IStateStore<>), typeof(StateStore<>));

            // DataTimeCaches
            services.AddTransient<IDateTimeCache, DateTimeCache>();

            // Domain EventSources
            services.AddTransient<IDomainEventSource, DomainEventSource>();

            // Mappers
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IMapper, Mapper>();

            // Repositories
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<EntityFrameworkDbContext>(options =>
                {
                    options.UseInMemoryDatabase(nameof(DaprCleanArchitecture));
                });
            }
            else if (configuration.GetValue<bool>("UseFileDatabase"))
            {
                services.AddDbContext<EntityFrameworkDbContext>(options =>
                {
                    options.UseSqlite(
                        configuration.GetConnectionString("File"),
                        b => b.MigrationsAssembly(typeof(EntityFrameworkDbContext).Assembly.FullName));
                    EntityFramework.Exceptions.Sqlite.ExceptionProcessorExtensions.UseExceptionProcessor(options);
                });
            }
            else
            {
                services.AddDbContext<EntityFrameworkDbContext>(options =>
                {
                    options.UseNpgsql(
                        configuration.GetConnectionString("Default"),
                        b => b.MigrationsAssembly(typeof(EntityFrameworkDbContext).Assembly.FullName));
                    EntityFramework.Exceptions.PostgreSQL.ExceptionProcessorExtensions.UseExceptionProcessor(options);
                });
            }

            services.AddScoped<DbContext>(provider => provider.GetService<EntityFrameworkDbContext>());

            services.AddScoped<IUnitOfWork>(provider => provider.GetService<EntityFrameworkDbContext>());

            services.AddTransient(typeof(ICommandRepository<>), typeof(DbContextCommandRepository<>));

            services.AddTransient(typeof(IQueryRepository<>), typeof(DbContextQueryRepository<>));

            return services;
        }
    }
}
