using work1;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using static System.Console;

class FileLogger<T> : ILogger<T>, System.IDisposable
{
    private string logfile;
    private static readonly object sync = new();

    private System.Diagnostics.TextWriterTraceListener listener;
    private System.IO.StreamWriter listenerWriter;

    public FileLogger(string file)
    {
        logfile = file;
#if TRACE
        var logname = logfile;
        System.Diagnostics.Trace.Listeners.Clear();
        System.IO.FileInfo listenerFile = new(logname);
        listenerWriter = listenerFile.CreateText();
        System.Diagnostics.TextWriterTraceListener listener = new(listenerWriter);
        System.Diagnostics.Trace.Listeners.Add(listener);
        System.Diagnostics.Trace.AutoFlush = true;
        System.Diagnostics.Trace.WriteLine($"Underlying logger type: {nameof(System.Diagnostics.TextWriterTraceListener)}");
#else
        WriteLogline(logfile, $"Underlying logger type: {nameof(System.IO.File)}.{nameof(System.IO.File.AppendAllText)}");
#endif
    }
    #region ILogger<T>
    public System.IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter)
    {
        if (formatter == null) return;
        System.Text.StringBuilder logline = new($"{logLevel} {System.DateTime.Now:s} {formatter(state, exception)}");
        for (int level = 0; exception != null; exception = exception.InnerException, ++level) logline.AppendFormat("\n[Level {0}] {1}: {2} {3}", level, exception.GetType().FullName, exception.Message, exception.StackTrace);
#if TRACE
        System.Diagnostics.Trace.WriteLine($"{logline}");
#else
        lock (sync)
        {
            WriteLogline(logfile, $"{logline}");
        }
#endif
    }
    #endregion

    private void WriteLogline(string logfile, string logline) => System.IO.File.AppendAllText(logfile, $"{logline}\n");

    #region Disposable support
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            if (listener != null && System.Diagnostics.Trace.Listeners.Contains(listener))
            {
                System.Diagnostics.Trace.Listeners.Remove(listener);
                listener?.Dispose();
                listenerWriter?.Dispose();
                listener = null;
                listenerWriter = null;
            }

            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~FileLogger()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
    #endregion
}

class FileLoggerProvider<T> : ILoggerProvider
{
    private List<FileLogger<T>> loggers;

    public FileLoggerProvider()
    {
        loggers = [];
    }

    public string LogFile { get; internal set; }

    #region ILoggerProvider
    public ILogger CreateLogger(string categoryName)
    {
        FileLogger<T> result = new(LogFile);
        loggers.Add(result);
        return result;
    }

    public void Dispose() { loggers.ForEach(x => x.Dispose()); WriteLine($"Disposed {loggers.Count} loggers."); }
    #endregion
}

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
        var logfile = System.IO.Path.Combine(System.Environment.CurrentDirectory, $"worker_{System.DateTime.Now:yyyyMMdd-HHmmss}.log");
        WriteLine($"{nameof(factory)}: {factory?.GetType().FullName}");
        WriteLine($"{nameof(logfile)}: {logfile}");
        //using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        FileLoggerProvider<Worker> filelogger_provider = new() { LogFile = logfile };
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