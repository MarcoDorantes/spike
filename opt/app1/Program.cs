using System;
using System.Linq;
using static System.Console;

namespace app1
{
    class Input
    {
        public void op1()
        {
            WriteLine($"This is {nameof(op1)}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WriteLine($"Count:{args.Length}"); for (int k = 0; k < args.Length; ++k) WriteLine($"{k}:[{args[k]}]");
                if (args.Length > 0)
                {
                    utility.Switch.AsType<Input>(args);
                }
                else
                {
                    utility.Switch.ShowUsage(typeof(Input));
                }
            }
            catch (Exception ex)
            {
                for (int level = 0; ex != null; ex = ex?.InnerException, ++level)
                {
                    WriteLine($"[{level}] {ex.GetType().FullName}: {ex.Message}");
                }
            }
        }
    }
}