using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;
using System.Collections.Concurrent;

using static System.Diagnostics.Trace;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DefaultDelegateThreading0()
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
            a.Fire();

            //Assert
            WriteLine($"[{Environment.CurrentManagedThreadId}] Assert {DateTime.Now:s}");
        }

        [TestMethod]
        public void DefaultDelegateThreading()
        {
            //Arrange
            var queue = new ConcurrentQueue<int>();

            var a = new A();
            Enumerable.Range(1, 10).ToList().ForEach(n => a.OnPulse += (s, e) =>
            {
                queue.Enqueue(Environment.CurrentManagedThreadId);
            });

            //Act
            a.Fire();

            //Assert
            queue.ToList().All(n => n == Environment.CurrentManagedThreadId);
        }
    }

    class A
    {
        public event EventHandler<DateTime> OnPulse;

        public void Fire()
        {
            OnPulse?.Invoke(this, DateTime.Now);
        }
    }
}