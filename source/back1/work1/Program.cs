using work1;

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

class FileLogger<T> : ILogger<T>
{
    private string logfile;
    private static readonly object sync = new();
    public FileLogger(string file)
    {
        logfile = file;
    }
    #region ILogger<T>
    public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (formatter == null) return;
        lock (sync)
        {
            System.Text.StringBuilder logline = new($"{logLevel} {DateTime.Now:s} {formatter(state, exception)}");
            for (int level = 0; exception != null; exception = exception.InnerException, ++level) logline.AppendFormat("\n[Level {0}] {1}: {2} {3}", level, exception.GetType().FullName, exception.Message, exception.StackTrace);
            System.IO.File.AppendAllText(logfile, $"{logline}\n");
        }
    }
    #endregion
}

class FileLoggerProvider<T> : ILoggerProvider
{
    public string LogFile { get; internal set; }

    #region ILoggerProvider
    public ILogger CreateLogger(string categoryName) => new FileLogger<T>(LogFile);

    public void Dispose(){}
    #endregion
}

class Exe
{
    static void Main(string[] args)
    {
        Func<IServiceProvider, Worker> implementationFactory = CreateSimilarAsFound;

        var builder = Host.CreateApplicationBuilder(args);
        //builder.Services.AddHostedService<Worker>();
        builder.Services.AddHostedService<Worker>(implementationFactory);

        var host = builder.Build();
        host.Run();
    }
    static Worker CreateSimilarAsFound(System.IServiceProvider factory)
    {
        var logfile = System.IO.Path.Combine(System.Environment.CurrentDirectory, $"worker_{DateTime.Now:yyyyMMdd-HHmmss}.log");
        Console.WriteLine($"{nameof(factory)}: {factory?.GetType().FullName}");
        Console.WriteLine($"{nameof(logfile)}: {logfile}");
        //using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddProvider(new FileLoggerProvider<Worker>() { LogFile = logfile }));
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