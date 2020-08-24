using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;

namespace UnitTestProject1
{
  [TestClass]
  public class RX_Spec
  {
    /*

http://reactivex.io
http://reactivex.io/intro.html
http://reactivex.io/documentation/operators.html

https://github.com/Reactive-Extensions/Rx.NET
https://msdn.microsoft.com/en-us/library/hh242985(v=vs.103).aspx
https://msdn.microsoft.com/en-us/library/system.reactive.linq.observable(v=vs.103).aspx

Reactive Extensions (Rx) v2.0
https://www.microsoft.com/en-us/download/details.aspx?id=30708

Reactive Extensions (Rx) - Main Library 3.1.1
https://www.nuget.org/packages/System.Reactive/
     */
    [TestMethod]
    public void basic0()
    {
      var t = new List<int>();
      var s = new List<int>();
      bool done1 = false;
      bool done2 = false;

      IObservable<int> source = System.Reactive.Linq.Observable.Range(1, 5);
      IDisposable subscription1 = source.Subscribe(n => t.Add(n), () => done1 = true);
      subscription1.Dispose();
      IDisposable subscription2 = source.Subscribe(n => s.Add(n), () => done2 = true);
      subscription2.Dispose();

      Assert.IsTrue(done1);
      Assert.IsTrue(done2);
      Assert.AreEqual<int>(5, t.Count);
      Assert.AreEqual<int>(5, s.Count);

      Trace.WriteLine(this.GetType().Assembly.GetReferencedAssemblies().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}\n", n.FullName)).ToString());
      Assert.AreEqual<string>("System.Reactive.Linq.Observable", typeof(System.Reactive.Linq.Observable).FullName);
      Assert.IsTrue(this.GetType().Assembly.GetReferencedAssemblies().Any(a => a.FullName.StartsWith("System.Reactive")));
    }
    class A
    {
      private System.Reactive.Subjects.IConnectableObservable<long> hot_prices;
      public A()
      {
        ColdPrices = System.Reactive.Linq.Observable.Timer(TimeSpan.Parse("00:00:02"), TimeSpan.Parse("00:00:03"));
        ColdPriceTimes= System.Reactive.Linq.Observable.Timestamp(ColdPrices);
        HotPrices = hot_prices = System.Reactive.Linq.Observable.Publish(System.Reactive.Linq.Observable.Interval(TimeSpan.FromSeconds(2)));

        IncomingValues = new BlockingCollection<int>();
        //ReadValues = System.Reactive.Linq.Observable.Publish(read_values().ToObservable());
        ReadValues = read_values().ToObservable();
      }
      public IObservable<long> ColdPrices { get; private set; }
      public IObservable<System.Reactive.Timestamped<long>> ColdPriceTimes { get; private set; }
      public IObservable<long> HotPrices { get; private set; }
      public IObservable<int> ReadValues { get; private set; }
      public BlockingCollection<int> IncomingValues { get; private set; }
      public void StartHotPrices() { hot_prices.Connect(); }
      private IEnumerable<int> read_values()
      {
        foreach (int n in IncomingValues.GetConsumingEnumerable())
        {
          yield return n;
        }
      }
    }
    [TestMethod]
    public void basic1()
    {
      var t = new List<long>();
      var s = new List<long>();

      A a = new A();
      IDisposable subscription1 = a.ColdPrices.Subscribe(n => t.Add(n));
      IDisposable subscription2 = a.ColdPrices.Subscribe(n => s.Add(n));

      Thread.Sleep(12000);
      subscription1.Dispose();
      subscription2.Dispose();

      Assert.AreEqual<int>(4, t.Count);
      Assert.AreEqual<long>(6, t.Sum());
      Assert.AreEqual<string>("0,1,2,3,", t.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
      Assert.AreEqual<int>(4, s.Count);
      Assert.AreEqual<long>(6, s.Sum());
      Assert.AreEqual<string>("0,1,2,3,", s.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
    }
    [TestMethod]
    public void basic2()
    {
      var t = new System.Collections.Concurrent.BlockingCollection<long>();

      IObserver<long> observer = System.Reactive.Observer.Create<long>(n => t.Add(n));

      A a = new A();
      IDisposable subscription1 = a.ColdPrices.Subscribe(observer);
      IDisposable subscription2 = a.ColdPrices.Subscribe(observer);

      Thread.Sleep(12000);
      subscription1.Dispose();
      subscription2.Dispose();

      Assert.AreEqual<int>(8, t.Count);
      Assert.AreEqual<long>(12, t.Sum());
      Assert.AreEqual<string>("0,0,1,1,2,2,3,3,", t.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
    }
    [TestMethod]
    public void basic3()
    {
      var t = new List<long>();

      IObserver<long> observer = System.Reactive.Observer.Create<long>(n => t.Add(n));

      A a = new A();
      IDisposable subscription1 = a.ColdPrices.Subscribe(observer);
      IDisposable subscription2 = a.ColdPriceTimes.Subscribe(n=>Trace.WriteLine($"{n.Value} {n.Timestamp}"));

      Thread.Sleep(12000);
      subscription1.Dispose();
      subscription2.Dispose();

      Assert.AreEqual<int>(4, t.Count);
      Assert.AreEqual<long>(6, t.Sum());
      Assert.AreEqual<string>("0,1,2,3,", t.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
    }
    [TestMethod]
    public void basic4()
    {
      var t = new List<long>();
      var s = new List<long>();

      A a = new A();
      IDisposable subscription1 = a.HotPrices.Subscribe(n => t.Add(n));
      a.StartHotPrices();
      Thread.Sleep(6000);
      IDisposable subscription2 = a.HotPrices.Subscribe(n => s.Add(n));
      Thread.Sleep(5000);

      subscription1.Dispose();
      subscription2.Dispose();

      Assert.AreEqual<int>(5, t.Count);
      Assert.AreEqual<long>(10, t.Sum());
      Assert.AreEqual<string>("0,1,2,3,4,", t.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
      Assert.AreEqual<int>(3, s.Count);
      Assert.AreEqual<long>(9, s.Sum());
      Assert.AreEqual<string>("2,3,4,", s.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString());
    }
    [TestMethod]
    public void basic5()
    {
      var t = new List<long>();
      A a = new A();
      Enumerable.Range(0, 50).ToList().ForEach(n => a.IncomingValues.Add(n));
      a.IncomingValues.CompleteAdding();
      IDisposable subscription = a.ReadValues.Subscribe(n => t.Add(n));
      subscription.Dispose();
      Assert.AreEqual<int>(50, t.Count);
    }
  }
}
/*
Each package is licensed to you by its owner. NuGet is not responsible for, nor does it grant any licenses to, third-party packages. Some packages may include dependencies which are governed by additional licenses. Follow the package source (feed) URL to determine any dependencies.

Package Manager Console Host Version 3.4.4.1321

Type 'get-help NuGet' to see all available NuGet commands.

PM> Install-Package System.Reactive
Attempting to gather dependency information for package 'System.Reactive.3.1.1' with respect to project 'UnitTestProject1', targeting '.NETFramework,Version=v4.6.1'
Attempting to resolve dependencies for package 'System.Reactive.3.1.1' with DependencyBehavior 'Lowest'
Resolving actions to install package 'System.Reactive.3.1.1'
Resolved actions to install package 'System.Reactive.3.1.1'
Adding package 'System.Reactive.Interfaces.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Interfaces.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Interfaces.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Interfaces 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Core.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Core.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Core.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Core 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Linq.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Linq.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Linq.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Linq 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.PlatformServices.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.PlatformServices.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.PlatformServices.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.PlatformServices 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Windows.Threading.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Windows.Threading.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Windows.Threading.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Windows.Threading 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.3.1.1', which only has dependencies, to project 'UnitTestProject1'.
Adding package 'System.Reactive.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive 3.1.1' to UnitTestProject1
*/
