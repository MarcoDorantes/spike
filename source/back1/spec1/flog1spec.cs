namespace flog1spec;

using System;
using System.IO;
using Microsoft.Extensions.Logging;

[TestClass, TestCategory("Component")]
public class app1spec
{
    private static string folder;

    [ClassInitialize]
    public static void Init(TestContext context)
    {
        folder = context.DeploymentDirectory;
    }

    [TestMethod]
    public void LogFileCreated()
    {
        Microsoft.Extensions.Logging.ILogger<app1spec> logger = null;
        flog1.FileLoggerProvider<app1spec> filelogger_provider = null;
        try
        {
            FileInfo logfile = new(Path.Combine(folder, $"{nameof(app1spec)}_{DateTime.Now:yyyyMMdd-HHmmss}.log"));
            filelogger_provider = new(logfile.FullName);
            using Microsoft.Extensions.Logging.ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddProvider(filelogger_provider));
            logger = logfactory.CreateLogger<app1spec>();

            //Act
            var file_precondition = logfile.Exists;

            Assert.IsTrue(file_precondition);
        }
        finally
        {
            IDisposable disposable = logger as IDisposable;
            if (disposable != null) logger.LogInformation("disposing logger at: {time}", DateTimeOffset.Now);
            disposable?.Dispose();
            filelogger_provider.Dispose();
        }
    }
}