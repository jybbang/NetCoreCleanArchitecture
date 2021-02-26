using DaprCleanArchitecture.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.Repositories
{
    public interface IUnitOfWork
    {
        IEnumerable<Entity> ChangeTracking();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
