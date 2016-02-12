using System;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

#region async_await_and_interface
namespace async_await_and_interface
{
  interface IA
  {
    Task f();
    Task f0();
    Task f2();
    Task<int> f3();
    Task<string> GetString();
  }

  class A : IA
  {
    public Task f()
    {
      return new Task(() => Console.WriteLine($"[{Environment.CurrentManagedThreadId}] task"));
    }
    public Task f0()
    {
      return Task.Run(() => Console.WriteLine($"[{Environment.CurrentManagedThreadId}] task0"));
    }
    public async Task f2()
    {
      await f0();
    }
    public async Task<int> _f3() => await Task.Run(() => 12 * 12);
    public async Task<int> f3()
    {
      return await _f3();
    }
    public async Task<string> GetString()
    {
      return await Task.Run(() => { for (var k = 0; k < 3; ++k) System.Threading.Thread.Sleep(600); return "returned string"; });
    }
  }

  class task1_main
  {
    static void g(IA a)
    {
      var t = a.f();
      t.Start();
    }
    static async void h(IA a)
    {
      await a.f2();
    }
    static async void i(IA a)
    {
      WriteLine($"[{Environment.CurrentManagedThreadId}] i-begin");
      int r = await a.f3();
      WriteLine($"[{Environment.CurrentManagedThreadId}] i-result: {r}");
    }
    static async Task<string> _j(IA a)
    {
      try
      {
        WriteLine($"[{Environment.CurrentManagedThreadId}] _j-0");
        Task<string> task = a.GetString();
        WriteLine($"[{Environment.CurrentManagedThreadId}] _j-1");
        string result = await task;
        WriteLine($"[{Environment.CurrentManagedThreadId}] _j-2");
        return result;
      }
      finally
      {
        WriteLine($"[{Environment.CurrentManagedThreadId}] _j-3");
      }
    }
    static async void j(IA a)
    {
      WriteLine($"[{Environment.CurrentManagedThreadId}] j-0");
      Task<string> result = _j(a);
      WriteLine($"[{Environment.CurrentManagedThreadId}] j-1");
      WriteLine($"[{Environment.CurrentManagedThreadId}] j-2: { await result }");
    }

    public static void async_await_and_interface()
    {
      //msdn.microsoft.com/en-us/library/hh191443.aspx
      WriteLine($"[{Environment.CurrentManagedThreadId}] Begin");
      var a = new A();
      g(a);
      h(a);
      i(a);
      j(a);

      WriteLine($"[{Environment.CurrentManagedThreadId}] Done");
      ReadLine();
    }
  }
}
#endregion

namespace nesting
{
  class tasknest_main
  {
    public static void child1()
    {
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] child1 start");
      Thread.Sleep(5000);
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] child1 end");
    }
    public static void parent()
    {
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] parent start");
      //var child1_task = Task.Factory.StartNew(child1, CancellationToken.None, TaskCreationOptions.AttachedToParent, TaskScheduler.Current);
      var child1_task = Task.Factory.StartNew(child1);
      Thread.Sleep(2500);
      //child1_task.Wait();
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] parent end");
    }
    public static void main()
    {
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] main start");
      var parent_task = Task.Factory.StartNew(parent);
      parent_task.Wait();
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] parent waited");
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] Press ENTER to exit"); ReadLine();
      WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] main end");
    }
    public static void main0()
    {
      var child1 = new Action(() => Thread.Sleep(5000));
      var parent = new Action(() => { Task.Factory.StartNew(child1); Thread.Sleep(2500); });
      var parent_task = Task.Factory.StartNew(parent);
    }
  }
}
class task1_exe
{
  public static void _Main()
  {
    try
    {
      //async_await_and_interface.task1_main.async_await_and_interface();
      nesting.tasknest_main.main();
    }
    catch (Exception ex) { WriteLine($"[{Environment.CurrentManagedThreadId} {DateTime.Now.TimeOfDay}] {ex.GetType().FullName}: {ex.Message}"); }
  }
}