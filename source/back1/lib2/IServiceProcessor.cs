namespace lib2;

public interface IServiceProcessor : System.IDisposable
{
    System.Threading.Tasks.Task Execute(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken);
}