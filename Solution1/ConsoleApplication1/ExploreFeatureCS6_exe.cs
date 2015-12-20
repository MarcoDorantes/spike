using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApplication1
{
  class X
  {
    public X()
    {
      Name = "uno";
    }
    public string Name { get; }
    public int Age => Name.Length;
    public override string ToString() => $"({Name},{Age})";
  }
  class ExploreFeatureCS6_exe
  {
    static bool log(Exception ex) { WriteLine($"An exception ocurred: {ex.GetType().FullName}"); return false; }
    static void with_logged_exception(int n)
    {
      try
      {
        int r = 10 / n;
      }
      catch (Exception ex) when (log(ex))
      {
        WriteLine("ex is null");
      }
    }
    static void exception_filter()
    {
      with_logged_exception(0);
      WriteLine("Done.");
    }

    public static void _Main_cs6()
    {
      try
      {
        var x = new X();
        WriteLine($"X: {x.Name} {x.Age} | {x}");
        WriteLine("{0} {1}", nameof(x), nameof(X));
        X y = null;
        if (y?.Name?.Length > 0)
        {
          WriteLine("Name len is ok");
        }
      }
      catch (Exception ex) { WriteLine($"{ex.GetType().FullName} : {ex.Message}"); }
    }
  }
}