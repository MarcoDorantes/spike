namespace flog1;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

public class FileLoggerProvider<T> : ILoggerProvider
{
    private List<FileLogger<T>> loggers;
    private Func<string, string> logfilenameformatter;
    public FileLoggerProvider(Func<string, string> nameformatter)
    {
        loggers = [];
        //LogFile = logfile;
        logfilenameformatter = nameformatter;
    }

    //public string LogFile { get; private set; }

    #region ILoggerProvider
    public ILogger CreateLogger(string categoryName)
    {
        var LogFile = logfilenameformatter($"{categoryName}{Math.Abs(GetHashCode())}");
        FileLogger<T> result = new(LogFile, categoryName);
        loggers.Add(result);
        return result;
    }

    public void Dispose() { loggers.ForEach(x => x.Dispose()); System.Console.WriteLine($"Disposed {loggers.Count} loggers."); }
    #endregion
}