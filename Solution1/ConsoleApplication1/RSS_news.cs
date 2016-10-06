﻿//tools\getWebfile.exe http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews vsnews_03Oct2016.xml
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
      r.Aggregate(result, (w, n) => { w.Root.Element("channel").Add(XElement.Parse(n.ToString())); return w; });
      return result;
    }
  }

  class RSS_news
  {
    public static void _Main(string[] args)
    {
      string sourceURL = "http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews";
      string targetfile = $"{Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])}_target.xml";

      //if (args.Length != 1) throw new Exception("No source file specified.");
      //var sourcefile = args[0];
      //if (!File.Exists(sourcefile)) throw new Exception($"No source file found: {sourcefile}");
      var source = XDocument.Load(sourceURL);
      WriteLine($"Source text received: {source}");
      if (!File.Exists(targetfile)) throw new Exception($"No target file found: {targetfile}");
      var target = XDocument.Load(targetfile);
      Write($"Target file: {targetfile}...");
      var m = new guid_based_comparer();
      var merged = m.merge(source, target);
      merged.Save(targetfile);
      WriteLine("merged.");
      var next = @"tools\windiff afile_copy.xml vsnews_merge_target.xml";
      System.Windows.Clipboard.SetText(next);
      WriteLine(next);
    }
  }
}