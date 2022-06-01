namespace NetCoreCleanArchitecture.Infrastructure.Dapr.Options
{
    public record InfrastructureDaprOptions
    {
        public string PubSubName { get; init; } = "eventbus";

        public string StoreName { get; init; } = "statestore";
    }
}
