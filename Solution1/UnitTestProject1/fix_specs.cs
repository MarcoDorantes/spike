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
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(711, "2") },
        new List<KeyValuePair<int, string>> { new KeyValuePair<int, string>(35,"y"), new KeyValuePair<int, string>(1050, "123") }
      };
      IEnumerable<string> line = to_tabular(fixlog);
      Assert.AreEqual<string>("35,711,1050,", line.ElementAt(0));
      Assert.AreEqual<string>("y,2,,", line.ElementAt(1));
      Assert.AreEqual<string>("y,,123,", line.ElementAt(2));
    }
    IEnumerable<string> to_tabular(IEnumerable<List<KeyValuePair<int, string>>> fixlog)
    {
      var heads = fixlog.Aggregate(new List<int>(), (w, n) =>
       {
         n.Aggregate(w, (w2, n2) =>
         {
           if (w2.IndexOf(n2.Key) == -1)
           {
             w2.Add(n2.Key);
           }
           return w2;
         });
         return w;
       });
      var heads_line = heads.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      yield return heads_line;

      int distinct_tag_count = heads.Count;
      foreach (var msg in fixlog)
      {
        string[] line = new string[distinct_tag_count];
        foreach (var pair in msg)
        {
          int index = heads.IndexOf(pair.Key);
          line[index] = pair.Value;
        }
        yield return line.Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0},", n)).ToString();
      }
    }
  }
}