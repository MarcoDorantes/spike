using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApplication1
{
  class IFormatter
  { }
  class MapPacker
  {
    private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serial;

    public MapPacker()
    {
      serial = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      serial.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
      serial.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesAlways;
      serial.FilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
    }
    public byte[] Pack(IDictionary<string, object> map)
    {
      using (var stream = new System.IO.MemoryStream())
      {
        serial.Serialize(stream, map);
        return stream.GetBuffer();
      }
    }

    public IDictionary<string, object> UnPack(byte[] packet)
    {
      using (var stream = new System.IO.MemoryStream(packet))
      {
        return serial.Deserialize(stream) as IDictionary<string, object>;
      }
    }
  }
  class packmap
  {

    public static void _Main()
    {
      var s1 = new MapPacker();
      var map1 = Enumerable.Range(0, 0xFFF).ToDictionary(n => n.ToString(), n => (object)(n * 2));
      map1["AppInstanceID"] = Guid.NewGuid().ToString();
      map1["AppName"] = "TestApp1";
      map1["NamespaceID"] = Guid.NewGuid().ToString();
      map1["NamespaceName"] = "namespace1";
      map1["ProcessorID"] = Guid.NewGuid().ToString();
      map1["ProcessorName"] = "namespace1";
      byte[] packet = s1.Pack(map1);
      Console.WriteLine($"map1: {packet.Length}");

      var s2 = new MapPacker();
      var map2 = s2.UnPack(packet);

      bool equals = map1.SequenceEqual(map2);
      Console.WriteLine($"=: {equals}");

      if (!equals)
      {
        Console.WriteLine("\nmap1:");
        Console.WriteLine(map1.Count);
        map1.Aggregate(Console.Out, (whole, next) => { whole.WriteLine($"[{next.Key}] : [{next.Value}]"); return whole; });
        Console.WriteLine("\nmap2:");
        Console.WriteLine(map2.Count);
        map2.Aggregate(Console.Out, (whole, next) => { whole.WriteLine($"[{next.Key}] : [{next.Value}]"); return whole; });
      }
    }
  }
  class Program
  {
    static void Main()
    {
      try
      {
        //ExploreFeatureCS6_exe._Main_cs6();
        //task1_exe._Main();
        packmap._Main();
      }
      catch (Exception ex) { Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
    }
  }
}