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
  public void _1_execute(string cmd)
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_1_execute)}({cmd}).");
    _2_inner_execute(cmd);
    //_2_inner_execute(cmd);
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_1_execute)}({cmd}) Done.");
  }
  private async void _2_inner_execute(string cmd)
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_2_inner_execute)}({cmd}).");

    //await _3_execute_twice(cmd);
    await _3_single_copy(cmd);

    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_2_inner_execute)}({cmd}) Done.");
  }
  private async Task _3_execute_twice(string cmd)
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_3_execute_twice)}({cmd}).");
    await _3_single_copy(cmd);
    await _3_single_copy(cmd);
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_3_execute_twice)}({cmd}) Done.");
  }
  private async Task _3_single_copy(string cmd)
  {
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_3_single_copy)}({cmd}).");
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
    WriteLine($"[{Environment.CurrentManagedThreadId,4}] {nameof(_3_single_copy)}({cmd}) Done.");
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
        x._1_execute(cmd);
        x._1_execute($"2_{cmd}");
      } while (true);
      x.stop();
      WriteLine($"\n[{Environment.CurrentManagedThreadId,4}] Done.");
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
  }
}