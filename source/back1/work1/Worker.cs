namespace work1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDisposable _logger_owner, _logger_owne2;
    private readonly ILoggerFactory _logger_factory;

    public Worker(ILogger<Worker> logger, IDisposable logger_owner, IDisposable logger_owne2, ILoggerFactory logfactory)
    {
        _logger = logger;
        _logger_owner = logger_owner;
        _logger_owne2 = logger_owne2;
        _logger_factory = logfactory;
    }

    ~Worker()
    {
        _logger?.LogInformation("{what} finalized at: {time}", nameof(Worker), DateTimeOffset.Now);
        Dispose(false);
    }

    public override void Dispose()
    {
        _logger.LogInformation("{what} disposed at: {time}", nameof(Worker), DateTimeOffset.Now);
        Dispose(true);
        base.Dispose();
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("{what} disposing({disposing}) at: {time}", nameof(Worker), disposing, DateTimeOffset.Now);
        IDisposable disposable = _logger as IDisposable;
        if (disposable != null) _logger.LogInformation("{what} disposing logger ({disposing}) at: {time}", nameof(Worker), disposing, DateTimeOffset.Now);
        disposable?.Dispose();
        _logger_factory.Dispose();
        _logger_owner.Dispose();
        _logger_owne2.Dispose();
    }

    //https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.backgroundservice.executeasync?view=net-8.0
    //https://github.com/dotnet/runtime/blob/c4d7f7c6f2e2f34f07e64c6caa3bf9b2ce915cc1/src/libraries/Microsoft.Extensions.Hosting.Abstractions/src/BackgroundService.cs
    //https://learn.microsoft.com/en-us/dotnet/core/extensions/workers
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{what} executed at: {time}", nameof(Worker), DateTimeOffset.Now);
        _logger.LogInformation(_logger.GetType().FullName);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("{what} running at: {time}", nameof(Worker), DateTimeOffset.Now);
            }

            ILogger<Engine> enginelogger = _logger_factory.CreateLogger<Engine>();
            using Engine engine = new(enginelogger);
            engine.Execute();

            await Task.Delay(4000, stoppingToken);
            _logger.LogInformation("next: {time}", DateTimeOffset.Now);
        }
        await Task.Delay(4000, stoppingToken);
        _logger.LogInformation("{what} ended at: {time}", nameof(Worker), DateTimeOffset.Now);
        await Task.Delay(4000);
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("started at: {time}", DateTimeOffset.Now);
        await base.StartAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("stopped at: {time}", DateTimeOffset.Now);
        await base.StopAsync(stoppingToken);
    }
}