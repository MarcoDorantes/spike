using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Diagnostics;

namespace UnitTestProject1
{
  [TestClass]
  public class RSS_Feed_Spec
  {
    [TestMethod]
    public void vsnews()
    {
      var feed_url = "http://sxp.microsoft.com/feeds/msdntn/VisualStudioNews";
      var feed_uri = new Uri(feed_url);
      var rss = XDocument.Load(feed_uri.AbsoluteUri);
      foreach (var item in rss.Root.Descendants("item"))
      {
        Trace.WriteLine(item.Element("title").Value);
        Trace.WriteLine(item.Element("link").Value);
        Trace.WriteLine(item.Element("pubDate").Value);
      }
    }
    [TestMethod]
    public void csharpnews()
    {
      var feed_url = "http://sxp.microsoft.com/feeds/3.0/msdntn/CSharpHeadlines";
      var feed_uri = new Uri(feed_url);
      var rss = XDocument.Load(feed_uri.AbsoluteUri);
      foreach (var item in rss.Root.Descendants("item"))
      {
        Trace.WriteLine(item.Element("title").Value);
        Trace.WriteLine(item.Element("link").Value);
        Trace.WriteLine(item.Element("pubDate").Value);
      }
    }
  }
}