using NetCoreCleanArchitecture.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Repositories
{
    public interface IUnitOfWork
    {
        ICommandRepository<TEntity> CommandSet<TEntity>() where TEntity : Entity;

        IQueryRepository<TEntity> QuerySet<TEntity>() where TEntity : Entity;

        IEnumerable<Entity> ChangeTracking();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
