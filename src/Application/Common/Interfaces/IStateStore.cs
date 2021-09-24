using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Interfaces
{
    public interface IStateStore<T>
    {
        Task<T> GetOrAddAsync(string key, T add, CancellationToken cancellationToken = default);

        Task<T> GetAsync(string key, CancellationToken cancellationToken = default);

        Task AddAsync(string key, T item, CancellationToken cancellationToken = default);

        Task DeleteAsync(string key, CancellationToken cancellationToken = default);

        Task<T> GetOrAddAsync(Guid key, T add, CancellationToken cancellationToken = default)
            => GetOrAddAsync(key.ToString(), add, cancellationToken);

        Task<T> GetAsync(Guid key, CancellationToken cancellationToken = default)
            => GetAsync(key.ToString(), cancellationToken);

        Task AddAsync(Guid key, T item, CancellationToken cancellationToken = default)
            => AddAsync(key.ToString(), item, cancellationToken);

        Task DeleteAsync(Guid key, CancellationToken cancellationToken = default)
            => DeleteAsync(key.ToString(), cancellationToken);
    }
}
