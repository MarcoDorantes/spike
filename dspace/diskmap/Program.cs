using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace diskmap
{
  class Program
  {
    static IEnumerable<FileInfo> scan0(DirectoryInfo folder)
    {
      foreach(var f in folder.EnumerateFiles("*.*",SearchOption.AllDirectories)) yield return f;
    }
    static IEnumerable<FileInfo> scan(DirectoryInfo folder)
    {
      IEnumerable<FileInfo> files=null;
      try{ files=folder.EnumerateFiles();} catch{ }
      if(files!=null) foreach(var file in files) yield return file;

      IEnumerable<DirectoryInfo> subfolders=null;
      try{ subfolders=folder.EnumerateDirectories(); } catch{ }
      if(subfolders!=null) foreach(var subfolder in subfolders) foreach(var file in scan(subfolder)) yield return file;
    }
    static void Main(string[] args)
    {
      try
      {
        if(args.Length < 1) {WriteLine("diskmap <path>");return;}
        foreach(var f in scan(new DirectoryInfo(args[0])).OrderByDescending(n=>n.Length))
        WriteLine($"{f.Length,15:N0}: {f.FullName}");
      }catch(Exception ex){WriteLine($"{ex.GetType().FullName}: {ex.Message}");}
    }
  }
}