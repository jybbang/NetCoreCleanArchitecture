namespace NetCoreCleanArchitecture.Infrastructure.Dapr.Common.Options
{
    public class DaprOptions
    {
        public string PubSubName { get; set; } = "eventbus";

        public string StoreName { get; set; } = "statestore";
    }
}
