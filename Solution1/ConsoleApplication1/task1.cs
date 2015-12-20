using System;
using System.Threading.Tasks;
using static System.Console;

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
    return await Task.Run(()=> { for (var k = 0; k < 3; ++k) System.Threading.Thread.Sleep(600); return "returned string"; });
  }
}

class task1_exe
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

  public static void _Main()
  {
    try
    {
      WriteLine($"[{Environment.CurrentManagedThreadId}] Begin");
      var a = new A();
      g(a);
      h(a);
      i(a);
      j(a);

      WriteLine($"[{Environment.CurrentManagedThreadId}] Done");
      ReadLine();
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
  }
}