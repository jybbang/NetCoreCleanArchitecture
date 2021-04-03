using NetCoreCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IApplicationEventSource
    {
        Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);
    }
}