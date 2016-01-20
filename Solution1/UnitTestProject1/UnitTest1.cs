using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.Generic;

namespace UnitTestProject1
{
  class A
  {
    public event EventHandler<DateTime> OnPulse;

    public void FireEventSync()
    {
      OnPulse?.Invoke(this, DateTime.Now);
    }
    public void FireEventAsync()
    {
      AsyncCallback callback = EndInvokeFireAsync;
      OnPulse?.BeginInvoke(this, DateTime.Now, callback, null);
    }
    private void EndInvokeFireAsync(IAsyncResult result) { }
  }
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void SyncDelegateThreading0()
    {
      //Arrange
      var queue = new ConcurrentQueue<string>();

      var a = new A();
      Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) =>
      {
        var logline = $"[{Environment.CurrentManagedThreadId}] {n} {e:s}";
        Trace.WriteLine(logline);
        Thread.Sleep(1000);
        queue.Enqueue(logline);
      });
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now:s}");

      //Act
      a.FireEventSync();

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now:s}");
      Assert.IsTrue(queue.ToList().All(n => n.StartsWith($"[{Environment.CurrentManagedThreadId}]")));
    }

    [TestMethod]
    public void SyncDelegateThreading()
    {
      //Arrange
      var queue = new ConcurrentQueue<int>();

      var a = new A();
      Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) =>
      {
        queue.Enqueue(Environment.CurrentManagedThreadId);
      });

      //Act
      a.FireEventSync();

      //Assert
      Assert.IsTrue(queue.ToList().All(n => n == Environment.CurrentManagedThreadId));
    }

    [TestMethod]
    public void AsyncDelegateThreading0()
    {
      //Arrange
      var a = new A();
      Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) => { });

      //Act
      try
      {
        a.FireEventAsync();
        Assert.Fail("Exception not thrown.");
      }
      catch (ArgumentException ex)
      {
        Assert.AreEqual<string>("The delegate must have only one target.", ex.Message);
      }
    }

    [TestMethod]
    public void AsyncDelegateThreading1()
    {
      //Arrange
      var list = new List<GeneralAlgorithm>();
      Enumerable.Range(1, 10).Aggregate(list,(whole, next) =>
      {
        whole.Add(new GeneralAlgorithm(next));
        return whole;
      });
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now:s}");

      //Act
      list.ForEach(x => x.EjecutarDelegado());
      Thread.Sleep(4000);

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now:s}");
      list.ForEach(x => Trace.WriteLine(x.Result));
    }
  }
  class GeneralAlgorithm
  {
    private AlgorithmExecutorDelegate aed;
    public GeneralAlgorithm(int n)
    {
      ID = n;
      aed = AlgorithmExecutor.Execute;
    }
    public int ID;
    public string Result;
    public virtual void EjecutarDelegado()
    {
      AsyncCallback callback = ExecuteDelegateCompleted;
      aed.BeginInvoke($"([{Environment.CurrentManagedThreadId}] BeginInvoke {ID} at {DateTime.Now:s})", callback, null);
    }
    private void ExecuteDelegateCompleted(IAsyncResult result)
    {
      try
      {
        Result = aed.EndInvoke(result);
      }
      catch (Exception ex)
      {
        Result = $"{ex.GetType().FullName}: {ex.Message}";
      }
    }
  }
  delegate string AlgorithmExecutorDelegate(string when);
  static class AlgorithmExecutor
  {
    public static string Execute(string when)
    {
      return $"[{Environment.CurrentManagedThreadId}] return: {DateTime.Now:s} {when}";
    }
  }
}