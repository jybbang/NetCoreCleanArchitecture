using System;

namespace NetCoreCleanArchitecture.Interface.Http.Options
{
    public record ApiOptions
    {
        public string AppId { get; init; } = Guid.NewGuid().ToString();

        public string AppName { get; init; } = "unknown";

        public string Version { get; init; } = "1.0.0";

        public string? SeqServerUrl { get; init; }

        public string? SeqApiKey { get; init; }
    }
}
