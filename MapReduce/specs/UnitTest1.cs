using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace specs
{
  public static class extensions
  {
    public static IEnumerable<TResult> MapReduce<TSource, TMapped, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TMapped>> map, Func<TMapped, TKey> keySelector, Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce)
    {
      return source.SelectMany(map)
      .GroupBy(keySelector)
      .SelectMany(reduce);
    }

    public static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(this ParallelQuery<TSource> source, Func<TSource, IEnumerable<TMapped>> map, Func<TMapped, TKey> keySelector, Func<IGrouping<TKey, TMapped>, IEnumerable<TResult>> reduce)
    {
      return source.SelectMany(map)
      .GroupBy(keySelector)
      .SelectMany(reduce);
    }
  }

  [TestClass]
  public class UnitTest1
  {
    [TestMethod, Description("Patterns_of_Parallel_Programming_CSharp_pdf")]
    public void word_count()
    {
      var dirPath = "";
      char[] delimiters = Enumerable.Range(0, 256).Select(i => (char)i).Where(c => Char.IsWhiteSpace(c) || Char.IsPunctuation(c)).ToArray();

      var files = Directory.EnumerateFiles(dirPath, "*.txt").AsParallel();
      var counts = files.MapReduce(
      path => File.ReadLines(path).SelectMany(line => line.Split(delimiters)),
      word => word,
      group => new[] { new KeyValuePair<string, int>(group.Key, group.Count()) });
    }

    [TestMethod]
    public void destinations()
    {
      var input = new Dictionary<string, string> { { "dest1", "A/B/1" }, { "dest2", "A/B/2" } };
      var r = input.SelectMany(pair => { var v = pair.Value.Split('/'); return new Dictionary<string, string> { { "m", v[0] }, { "c", v[1] }, { "h", v[2] } }; });
      //Write(r.Count());
      //foreach(var x in r) Write($"{x} ");
      var gs = r.GroupBy(p => p.Key);
      gs.Aggregate(new StringWriter(), (whole, next) => { whole.WriteLine($"[{next.Key}]" + next.Aggregate(new System.Text.StringBuilder(), (w, n) => w.AppendFormat($"{n} ")).ToString()); return whole; });

      /*
      [m][m, A] [m, A]
      [c][c, B] [c, B]
      [h][h, 1] [h, 2]
      */
    }
  }
  [TestClass]
  public class play
  {
    [TestMethod]
    public void MCD_0()
    {
      Assert.AreEqual<int>(1,MCD(2,3));
      Assert.AreEqual<int>(6, mcm(2, 3));
      Assert.AreEqual<int>(3, MCD(153, 93, 78));
      Assert.AreEqual<int>(123318, mcm(153, 93, 78));
    }
    private int MCD(params int[] n)
    {
      return 0;
    }
    private int mcm(params int[] n)
    {
      return 0;
    }
  }
}