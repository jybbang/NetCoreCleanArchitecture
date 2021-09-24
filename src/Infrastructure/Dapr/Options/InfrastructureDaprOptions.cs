using System;

namespace NetCoreCleanArchitecture.Infrastructure.Dapr.Options
{
    public record InfrastructureDaprOptions
    {
        public bool UseDatetimeCache { get; init; } = false;

        public string DatetimeKey { get; init; } = "now";

        public string PubSubName { get; init; } = "eventbus";

        public string StoreName { get; init; } = "statestore";
    }
}
