namespace NetCoreCleanArchitecture.Infrastructure.Dapr.Options
{
    public record DaprOptions
    {
        public string PubSubName { get; init; } = "eventbus";

        public string StoreName { get; init; } = "statestore";
    }
}
