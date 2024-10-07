namespace lib2;

public interface IEngineOperationalWindowCycle : System.IDisposable
{
    System.Threading.Tasks.Task Start(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken);
    System.Threading.Tasks.Task Stop(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken);
}