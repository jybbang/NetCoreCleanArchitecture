using System;

namespace NetCoreCleanArchitecture.Application.Common.Options
{
    public record EventBufferServiceOptions
    {
        public string Topic { get; init; } = "BufferedEvents";

        public int BufferCount { get; init; } = 1000;

        public TimeSpan BufferTime { get; init; } = TimeSpan.FromMilliseconds(1000);
    }
}
