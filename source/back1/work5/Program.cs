using work5;
/*
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run(); */

//GitHub Copilot Chat GPT-4.1: How to implement a simple custom file .NET ILogger for a .NET Worker app?

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddProvider(new FileLoggerProvider("logs.txt"));
        // Optionally add other loggers, e.g., logging.AddConsole();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();