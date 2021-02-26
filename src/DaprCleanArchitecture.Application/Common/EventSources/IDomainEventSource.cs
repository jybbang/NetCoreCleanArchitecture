using DaprCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.EventSources
{
    public interface IDomainEventSource
    {
        Task PublishEvent<T>(T message, CancellationToken cancellationToken = default) where T : DomainEvent;
    }
}
