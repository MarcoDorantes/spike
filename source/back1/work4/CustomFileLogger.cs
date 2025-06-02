using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class CustomFileLogger : ILogger
{
    private readonly string _categoryName;
    private readonly string _filePath;
    private readonly LogLevel _minLogLevel;
    private static readonly object _lock = new object(); // For thread-safe file writing

    public CustomFileLogger(string categoryName, string filePath, LogLevel minLogLevel)
    {
        _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _minLogLevel = minLogLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null; // Not implementing scopes for this simple logger
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _minLogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        if (formatter == null)
        {
            throw new ArgumentNullException(nameof(formatter));
        }

        var message = formatter(state, exception);
        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel.ToString().ToUpper()}] {_categoryName}: {message}";

        if (exception != null)
        {
            logEntry += Environment.NewLine + exception.ToString();
        }

        lock (_lock)
        {
            File.AppendAllText(_filePath, logEntry + Environment.NewLine);
        }
    }
}