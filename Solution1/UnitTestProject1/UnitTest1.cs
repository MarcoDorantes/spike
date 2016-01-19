using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

using static System.Diagnostics.Trace;

namespace UnitTestProject1
{
    class A
    {
        public event EventHandler<DateTime> OnPulse;

        public void FireSync()
        {
            OnPulse?.Invoke(this, DateTime.Now);
        }
        public void FireAsync()//System.ArgumentException: The delegate must have only one target.
        {
            AsyncCallback callback = EndInvokeFireAsync;
            OnPulse?.BeginInvoke(this, DateTime.Now, callback , null);
        }
        private void EndInvokeFireAsync(IAsyncResult result){}
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
                WriteLine(logline);
                Thread.Sleep(1000);
                queue.Enqueue(logline);
            });
            WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now:s}");

            //Act
            a.FireSync();

            //Assert
            WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now:s}");
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
            a.FireSync();

            //Assert
            queue.ToList().All(n => n == Environment.CurrentManagedThreadId);
        }

        [TestMethod]
        public void AsyncDelegateThreading0()
        {
            //Arrange
            var queue = new ConcurrentQueue<string>();

            var a = new A();
            Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) =>
            {
                var logline = $"[{Environment.CurrentManagedThreadId}] {n} {e:s}";
                WriteLine(logline);
                Thread.Sleep(1000);
                queue.Enqueue(logline);
            });
            WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now:s}");

            //Act
            try
            {
                a.FireAsync();
                Assert.Fail("Exception not thrown.");
            } catch (ArgumentException ex)
            {
                //Assert
                Assert.
                //The delegate must have only one target.
                     }
        }


        /*[TestMethod]
        public void AsyncDelegateThreading0()
        {
            //Arrange
            var queue = new ConcurrentQueue<string>();

            var a = new A();
            Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) =>
            {
                var logline = $"[{Environment.CurrentManagedThreadId}] {n} {e:s}";
                WriteLine(logline);
                Thread.Sleep(1000);
                queue.Enqueue(logline);
            });
            WriteLine($"[{Environment.CurrentManagedThreadId}] Arrange {DateTime.Now:s}");

            //Act
            a.FireAsync();
            AlgorithmExecutorDelegate aed = AlgorithmExecutor.Execute;
            aed.BeginInvoke(_params, t, ExecuteDelegateCompleted, aed);

            //Assert
            WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now:s}");
        }*/
    }

    /*    class GeneralAlgorithm
        {
            public virtual void EjecutarDelegado()
            {
            }
        }
        delegate DateTime AlgorithmExecutorDelegate(DateTime when);
        static class AlgorithmExecutor
        {
            public static DateTime Execute(DateTime when)
            {
                return DateTime.Now;
            }
        }*/
}