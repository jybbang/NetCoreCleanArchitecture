using System;

namespace NetCoreCleanArchitecture.Domain.Common
{
    public abstract class BufferedEvent : BaseEvent
    {
        public int BufferCount { get; protected set; } = 1000;

        public TimeSpan BufferTime { get; protected set; } = TimeSpan.FromMilliseconds(1000);

        public TimeSpan PublishTimeout { get; protected set; } = TimeSpan.FromMilliseconds(10000);
    }
}
