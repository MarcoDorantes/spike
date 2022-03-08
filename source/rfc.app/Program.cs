string name = args[0];
string simple = args[1];

Console.WriteLine($"name: [{name}]");
Console.WriteLine($"simple: [{simple}]");
Console.WriteLine($"\nRFC: {rfc.lib.rfc.getfullrfc(name, simple)}");

// See https://aka.ms/new-console-template for more information