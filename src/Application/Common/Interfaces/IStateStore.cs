using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.Interfaces
{
    public interface IStateStore<T>
    {
        Task<T> GetAsync(string key, CancellationToken cancellationToken = default);

        Task AddAsync(string key, T item, CancellationToken cancellationToken = default);

        Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    }
}
