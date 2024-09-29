namespace lib1;

using System.Threading.Tasks;

using static System.Console;

public class Class2
{
    public async Task Start(string[] args)
    {
        WriteLine($"{nameof(Class2)}.{nameof(Start)} begin");
        await Task.Delay(100);
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