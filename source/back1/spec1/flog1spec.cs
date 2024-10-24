namespace flog1spec;

using System;
using System.IO;
using System.Linq;
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

    [TestMethod, Ignore]
    public void LogFileCreated()
    {
        Microsoft.Extensions.Logging.ILogger<app1spec> logger = null;
        flog1.FileLoggerProvider<app1spec> filelogger_provider = null;
        try
        {
            FileInfo logfile = null;
            string filename_formatter(string category)
            {
                logfile = new(Path.Combine(folder, $"{nameof(app1spec)}_{DateTime.Now:yyyyMMdd-HHmmss}.log"));
                return logfile.FullName;
            }
            filelogger_provider = new(filename_formatter);
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
    async System.Threading.Tasks.Task SendAsync(int n)
    {
        await System.IO.File.AppendAllTextAsync(@"C:\temp\log\tstate.log",$"{n}\n");
    }
    void sendm(int n)
    {
      //var t=SendAsync(n);t.Wait();
      //System.Threading.Tasks.Task.Run(async () => await SendAsync(n).ConfigureAwait(false));
      //System.IO.File.AppendAllText(@"C:\temp\log\tstate0.log",$"{n}\n");
    }
    [TestMethod, Ignore]
    public void tstate()
    {
        for(int k=0;k<1000;++k) sendm(k);
        System.Threading.Thread.Sleep(15000);
    }
    class Exe
    {
        public System.Threading.Tasks.Task task;
        public async System.Threading.Tasks.Task SendAsync(int n)
        {
            await System.IO.File.AppendAllTextAsync(@"C:\temp\log\tstate.log",$"{n}\n");
        }
        public void sendm(int n)
        {
            task = System.Threading.Tasks.Task.Run(async () => await SendAsync(n).ConfigureAwait(false));
        }
    }
    [TestMethod]
    public void tstates()
    {
        var exes=Enumerable.Range(0,1000).Aggregate(new System.Collections.Generic.List<Exe>(),(whole,next)=>{whole.Add(new Exe());return whole;});
        for(int k=0;k<1000;++k) exes[k].sendm(k);
        System.Threading.Tasks.Task.WaitAll(exes.Select(e=>e.task).ToArray());
    }
}