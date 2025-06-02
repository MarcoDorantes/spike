using Microsoft.Extensions.Logging;

using System;
using System.IO;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly string _categoryName;

    public FileLogger(string filePath, string categoryName)
    {
        _filePath = filePath;
        _categoryName = categoryName;
    }

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
        if (exception != null)
            message += Environment.NewLine + exception;

        File.AppendAllText(_filePath, message + Environment.NewLine);
    }
}