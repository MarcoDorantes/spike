//tools\getWebfile.exe http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews vsnews_03Oct2016.xml
//csc /r:"\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6\PresentationCore.dll" vsnews_merge.cs
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using static System.Console;

namespace ConsoleApplication1
{

  class guid_based_comparer : IEqualityComparer<XElement>
  {
    public bool Equals(XElement x, XElement y) => x.Element("guid").Value == y.Element("guid").Value;

    public int GetHashCode(XElement x) => x.Element("guid").Value.GetHashCode();

    public XDocument merge(XDocument source, XDocument target)
    {
      var r = source.Root.Descendants("item").Except(target.Root.Descendants("item"), this);
      var result = XDocument.Parse(target.ToString());
      r.Aggregate(result, (w, n) => { var @new = XElement.Parse(n.ToString()); w.Root.Element("channel").Add(@new); WriteLine($"{@new.Element("link").Value}\t{@new.Element("title").Value}"); return w; });
      return result;
    }
  }

  class RSS_news
  {
    public static void _Main(string[] args)
    {
      try
      {
        string targetfile = $"{Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])}_target.xml";
        if (args.Length == 0)
        {
          string sourceURL = "http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews";
          /*
https://go.microsoft.com/fwlink/p/?LinkId=659113&clcid=409
https://blogs.msdn.microsoft.com/visualstudio/
https://blogs.msdn.microsoft.com/visualstudio/feed/
          */

          //if (args.Length != 1) throw new Exception("No source file specified.");
          //var sourcefile = args[0];
          //if (!File.Exists(sourcefile)) throw new Exception($"No source file found: {sourcefile}");
          var source = XDocument.Load(sourceURL);
          WriteLine($"Source text received: {source}");
          if (!File.Exists(targetfile)) throw new Exception($"No target file found: {targetfile}");
          var target = XDocument.Load(targetfile);
          Write($"Target file: {targetfile}...");
          var m = new guid_based_comparer();
          WriteLine("\n");
          var merged = m.merge(source, target);
          merged.Save(targetfile);
          WriteLine("merged.");
          var next = @"tools\windiff vsnews_merge_target.xml afile_copy.xml";
          System.Windows.Clipboard.SetText(next);
          WriteLine(next);
        }
        else
        {
          var target = XDocument.Load(targetfile);
          WriteLine($"Links in target file: {targetfile}...");
          target.Root.Descendants("item").ToList().ForEach(n => { WriteLine($"{n.Element("link").Value}\t{n.Element("title").Value}"); });
        }
      }
      catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}

/*class exe
{
  [STAThread]
  static void Main(string[] args) { ConsoleApplication1.RSS_news._Main(args); }
}*/