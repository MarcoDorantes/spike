// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using System;

using static System.Console;

/*
dotnet publish .\source\deps\app1\ -c Release --force --runtime win-x64 --self-contained -o C:\temp\app1

dotnet build .\source\deps\lib2\ -c Release
dotnet publish .\source\deps\lib2\ -c Release -o C:\temp\lib2
 */

class Program
{
    static void Main()
    {
        try
        {
            WriteLine($"Setup start {nameof(lib1.Class1)}");
            lib1.Class1 x = Activator.CreateInstance(Type.GetType("lib2.Class2, lib2")) as lib1.Class1;
            WriteLine($"Setup end {nameof(lib1.Class1)} version: [{x?.GetVersion()}]");
        }
        catch (Exception ex)
        {
            for (int level = 0; ex != null; ex = ex.InnerException, ++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
        }
    }
}