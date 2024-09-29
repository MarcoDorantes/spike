namespace work1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("started at: {time}", DateTimeOffset.Now);
        _logger.LogInformation(_logger.GetType().FullName);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(4000, stoppingToken);
            _logger.LogInformation("next: {time}", DateTimeOffset.Now);
        }
        _logger.LogInformation("ended at: {time}", DateTimeOffset.Now);
    }
}