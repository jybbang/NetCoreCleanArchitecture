using System;

namespace NetCoreCleanArchitecture.Interface.Http.Options
{
    public record ApiOptions
    {
        public string AppName { get; init; } = "unknown";

        public string AppId { get; init; } = Guid.NewGuid().ToString();

        public string? SeqServerUrl { get; init; }

        public string? SeqApiKey { get; init; }
    }
}
