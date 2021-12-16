using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Repositories
{
    public interface IApplicationDbContext
    {
        ICommandRepository<TEntity> CommandSet<TEntity>() where TEntity : Entity;

        IQueryRepository<TEntity> QuerySet<TEntity>() where TEntity : Entity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
