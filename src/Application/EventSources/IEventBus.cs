using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.EventSources
{
    public interface IEventBus
    {
        ValueTask PublishAsync<TDomainEvent>(string topic, TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent;
    }
}
