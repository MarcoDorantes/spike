using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
/*
https://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs
https://referencesource.microsoft.com/#mscorlib/system/Collections/Concurrent/ConcurrentDictionary.cs,1140
https://msdn.microsoft.com/en-us/library/dd287191(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/dd997369(v=vs.110).aspx
https://blogs.msdn.microsoft.com/pfxteam/2011/11/08/concurrentdictionary-performance-improvements-in-net-4-5
http://stackoverflow.com/questions/8225775/concurrent-dictionary-correct-usage
https://www.simple-talk.com/blogs/inside-the-concurrent-collections-concurrentdictionary
http://geekswithblogs.net/BlackRabbitCoder/archive/2010/06/09/c-4-the-curious-concurrentdictionary.aspx
*/
namespace UnitTestProject1
{
  class Suffix
  {
    public string Nick;
    public DateTime LastUpdated;
  }
  [TestClass]
  public class singlemap_Spec
  {
    [TestMethod]
    public void basics()
    {
      var map = new ConcurrentDictionary<string, Suffix>();
      Assert.IsTrue(map.TryAdd("name1", new Suffix { Nick = "1" }));
      Assert.IsTrue(map.TryAdd("name2", new Suffix { Nick = "2" }));
      Assert.IsFalse(map.TryAdd("name2", new Suffix { Nick = "1" }));

      Suffix s;
      Assert.IsTrue(map.TryGetValue("name1", out s));
      Assert.AreEqual<string>("1", s.Nick);
      Assert.IsTrue(map.TryGetValue("name2", out s));
      Assert.AreEqual<string>("2", s.Nick);
      Assert.IsFalse(map.TryGetValue("name3", out s));

      Suffix prev;
      Assert.IsTrue(map.TryGetValue("name1", out prev));
      Assert.IsTrue(map.TryUpdate("name1", new Suffix { Nick = "uno" }, prev));
      Suffix @new;
      Assert.IsTrue(map.TryGetValue("name1", out @new));
      Assert.AreEqual<string>("uno", @new.Nick);

      @new = new Suffix { Nick = "3" };
      Suffix updated = new Suffix { Nick = "3" };
      Suffix @this = map.AddOrUpdate("name3", key => @new, (key, found) => updated);
      Assert.AreSame(@new, @this);
      @this = map.AddOrUpdate("name3", @new, (key, found) => updated);
      Assert.AreSame(@updated, @this);

      @new = new Suffix { Nick = "4" };
      updated = new Suffix { Nick = "4" };
      @this = map.AddOrUpdate("name4", @new, (key, found) => { throw new Exception("is there a 4 key?"); });
      Assert.AreSame(@new, @this);
      @this = map.AddOrUpdate("name4", updated, (key, found) => updated);
      Assert.AreSame(@updated, @this);

      @new = new Suffix { Nick = "5", LastUpdated = DateTime.Now };
      @this = map.AddOrUpdate("name5", @new, (key, found) => { throw new Exception("is there a 5 key?"); });
      Assert.AreSame(@new, @this);
      Thread.Sleep(1000);
      updated = new Suffix { Nick = "5", LastUpdated = DateTime.Now };
      @this = map.AddOrUpdate("name5", updated, (key, found) => found.LastUpdated < updated.LastUpdated ? updated : found);
      Assert.AreSame(@updated, @this);
    }
  }
}

namespace InstantiationSituation
{
  class TypeA { }
  class TypeB { }
  class TypeN { }
  [TestClass]
  public class ApplicationInitializationSpec
  {
    TypeA ExternalInstantiationOfTypeA() => new TypeA();
    TypeB ExternalInstantiationOfTypeB() => new TypeB();
    TypeN ExternalInstantiationOfTypeN() => new TypeN();

    dynamic MyMagicalMethod(string typename)
    {
      switch (typename)
      {
        case "TypeA": return ExternalInstantiationOfTypeA();
        case "TypeB": return ExternalInstantiationOfTypeB();
        case "TypeN": return ExternalInstantiationOfTypeN();
        default: throw new Exception($"Unknown type: {typename}");
      }
    }
    [TestMethod]
    public void app_init()
    {
      // Arrange
      var type_list = new List<string> { "TypeA", "TypeB", "TypeN" };

      // Act
      var instance = type_list.Aggregate(new List<dynamic>(), (whole, next) => { whole.Add(MyMagicalMethod(next)); return whole; });
      TypeA a = instance.ElementAtOrDefault(0);
      TypeB b = instance.ElementAtOrDefault(1);
      TypeN n = instance.ElementAtOrDefault(2);

      // Assert
      Assert.IsNotNull(a);
      Assert.IsNotNull(b);
      Assert.IsNotNull(n);
      Assert.IsInstanceOfType(a, typeof(TypeA));
      Assert.IsInstanceOfType(b, typeof(TypeB));
      Assert.IsInstanceOfType(n, typeof(TypeN));
    }
  }
}