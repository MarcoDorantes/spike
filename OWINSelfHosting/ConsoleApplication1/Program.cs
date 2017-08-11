using System;

namespace ConsoleApplication1
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        SelfHostTry1.Run(args);
        //DuplexSpike.Run(args);
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);
          ex = ex.InnerException;
        }
      }
    }
  }
}