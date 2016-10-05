using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

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

    [TestMethod]
    public void addnews()
    {
      var target_text = @"<?xml version='1.0' encoding='utf-8'?>
<rss xmlns:a10='http://www.w3.org/2005/Atom' version='2.0'>
  <channel xmlns:sxp='http://sxpdata.microsoft.com/sxp'>
    <title>Visual Studio News</title>
    <item>
      <guid isPermaLink='false'>5C405304-E713-47B1-9C1A-D595382DFE59</guid>
      <title>Animations with the Visual Layer (UWP)</title>
      <description>When the layout of your Universal Windows Platform (UWP) app changes, there is often a slight pause as your app rearranges content to fit the new window size or orientation. Composition APIs let you create smooth-as-butter animations between these states so your layout changes won’t jar your users. After all, layout changes are a basic fact of the app life cycle. It doesn’t mean we can’t handle them gracefully.</description>
      <pubDate>Fri, 30 Sep 2016 18:34:50 Z</pubDate>
      <link>http://aka.ms/Fqs8i5</link>
    </item>
    <item>
      <guid isPermaLink='false'>943E6A37-9623-4235-8390-AFE5FA796C3E</guid>
      <title>Smooth Interaction and Motion with the Visual Layer (UWP)</title>
      <description>The Composition APIs come with a robust animation engine that provides quick and fluid motion running in a separate process from your Universal Windows Platform (UWP) app. This provides a consistent 60 frames per second when running your app on an IoT device as well as on a screaming gaming machine. It is, quite simply, fast.</description>
      <pubDate>Thu, 22 Sep 2016 17:54:36 Z</pubDate>
      <link>http://aka.ms/Jmoyam</link>
    </item>
  </channel>
</rss>";

      var source_text = @"<?xml version='1.0' encoding='utf-8'?>
<rss xmlns:a10='http://www.w3.org/2005/Atom' version='2.0'>
  <channel xmlns:sxp='http://sxpdata.microsoft.com/sxp'>
    <title>Visual Studio News</title>
    <item>
      <guid isPermaLink='false'>C4951352-655E-4D02-A056-734EA0980AAC</guid>
      <title>Pricing for Release Management in TFS “15”</title>
      <description>Since the new version of Release Management was introduced in TFS 2015 Update 2, it has been in “trial mode“. Any user with Basic access level was able to access all features of Release Management. For the last few months, we have been hard at work to finalize the pricing model for Release Management in time for the release of TFS “15” RTM.</description>
      <pubDate>Tue, 04 Oct 2016 17:32:18 Z</pubDate>
      <link>http://aka.ms/U7doln</link>
    </item>
    <item>
      <guid isPermaLink='false'>5C405304-E713-47B1-9C1A-D595382DFE59</guid>
      <title>Animations with the Visual Layer (UWP)</title>
      <description>When the layout of your Universal Windows Platform (UWP) app changes, there is often a slight pause as your app rearranges content to fit the new window size or orientation. Composition APIs let you create smooth-as-butter animations between these states so your layout changes won’t jar your users. After all, layout changes are a basic fact of the app life cycle. It doesn’t mean we can’t handle them gracefully.</description>
      <pubDate>Fri, 30 Sep 2016 18:34:50 Z</pubDate>
      <link>http://aka.ms/Fqs8i5</link>
    </item>
  </channel>
</rss>";

      var target = XDocument.Load(new StringReader(target_text));
      var source = XDocument.Load(new StringReader(source_text));

      Assert.AreEqual<int>(2, target.Root.Descendants("item").Count());
      Assert.AreEqual<int>(2, source.Root.Descendants("item").Count());
    }
  }
}