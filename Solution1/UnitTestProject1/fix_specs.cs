using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace UnitTestProject1
{
  [TestClass]
  public class fix_specs
  {
    [TestMethod]
    public void add_as_tags_come()
    {
      IEnumerable<List<KeyValuePair<int,string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,710,1050,", line.ElementAt(0));
      Assert.AreEqual<string>("y,2,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,123,", line.ElementAt(2));
    }
    [TestMethod]
    public void add_as_tags_come2()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,", line.ElementAt(3));
    }
    [TestMethod]
    public void add_as_tags_come_rgroup()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(711, "1"), new KeyValuePair<int, string>(361, "A"), new KeyValuePair<int, string>(362, "Z") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,711,361,362,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,,,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,,,,", line.ElementAt(3));
      Assert.AreEqual<string>("y,,,,1,A,Z,", line.ElementAt(4));
    }
    [TestMethod]
    public void add_as_tags_come_rgroups()
    {
      IEnumerable<List<KeyValuePair<int, string>>> fixlog = new List<List<KeyValuePair<int, string>>>
      {
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"8"), new KeyValuePair<int, string>(22, "X2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(710, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(711, "2"), new KeyValuePair<int, string>(361, "A1"), new KeyValuePair<int, string>(362, "Z1"), new KeyValuePair<int, string>(361, "A2"), new KeyValuePair<int, string>(362, "Z2") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,22,710,1050,711,361,362,361,362,", line.ElementAt(0));
      Assert.AreEqual<string>("8,X2,,,,,,,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,2,,,,,,,", line.ElementAt(2));
      Assert.AreEqual<string>("y,,,123,,,,,,", line.ElementAt(3));
      Assert.AreEqual<string>("y,,,,2,A1,Z1,A2,Z2,", line.ElementAt(4));
    }
    IEnumerable<string> to_tabular(IEnumerable<List<KeyValuePair<int, string>>> fixlog)
    {
      var heads = to_tabular_heads(fixlog);
      var heads_line = heads.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      yield return heads_line;

      int distinct_tag_count = heads.Count;
      foreach (var msg in fixlog)
      {
        string[] line = new string[distinct_tag_count];
        foreach (var pair in msg)
        {
          int repeated_tag_count_in_msg = msg.Count(p => p.Key == pair.Key);
          if (repeated_tag_count_in_msg == 1)
          {
            int index = heads.IndexOf(pair.Key);
            line[index] = pair.Value;
          }
          else
          {
            int start_index = 0;
            do
            {
              int index = heads.FindIndex(start_index, t => t == pair.Key);
              if (line[index] == null)
              {
                line[index] = pair.Value;
                break;
              }
              else
              {
                start_index = index + 1;
              }
            } while (start_index < distinct_tag_count);
          }
        }
        yield return line.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      }
    }
    List<int> to_tabular_heads(IEnumerable<List<KeyValuePair<int, string>>> fixlog)
    {
      return fixlog.Aggregate(new List<int>(), (whole, msg) =>
      {
        msg.Aggregate(whole, (same_whole, pair) =>
        {
          if (same_whole.IndexOf(pair.Key) == -1)
          {
            same_whole.Add(pair.Key);
          }
          else
          {
            int repeated_tag_count_in_msg = msg.Count(p => p.Key == pair.Key);
            if (repeated_tag_count_in_msg > 1)
            {
              same_whole.Add(pair.Key);
            }
          }
          return same_whole;
        });
        return whole;
      });
    }
  }
}