namespace lib2;

using System;
using Microsoft.Extensions.Logging;

public class EngineOperationalWindow(ILogger<EngineOperationalWindow> _logger) : IEngineOperationalWindow
{
    /*protected readonly ILogger<EngineOperationalWindow> _logger;

    public EngineOperationalWindow(ILogger<EngineOperationalWindow> logger)
    {
        _logger = logger;
    }*/

    public async System.Threading.Tasks.Task Open(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} opened at {time:yyyy-MM-dd HH:mm:ss.fffffff}", GetType().Name, DateTimeOffset.Now);
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

    public /*async*/ void Close(/*System.IServiceProvider services, System.Threading.CancellationToken stoppingToken*/)
    {
        _logger.LogInformation("{what} closed at {time:yyyy-MM-dd HH:mm:ss.fffffff}", GetType().Name, DateTimeOffset.Now);
    }

    #region Disposable support
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("{what} disposed({disposing}) at {time:yyyy-MM-dd HH:mm:ss.fffffff}", GetType().Name, disposing, DateTimeOffset.Now);
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            Close();
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~EngineOperationalWindow()
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