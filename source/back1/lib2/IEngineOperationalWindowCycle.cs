namespace lib2;

public interface IEngineOperationalWindow : System.IDisposable
{
    System.Threading.Tasks.Task Open(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken);
    void /*System.Threading.Tasks.Task*/ Close(/*System.IServiceProvider services, System.Threading.CancellationToken stoppingToken*/);
}