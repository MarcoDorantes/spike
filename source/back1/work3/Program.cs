using work3;
/*
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run(); */

//Copilot: How to implement a simple custom file .NET ILogger for a .NET Worker app?

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<ILoggerProvider>(provider => new FileLoggerProvider("logs.txt"));
    })
    .Build();

await host.RunAsync();