namespace flog1;

using Microsoft.Extensions.Logging;

public class FileLogger<T> : ILogger<T>, System.IDisposable
{
    private string logfile;
    private static readonly object sync;

    private System.Diagnostics.TextWriterTraceListener listener;
    private System.IO.StreamWriter listenerWriter;

    static FileLogger()
    {
        sync = new();
    }

    public FileLogger(string file, string categoryName)
    {
        logfile = file;
#if TRACE
        var logname = logfile;
        System.Diagnostics.Trace.Listeners.Clear();
        System.IO.FileInfo listenerFile = new(logname);
        listenerWriter = listenerFile.CreateText();
        listener = new(listenerWriter);
        //listener.WriteLine(
        System.Diagnostics.Trace.Listeners.Add(listener);
        System.Diagnostics.Trace.AutoFlush = true;
        System.Diagnostics.Trace.WriteLine($"Underlying logger type: {nameof(System.Diagnostics.TextWriterTraceListener)}");
        System.Diagnostics.Trace.WriteLine($"{nameof(categoryName)}: [{categoryName}]");//[work1.Worker]
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