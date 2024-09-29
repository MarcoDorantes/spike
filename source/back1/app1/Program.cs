namespace app1;

using System;
using System.Threading.Tasks;

using static System.Console;

class Exe
{
    static async Task Main(string[] args)
    {
        try
        {
            lib1.Class1 x = new();
            await x.Start(args);
        }
        catch(Exception ex)
        {
            for(int level=0; ex!=null; ex=ex.InnerException,++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
        }
    }
}