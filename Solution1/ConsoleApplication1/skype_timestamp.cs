using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.Generic;
using static System.Console;
using System.Text.RegularExpressions;

class SkypeChatReader : IDisposable
{
  const string NoLine = "\0x0\0x3\0x1\0x2";
  System.IO.StreamReader reader;
  string next_line;
  public SkypeChatReader(System.IO.StreamReader s) { reader = s; next_line = NoLine; }
  public string ReadLine()
  {
    var result = new StringBuilder();
    next_line = next_line == NoLine ? reader.ReadLine() : next_line;
    if (next_line == null) return null;
    if (next_line.StartsWith("live:") == false) throw new Exception("No proper Skype chat fragment");
    do
    {
      result.Append(next_line);
      next_line = reader.ReadLine();
      if (next_line?.StartsWith("live:") == false)
      {
        result.Append("\r\n");
      }
      else break;
    } while (true);
    return $"{result}";
  }
  public void Dispose() { }
}
class WhatsappChatReader : IDisposable
{
  public const string pattern = @"(?<line>(?<head>(?<when>\d\d/\d\d/\d\d\d\d \d\d:\d\d:\d\d \w*\. \w+\.): (?<who>\w*): )(?<what>.*))";
  const string NoLine = "\0x0\0x3\0x1\0x2";
  System.IO.StreamReader reader;
  string next_line;
  Regex regex;
  public WhatsappChatReader(System.IO.StreamReader s)
  {
    reader = s;
    next_line = NoLine;
    regex = new Regex(pattern);
  }
  public string ReadLine()
  {
    var result = new StringBuilder();
    Match match = null;
    do
    {
      next_line = next_line == NoLine ? reader.ReadLine() : next_line;
      if (next_line == null) return null;
      match = regex.Match(next_line);
      if (match.Success == false) throw new Exception("No proper Whatsapp chat fragment");
      if (string.IsNullOrWhiteSpace(match.Groups["what"].Value))
      {
        next_line = NoLine;
        continue;
      }
      else break;
    } while (true);
    do
    {
      result.Append(next_line);
      next_line = reader.ReadLine();
      if (next_line == null) break;
      match = regex.Match(next_line);
      if (match.Success == false)
      {
        result.Append("\r\n");
      }
      else break;
    } while (true);
    return $"{result}";
  }
  public void Dispose() { }
}
class field : List<string>
{
  public field(string value)
  {
    Add(value);
  }
  public field(IList<string> values)
  {
    AddRange(values);
  }
}
class skype
{
  static void calc_timestamp()
  {
    long TimestampMs = 1515796215068;//1515796233475L; //1516851138514L;

    long total_s = TimestampMs / 1000L;
    long r_ms = TimestampMs % 1000L;
    WriteLine($"total_s: {total_s}\nr_ms: {r_ms}");

    long total_m = total_s / 60L;
    long r_s = total_s % 60L;
    WriteLine($"total_m: {total_m}\nr_s: {r_s}");

    long total_h = total_m / 60L;
    long r_m = total_m % 60L;
    WriteLine($"total_h: {total_h}\nr_m: {r_m}");

    long total_d = total_h / 24L;
    long r_h = total_h % 24L;
    WriteLine($"total_d: {total_d}\nr_h: {r_h}");

    var calc = (((total_d * 24L + r_h) * 60L + r_m) * 60L + r_s) * 1000L + r_ms;
    WriteLine($"TimestampMs match: {TimestampMs == calc}");

    var span = new TimeSpan((int)total_d, (int)r_h, (int)r_m, (int)r_s, (int)r_ms);
    //WriteLine(span);
    var t0 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var when = t0.Add(span).ToLocalTime();
    WriteLine(when);
  }
  static DateTime timestamp(ulong TimestampMs)
  {

    ulong total_s = TimestampMs / 1000UL;
    ulong r_ms = TimestampMs % 1000UL;

    ulong total_m = total_s / 60UL;
    ulong r_s = total_s % 60UL;

    ulong total_h = total_m / 60UL;
    ulong r_m = total_m % 60UL;

    ulong total_d = total_h / 24UL;
    ulong r_h = total_h % 24UL;

    var calc = (((total_d * 24UL + r_h) * 60UL + r_m) * 60UL + r_s) * 1000UL + r_ms;

    var span = new TimeSpan((int)total_d, (int)r_h, (int)r_m, (int)r_s, (int)r_ms);
    var t0 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var when = t0.Add(span).ToLocalTime();
    return when;
  }
  static IList<string> split_fields_0(string line)
  {
    var result = new List<string>();
    int k = 0;
    int delimiter_index = -1, prev_delimiter_index = -1;
    do
    {
      if (k > 4) break;
      int startIndex = prev_delimiter_index == -1 ? 0 : prev_delimiter_index;
      delimiter_index = line.IndexOf(',', startIndex);
      if (delimiter_index < 0 || delimiter_index >= line.Length) break;
      int length = delimiter_index - startIndex - 1;
      var value = length > 0 ? line.Substring(prev_delimiter_index + 1, length) : "";
      prev_delimiter_index = delimiter_index;
      result.Add(value.Trim('"'));
      ++k;
    } while (true);
    var last = k == 5 ? line.Substring(prev_delimiter_index + 1) : "";
    result.Add(last);
    return result;
  }
  static IList<field> skype_parse_fields(string line)
  {
    var result = new List<field>();
    var value = new System.Text.StringBuilder();
    int count = 0;
    int k;
    for (k = 0; k < line.Length && count < 6; ++k)
    {
      char c = line[k];
      if (c == ',')
      {
        result.Add(new field($"{value}".Trim('"')));
        value.Clear();
        ++count;
      }
      else value.Append(c);
    }
    if (count == 6)
    {
      var ts = line.Substring(k).Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Aggregate(new List<string>(), (w, n) => { w.Add(n.Trim('"')); return w; });
      var content_field = new field(ts);
      result.Add(content_field);
    }
    return result;
  }
  static IList<field> whatsapp_parse_fields(string line)
  {
    var result = new List<field> { null, null, null };
    var what = new List<string>();
    var regex = new Regex(WhatsappChatReader.pattern);
    foreach (var subline in line.Split(new[] { "\r\n" }, StringSplitOptions.None))
    {
      var match = regex.Match(subline);
      if (match.Success)
      {
        result[0] = new field(match.Groups["when"].Value);
        result[1] = new field(match.Groups["who"].Value);
        what.Add(match.Groups["what"].Value);
      }
      else what.Add(subline);
    }
    result[2] = new field(what);
    return result;
  }
  static IEnumerable<IList<field>> read_skype()
  {
    //ConversationId,ConversationName,AuthorId,AuthorName,HumanTime,TimestampMs,ContentXml
    using (var text_reader = System.IO.File.OpenText("SkypeChatHistory.csv"))
    using (var reader = new SkypeChatReader(text_reader))
      do
      {
        var line = reader.ReadLine();
        if (line == null) break;
        line = line.Trim();
        if (string.IsNullOrWhiteSpace(line)) continue;
        yield return skype_parse_fields(line);
      } while (true);
  }
  static IEnumerable<IList<field>> read_whatsapp()
  {
    //when,who,what
    using (var text_reader = System.IO.File.OpenText("WhatsAppChat.txt"))
    using (var reader = new WhatsappChatReader(text_reader))
      do
      {
        var line = reader.ReadLine();
        if (line == null) break;
        line = line.Trim();
        if (string.IsNullOrWhiteSpace(line)) continue;
        yield return whatsapp_parse_fields(line);
      } while (true);
  }
  static void skype_show()
  {
    foreach (var fields in read_skype())
    {
      if (fields?.Count == 0) continue;
      foreach (var f in fields) WriteLine($"\t{f.Aggregate(new StringBuilder(),(w,n)=>w.AppendFormat("{0} ",n))}"); WriteLine();
      if (!fields[0][0].Contains("_549")) continue;
      /*var who = fields[3][0];
      var what = fields[6].Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0} ", n));
      WriteLine($"{who}:\n  {what}");*/
    }
  }
  static void whatsapp_show()
  {
    foreach (var fields in read_whatsapp())
    {
      if (fields?.Count == 0) continue;
      foreach (var f in fields) WriteLine($"\t{f.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0} ", n))}"); WriteLine();
    }
  }
  static XDocument skype_toXDocument()
  {
    var says = new List<object>();
    foreach (var fields in read_skype())
    {
      if (fields?.Count == 0) continue;
      if (!fields[0][0].Contains("_549")) continue;
      int count = 0;
      var saychilds = fields.Take(6).Aggregate(new List<object>(), (w, n) => { w.Add(new XElement($"_{count++}", n)); return w; });
      var _6childs = fields[6].Aggregate(new List<object>(), (w, n) => { w.Add(new XElement($"t", n)); return w; });
      saychilds.Add(new XElement($"_6", _6childs));
      saychilds.Add(new XElement("when", $"{timestamp(ulong.Parse(fields[5][0])).ToString("yyyy-MM-dd HH:mm:ss")}"));
      says.Add(new XElement(new XElement("say", saychilds)));
    }
    return new XDocument(new XElement("chat", says));
  }
  static XDocument whatsapp_toXDocument()
  {
    var says = new List<object>();
    foreach (var fields in read_whatsapp())
    {
      if (fields?.Count == 0) continue;
      var saychilds = new List<object> { new XElement($"_0", fields[0][0]), new XElement($"_3", fields[1][0]) };
      var _6childs = fields[2].Aggregate(new List<object>(), (w, n) => { w.Add(new XElement($"t", n)); return w; });
      saychilds.Add(new XElement($"_6", _6childs));
      saychilds.Add(new XElement("when", $"{DateTime.ParseExact(fields[0][0].Replace("a. m.", "am").Replace("p. m.", "pm"), "dd/MM/yyyy hh:mm:ss tt", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss")}"));
      says.Add(new XElement(new XElement("say", saychilds)));
    }
    return new XDocument(new XElement("chat", says));
  }
  static XDocument toCleanXDocument(XDocument xml)
  {
    return new XDocument(new object[] { toCleanElement(xml.Root) });
  }
  static XElement toCleanElement(XElement node)
  {
    if(!node.HasElements)
    {
      if (node.Value?.StartsWith("<ss") == true)
      {
        var sub = XDocument.Parse(node.Value.Replace("\"\"", "'"));
        return new XElement(node.Name, sub.Root.Value);
      }
      else
      {
        return new XElement(node.Name, node.Value);
      }
    }
    else
    {
      return new XElement(node.Name, toCleanElements(node.Elements()));
    }
  }
  static object[] toCleanElements(IEnumerable<XElement> nodes)
  {
    return nodes.Aggregate(new List<object>(), (w, n) => { w.Add(toCleanElement(n)); return w; }).ToArray();
  }
  static void skype_toxml()
  {
//<?xml-stylesheet type="text/xsl" href="file:///C:\design\github_spike\spike\Solution1\ConsoleApplication1\skype_conversation_o365.xslt"?>
    var chat = skype_toXDocument();
    using (var writer = System.Xml.XmlWriter.Create("skypetoxml.xml")) chat.Save(writer);
  }
  static void whatsapp_toxml()
  {
    var chat = whatsapp_toXDocument();
    using (var writer = System.Xml.XmlWriter.Create("whatsapptoxml.xml")) chat.Save(writer);
  }
  static void toflatxml()
  {
    var chat = toCleanXDocument(skype_toXDocument());
    using (var writer = System.Xml.XmlWriter.Create("skypetoxml.xml")) chat.Save(writer);
  }
  static void skype_tohtml(string[] args)
  {
    string html_output_file = args.ElementAtOrDefault(0);
    if (string.IsNullOrWhiteSpace(html_output_file)) throw new ArgumentNullException($"{nameof(html_output_file)}");
    var chat = toCleanXDocument(skype_toXDocument());
    using (var writer = System.Xml.XmlWriter.Create("skypetohtml.html", new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true }))
    {
      var xslt = new System.Xml.Xsl.XslCompiledTransform();
      xslt.Load(System.Xml.XmlReader.Create(html_output_file));
      xslt.Transform(chat.CreateReader(), writer);
    }
  }
  static void whatsapp_tohtml(string[] args)
  {
    string html_output_file = args.ElementAtOrDefault(0);
    if (string.IsNullOrWhiteSpace(html_output_file)) throw new ArgumentNullException($"{nameof(html_output_file)}");
    var chat = whatsapp_toXDocument();
    using (var writer = System.Xml.XmlWriter.Create("whatsapptohtml.html", new System.Xml.XmlWriterSettings { OmitXmlDeclaration = true }))
    {
      var xslt = new System.Xml.Xsl.XslCompiledTransform();
      xslt.Load(System.Xml.XmlReader.Create(html_output_file));
      xslt.Transform(chat.CreateReader(), writer);
    }
  }
  static void initial()
  {
    //ConversationId,ConversationName,AuthorId,AuthorName,HumanTime,TimestampMs,ContentXml
    using (var reader = System.IO.File.OpenText("SkypeChatHistory.csv"))
      do
      {
        var line = reader.ReadLine();
        if (line == null) break;
        line = line.Trim();
        if (string.IsNullOrWhiteSpace(line)) continue;
        var fields = skype_parse_fields(line);
        foreach (var f in fields) WriteLine($"\t{f}"); WriteLine();
        if (fields.Count == 0) continue;
        if (!fields[0].Contains("_549")) continue;
        /*var who = fields[3];
        var what = fields[6];
        WriteLine($"{who}:\n  {what}");*/
      } while (true);
  }
  public static void _Main(string[] args)
  {
    try
    {
      //skype_show();
      //skype_toxml();
      //toflatxml();
      //skype_tohtml(args);

      //whatsapp_show();
      //whatsapp_toxml();
      whatsapp_tohtml(args);
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}"); }
  }
}
/*delete-1
using System;
using System.Collections.Generic;

namespace ChatHistoryModule
{
  public class ChatRecordParser
  {
    public ChatRecord Parse(string entry)
    {
      var fields = parse_fields(entry);
      return new ChatRecord
      {
        ConversationId = fields[0],
        ConversationName = fields[1],
        AuthorId = fields[2],
        AuthorName = fields[3],
        HumanTime = fields[4],
        TimestampMs = ulong.Parse(fields[5]),
        When = timestamp(ulong.Parse(fields[5])),
        ContentXml = fields[6]
      };
    }
    static IList<string> parse_fields(string line)
    {
      var result = new List<string>();
      var value = new System.Text.StringBuilder();
      int count = 0;
      int k;
      for (k = 0; k < line.Length && count < 6; ++k)
      {
        char c = line[k];
        if (c == ',')
        {
          result.Add($"{value}".Trim('"'));
          value.Clear();
          ++count;
        }
        else value.Append(c);
      }
      if (count == 6) result.Add(line.Substring(k).Trim('"'));
      return result;
    }
    static DateTime timestamp(ulong TimestampMs)
    {

      ulong total_s = TimestampMs / 1000UL;
      ulong r_ms = TimestampMs % 1000UL;

      ulong total_m = total_s / 60UL;
      ulong r_s = total_s % 60UL;

      ulong total_h = total_m / 60UL;
      ulong r_m = total_m % 60UL;

      ulong total_d = total_h / 24UL;
      ulong r_h = total_h % 24UL;

      var calc = (((total_d * 24UL + r_h) * 60UL + r_m) * 60UL + r_s) * 1000UL + r_ms;

      var span = new TimeSpan((int)total_d, (int)r_h, (int)r_m, (int)r_s, (int)r_ms);
      var t0 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      var when = t0.Add(span).ToLocalTime();
      return when;
    }
  }
}
delete-1*/

