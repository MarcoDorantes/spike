using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

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
  public static class Constant
  {
    public const string PrecisionFormat = "yyyy-MM-ddThh-mm-ss.fffffff";
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
        var logline = $"[{Environment.CurrentManagedThreadId}] {n} {e.ToString(Constant.PrecisionFormat)}";
        Trace.WriteLine(logline);
        Thread.Sleep(1000);
        queue.Enqueue(logline);
      });
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now.ToString(Constant.PrecisionFormat)}");

      //Act
      a.FireEventSync();

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now.ToString(Constant.PrecisionFormat)}");
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
      Enumerable.Range(1, 10).Aggregate(list, (whole, next) =>
       {
         whole.Add(new GeneralAlgorithm(next));
         return whole;
       });
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now.ToString(Constant.PrecisionFormat)}");

      //Act
      list.ForEach(x => x.EjecutarDelegado());
      Thread.Sleep(4000);

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now.ToString(Constant.PrecisionFormat)}");
      list.ForEach(x => Trace.WriteLine(x.Result));
      Assert.IsTrue(list.All(n => !n.Result.StartsWith($"[{Environment.CurrentManagedThreadId}]")));
    }
    [TestMethod]
    public void AsyncDelegateThreading2()
    {
      //Arrange
      var x = new GeneralAlgorithm(123);
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now.ToString(Constant.PrecisionFormat)}");

      //Act
      for (int k = 0; k < 70; ++k)
      {
        x.AddResultInTheBackground(k);
      }
      Thread.Sleep(4000);

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now.ToString(Constant.PrecisionFormat)}");
      var results = x.Results.ToList();
      results.ForEach(_ => Trace.WriteLine(_));
      Assert.IsTrue(results.All(_ => !_.StartsWith($"[{Environment.CurrentManagedThreadId}]")));
    }
    [TestMethod]
    public void AsyncDelegateThreading3()
    {
      //Arrange
      int minW, minIO, minAvailW, maxW, maxIO, maxAvailIO;
      ThreadPool.GetMinThreads(out minW, out minIO);
      ThreadPool.GetMaxThreads(out maxW, out maxIO);
      ThreadPool.GetAvailableThreads(out minAvailW, out maxAvailIO);
      Trace.WriteLine($"ProcessorCount: {Environment.ProcessorCount} | {nameof(minW)}:{minW} |{nameof(minIO)}:{minIO} | {nameof(maxW)}:{maxW} | {nameof(maxIO)}:{maxIO} | {nameof(minAvailW)}:{minAvailW} | {nameof(maxAvailIO)}:{maxAvailIO}");
      var x = new GeneralAlgorithm(321);
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now.ToString(Constant.PrecisionFormat)}");

      //Act
      for (int k = 0; k < 70; ++k)
      {
        x.AddResultInTheBackground2(k);
      }
      Thread.Sleep(4000);

      //Assert
      Trace.WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now.ToString(Constant.PrecisionFormat)}");
      var results = x.Results.ToList();
      results.ForEach(_ => Trace.WriteLine(_));
      Assert.IsTrue(results.All(_ => !_.StartsWith($"[{Environment.CurrentManagedThreadId}]")));
    }
  }
  class GeneralAlgorithm
  {
    private AlgorithmExecutorDelegate aed;
    private AddResultInTheBackground_Delegate AddResultInTheBackground_Work;
    private AddResultInTheBackground_Delegate AddResultInTheBackground_Work2;
    private AlgorithmExecutor2 algorithm;

    public GeneralAlgorithm(int n)
    {
      algorithm = new AlgorithmExecutor2();
      ID = n;
      Results = new ConcurrentQueue<string>();
      aed = AlgorithmExecutor.Execute;
      AddResultInTheBackground_Work = AlgorithmExecutor.Execute;
      AddResultInTheBackground_Work2 = algorithm.Execute;
    }
    public int ID;
    public string Result;
    public ConcurrentQueue<string> Results;
    public virtual void EjecutarDelegado()
    {
      AsyncCallback callback = ExecuteDelegateCompleted;
      aed.BeginInvoke($"([{Environment.CurrentManagedThreadId}] BeginInvoke {ID} at {DateTime.Now.ToString(Constant.PrecisionFormat)})", callback, null);
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

    public void AddResultInTheBackground(int input)
    {
      AddResultInTheBackground_Work.BeginInvoke($"([{Environment.CurrentManagedThreadId}] BeginInvoke {ID} with input {input} at {DateTime.Now.ToString(Constant.PrecisionFormat)})", AddResultInTheBackground_Callback, input);
    }
    private void AddResultInTheBackground_Callback(IAsyncResult result)
    {
      try
      {
        var entry = $"[{Environment.CurrentManagedThreadId}] callback at {DateTime.Now.ToString(Constant.PrecisionFormat)}";
        Results.Enqueue($"{entry} | {AddResultInTheBackground_Work.EndInvoke(result)} | {result.AsyncState}");
      }
      catch (Exception ex)
      {
        Results.Enqueue($"{ex.GetType().FullName}: {ex.Message}");
      }
    }

    public void AddResultInTheBackground2(int input)
    {
      AddResultInTheBackground_Work2.BeginInvoke($"([{Environment.CurrentManagedThreadId}] BeginInvoke {ID} with input {input} at {DateTime.Now.ToString(Constant.PrecisionFormat)})", AddResultInTheBackground_Callback2, input);
    }
    private void AddResultInTheBackground_Callback2(IAsyncResult result)
    {
      try
      {
        Results.Enqueue($"{AddResultInTheBackground_Work2.EndInvoke(result)} | {result.AsyncState}");
      }
      catch (Exception ex)
      {
        Results.Enqueue($"{ex.GetType().FullName}: {ex.Message}");
      }
    }
  }
  delegate string AlgorithmExecutorDelegate(string when);
  delegate string AddResultInTheBackground_Delegate(string input);
  static class AlgorithmExecutor
  {
    public static string Execute(string when)
    {
      return $"[{Environment.CurrentManagedThreadId}] return: {DateTime.Now.ToString(Constant.PrecisionFormat)} {when}";
    }
  }
  class AlgorithmExecutor2
  {
    public string Execute(string when)
    {
      return $"[{Environment.CurrentManagedThreadId}] return: {DateTime.Now.ToString(Constant.PrecisionFormat)} {when}";
    }
  }

  [TestClass]
  public class ReactiveDataflowSpec
  {
    [TestMethod]
    public void _basic0()
    {
      //Arrange
      var datastream = new int[] { 1, 2, 3 };
      var calls = new List<int>();
      var match = new Func<int, bool>(x => x >= 2);
      var fire = new Action<int>(x => calls.Add(x));

      //Act
      foreach (var n in datastream)
      {
        if (match(n))
        {
          fire(n);
        }
      }

      //Assert
      Assert.AreEqual<int>(2, calls.Count);
    }
    class SymbolInterestSubscriber
    {
      private BlockingCollection<KeyValuePair<string, int>> quotes;
      public SymbolInterestSubscriber(string symbol, BlockingCollection<KeyValuePair<string, int>> quotes)
      {
        this.Symbol = symbol;this.quotes = quotes;
      }
      public string Symbol;
      public void EvaluateNewQuote(KeyValuePair<string, int> quote)
      {
        if (string.Compare(quote.Key, Symbol) == 0)
        {
          quotes.Add(quote);
        }
      }
    }
    [TestMethod]
    public void _basic1()
    {
      //Arrange
      var datastream = new List<KeyValuePair<string, int>>
      {
        new KeyValuePair<string,int>("s2",2),
        new KeyValuePair<string, int>("s2", 2),
        new KeyValuePair<string,int>("s3",3),
        new KeyValuePair<string, int>("s2", 2),
        new KeyValuePair<string,int>("s3",3),
        new KeyValuePair<string,int>("s2",2),
      };
      var calls = new BlockingCollection<KeyValuePair<string, int>>();
      var subs = new List<SymbolInterestSubscriber>
      {
        new SymbolInterestSubscriber("s2", calls),
        new SymbolInterestSubscriber("s2", calls),
        new SymbolInterestSubscriber("s3", calls)
      };

      //Act
      foreach (var quote in datastream)
      {
        Parallel.ForEach(subs, s => s.EvaluateNewQuote(quote));
      }
      calls.CompleteAdding();
      var executed_calls = calls.GetConsumingEnumerable().ToList();

      //Assert
      Assert.AreEqual<int>(10, executed_calls.Count);
      Assert.AreEqual<int>(8 * 2, executed_calls.Where(q => q.Key == "s2").Sum(q => q.Value));
      Assert.AreEqual<int>(3 * 2, executed_calls.Where(q => q.Key == "s3").Sum(q => q.Value));
    }

    [TestMethod]
    public void basic0()
    {/*
      //Arrange
      var datastream = new int[] { 1, 2, 3 };
      var reactor = new Reactor(source,subs,target);

      //Act
      reactor.Start();

      //Assert
      Assert.AreEqual<int>(2, calls.Count);*/
    }
  }
}