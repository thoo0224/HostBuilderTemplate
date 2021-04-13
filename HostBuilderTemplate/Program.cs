using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Core;
using Serilog.Events;

using System;
using System.Threading.Tasks;

namespace HostBuilderTemplate
{
    public static class Program
    {

        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            Log.Logger = CreateLoggerConfiguration();
            try
            {
                await CreateHostBuilder(args, configuration).Build().RunAsync();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Host terminated unexpectedly ({Message})", e.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfigurationRoot configuration)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(configuration);
                })
                .UseSerilog();
        }

        public static ILogger CreateLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", new LoggingLevelSwitch(LogEventLevel.Warning))
                .MinimumLevel.Override("System", new LoggingLevelSwitch(LogEventLevel.Warning))
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", new LoggingLevelSwitch())
                .WriteTo.Console(
                    theme: Globals.ConsoleTheme,
                    outputTemplate: "[{Timestamp:G}] [{Level:u3}] {Message:l}{NewLine}")
                .CreateLogger();
        }

    }
}
