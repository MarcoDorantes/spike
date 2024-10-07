namespace lib2;

public interface IEngineProcessor : System.IDisposable
{
    System.Threading.Tasks.Task Execute(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken);
}