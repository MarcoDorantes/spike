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
  public class Patterns_of_Parallel_Programming_CSharp_pdf
  {
    [TestMethod]
    public void word_count()
    {
      var dirPath = "";
      char[] delimiters=null;

      var files = Directory.EnumerateFiles(dirPath, "*.txt").AsParallel();
      var counts = files.MapReduce(
      path => File.ReadLines(path).SelectMany(line => line.Split(delimiters)),
      word => word,
      group => new[] { new KeyValuePair<string, int>(group.Key, group.Count()) });
    }
  }
}