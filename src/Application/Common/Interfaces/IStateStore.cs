using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Interfaces
{
    public interface IStateStore<T>
    {
        Task<T> GetOrAddAsync(string key, T add, CancellationToken cancellationToken);

        Task<T> GetAsync(string key, CancellationToken cancellationToken);

        Task AddAsync(string key, T item, CancellationToken cancellationToken);

        Task DeleteAsync(string key, CancellationToken cancellationToken);

        Task<T> GetOrAddAsync(Guid key, T add, CancellationToken cancellationToken)
            => GetOrAddAsync(key.ToString(), add, cancellationToken);

        Task<T> GetAsync(Guid key, CancellationToken cancellationToken)
            => GetAsync(key.ToString(), cancellationToken);

        Task AddAsync(Guid key, T item, CancellationToken cancellationToken)
            => AddAsync(key.ToString(), item, cancellationToken);

        Task DeleteAsync(Guid key, CancellationToken cancellationToken)
            => DeleteAsync(key.ToString(), cancellationToken);
    }
}
