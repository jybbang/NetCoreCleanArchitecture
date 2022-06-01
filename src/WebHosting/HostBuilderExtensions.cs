using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCoreCleanArchitecture.WebHosting.Options;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Span;
using System;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder builder, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var opt = configuration.GetValue<NetCoreCleanArchitectureOptions>("NetCoreCleanArchitecture");

            var levelSwitch = new LoggingLevelSwitch();

            Log.Logger = new LoggerConfiguration().ReadFrom
                .Configuration(configuration)
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithClientIp()
                .Enrich.WithSpan()
                .Enrich.WithProperty("ApplicationName", string.IsNullOrWhiteSpace(opt.AppId) ? opt.AppName : opt.AppId)
                .WriteTo.Console()
                .WriteTo.Seq(opt.SeqServerUrl, apiKey: opt.SeqApiKey, controlLevelSwitch: levelSwitch)
                .CreateLogger();

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }
    }
}
