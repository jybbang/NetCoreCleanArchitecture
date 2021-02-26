using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace DaprCleanArchitecture.Hosting
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseSerilog(this IHostBuilder builder, string appName)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("AppSettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true,
                    true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration().ReadFrom
                .Configuration(configuration)
                .Enrich.WithProperty("ApplicationContext", appName)
                .CreateLogger();

            SerilogHostBuilderExtensions.UseSerilog(builder);

            return builder;
        }
    }
}
