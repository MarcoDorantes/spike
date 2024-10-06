namespace work2;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

public class Worker : BackgroundService
{
    protected readonly ILogger<Worker> _logger;
    protected readonly IClass1 _c;
    protected readonly System.IServiceProvider _services;

    public Worker(ILogger<Worker> logger, IClass1 c, System.IServiceProvider services)
    {
        _logger = logger;
        _c=c;
        _services=services;
    }

    ~Worker()
    {
        _logger?.LogInformation("{what} finalized at: {time}", GetType().Name, DateTimeOffset.Now);
        Dispose(false);
    }
    public override void Dispose()
    {
        _logger.LogInformation("{what} disposed at: {time}", GetType().Name, DateTimeOffset.Now);
        Dispose(true);
        base.Dispose();
      //GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("{what} disposing({disposing}) at: {time}", GetType().Name, disposing, DateTimeOffset.Now);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} executed at: {time}", GetType().Name, DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{what} running at: {time}", GetType().Name, DateTimeOffset.Now);
            }
            //https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-basics
            //https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines
            //https://code.visualstudio.com/docs/getstarted/tips-and-tricks#_preventing-dirty-writes

            IClass1 c2 = _services.GetRequiredService<IClass1>();
            var same = object.ReferenceEquals(_c,c2);
            _logger.LogInformation("{what} same: {same}", GetType().Name, same);
            
            await Task.Delay(1000, stoppingToken);
        }
        await Task.Delay(4000, stoppingToken);
        _logger.LogInformation("{what} ended at: {time}", GetType().Name, DateTimeOffset.Now);
        await Task.Delay(4000);
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} started at: {time}", GetType().Name, DateTimeOffset.Now);
        await base.StartAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} stopped at: {time}", GetType().Name, DateTimeOffset.Now);
        await base.StopAsync(stoppingToken);
    }
}

public class Worker2 : Worker
{
    public Worker2(ILogger<Worker> logger, IClass1 c, System.IServiceProvider services):base(logger, c, services) {}
}