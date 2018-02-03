using System;
using System.Collections.Generic;
using static System.Console;

class skype
{
  static void timestamp()
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
  public static void _Main(string[] args)
  {
    try
    {
      //ConversationId,ConversationName,AuthorId,AuthorName,HumanTime,TimestampMs,ContentXml
      using (var reader = System.IO.File.OpenText("SkypeChatHistory.csv"))
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          line = line.Trim();
          if (string.IsNullOrWhiteSpace(line)) continue;
          var fields = parse_fields(line);
          //foreach (var f in fields) WriteLine($"{f}"); WriteLine();
          if (fields.Count == 0) continue;
          if (!fields[0].Contains("_549")) continue;
          var who = fields[3];
          var what = fields[6];
          WriteLine($"{who}:\n  {what}");
        } while (true);
    }
    catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}"); }
  }
}
//delete
namespace ChatHistoryModule
{
  //class ChatRecordParser { public ChatRecord Parse(string entry) => null; }
  class ChatRecordParser
  {
    public ChatRecord Parse(string entry)
    {
      var fields = parse_fields(entry);
      return new ChatRecord { ContentXml = fields[6] };
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
  }
  class ChatRecord
  {
    public string ConversationId;
    public string ConversationName;
    public string AuthorId;
    public string AuthorName;
    public string HumanTime;
    public ulong TimestampMs;
    public string ContentXml;
  }
}
//delete