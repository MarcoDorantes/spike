namespace lib2;

public class EngineProcessor : IEngineProcessor
{
    public async System.Threading.Tasks.Task Start(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }

    public async System.Threading.Tasks.Task Stop(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
    }
}