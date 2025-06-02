using Microsoft.Extensions.Logging;
using System;

public static class CustomFileLoggerExtensions
{
    public static ILoggingBuilder AddCustomFileLogger(this ILoggingBuilder builder, string filePath, LogLevel minLogLevel)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.AddProvider(new CustomFileLoggerProvider(filePath, minLogLevel));
        return builder;
    }
}