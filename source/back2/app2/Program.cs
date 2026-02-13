// See https://aka.ms/new-console-template for more information
//dotnet publish source\back2\app2 -c Release --force --runtime win-x64 --self-contained -o E:\temp\app2
using System;
using System.Linq;
using static System.Console;

var p=System.Diagnostics.Process.GetCurrentProcess();
var cpu=Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
var processor_host=@"E:\temp\app1\app1.exe";
System.IO.FileInfo file=new(processor_host);
var role="processor1";
var pars=string.Join(' ',args);
var outfilename=args.FirstOrDefault();
System.IO.FileInfo outfile=string.IsNullOrWhiteSpace(outfilename)?null:new(outfilename);
var writer=outfile!=null?outfile.CreateText():Out;
writer.WriteLine(outfilename);
writer.WriteLine($"[PID {p.Id} {p.StartTime:s} CPU {cpu}] ServiceHost started {pars} {nameof(Environment.UserInteractive)} {Environment.UserInteractive} {nameof(Console.IsInputRedirected)} {Console.IsInputRedirected} {nameof(Console.IsOutputRedirected)} {Console.IsOutputRedirected} {nameof(Console.IsErrorRedirected)} {Console.IsErrorRedirected}");

var subinput=new System.IO.StringReader(string.Join(null,System.Linq.Enumerable.Range(0,2).Select(n=>$"{n}{Environment.NewLine}")));
string output = null;
string error = null;
using System.Diagnostics.Process collect = new();
collect.StartInfo = new(file.FullName, role)
{
    CreateNoWindow = true,
    UseShellExecute = false,
    RedirectStandardInput = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true
};
collect.Start();
if(collect.StartInfo.RedirectStandardInput) collect.StandardInput.Write(subinput);
var subcpu=collect.StartInfo?.Environment?.TryGetValue("PROCESSOR_ARCHITECTURE", out string _v) == true ? _v : null;
writer.WriteLine($"[PID {p.Id} {p.StartTime:s} CPU {cpu}] Sub-process PID {collect.Id} {collect.StartTime:s} \"CPU {subcpu}\" {pars}");
if(collect.StartInfo.RedirectStandardOutput) output = collect.StandardOutput.ReadToEnd();
if(collect.StartInfo.RedirectStandardError) error = collect.StandardError.ReadToEnd();

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
writer.WriteLine(body);

//ReadLine();
writer.WriteLine($"[PID {p.Id} CPU {cpu}] ServiceHost stopped {pars}");
if(outfilename!=null){writer.Flush();writer.Dispose();}
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
---
In .NET 10 on Windows 10, you can determine if the current process was executed by another process by examining the **parent process ID (PPID)** and comparing it against expected values.

Here's how to do this:

```csharp
using System;
using System.Diagnostics;
using System.Management;

class Program
{
    static void Main()
    {
        var currentProcess = Process.GetCurrentProcess();
        
        // Get the parent process ID
        int parentProcessId = GetParentProcessId(currentProcess.Id);
        
        if (parentProcessId > 0)
        {
            try
            {
                var parentProcess = Process.GetProcessById(parentProcessId);
                Console.WriteLine($"Current process: {currentProcess.ProcessName} (PID: {currentProcess.Id})");
                Console.WriteLine($"Parent process: {parentProcess.ProcessName} (PID: {parentProcess.Id})");
                
                // Determine if started by another process vs. user/shell
                bool wasExecutedByAnotherProcess = !IsShellProcess(parentProcess.ProcessName);
                Console.WriteLine($"Executed by another process: {wasExecutedByAnotherProcess}");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Parent process no longer exists");
            }
        }
    }

    static int GetParentProcessId(int processId)
    {
        try
        {
            using var query = new ManagementObjectSearcher(
                $"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}");
            
            using var results = query.Get();
            foreach (ManagementObject result in results)
            {
                return Convert.ToInt32(result["ParentProcessId"]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting parent process: {ex.Message}");
        }
        
        return -1;
    }

    static bool IsShellProcess(string processName)
    {
        // Common shell/launcher processes
        string[] shellProcesses = { 
            "explorer",      // Windows Explorer
            "cmd",           // Command Prompt
            "powershell",    // PowerShell
            "pwsh",          // PowerShell Core
            "WindowsTerminal" // Windows Terminal
        };
        
        return Array.Exists(shellProcesses, 
            shell => processName.Equals(shell, StringComparison.OrdinalIgnoreCase));
    }
}
```

**Important Notes:**

1. **WMI Requirement**: The `System.Management` namespace requires the `System.Management` NuGet package. Add it to your project:
   ```
   dotnet add package System.Management
   ```

2. **Alternative for .NET 5+**: You can use P/Invoke with native Windows APIs for better performance:

```csharp
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        int processInformationClass,
        ref PROCESS_BASIC_INFORMATION processInformation,
        int processInformationLength,
        out int returnLength);

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_BASIC_INFORMATION
    {
        public IntPtr Reserved1;
        public IntPtr PebBaseAddress;
        public IntPtr Reserved2_0;
        public IntPtr Reserved2_1;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    static int GetParentProcessId(Process process)
    {
        var pbi = new PROCESS_BASIC_INFORMATION();
        int returnLength;
        
        int status = NtQueryInformationProcess(
            process.Handle,
            0, // ProcessBasicInformation
            ref pbi,
            Marshal.SizeOf(pbi),
            out returnLength);
            
        if (status != 0)
            return -1;
            
        return pbi.InheritedFromUniqueProcessId.ToInt32();
    }

    static void Main()
    {
        var currentProcess = Process.GetCurrentProcess();
        int parentPid = GetParentProcessId(currentProcess);
        
        if (parentPid > 0)
        {
            try
            {
                var parentProcess = Process.GetProcessById(parentPid);
                Console.WriteLine($"Parent: {parentProcess.ProcessName}");
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Parent process has exited");
            }
        }
    }
}
```

**Key Considerations:**

- The parent process ID tells you which process launched the current one
- If parent is `explorer.exe`, the user likely double-clicked the executable
- If parent is another custom process, it was programmatically executed
- The parent process may have already exited, causing `GetProcessById` to throw an exception
- This approach works on Windows but isn't cross-platform
*/