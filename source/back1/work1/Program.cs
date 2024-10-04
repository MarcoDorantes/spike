using work1;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using static System.Console;

class Exe
{
    static void Main(string[] args)
    {
        System.Func<System.IServiceProvider, Worker> implementationFactory = CreateWithFileLogger;

        var builder = Host.CreateApplicationBuilder(args);
        //builder.Services.AddHostedService<Worker>();
        builder.Services.AddHostedService<Worker>(implementationFactory);

        var host = builder.Build();
        host.Run();
    }
    /*static Worker CreateSimilarAsFound(System.IServiceProvider factory)
    {
        WriteLine($"{nameof(factory)}: {factory?.GetType().FullName}");
        using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        ILogger<Worker> logger = logfactory.CreateLogger<Worker>();
        Worker result = new(logger);
        return result;
    }*/
    static Worker CreateWithFileLogger(System.IServiceProvider factory)
    {
        var logfile = System.IO.Path.Combine(System.Environment.CurrentDirectory, $"worker_{System.DateTime.Now:yyyyMMdd-HHmmss}.log");
        WriteLine($"{nameof(factory)}: {factory?.GetType().FullName}");
        WriteLine($"{nameof(logfile)}: {logfile}");
        flog1.FileLoggerProvider<Worker> filelogger_provider = new() { LogFile = logfile };
        using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddProvider(filelogger_provider));
        ILogger<Worker> logger = logfactory.CreateLogger<Worker>();
        Worker result = new(logger, filelogger_provider);
        return result;
    }
}/*
https://learn.microsoft.com/en-us/dotnet/core/extensions/logging
logger.LogInformation("Hello World! Logging is {Description}.", "fun");
https://stackoverflow.com/questions/40073743/how-to-log-to-a-file-without-using-third-party-logger-in-net-core

https://learn.microsoft.com/en-us/dotnet/standard/generics/covariance-and-contravariance
*/