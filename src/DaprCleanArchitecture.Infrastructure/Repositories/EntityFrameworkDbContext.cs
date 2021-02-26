using DaprCleanArchitecture.Application.Common.Repositories;
using DaprCleanArchitecture.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DaprCleanArchitecture.Infrastructure.Repositories
{
    public class EntityFrameworkDbContext : DbContext, IUnitOfWork
    {
        public EntityFrameworkDbContext(DbContextOptions options)
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
