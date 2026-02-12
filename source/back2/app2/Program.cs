// See https://aka.ms/new-console-template for more information
using System;

var p=System.Diagnostics.Process.GetCurrentProcess();
var cpu=Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
var processor_host=@"E:\temp\app1\app1.exe";
System.IO.FileInfo file=new(processor_host);
var role="";
Console.WriteLine($"[PID {p.Id} CPU {cpu}] ServiceHost started {string.Join(' ',args)}");

string output = null;
string error = null;
using var collect = new System.Diagnostics.Process();
collect.StartInfo = new(file.FullName, role)
{
    CreateNoWindow = true,
    UseShellExecute = false,
    RedirectStandardInput = true,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};
collect.Start();
output = collect.StandardOutput.ReadToEnd();
error = collect.StandardError.ReadToEnd();

const int mm = 60;
collect.WaitForExit(mm * 60 * 1000);

System.Text.StringBuilder payload = new($"{output}");
if (!string.IsNullOrWhiteSpace(error))
{
    payload.Append($"\n\n\nError:[{error}]");
}
var when = DateTimeOffset.UtcNow;
payload.AppendLine();
payload.AppendLine($"This email was sent at: {when:s}");
payload.AppendLine($"This email was sent from: {System.Net.Dns.GetHostName()}");
var body = $"{payload}";
Console.WriteLine(body);

