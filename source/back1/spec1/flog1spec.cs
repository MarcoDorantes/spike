namespace flog1spec;

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

[TestClass, TestCategory("Component")]
public class app1spec
{
    private static string folderbase;

    [ClassInitialize]
    public static void Init(TestContext context)
    {
        folderbase = context.DeploymentDirectory;
    }

    [ClassCleanup]
    public static void UnInit() => Directory.EnumerateDirectories(folderbase).ToList().ForEach(f=>Directory.Delete(f,true));

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
                logfile = new(Path.Combine(folderbase, $"{nameof(app1spec)}_{DateTime.Now:yyyyMMdd-HHmmss}.log"));
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
    async System.Threading.Tasks.Task SendAsync(int n, string folder, string tag)
    {
        var filename=Path.Combine(folder, $"{tag}_{n}.log");
        await File.AppendAllTextAsync(filename,$"{n}");
    }
    void sendm(int n, string folder, string tag)
    {
      //var t=SendAsync(n, folder, tag);t.Wait();
      System.Threading.Tasks.Task.Run(async () => await SendAsync(n, folder, tag).ConfigureAwait(false));
    }
    [TestMethod]
    public void tstate()
    {
        var folder=Path.Combine(folderbase, nameof(tstate));
        if(Directory.Exists(folder)) Directory.Delete(folder, true);
        Directory.CreateDirectory(folder);
        for(int k=0;k<1000;++k) sendm(k, folder, nameof(tstate));
        System.Threading.Thread.Sleep(15000);
        var index = Directory.EnumerateFiles(folder,"*.log").Select(f=>int.Parse(File.ReadAllText(f))).Distinct();
        Assert.AreEqual(1000,index.Count());
        Directory.EnumerateFiles(folder,"*.log").ToList().ForEach(f=>File.Delete(f));
    }
    class Exe(string folder, string tag):IDisposable
    {
        public System.Threading.Tasks.Task task;
        public string filename;
        public async System.Threading.Tasks.Task SendAsync(int n)
        {
            filename = Path.Combine(folder, $"{tag}_{n}.log");
            await File.AppendAllTextAsync(filename,$"{n}");
        }
        public void sendm(int n)
        {
            task = System.Threading.Tasks.Task.Run(async () => await SendAsync(n).ConfigureAwait(false));
        }
        public void Dispose(){if(File.Exists(filename)) File.Delete(filename);}
    }
    [TestMethod]
    public void tstates()
    {
        var folder=Path.Combine(folderbase, nameof(tstates));
        if(Directory.Exists(folder)) Directory.Delete(folder, true);
        Directory.CreateDirectory(folder);
        var exes=Enumerable.Range(0,1000).Aggregate(new System.Collections.Generic.List<Exe>(),(whole,next)=>{whole.Add(new Exe(folder, nameof(tstates)));return whole;});
        for(int k=0;k<1000;++k) exes[k].sendm(k);
        System.Threading.Thread.Sleep(15000);
        Assert.AreEqual(1000,exes.Where(e=>e.task!=null).Count());
        System.Threading.Tasks.Task.WaitAll(exes.Where(e=>e.task!=null).Select(e=>e.task).ToArray());
        //ls F:\tep\log\tstateB*.log|cat|%{[PSCustomObject]@{Index=[int]::Parse($_)}}|select -Unique Index|measure
        var index = Directory.EnumerateFiles(folder,"*.log").Select(f=>int.Parse(File.ReadAllText(f))).Distinct();
        Assert.AreEqual(1000,index.Count());
        exes.ForEach(e=>e.Dispose());
    }
}