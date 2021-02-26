
using Dapr.Client;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores
{
    public class StateStore<T> : IStateStore<T>
    {
        private readonly DaprClient _client;

        public StateStore(DaprClient client)
        {
            _client = client;
        }

        public Task<T> GetAsync(string key, CancellationToken cancellationToken = default)
            => _client.GetStateAsync<T>(typeof(T).Name, key, cancellationToken: cancellationToken);

        public Task AddAsync(string key, T item, CancellationToken cancellationToken = default)
            => _client.SaveStateAsync(typeof(T).Name, key, item, cancellationToken: cancellationToken);

        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
            => _client.DeleteStateAsync(typeof(T).Name, key, cancellationToken: cancellationToken);
    }
}
