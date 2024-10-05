namespace app1;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using static System.Console;

abstract class Base
{
    private Task _executeTask;
    private CancellationTokenSource cancel;

    protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        cancel = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executeTask = ExecuteAsync(cancellationToken);

        // If the task is completed then return it, this will bubble cancellation and failure to the caller
        if (_executeTask.IsCompleted)
        {
            return _executeTask;
        }

        // Otherwise it's running
        return Task.CompletedTask;
    }
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        WriteLine($"{nameof(StopAsync)} started.");
        // Stop called without start
        if (_executeTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            cancel?.Cancel();
            WriteLine($"{nameof(StopAsync)} cancelled.");
        }
        finally
        {
#if NET8_0_OR_GREATER
            await _executeTask.WaitAsync(cancellationToken).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
#else
                // Wait until the task completes or the stop token triggers
                var tcs = new TaskCompletionSource<object>();
                using CancellationTokenRegistration registration = cancellationToken.Register(s => ((TaskCompletionSource<object>)s!).SetCanceled(), tcs);
                // Do not await the _executeTask because cancelling it will throw an OperationCanceledException which we are explicitly ignoring
                await Task.WhenAny(_executeTask, tcs.Task).ConfigureAwait(false);
#endif
        }
        WriteLine($"{nameof(StopAsync)} ended.");
    }
}

class A : Base
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => work(stoppingToken));
    }

    private void work(CancellationToken stoppingToken)
    {
        do
        {
            if (stoppingToken.IsCancellationRequested) break;
            Thread.Sleep(1000);
        } while (true);
    }
}

class Program
{
    public static A worker;
    public static void _Main(CancellationToken cancel)
    {
        worker = new();
        worker.StartAsync(cancel);
    }
}

class App1
{
    static async Task Main(string[] args)
    {
        try
        {
            var logfile = System.IO.Path.Combine(Environment.CurrentDirectory, $"{nameof(App1)}_{DateTime.Now:yyyyMMdd-HHmmss}.log");
            flog1.FileLoggerProvider<App1> filelogger_provider = new(logfile);
            using Microsoft.Extensions.Logging.ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => {builder.AddConsole(); builder.AddProvider(filelogger_provider);});
            Microsoft.Extensions.Logging.ILogger<App1> logger = logfactory.CreateLogger<App1>();

            lib1.Class1 x = new(logger);
            await x.Start(args);

            CancellationTokenSource cancel = new();
            Program._Main(cancel.Token);
            await Task.Delay(3000);
            cancel.Cancel();
            await Program.worker.StopAsync(cancel.Token);
            IDisposable disposable = logger as IDisposable;
            if (disposable != null) logger.LogInformation("disposing logger at: {time}", DateTimeOffset.Now);
            disposable?.Dispose();
            filelogger_provider.Dispose();
        }
        catch (Exception ex)
        {
            for(int level=0; ex!=null; ex=ex.InnerException,++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
        }
    }
}