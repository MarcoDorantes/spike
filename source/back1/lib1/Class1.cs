namespace lib1;

using System;
using System.Threading.Tasks;

public class Class1
{
    private Microsoft.Extensions.Logging.ILogger logger;

    public Class1(Microsoft.Extensions.Logging.ILogger logger)
    {
        this.logger = logger;
    }

    public async Task Start(string[] args)
    {
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 1, $"{nameof(Class1)}.{nameof(Start)} begin", null, LogFormatter.Format);
        Class2 y = new(logger);
        do
        {
            try
            {
                await y.Start(args);
                break;
            }
            catch (Exception ex)
            {
                logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 2, $"{ex.GetType().FullName}: {ex.Message}", null, LogFormatter.Format);
            }
        } while (true);
        logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, 3, $"{nameof(Class1)}.{nameof(Start)} end", null, LogFormatter.Format);
    }
}