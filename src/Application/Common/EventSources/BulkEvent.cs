using System;
using System.Collections.Generic;
using System.Text;
using NetCoreCleanArchitecture.Domain.Common;

namespace NetCoreCleanArchitecture.Application.Common.EventSources
{
    public class BulkEvent : BaseEvent
    {
        public BulkEvent(string topic) : base(topic)
        {
        }

        public IList<object> DomainEvents { get; set; } = null!;
    }
}
