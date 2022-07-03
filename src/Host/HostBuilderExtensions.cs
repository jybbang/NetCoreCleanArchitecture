using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NetCoreCleanArchitecture.Host.Options;
using Serilog;
using Serilog.Core;
using Serilog.Enrichers.Span;
using System;

namespace NetCoreCleanArchitecture.Host
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

            var opt = configuration.GetValue<ApiOptions>("Api") ?? new ApiOptions();

            var levelSwitch = new LoggingLevelSwitch();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithClientIp()
                .Enrich.WithSpan()
                .Enrich.WithProperty("ApplicationName", string.IsNullOrWhiteSpace(opt.AppId) ? opt.AppName : opt.AppId)
                .WriteTo.Console();

            if (!string.IsNullOrEmpty(opt.SeqServerUrl))
            {
                loggerConfiguration = loggerConfiguration.WriteTo.Seq(opt.SeqServerUrl, apiKey: opt.SeqApiKey, controlLevelSwitch: levelSwitch);
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }
    }
}
