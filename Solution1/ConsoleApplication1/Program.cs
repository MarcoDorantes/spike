using System;
using System.Collections.Generic;
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
    class Program
    {
        static void f()
        {
            var csc = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v6.0" } });
        }
        static void Main(string[] args)
        {
            try
            {
                f();
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