using System;
using System.IO;
using System.Linq;
using static System.Console;

namespace diskmap
{
    class Program
    {
        static void Main(string[] args)
        {
	try{

if(args.Length < 1) {WriteLine("diskmap <path>");return;}
var dir=new DirectoryInfo(args[0]);
var files=dir.EnumerateFiles("*.*",SearchOption.AllDirectories);
WriteLine($"count:{files.Count()}");
foreach(var f in files.OrderByDescending(n=>n.Length))
WriteLine($"{f.Length,15:N0}: {f.FullName}");
	}catch(Exception ex){WriteLine($"{ex.GetType().FullName}: {ex.Message}");}
        }
    }
}