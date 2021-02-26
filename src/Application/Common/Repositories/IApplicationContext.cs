using DaprCleanArchitecture.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.Repositories
{
    public interface IApplicationContext
    {
        ICommandRepository<TodoItem> TodoItemCommands { get; }

        IQueryRepository<TodoItem> TodoItemQueries { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
