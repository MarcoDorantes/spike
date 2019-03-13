using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Console;

namespace diskmap
{
  class Input
  {
    public DirectoryInfo folder;
    public string files;
    public bool group;
    public bool pause;
    public void fsize()
    {
      if (folder == null) folder = new DirectoryInfo(Directory.GetCurrentDirectory());
      WriteLine($"Folder: {folder.FullName}");
      if (string.IsNullOrWhiteSpace(files)) files = "*.*";
      WriteLine($"Files: {files}");
      WriteLine($"Group by: {(group ? "DirectoryName" : "none")}");
      if (pause)
      {
        WriteLine("Press ENTER to continue"); ReadLine();
      }
      IEnumerable<FileInfo> found = folder.GetFiles(files, SearchOption.AllDirectories).OrderByDescending(f => f.Length);
      if (group)
      {
        var grouped = found.GroupBy(f => f.DirectoryName);
        foreach (var G in grouped)
        {
          WriteLine($"Folder= {G.Key}");
          foreach (var F in G)
          {
            WriteLine($"{F.Length,15:N0} {F.FullName}");
          }
        }
      }
      else
      {
        foreach (var F in found)
        {
          WriteLine($"{F.Length,15:N0} {F.FullName}");
        }
      }
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        if (args.Length < 1)
        {
          WriteLine("Usage:");
          nutility.Switch.ShowUsage(typeof(Input), Out);
        }
        else
        {
          nutility.Switch.AsType<Input>(args);
        }
      }
      catch (Exception ex)
      {
        for (int level = 0; ex != null; ex = ex?.InnerException, ++level)
        {
          WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}");
        }
      }
    }

    #region old attempts
    static IEnumerable<FileInfo> scan0(DirectoryInfo folder)
    {
      foreach (var f in folder.EnumerateFiles("*.*", SearchOption.AllDirectories)) yield return f;
    }
    static IEnumerable<FileInfo> scan(DirectoryInfo folder)
    {
      IEnumerable<FileInfo> files = null;
      files = folder.EnumerateFiles();
      if (files != null) foreach (var file in files) yield return file;

      IEnumerable<DirectoryInfo> subfolders = null;
      subfolders = folder.EnumerateDirectories();
      if (subfolders != null) foreach (var subfolder in subfolders) foreach (var file in scan(subfolder)) yield return file;
    }
    static void original_main(string[] args)
    {
      foreach (var f in scan(new DirectoryInfo(args[0])).OrderByDescending(n => n.Length))
        WriteLine($"{f.Length,15:N0}: {f.FullName}");
    }
    #endregion
  }
}