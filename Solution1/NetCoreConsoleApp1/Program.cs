using System;
using static System.Console;

namespace NetCoreConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            System.Threading.Thread.Sleep(450);
            watch.Stop();
            WriteLine($"watch: {watch.IsRunning} {watch.ElapsedMilliseconds}");
        }
    }
}