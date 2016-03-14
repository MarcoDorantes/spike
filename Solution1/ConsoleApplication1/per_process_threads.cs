using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ConsoleApplication1
{

  class A
  {
    ulong id, a, b;
    Task task;
    public A(ulong id) { this.id = id; a = 0; b = 1; }
    public void start()
    {
      task = Task.Factory.StartNew(() => fibonacci());
    }
    private void fibonacci()
    {
      Console.WriteLine("{0,5:N0} {1,5:N0} STARTED\t{2} ", id, Environment.CurrentManagedThreadId, DateTime.Now.ToString("HH:mm:ss.fffffff"));
      for (;;)
      {
        //0112358
        ulong c = a + b;
        a = b;
        b = c;
        Trace.WriteLine(string.Format("{0},{1},{2}", id, Environment.CurrentManagedThreadId, c));
        Thread.Sleep(100);
      }
    }
  }

  class per_process_threads
  {
    static void start(string[] args)
    {
      int workerThreads, completionPortThreads;
      var all = new List<A>();
      ulong limit = 0;
      if (args.Length < 1 || ulong.TryParse(args[0], out limit) == false)
        limit = ulong.MaxValue;

      Trace.WriteLine(string.Format("id, thread, next"));

      System.Threading.ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
      Console.WriteLine("[{2}]\t{0} | {1}", workerThreads, completionPortThreads, Process.GetCurrentProcess().Id);

      Console.WriteLine("limit: {0:N0}\nPress ENTER to start", limit); Console.ReadLine();
      for (ulong k = 0; k < limit; ++k)
      {
        A a = new A((ulong)all.LongCount());
        all.Add(a);
        a.start();
        System.Threading.ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
        Console.WriteLine("\t{0} | {1}", workerThreads, completionPortThreads);
      }
      Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
    }
    public static void _Main(string[] args)
    {
      try
      {
        start(args);
      }
      catch (Exception ex) { Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message); }
    }
  }
}