using System;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class BufferedDomainEvent : DomainEvent
    {
        protected BufferedDomainEvent(string topic) : base(topic)
        {
        }

        public int BufferCount { get; set; } = 1000;

        public TimeSpan BufferTime { get; set; } = TimeSpan.FromMilliseconds(1000);

        public TimeSpan PublishTimeout { get; set; } = TimeSpan.FromMilliseconds(10000);
    }
}
