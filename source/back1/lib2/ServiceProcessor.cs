namespace lib2;

using Microsoft.Extensions.DependencyInjection;

public class ServiceProcessor : IServiceProcessor
{
    public async System.Threading.Tasks.Task Execute(System.IServiceProvider services, System.Threading.CancellationToken stoppingToken)
    {
        IEngineProcessor engine = services.GetRequiredService<IEngineProcessor>();
        await engine.Start(services, stoppingToken);
        await System.Threading.Tasks.Task.Delay(System.Threading.Timeout.Infinite, stoppingToken);
        await engine.Stop(services, stoppingToken);
    }
}