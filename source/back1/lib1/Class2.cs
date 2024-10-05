namespace lib1;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Class2
{
    private Microsoft.Extensions.Logging.ILogger logger;

    public Class2(Microsoft.Extensions.Logging.ILogger logger)
    {
        this.logger = logger;
    }

    public async Task Start(string[] args)
    {
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 1, $"{nameof(Class2)}.{nameof(Start)} begin", null, LogFormatter.Format);
        var file = args.FirstOrDefault();
        if (File.Exists(file))
        {
            uint count = 0U;
            string input = "";
            do
            {
                ++count;
                if (string.Compare(input, "END", true) == 0) break;
                if (string.Compare(input, "err", true) == 0) throw new Exception($"Exception at {count}");
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 2, $"[{System.Threading.Thread.CurrentThread.ManagedThreadId,2}] {DateTime.Now:s} Begin {count}", null, LogFormatter.Format);

                input = await File.ReadAllTextAsync(file);
                await Task.Delay(1000);
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 2, $"[{System.Threading.Thread.CurrentThread.ManagedThreadId,2}] {DateTime.Now:s} End   {count}", null, LogFormatter.Format);
            } while (true);
        }
        else await Task.Delay(100);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 3, $"{nameof(Class2)}.{nameof(Start)} end", null, LogFormatter.Format);
    }
}