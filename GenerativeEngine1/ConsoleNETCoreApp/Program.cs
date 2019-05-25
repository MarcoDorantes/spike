using System;
using static System.Console;

namespace ConsoleNETCoreApp
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        var c = new Generated.Derived();
        WriteLine("Executing...");
        WriteLine($"{c.Execute()}");
      }
      catch (Exception ex)
      {
        for (int level = 0; ex != null; ex = ex.InnerException, ++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
      }
    }
  }
}