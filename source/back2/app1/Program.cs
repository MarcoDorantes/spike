// See https://aka.ms/new-console-template for more information
/*
dotnet publish source\back2\app1 -c Release --force --runtime win-x86 --self-contained -o E:\temp\app1
dumpbin /headers E:\temp\app1\app1.exe | findstr machine
corflags E:\temp\app1\app1.exe

# Check PE header
$file = 'E:\temp\app1\app1.exe'
$bytes = [System.IO.File]::ReadAllBytes($file)
$peOffset = [BitConverter]::ToInt32($bytes, 0x3C)
$machine = [BitConverter]::ToUInt16($bytes, $peOffset + 4)

switch ($machine) {
    0x014C { "32-bit (x86)" }
    0x8664 { "64-bit (x64)" }
    0xAA64 { "ARM64" }
    default { "Unknown: 0x$($machine.ToString('X4'))" }
}
*/
using System;

var p=System.Diagnostics.Process.GetCurrentProcess();
//var cpu=p.StartInfo?.Environment?.TryGetValue("PROCESSOR_ARCHITECTURE", out string _v) == true ? _v : null;
var cpu=Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
var role=string.Join(' ',args);
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor host started {role}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Setup begun {role}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Setup ended {role}");

var input1=Console.ReadLine();//Pipe receive
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Input received {input1}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Start begun {role}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Start ended {role}");

var input2=Console.ReadLine();//Pipe receive
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Input received {input2}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Stop begun {role}");
Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor Stop ended {role}");

Console.WriteLine($"{DateTime.Now:s} [PID {p.Id} CPU {cpu}] Processor host stopped {role}");