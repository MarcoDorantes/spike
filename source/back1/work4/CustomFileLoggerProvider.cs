using Microsoft.Extensions.Logging;
using System;
using System.IO;

public class CustomFileLoggerProvider : ILoggerProvider
{
    private readonly string _filePath;
    private readonly LogLevel _minLogLevel;

    public CustomFileLoggerProvider(string filePath, LogLevel minLogLevel)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _minLogLevel = minLogLevel;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new CustomFileLogger(categoryName, _filePath, _minLogLevel);
    }

    public void Dispose()
    {
        // No resources to dispose for this simple logger
    }
}