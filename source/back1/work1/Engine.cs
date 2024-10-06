namespace work1;

using System;
using Microsoft.Extensions.Logging;

internal class Engine : IDisposable
{
    private ILogger<Engine> _logger;

    public Engine(ILogger<Engine> logger)
    {
        _logger = logger;
    }

    ~Engine()
    {
        _logger?.LogInformation("{what} finalized at: {time}", nameof(Engine), DateTimeOffset.Now);
        Dispose(false);
    }

    public virtual void Dispose()
    {
        _logger.LogInformation("{what} disposed at: {time}", nameof(Engine), DateTimeOffset.Now);
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    protected virtual void Dispose(bool disposing)
    {
        _logger.LogInformation("{what} disposing({disposing}) at: {time}", nameof(Engine), disposing, DateTimeOffset.Now);
        IDisposable disposable = _logger as IDisposable;
        if (disposable != null) _logger.LogInformation("{what} disposing logger ({disposing}) at: {time}", nameof(Engine), disposing, DateTimeOffset.Now);
        disposable?.Dispose();
//        _logger_factory.Dispose();
//        _logger_owner.Dispose();
    }

    public void Execute()=> _logger.LogInformation("{what} running at: {time}", nameof(Engine), DateTimeOffset.Now);
}