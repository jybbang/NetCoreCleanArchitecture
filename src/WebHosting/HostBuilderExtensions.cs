using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using System;

namespace NetCoreCleanArchitecture.WebHosting
{
    public static class HostBuilderExtensions
    {
        private const string APP_NAME_SECTION = "AppName";
        private const string APP_ID_SECTION = "AppId";
        private const string SEQ_URL_SECTION = "SeqServerUrl";

        public static IHostBuilder UseSerilog(this IHostBuilder builder, string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var appName = configuration.GetValue<string>(APP_NAME_SECTION);
            var appId = configuration.GetValue<string>(APP_ID_SECTION);
            var serverUrl = configuration.GetValue<string>(SEQ_URL_SECTION);

            Log.Logger = new LoggerConfiguration().ReadFrom
                .Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithClientIp()
                .Enrich.WithSpan()
                .Enrich.WithProperty("ApplicationName", string.IsNullOrWhiteSpace(appId) ? string.IsNullOrWhiteSpace(appName) ? "unknown" : appName : appId )
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(serverUrl) ? "http://seq" : serverUrl)
                .CreateLogger();

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }
    }
}
