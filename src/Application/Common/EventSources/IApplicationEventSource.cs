using NetCoreCleanArchitecture.Domain.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public interface IApplicationEventSource
    {
        Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, DateTimeOffset timestamp, CancellationToken cancellationToken) where TDomainEvent : DomainEvent;
    }
}