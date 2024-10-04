namespace lib1;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static System.Console;

public class Class2
{
    public async Task Start(string[] args)
    {
        WriteLine($"{nameof(Class2)}.{nameof(Start)} begin");
        var file = args.FirstOrDefault();
        if (File.Exists(file))
        {
            uint count = 0U;
            string input = "";
            do
            {
                ++count;
                if (string.Compare(input, "END", true) == 0) break;
                WriteLine($"[{System.Threading.Thread.CurrentThread.ManagedThreadId}] {DateTime.Now:s} Begin {count}");
                input = await File.ReadAllTextAsync(file);
                await Task.Delay(1000);
                WriteLine($"[{System.Threading.Thread.CurrentThread.ManagedThreadId}] {DateTime.Now:s} End {count}");
            } while (true);
        }
        else await Task.Delay(100);
        WriteLine($"{nameof(Class2)}.{nameof(Start)} end");
    }
}

public class Class1
{
    public async Task Start(string[] args)
    {
        WriteLine($"{nameof(Class1)}.{nameof(Start)} begin");
        Class2 y=new();
        await y.Start(args);
        WriteLine($"{nameof(Class1)}.{nameof(Start)} end");
    }
}