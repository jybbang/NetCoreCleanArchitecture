namespace NetCoreCleanArchitecture.WebHosting.Options
{
    public record NetCoreCleanArchitectureOptions
    {
        public string AppName { get; init; } = "unknown";

        public string AppId { get; init; }

        public string SeqServerUrl { get; init; } = "http://seq";

        public string SeqApiKey { get; init; }
    }
}
