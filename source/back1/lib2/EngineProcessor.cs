namespace lib2;

using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

public class EngineProcessor : IEngineProcessor
{
    protected readonly ILogger<EngineProcessor> _logger;

    public EngineProcessor(ILogger<EngineProcessor> logger)
    {
        _logger = logger;
    }

    public async System.Threading.Tasks.Task Execute(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} executed at {time}", GetType().Name, DateTimeOffset.Now);
        using IEngineOperationalWindowCycle cycle = services.GetRequiredService<IEngineOperationalWindowCycle>();
        await cycle.Start(services, stoppingToken);
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
        await cycle.Stop(services, stoppingToken);
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
    ~EngineProcessor()
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