/*delete-2
namespace HouseRobber
{
  public static class HouseRobber_I
    {
    public static int Algorithm1(int[] house)
    {
      if (house == null)
      {
        throw new ArgumentNullException(nameof(house));
      }
      if (house.Length == 0)
      {
        return 0;
      }
      if (house.Any(n => n < 0))
      {
        throw new ArgumentOutOfRangeException(nameof(house));
      }
      if (house.Length == 1) return house[0];
      if (house.Length == 2) Math.Max(house[0], house[1]);
      return Math.Max(algorithm1(house, 0), algorithm1(house, 1));
    }
    private static int algorithm1(int[] house, int start_index)
    {
      int skip1_sum = -1;
      if (start_index + 2 < house.Length)
      {
        skip1_sum = house[start_index] + algorithm1(house, start_index + 2);
      }
      int skip2_sum = -1;
      if (start_index + 3 < house.Length)
      {
        skip2_sum = house[start_index] + algorithm1(house, start_index + 3);
      }
      if (skip1_sum > skip2_sum)
      {
        return house[start_index] + algorithm1(house, start_index + 2);
      }
      else if (skip1_sum < skip2_sum)
      {
        return house[start_index] + algorithm1(house, start_index + 3);
      }
      else return house[start_index];
    }
    public static int Algorithm2(int[] num)
    {
      if (num == null)
      {
        throw new ArgumentNullException(nameof(num));
      }
      if (num.Any(n => n < 0))
      {
        throw new ArgumentOutOfRangeException(nameof(num));
      }
      int sz = num.Length;
      if (sz == 0) return 0;
      if (sz == 1) return num[0];
      if (sz == 2) return Math.Max(num[0], num[1]);
      int[] f = new int[sz];
      Array.Copy(num, f, sz);
      f[0] = num[0];
      f[1] = Math.Max(num[0], num[1]);
      for (int i = 2; i < sz; ++i)
      {
        f[i] = Math.Max(f[i - 2] + num[i], f[i - 1]);
      }
      return f[sz - 1];
    }
  }
}
delete-2*/
