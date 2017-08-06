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
        public static string GetSourceFilePattern(string filename_start, string source_folder, DateTime? date = null)
        {
            var when = date.HasValue ? date.Value : DateTime.Now.Date;
            return $"{filename_start}_{DateTime.Now.ToString("MMMdd")}*.log";
        }
        //public static FileInfo GetFileInfoFor(string filename_start, string source_folder, DateTime? date = null)
        //{
        //    var when = date.HasValue ? date.Value : DateTime.Now.Date;
        //    var path = Path.Combine(source_folder, $"{filename_start}_{DateTime.Now.ToString("MMMdd")}*.log");
        //    return new FileInfo(path);
        //}
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
            Assert.AreEqual<int>(0,sourcefiles.Count);
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
                    var startinfo = new ProcessStartInfo("cmd.exe",$"/c robocopy {source_folder} {target_folder} {sourceSearchPattern}");
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                    var p = new Process();
                    p.StartInfo = startinfo;
                    p.Start();
                    bool exited = p.WaitForExit(timeout_after);
                    Assert.IsTrue(exited);
                    Assert.IsTrue(p.ExitCode == 0 || p.ExitCode == 1);//https://blogs.technet.microsoft.com/deploymentguys/2008/06/16/robocopy-exit-codes/
                    copied_sources.Add(sourcefile, p);
                }
                Assert.AreEqual<int>(1, copied_sources.Count);
            }
            finally
            {
            }
        }
    }
}