// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;

using static System.Console;

namespace matermind
{
    class Exe
    {
        static void Main(string[] args)
        {
            var k = System.Console.ReadKey();
            WriteLine($"\n{k.Key}\n");
            WriteLine(string.Join(" ",Enum.GetNames(typeof(ConsoleKey))));
        }
    }
}