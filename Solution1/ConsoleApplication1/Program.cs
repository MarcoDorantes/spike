using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApplication1
{
  interface IMapPacker
  {
    byte[] Pack(IDictionary<string, object> map);
    IDictionary<string, object> UnPack(byte[] packet);
  }
  class MapPacker : IMapPacker
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
  class WnMapPacker : IMapPacker
  {
    public byte[] Pack(IDictionary<string, object> map)
    {
      return Encoding.UTF8.GetBytes(map.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}\x1{1}\x0", next.Key, next.Value)).ToString());
    }

    public IDictionary<string, object> UnPack(byte[] packet)
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
  class packmap
  {
    IMapPacker packer;
    public packmap(IMapPacker packer)
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

      var s1 = new MapPacker();
      byte[] packet = s1.Pack(map1);
      WriteLine($"map1: {packet.Length}");

      var s2 = new MapPacker();
      var map2 = s2.UnPack(packet);

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
      var map = Enumerable.Range(0, 0xF).ToDictionary(n => n.ToString(), n => (object)((decimal)(n * 2)));
      map["AppInstanceID"] = Guid.NewGuid().ToString();
      map["AppName"] = "TestApp1";
      map["NamespaceID"] = Guid.NewGuid().ToString();
      map["NamespaceName"] = "namespace1";
      map["ProcessorID"] = Guid.NewGuid().ToString();
      map["ProcessorName"] = "namespace1";

      for (int k = 0; k < limit; ++k)
      {
        map["Count1"] = k.ToString("N0");
        byte[] packet = packer.Pack(map);
        send(packet);
        System.Threading.Thread.Sleep(100);
      }
    }
    const int limit = 100;
    /*
    Done:10 in 1110
    Checked:10

    Done:100 in 10926ms
    Checked:100
    */
    void otherend(List<byte[]> packets)
    {
      for (int k = 0; k < packets.Count; ++k)
      {
        var p = packets[k];
        var map = packer.UnPack(p);
        if (map["Count1"].ToString() != k.ToString("N0"))
        {
          WriteLine($"{k} Count1 does not match");
        }
      };
    }
    public static void _Main()
    {
      var packer =
        new MapPacker();//built_in
        //new WnMapPacker();//custom

      var packets = new List<byte[]>();
      var x = new packmap(packer);
      var watch = System.Diagnostics.Stopwatch.StartNew();
      x.start(packet => packets.Add(packet));
      watch.Stop();
      WriteLine($"Done:{packets.Count} in {watch.ElapsedMilliseconds}ms");
      x.otherend(packets);
      WriteLine($"Checked:{packets.Count}");
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
      catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}"); }
    }
  }
}