
using Dapr.Client;
using Microsoft.Extensions.Options;
using NetCoreCleanArchitecture.Application.Common.Interfaces;
using NetCoreCleanArchitecture.Infrastructure.Dapr.Options;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.StateStores
{
    public class DaprStateStore<T> : IStateStore<T>
    {
        private readonly InfrastructureDaprOptions _opt;
        private readonly DaprClient _client;

        public DaprStateStore(IOptions<InfrastructureDaprOptions> opt, DaprClient client)
        {
            _opt = opt.Value;
            _client = client;
        }

        public Task<T> GetAsync(string key, CancellationToken cancellationToken = default)
            => _client.GetStateAsync<T>(_opt.StoreName, key, cancellationToken: cancellationToken);

        public Task AddAsync(string key, T item, CancellationToken cancellationToken = default)
            => _client.SaveStateAsync(_opt.StoreName, key, item, cancellationToken: cancellationToken);

        public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
            => _client.DeleteStateAsync(_opt.StoreName, key, cancellationToken: cancellationToken);
    }
}
