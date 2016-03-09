using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApplication1
{
  interface IMapFormatter
  {
    byte[] Serialize(IDictionary<string, object> map);
    IDictionary<string, object> Deserialize(byte[] packet);
  }
  class MapFormatter : IMapFormatter
  {
    private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serial;

    public MapFormatter()
    {
      serial = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      serial.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
      serial.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
      serial.FilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
    }
    public byte[] Serialize(IDictionary<string, object> map)
    {
      using (var stream = new System.IO.MemoryStream())
      {
        serial.Serialize(stream, map);
        return stream.GetBuffer();
      }
    }

    public IDictionary<string, object> Deserialize(byte[] packet)
    {
      using (var stream = new System.IO.MemoryStream(packet))
      {
        return serial.Deserialize(stream) as IDictionary<string, object>;
      }
    }
  }
  class SingleLevelMapFormatter : IMapFormatter
  {
    public byte[] Serialize(IDictionary<string, object> map)
    {
      return Encoding.UTF8.GetBytes(map.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}\x1{1}\x0", next.Key, next.Value)).ToString());
    }

    public IDictionary<string, object> Deserialize(byte[] packet)
    {
      var result = new Dictionary<string, object>();
      var text = Encoding.UTF8.GetString(packet);
      foreach (var pair in text.Split('\x0'))
      {
        if (string.IsNullOrWhiteSpace(pair)) continue;
        string[] keyvalue = pair.Split('\x1');
        result[keyvalue[0]] = keyvalue[1];
      }
      return result;
    }
  }
  class MultiLevelMapFormatter : IMapFormatter
  {
    public byte[] Serialize(IDictionary<string, object> map)
    {
      return Encoding.UTF8.GetBytes(GetStringFor(map, 1));
    }
    private string GetStringFor(IDictionary<string, object> map, byte level)
    {
      if (level > 7)
      {
        throw new NotSupportedException($"Unsupported nesting level ({level}) for {nameof(MultiLevelMapFormatter)}.");
      }
      char pair_separator = (char)(level - 1);
      char keyvalue_separator = (char)(level);
      return map.Aggregate(new StringBuilder(), (whole, next) =>
      {
        object value = next.Value;
        if (value is IDictionary<string, object>)
        {
          value = GetStringFor(value as IDictionary<string, object>, (byte)(level + 2));
        }
        return whole.AppendFormat("{0}{1}{2}{3}", next.Key, keyvalue_separator, value, pair_separator);
      }).ToString();
    }

    public IDictionary<string, object> Deserialize(byte[] packet)
    {
      return GetMapFrom(Encoding.UTF8.GetString(packet), 1);
    }
    private IDictionary<string, object> GetMapFrom(string text, byte level)
    {
      //TODO null input text parameter

      char pair_separator = (char)(level - 1);
      char keyvalue_separator = (char)(level);

      if (!text.Contains(pair_separator))
      {
        return null;
      }

      var result = new Dictionary<string, object>();
      foreach (var pair in text.Split(pair_separator))
      {
        if (string.IsNullOrWhiteSpace(pair)) continue;
        string[] keyvalue = pair.Split(keyvalue_separator);
        string value = keyvalue[1];
        var map = GetMapFrom(value, (byte)(level + 1));
        if (map != null)
        {
          result[keyvalue[0]] = map;
        }
        else
        {
          result[keyvalue[0]] = value;
        }
      }
      return result;
    }
  }

  class packmap
  {
    IMapFormatter packer;
    public packmap(IMapFormatter packer)
    {
      this.packer = packer;
    }
    static void try0()
    {
      var map1 = Enumerable.Range(0, 0xFFF).ToDictionary(n => n.ToString(), n => (object)(n * 2));
      map1["AppInstanceID"] = Guid.NewGuid().ToString();
      map1["AppName"] = "TestApp1";
      map1["NamespaceID"] = Guid.NewGuid().ToString();
      map1["NamespaceName"] = "namespace1";
      map1["ProcessorID"] = Guid.NewGuid().ToString();
      map1["ProcessorName"] = "namespace1";

      var s1 = new MapFormatter();
      byte[] packet = s1.Serialize(map1);
      WriteLine($"map1: {packet.Length}");

      var s2 = new MapFormatter();
      var map2 = s2.Deserialize(packet);

      bool equals = map1.SequenceEqual(map2);
      WriteLine($"=: {equals}");

      if (!equals)
      {
        WriteLine("\nmap1:");
        WriteLine(map1.Count);
        map1.Aggregate(Console.Out, (whole, next) => { whole.WriteLine($"[{next.Key}] : [{next.Value}]"); return whole; });
        WriteLine("\nmap2:");
        WriteLine(map2.Count);
        map2.Aggregate(Console.Out, (whole, next) => { whole.WriteLine($"[{next.Key}] : [{next.Value}]"); return whole; });
      }
    }

    void start(Action<byte[]> send)
    {
      var map = Enumerable.Range(0, 0xF).ToDictionary(n => n.ToString(), n => (object)((decimal)(n * 2)).ToString("N2"));
      map["AppInstanceID"] = Guid.NewGuid().ToString();
      map["AppName"] = "TestApp1";
      map["NamespaceID"] = Guid.NewGuid().ToString();
      map["NamespaceName"] = "namespace1";
      map["ProcessorID"] = Guid.NewGuid().ToString();
      map["ProcessorName"] = "namespace1";

      //map["sub1"] = map;//Invalid
      map["sub1"] = new Dictionary<string, object> { { "f1", "val1" }, { "f2", "val2" } };

      for (int k = 0; k < limit; ++k)
      {
        map["Count1"] = k.ToString("N0");
        byte[] packet = packer.Serialize(map);
        send(packet);
        //        System.Threading.Thread.Sleep(100);
      }
    }
    const int limit = 1000;
    /*
    Done:10 in 1110
    Checked:10

    Done:100 in 10926ms
    Checked:100

    Done:1000 in 277m
    Checked:1000

    Done:1000 in 283m
    Checked:1000

    Done:1000 in 98ms
    Checked:1000

    Done:1000 in 29ms
    Checked:1000
    */
    void otherend(List<byte[]> packets)
    {
      for (int k = 0; k < packets.Count; ++k)
      {
        var p = packets[k];
        var map = packer.Deserialize(p);
        if (map["Count1"].ToString() != k.ToString("N0"))
        {
          WriteLine($"{k} Count1 does not match");
        }
      };
    }
    static void test_limit_count()
    {
      var packer =
      //new MapFormatter();//built_in
      //new SingleLevelMapPacker();//custom
      new MultiLevelMapFormatter();

      var packets = new List<byte[]>();
      var x = new packmap(packer);
      var watch = System.Diagnostics.Stopwatch.StartNew();
      x.start(packet => packets.Add(packet));
      watch.Stop();
      WriteLine($"Done:{packets.Count} in {watch.ElapsedMilliseconds}ms");
      x.otherend(packets);
      WriteLine($"Checked:{packets.Count}");
    }
    static void to_file()
    {
      var packer =
      //new SingleLevelMapPacker();
      new MultiLevelMapFormatter();

      var packets = new List<byte[]>();
      var x = new packmap(packer);
      x.start(packet => packets.Add(packet));
      using (var writer = System.IO.File.Create("map1.dat"))
      {
        var packet = packets[0];
        writer.Write(packet, 0, packet.Length);
      }
    }
    public static void _Main()
    {
      test_limit_count();
      //to_file();
    }
  }

  #region tcps
  [System.ServiceModel.ServiceContract]
  public interface IServiceT
  {
    [System.ServiceModel.OperationContract]
    string gettime(int n);
  }
  [System.ServiceModel.ServiceBehavior(InstanceContextMode = System.ServiceModel.InstanceContextMode.PerCall, ConcurrencyMode = System.ServiceModel.ConcurrencyMode.Multiple)]
  class ServiceT : IServiceT
  {
    public string gettime(int n) => $"{n} | {DateTime.Now.ToString("s")}";
  }
  class tcps
  {
    const string addr = "net.tcp://localhost:9001/ServiceT";
    static void start_service()
    {
      using (var host = new System.ServiceModel.ServiceHost(typeof(ServiceT)))
      {

        host.AddServiceEndpoint(typeof(IServiceT), new System.ServiceModel.NetTcpBinding(), new Uri(addr));
        host.Open();
        WriteLine("Listening at:");
        foreach (var e in host.Description.Endpoints) WriteLine(e.Address.Uri);
        WriteLine("Press ENTER to exit");
        ReadLine();
      }
    }
    static IServiceT start_client()
    {
      //new System.ServiceModel.ChannelFactory<IServiceT>(new System.ServiceModel.NetTcpBinding(), "");
      return null;
    }
    static void call()
    {
      using (var f = new System.ServiceModel.ChannelFactory<IServiceT>(new System.ServiceModel.NetTcpBinding(), addr))
      {
        IServiceT proxy = f.CreateChannel();
        WriteLine(proxy.gettime(1));
      }
    }

    public static void _Main()
    {
      bool running = true;
      //tcps client;
      do
      {
        WriteLine("usage: call | client | service | exit");
        string cmd = ReadLine();
        switch (cmd)
        {
          case "client":
            //client=start_client();
            break;
          case "call":
            call();
            break;
          case "service":
            start_service();
            break;
          case "exit":
            running = false;
            break;
          default: WriteLine($"unknown command: {cmd}"); break;
        }
      } while (running);
    }
  }
  #endregion

  #region pcinfo
  //http://www.pinvoke.net/default.aspx/Structures/MEMORYSTATUSEX.html
  /// <summary>
  /// contains information about the current state of both physical and virtual memory, including extended memory
  /// </summary>
  [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
  public class MEMORYSTATUSEX
  {
    /// <summary>
    /// Size of the structure, in bytes. You must set this member before calling GlobalMemoryStatusEx. 
    /// </summary>
    public uint dwLength;

    /// <summary>
    /// Number between 0 and 100 that specifies the approximate percentage of physical memory that is in use (0 indicates no memory use and 100 indicates full memory use). 
    /// </summary>
    public uint dwMemoryLoad;

    /// <summary>
    /// Total size of physical memory, in bytes.
    /// </summary>
    public ulong ullTotalPhys;

    /// <summary>
    /// Size of physical memory available, in bytes. 
    /// </summary>
    public ulong ullAvailPhys;

    /// <summary>
    /// Size of the committed memory limit, in bytes. This is physical memory plus the size of the page file, minus a small overhead. 
    /// </summary>
    public ulong ullTotalPageFile;


    /// <summary>
    /// Size of available memory to commit, in bytes. The limit is ullTotalPageFile. 
    /// </summary>
    public ulong ullAvailPageFile;

    /// <summary>
    /// Total size of the user mode portion of the virtual address space of the calling process, in bytes. 
    /// </summary>
    public ulong ullTotalVirtual;

    /// <summary>
    /// Size of unreserved and uncommitted memory in the user mode portion of the virtual address space of the calling process, in bytes. 
    /// </summary>
    public ulong ullAvailVirtual;

    /// <summary>
    /// Size of unreserved and uncommitted memory in the extended portion of the virtual address space of the calling process, in bytes. 
    /// </summary>
    public ulong ullAvailExtendedVirtual;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MEMORYSTATUSEX"/> class.
    /// </summary>
    public MEMORYSTATUSEX()
    {
      this.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(MEMORYSTATUSEX));
    }
  }
  [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
  public struct PERFORMANCE_INFORMATION
  {
    public uint cb;
    public UIntPtr CommitTotal;
    public UIntPtr CommitLimit;
    public UIntPtr CommitPeak;
    public UIntPtr PhysicalTotal;
    public UIntPtr PhysicalAvailable;
    public UIntPtr SystemCache;
    public UIntPtr KernelTotal;
    public UIntPtr KernelPaged;
    public UIntPtr KernelNonpaged;
    public UIntPtr PageSize;
    public uint HandleCount;
    public uint ProcessCount;
    public uint ThreadCount;
  }

  class pcinfo
  {
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern bool GlobalMemoryStatusEx([System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] MEMORYSTATUSEX memory_status);

    [System.Runtime.InteropServices.DllImport("psapi.dll", SetLastError = true)]
    static extern bool GetPerformanceInfo(out PERFORMANCE_INFORMATION performance_data, uint input_size);

    public static void _Main()
    {
      //http://stackoverflow.com/questions/105031/how-do-you-get-total-amount-of-ram-the-computer-has
      var x = "System Code Total Bytes";
      var pc = new System.Diagnostics.PerformanceCounter("Memory", x);
      Console.WriteLine("{0}: {1}", x, pc.RawValue);

      var mc = new System.Management.ManagementClass("Win32_ComputerSystem");
      System.Management.ManagementObjectCollection moc = mc.GetInstances();
      foreach (System.Management.ManagementObject item in moc)
      {
        double n=Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value);
        Console.WriteLine("{0} {1}",Math.Round(n / 1048576, 0) + " MB", n);
      }

      string wmiquery = "SELECT Capacity FROM Win32_PhysicalMemory";
      var searcher = new System.Management.ManagementObjectSearcher(wmiquery);
      ulong capacity = 0L;
      foreach (System.Management.ManagementObject wmiobject in searcher.Get())
      {
        ulong n = Convert.ToUInt64(wmiobject.Properties["Capacity"].Value);
        capacity += n;
        Console.WriteLine(n);
      }
      Console.WriteLine($"capacity: {capacity}");

      long memKb;
      bool result_code=GetPhysicallyInstalledSystemMemory(out memKb);
      Console.WriteLine($"(GetPhysicallyInstalledSystemMemory result code: {result_code}) {(memKb / 1024 / 1024)} GB of RAM installed. memKb: {memKb}");

      //https://msdn.microsoft.com/en-us/library/windows/desktop/aa366589(v=vs.85).aspx
      var memory_status = new MEMORYSTATUSEX();
      result_code = GlobalMemoryStatusEx(memory_status);
      Console.WriteLine($"(GlobalMemoryStatusEx result code: {result_code})");
      foreach (var field in memory_status.GetType().GetFields())
      {
        Console.WriteLine($"{field.Name} = {field.GetValue(memory_status)}");
      }

      //https://msdn.microsoft.com/en-us/library/windows/desktop/ms683210(v=vs.85).aspx
      var performance_data = new PERFORMANCE_INFORMATION();
      uint input_size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(PERFORMANCE_INFORMATION));
      result_code = GetPerformanceInfo(out performance_data, input_size);
      Console.WriteLine($"(GetPerformanceInfo result code: {result_code})");
      foreach (var field in performance_data.GetType().GetFields())
      {
        Console.WriteLine($"{field.Name} = {field.GetValue(performance_data)}");
      }
    }
    #endregion
  }

  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        //ExploreFeatureCS6_exe._Main_cs6();
        //task1_exe._Main();
        //packmap._Main();
        //tcps._Main();
        pcinfo._Main();
      }
      catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}"); }
    }
  }
}