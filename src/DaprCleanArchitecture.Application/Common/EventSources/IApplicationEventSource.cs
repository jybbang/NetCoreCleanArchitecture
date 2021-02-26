using DaprCleanArchitecture.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DaprCleanArchitecture.Application.Common.EventSources
{
    public interface IApplicationEventSource
    {
        long Published { get; }

        Task Publish<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : DomainEvent;
    }
}