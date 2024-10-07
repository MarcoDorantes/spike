namespace lib2;

public class EngineOperationalWindowCycle : IEngineOperationalWindowCycle
{
    public async System.Threading.Tasks.Task Start(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

    public async System.Threading.Tasks.Task Stop(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

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