Console.ReadLine();
Console.WriteLine($"[PID {p.Id} CPU {cpu}] ServiceHost stopped {string.Join(' ',args)}");
/*
https://learn.microsoft.com/en-us/dotnet/standard/io/pipe-operations
https://learn.microsoft.com/en-us/dotnet/api/system.io.pipes?view=net-10.0
--
Here's how to send and receive data between parent and child processes using Windows named pipes in .NET 10 with C#:

## Complete Example

### Parent Process (Server)

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

class ParentProcess
{
    static async Task Main(string[] args)
    {
        string pipeName = $"MyPipe_{Guid.NewGuid()}";
        
        // Create named pipe server
        using var pipeServer = new NamedPipeServerStream(
            pipeName,
            PipeDirection.InOut,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);
        
        Console.WriteLine($"Parent: Starting child process...");
        
        // Start child process and pass pipe name
        var startInfo = new ProcessStartInfo
        {
            FileName = "ChildProcess.exe",
            Arguments = pipeName,
            UseShellExecute = false
        };
        
        using var childProcess = Process.Start(startInfo);
        
        Console.WriteLine("Parent: Waiting for child to connect...");
        await pipeServer.WaitForConnectionAsync();
        Console.WriteLine("Parent: Child connected!");
        
        // Send data to child
        await SendMessageAsync(pipeServer, "Hello from parent!");
        
        // Receive data from child
        string response = await ReceiveMessageAsync(pipeServer);
        Console.WriteLine($"Parent: Received from child: {response}");
        
        // Send another message
        await SendMessageAsync(pipeServer, "Goodbye from parent!");
        
        // Wait for child to finish
        childProcess.WaitForExit();
        Console.WriteLine("Parent: Child process exited.");
    }
    
    static async Task SendMessageAsync(PipeStream pipe, string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        
        await pipe.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await pipe.WriteAsync(buffer, 0, buffer.Length);
        await pipe.FlushAsync();
        
        Console.WriteLine($"Parent: Sent: {message}");
    }
    
    static async Task<string> ReceiveMessageAsync(PipeStream pipe)
    {
        byte[] lengthBuffer = new byte[4];
        await pipe.ReadAsync(lengthBuffer, 0, 4);
        int length = BitConverter.ToInt32(lengthBuffer, 0);
        
        byte[] buffer = new byte[length];
        int totalRead = 0;
        while (totalRead < length)
        {
            int read = await pipe.ReadAsync(buffer, totalRead, length - totalRead);
            totalRead += read;
        }
        
        return Encoding.UTF8.GetString(buffer);
    }
}
```

### Child Process (Client)

```csharp
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

class ChildProcess
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Child: No pipe name provided!");
            return;
        }
        
        string pipeName = args[0];
        Console.WriteLine($"Child: Connecting to pipe: {pipeName}");
        
        // Connect to parent's pipe
        using var pipeClient = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        
        await pipeClient.ConnectAsync(5000); // 5 second timeout
        Console.WriteLine("Child: Connected to parent!");
        
        // Receive data from parent
        string message = await ReceiveMessageAsync(pipeClient);
        Console.WriteLine($"Child: Received from parent: {message}");
        
        // Send response to parent
        await SendMessageAsync(pipeClient, "Hello from child!");
        
        // Receive another message
        message = await ReceiveMessageAsync(pipeClient);
        Console.WriteLine($"Child: Received from parent: {message}");
        
        Console.WriteLine("Child: Exiting...");
    }
    
    static async Task SendMessageAsync(PipeStream pipe, string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        
        await pipe.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
        await pipe.WriteAsync(buffer, 0, buffer.Length);
        await pipe.FlushAsync();
        
        Console.WriteLine($"Child: Sent: {message}");
    }
    
    static async Task<string> ReceiveMessageAsync(PipeStream pipe)
    {
        byte[] lengthBuffer = new byte[4];
        await pipe.ReadAsync(lengthBuffer, 0, 4);
        int length = BitConverter.ToInt32(lengthBuffer, 0);
        
        byte[] buffer = new byte[length];
        int totalRead = 0;
        while (totalRead < length)
        {
            int read = await pipe.ReadAsync(buffer, totalRead, length - totalRead);
            totalRead += read;
        }
        
        return Encoding.UTF8.GetString(buffer);
    }
}
```

## Alternative: Using Anonymous Pipes

For simpler one-way communication from parent to child:

### Parent Process (Anonymous Pipe)

```csharp
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

class ParentWithAnonymousPipe
{
    static void Main()
    {
        using var pipeServer = new AnonymousPipeServerStream(
            PipeDirection.Out,
            HandleInheritability.Inheritable);
        
        // Start child and pass pipe handle
        var startInfo = new ProcessStartInfo
        {
            FileName = "ChildProcess.exe",
            Arguments = pipeServer.GetClientHandleAsString(),
            UseShellExecute = false
        };
        
        using var childProcess = Process.Start(startInfo);
        
        // Dispose client handle in parent
        pipeServer.DisposeLocalCopyOfClientHandle();
        
        // Send data
        using var writer = new StreamWriter(pipeServer);
        writer.AutoFlush = true;
        writer.WriteLine("Message from parent");
        
        childProcess.WaitForExit();
    }
}
```

### Child Process (Anonymous Pipe)

```csharp
using System;
using System.IO;
using System.IO.Pipes;

class ChildWithAnonymousPipe
{
    static void Main(string[] args)
    {
        if (args.Length == 0) return;
        
        using var pipeClient = new AnonymousPipeClientStream(
            PipeDirection.In,
            args[0]);
        
        using var reader = new StreamReader(pipeClient);
        string message = reader.ReadLine();
        Console.WriteLine($"Child received: {message}");
    }
}
```

## Bidirectional Anonymous Pipes

```csharp
// Parent creates two pipes
using var pipeToChild = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
using var pipeFromChild = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

var startInfo = new ProcessStartInfo
{
    FileName = "ChildProcess.exe",
    Arguments = $"{pipeToChild.GetClientHandleAsString()} {pipeFromChild.GetClientHandleAsString()}",
    UseShellExecute = false
};

using var childProcess = Process.Start(startInfo);

pipeToChild.DisposeLocalCopyOfClientHandle();
pipeFromChild.DisposeLocalCopyOfClientHandle();

// Use the pipes...
```

## Key Concepts

**Named Pipes:**
- Can communicate between unrelated processes
- Support bidirectional communication
- Require unique names
- More flexible but slightly more complex

**Anonymous Pipes:**
- Simpler for parent-child communication
- Handles passed via command line
- Typically one-way (need two pipes for bidirectional)
- More efficient for simple scenarios

**Message Framing:**
- Use length prefixes to avoid message fragmentation
- Always flush after writing
- Handle partial reads in a loop

**Security:**
- Named pipes can set ACLs via PipeSecurity
- Anonymous pipes inherit parent process security

This gives you robust inter-process communication between .NET processes!
*/