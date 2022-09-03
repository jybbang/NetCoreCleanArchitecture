using System.Threading;
using System.Threading.Tasks;
using NetCoreCleanArchitecture.Application.EventSources;
using NetCoreCleanArchitecture.Application.Identities;
using NetCoreCleanArchitecture.Domain.Common;
using Results.Fluent;

namespace NetCoreCleanArchitecture.Application.Common.Dummies
{
    public class DummyEventBus : IEventBus
    {
        public ValueTask PublishAsync<TDomainEvent>(TDomainEvent message, CancellationToken cancellationToken) where TDomainEvent : BaseEvent
        {
            return new ValueTask();
        }
    }
}
