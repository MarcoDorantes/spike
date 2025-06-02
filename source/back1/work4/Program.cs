using work4;
/*
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run(); */

//Gemini: How to implement a simple custom file .NET ILogger for a .NET Worker app?

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders(); // Clear existing providers (e.g., console)
                loggingBuilder.SetMinimumLevel(LogLevel.Information); // Set global minimum level

                // Add your custom file logger
                loggingBuilder.AddCustomFileLogger("app.log", LogLevel.Information);

                // You can also add other loggers if needed (e.g., Console logger)
                loggingBuilder.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
            });
}