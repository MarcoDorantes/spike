using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;

namespace UnitTestProject1
{
    static class WriterLogCopy
    {
        const int TimeoutAfter = 20 * 60 * 60 * 1000; // 20 minutes

        public static string GetSourceFilePattern(string filename_start, string source_folder, DateTime? date = null)
        {
            var when = date.HasValue ? date.Value : DateTime.Now.Date;
            return $"{filename_start}_{when.ToString("MMMdd")}*.log";
        }
        //public static FileInfo GetFileInfoFor(string filename_start, string source_folder, DateTime? date = null)
        //{
        //    var when = date.HasValue ? date.Value : DateTime.Now.Date;
        //    var path = Path.Combine(source_folder, $"{filename_start}_{DateTime.Now.ToString("MMMdd")}*.log");
        //    return new FileInfo(path);
        //}
        public static IDictionary<FileInfo, KeyValuePair<FileInfo, Process>> Copy(string[][] sources, string target_folder, DateTime? date = null)
        {
            var copied_sources = new Dictionary<FileInfo, KeyValuePair<FileInfo, Process>>();
            foreach (var pair in sources)
            {
                string filename_start = pair[0];
                string source_folder = pair[1];
                Trace.WriteLine($"To search for {filename_start} at {source_folder}.");
                var sourceSearchPattern = GetSourceFilePattern(filename_start, source_folder, date);

                Trace.WriteLine($"\tSearching for {sourceSearchPattern}...");
                var from_folder = new DirectoryInfo(source_folder);
                foreach (var sourcefile in from_folder.GetFiles(sourceSearchPattern))
                {
                    Trace.WriteLine($"\tCopying {source_folder} {target_folder} {sourceSearchPattern}...");
                    var startinfo = new ProcessStartInfo("cmd.exe", $"/c robocopy {source_folder} {target_folder} {sourceSearchPattern}");
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startinfo.UseShellExecute = false;
                    startinfo.RedirectStandardError = true;
                    startinfo.RedirectStandardOutput = true;
                    var p = new Process();
                    p.StartInfo = startinfo;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd(); //https://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput(v=vs.110).aspx
                    bool exited = p.WaitForExit(TimeoutAfter);
                    Trace.WriteLine($"\tCopy exited: {exited} ExitCode: {p.ExitCode} Output: {output}");

                    var target = new FileInfo(@"C:\some");
                    copied_sources.Add(sourcefile, new KeyValuePair<FileInfo, Process>(target, p));
                }
            }
            return copied_sources;
        }
    }
    /*
GBMSOLACEWIN02
C:\FixWriter\FolioWriter\Primary\FixWriterFolioService1_ago.03-045959-9420868.log

dir \\GBMSOLACEWIN02\C$\FixWriter\FolioWriter\Primary

GBMSOLACEWIN03
C:\FixWriter\VIRTU-DROPCOPY\Primary\FixWriter\ FixWriterVirtuDCService1_ago.03-050000-5277071.log
C:\FixWriter\VIRTU-DROPCOPY\Primary\FixWriter2
.
.
.
FixWriter5
     */
    [TestClass]
    public class aSpec_withCmd
    {
        const int timeout_after = 20 * 60 * 60 * 1000; // 20 minutes

        [TestMethod]
        public void no_source_found()
        {
            var source_folder = @"\\HOMESTATION2\C$\Users\Marco\AppData\Local\temp\source";

            var sourceSearchPattern = WriterLogCopy.GetSourceFilePattern("FixWriterFolioService1", source_folder);
            Assert.AreEqual("FixWriterFolioService1_ago.06*.log", sourceSearchPattern);

            var s = new DirectoryInfo(source_folder);
            var sourcefiles = s.GetFiles(sourceSearchPattern).ToList();
            Assert.AreEqual<int>(0, sourcefiles.Count);
        }
        [TestMethod]
        public void copy0()
        {
            try
            {
                var source_folder = @"\\HOMESTATION2\C$\Users\Marco\AppData\Local\temp\source";
                var target_folder = @"C:\Users\Marco\AppData\Local\temp\target";

                var sourceSearchPattern = WriterLogCopy.GetSourceFilePattern("FixWriterFolioService1", source_folder);
                Assert.AreEqual("FixWriterFolioService1_ago.06*.log", sourceSearchPattern);

                var copied_sources = new Dictionary<FileInfo, Process>();
                var from_folder = new DirectoryInfo(source_folder);
                foreach (var sourcefile in from_folder.GetFiles(sourceSearchPattern))
                {
                    var startinfo = new ProcessStartInfo("cmd.exe", $"/c robocopy {source_folder} {target_folder} {sourceSearchPattern}");
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startinfo.UseShellExecute = false;
                    startinfo.RedirectStandardError = true;
                    startinfo.RedirectStandardOutput = true;
                    var p = new Process();
                    p.StartInfo = startinfo;
                    p.Start();
                    string output = p.StandardOutput.ReadToEnd();
                    bool exited = p.WaitForExit(timeout_after);
                    Assert.IsTrue(exited);
                    Assert.IsTrue(p.ExitCode == 0 || p.ExitCode == 1);//https://blogs.technet.microsoft.com/deploymentguys/2008/06/16/robocopy-exit-codes/
                    Trace.WriteLine(output);
                    copied_sources.Add(sourcefile, p);
                }
                Assert.AreEqual<int>(1, copied_sources.Count);
            }
            finally{}
        }
        [TestMethod]
        public void copy1()
        {
            try
            {
                var target_folder = @"C:\Users\Marco\AppData\Local\temp\target";

                string[][] sources = {
                    new[] { "FixWriterFolioService1", @"\\HOMESTATION2\C$\Users\Marco\AppData\Local\temp\source1" },
                    new[] { "FixWriterVirtuDCService1", @"\\HOMESTATION2\C$\Users\Marco\AppData\Local\temp\source2" },
//                    new[] { "FixWriterVirtuDCService2", @"\\HOMESTATION2\C$\Users\Marco\AppData\Local\temp\source3" }
                };
                var copied = WriterLogCopy.Copy(sources, target_folder, new DateTime(2017, 8, 6));

                foreach (var copy in copied.Keys) Trace.WriteLine($"\nCopy: {copy.FullName}");
            }
            finally { }
        }
    }
}