using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IEventStore
    {
        Task PublishEvent<T>(T message, CancellationToken cancellationToken = default) where T : DomainEvent;
    }
}
