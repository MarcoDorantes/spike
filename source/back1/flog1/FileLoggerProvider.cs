namespace flog1;

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class FileLoggerProvider<T> : ILoggerProvider
{
    private List<FileLogger<T>> loggers;

    public FileLoggerProvider(string logfile)
    {
        loggers = [];
        LogFile = logfile;
    }

    public string LogFile { get; private set; }

    #region ILoggerProvider
    public ILogger CreateLogger(string categoryName)
    {
        FileLogger<T> result = new(LogFile);
        loggers.Add(result);
        return result;
    }

    public void Dispose() { loggers.ForEach(x => x.Dispose()); System.Console.WriteLine($"Disposed {loggers.Count} loggers."); }
    #endregion
}