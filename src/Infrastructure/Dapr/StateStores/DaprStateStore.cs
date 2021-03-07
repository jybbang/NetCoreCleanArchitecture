
using Dapr.Client;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores
{
    public class DaprStateStore<T> : IStateStore<T>
    {
        private const string STORE_NAME = "statestore";

        private readonly DaprClient _client;

        public DaprStateStore(DaprClient client)
        {
            _client = client;
        }

        public Task<T> GetAsync(string key, CancellationToken cancellationToken = default)
            => _client.GetStateAsync<T>(STORE_NAME, key, cancellationToken: cancellationToken);

        public Task AddAsync(string key, T item, CancellationToken cancellationToken = default)
            => _client.SaveStateAsync(STORE_NAME, key, item, cancellationToken: cancellationToken);

        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
            => _client.DeleteStateAsync(STORE_NAME, key, cancellationToken: cancellationToken);
    }
}
