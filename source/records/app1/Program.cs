// See https://aka.ms/new-console-template for more information Console.WriteLine("Hello, World!");
using System;

using static System.Console;

public static class Program
{
    static void asfound()//https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history#c-version-9
    {
        var phoneNumbers1 = new string[2];
        var phoneNumbers2 = new string[2];
        Person person1 = new("Nancy", "Davolio", phoneNumbers1);
        Person person2 = new("Nancy", "Davolio", phoneNumbers2);
        WriteLine(person1 == person2); // output: True

        person1.PhoneNumbers[0] = "555-1234";
        WriteLine(person1 == person2); // output: True

        WriteLine(ReferenceEquals(person1, person2)); // output: False
    }
    static void attrs()
    {
        objectx.Class1 x = new("name1", "last1", null);
        foreach (var p in x.GetType().GetProperties())
        {
            WriteLine($"{p.Name}");
        }
    }

    public static void Main()
    {
        asfound();
        attrs();
    }
}