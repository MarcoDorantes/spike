//tools\getWebfile.exe http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews vsnews_03Oct2016.xml
//tools\getWebfile.exe https://vsstartpage.blob.core.windows.net/news/vs vsnews_27Jan2017.xml
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
    public bool Equals(XElement x, XElement y) => x.Element("link").Value == y.Element("link").Value;

    public int GetHashCode(XElement x) => x.Element("link").Value.GetHashCode();

    public XDocument merge(XDocument source, XDocument target)
    {
      var r = source.Root.Descendants("item").Except(target.Root.Descendants("item"), this);
      var result = XDocument.Parse(target.ToString());
      r.Aggregate(result, (w, n) => { var @new = XElement.Parse(n.ToString()); w.Root.Element("channel").Add(@new); WriteLine($"\t{@new.Element("link").Value}\t{@new.Element("title").Value}"); return w; });
      return result;
    }
  }

  class RSS_news
  {
    static void add(string sourceURL, string targetfile)
    {try{
      var source = XDocument.Load(sourceURL);
      WriteLine($"\nSource received: {source.Root.Element("channel").Element("title").Value} ({source.Root.Element("channel").Descendants("item").Count()} items)");
      if (!File.Exists(targetfile)) throw new Exception($"No target file found: {targetfile}");
      var target = XDocument.Load(targetfile);
      WriteLine($"Target file: {targetfile}...");
      var m = new guid_based_comparer();
      var merged = m.merge(source, target);
      merged.Save(targetfile);
      WriteLine("merged.");
}catch(Exception ex){WriteLine($"Not merged: {ex.GetType().FullName}: {ex.Message} on URL:\n{sourceURL}");}
    }
    public static void _Main(string[] args)
    {
      try
      {
        string targetfile = $"{Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])}_target.xml";
        if (args.Length == 0)
        {
          /*
https://go.microsoft.com/fwlink/p/?LinkId=659113&clcid=409
https://blogs.msdn.microsoft.com/visualstudio/
https://blogs.msdn.microsoft.com/visualstudio/feed/

HKEY_CURRENT_USER\SOFTWARE\Microsoft\VisualStudio\14.0_Config\StartPage
https://go.microsoft.com/fwlink/?linkid=87676&clcid=%VSSPV_LCID_HEX%
https://go.microsoft.com/fwlink/?linkid=87676&clcid=409
https://vsstartpage.blob.core.windows.net/news/vs
          */

          add("http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews", targetfile);
          add("https://vsstartpage.blob.core.windows.net/news/vs", targetfile);

          var next = @"tools\windiff vsnews_merge_target.xml afile_copy.xml";
          System.Windows.Clipboard.SetText(next);
          WriteLine($"\n{next}");
        }
        else
        {
          var target = XDocument.Load(targetfile);
          WriteLine($"Links in target file: {targetfile}...");
          var list = target.Root.Descendants("item").ToList();
          list.ForEach(n => { WriteLine($"{n.Element("link").Value}\t{n.Element("title").Value}"); });
          WriteLine($"Items: {list.Count}");
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