using Microsoft.EntityFrameworkCore;
using NetCoreCleanArchitecture.Application.Common.Repositories;
using NetCoreCleanArchitecture.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NetCoreCleanArchitecture.Persistence.EFCore.Repositories
{
    public class EFCoreDbContext : DbContext, IUnitOfWork
    {
        public EFCoreDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        public ICommandRepository<TEntity> CommandSet<TEntity>() where TEntity : Entity
            => new DbContextCommandRepository<TEntity>(this);

        public IQueryRepository<TEntity> QuerySet<TEntity>() where TEntity : Entity
            => new DbContextQueryRepository<TEntity>(this);

        public IEnumerable<Entity> ChangeTracking()
            => ChangeTracker.Entries<Entity>().Select(e => e.Entity);
    }
}
