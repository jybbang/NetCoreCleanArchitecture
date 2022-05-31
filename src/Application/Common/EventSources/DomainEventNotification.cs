using MediatR;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public TDomainEvent DomainEvent { get; }

        public DomainEventNotification(TDomainEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
