using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IApplicationEventSource
    {
        Task Publish<T>(T domainEvent, CancellationToken cancellationToken = default) where T : DomainEvent;
    }
}