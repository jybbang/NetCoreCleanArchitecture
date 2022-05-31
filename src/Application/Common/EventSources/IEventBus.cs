using NetCoreCleanArchitecture.Domain.Common;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IEventBus
    {
        Task PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : DomainEvent;

        Task PublishAsync(string topic, IList<object> messages, CancellationToken cancellationToken);
    }
}
