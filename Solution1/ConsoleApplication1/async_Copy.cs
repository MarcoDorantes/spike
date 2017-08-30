using System;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

class CopyProcessor
{
  const string source = "file1.dat";
  const string target = "file2.dat";

  public void start() { }
  public void stop() { }
  public void execute(string cmd)
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] execute({cmd}).");
    _execute();
    //_execute();
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] execute({cmd}) Done.");
  }
  private async void _execute()
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] _execute().");

    //await _execute_twice();
    await single_copy();

    WriteLine($"[{Environment.CurrentManagedThreadId,4}] _execute() Done.");
  }
  private async Task _execute_twice()
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] _execute_twice().");
    await single_copy();
    await single_copy();
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] _execute_twice() Done.");
  }
  private async Task single_copy()
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] single_copy().");
    try
    {
      using (var sourceStream = File.Open(source, FileMode.Open))
      {
        using (var targetStream = File.Create(target))
        {
          await sourceStream.CopyToAsync(targetStream);
        }
      }
    }
    catch (Exception ex) { WriteLine($"[{Environment.CurrentManagedThreadId,4}] {ex.GetType().FullName}: {ex.Message}"); }
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] single_copy() Done.");
  }
}

class async_Copy
{
  public static void _Main(string[] args)
  {
    try
    {
      var x = new CopyProcessor();
      x.start();
      do
      {
        Write($"cmd [{Environment.CurrentManagedThreadId,4}] >");
        var cmd = ReadLine();
        if (cmd == "") break;
        WriteLine($"[{Environment.CurrentManagedThreadId,4}] {cmd}");
        x.execute(cmd);
      } while (true);
      x.stop();
      WriteLine($"\n[{Environment.CurrentManagedThreadId,4}] Done.");
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
  }
}