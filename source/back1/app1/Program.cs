namespace app1;

using System;
using System.Threading;
using System.Threading.Tasks;

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
        // Stop called without start
        if (_executeTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method
            cancel?.Cancel();
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
    public static void _Main(CancellationToken cancel)
    {
        A a = new();
        Task t = a.StartAsync(cancel);
        t.Wait();
    }
}

class Exe
{
    static async Task Main(string[] args)
    {
        try
        {
            lib1.Class1 x = new();
            await x.Start(args);

            CancellationTokenSource cancel = new();
            Task t = Task.Run(() => { Thread.Sleep(3000); cancel.Cancel(); });
            Program._Main(cancel.Token);
            t.Wait();
        }
        catch (Exception ex)
        {
            for(int level=0; ex!=null; ex=ex.InnerException,++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
        }
    }
}