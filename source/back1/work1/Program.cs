using work1;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

class Exe
{
    static void Main(string[] args)
    {
        System.Func<System.IServiceProvider, Worker> implementationFactory = CreateSimilarAsFound;

        var builder = Host.CreateApplicationBuilder(args);
        //builder.Services.AddHostedService<Worker>();
        builder.Services.AddHostedService<Worker>(implementationFactory);

        var host = builder.Build();
        host.Run();
    }
    static Worker CreateSimilarAsFound(System.IServiceProvider factory)
    {
        System.Console.WriteLine($"{nameof(factory)}: {factory?.GetType().FullName}");
        using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<Worker> logger = logfactory.CreateLogger<Worker>();
        Worker result = new(logger);
        return result;
    }
}/*
https://learn.microsoft.com/en-us/dotnet/core/extensions/logging
logger.LogInformation("Hello World! Logging is {Description}.", "fun");
https://stackoverflow.com/questions/40073743/how-to-log-to-a-file-without-using-third-party-logger-in-net-core

https://learn.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance
*/