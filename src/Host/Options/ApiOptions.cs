﻿using System;

namespace NetCoreCleanArchitecture.Host.Options
{
    public record ApiOptions
    {
        public string AppName { get; init; } = "unknown";

        public string AppId { get; init; } = Guid.NewGuid().ToString();

        public string? SeqServerUrl { get; init; } = "http://seq";

        public string? SeqApiKey { get; init; }
    }
}