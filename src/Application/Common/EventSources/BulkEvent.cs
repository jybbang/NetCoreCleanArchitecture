using System.Collections.Generic;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class BulkEvent : BaseEvent
    {
        public BulkEvent(string topic)
        {
            Topic = topic;
        }

        public IList<object> DomainEvents { get; set; } = null!;
    }
}
