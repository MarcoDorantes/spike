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
    }
    public byte[] Pack(IDictionary<string, string> map)
    {
      using (var stream = new System.IO.MemoryStream())
      {
        serial.Serialize(stream, map);
        return stream.GetBuffer();
      }
    }

    public IDictionary<string, string> UnPack(byte[] packet)
    {
      using (var stream = new System.IO.MemoryStream(packet))
      {
        return serial.Deserialize(stream) as IDictionary<string, string>;
      }
    }
  }
  class packmap
  {
    public static void _Main()
    {
      /*var serial = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      var map=Enumerable.Range(0, 0xFFF).ToDictionary(n => n, n => n * 2);
      var stream = new System.IO.MemoryStream();
      serial.Serialize(stream, map);
      Console.WriteLine($"{stream.GetBuffer().Length}");*/

      var s1 = new MapPacker();
      var map1 = Enumerable.Range(0, 0xFFF).ToDictionary(n => n.ToString(), n => (n * 2).ToString());
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

//      Console.WriteLine(int.Parse("1,234", System.Globalization.NumberStyles.AllowThousands));
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