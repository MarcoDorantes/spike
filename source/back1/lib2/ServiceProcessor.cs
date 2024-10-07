namespace lib2;

using Microsoft.Extensions.DependencyInjection;

public class ServiceProcessor : IServiceProcessor
{
    public async System.Threading.Tasks.Task Execute(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        using IEngineProcessor engine = services.GetRequiredService<IEngineProcessor>();
        await engine.Execute(services, stoppingToken);
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
    ~ServiceProcessor()
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