namespace lib2;

using System;
using Microsoft.Extensions.Logging;

public class EngineOperationalWindowCycle : IEngineOperationalWindowCycle
{
    protected readonly ILogger<EngineOperationalWindowCycle> _logger;

    public EngineOperationalWindowCycle(ILogger<EngineOperationalWindowCycle> logger)
    {
        _logger = logger;
    }

    public async System.Threading.Tasks.Task Open(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} started at {time}", GetType().Name, DateTimeOffset.Now);
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

    public async System.Threading.Tasks.Task Close(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} stopped at {time}", GetType().Name, DateTimeOffset.Now);
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

    #region Disposable support
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("{what} disposed({disposing}) at {time}", GetType().Name, disposing, DateTimeOffset.Now);
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer

            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~EngineOperationalWindowCycle()
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