using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.StateStores
{
    public interface IStateStore<T>
    {
        Task<T> GetOrCreateAsync(string key, Func<Task<T>> factory, CancellationToken cancellationToken);

        Task<T> GetAsync(string key, CancellationToken cancellationToken);

        Task AddAsync(string key, T item, CancellationToken cancellationToken);

        Task RemoveAsync(string key, CancellationToken cancellationToken);
    }
}
