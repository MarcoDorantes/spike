using System;
using System.IO;
using System.Diagnostics;
using System.Security.Permissions;

/*
https://msdn.microsoft.com/en-us/library/system.appdomain.unhandledexception(v=vs.110).aspx
https://msdn.microsoft.com/en-us/library/system.runtime.exceptionservices.handleprocesscorruptedstateexceptionsattribute(v=vs.110).aspx
https://blogs.msdn.microsoft.com/lexli/2009/04/28/how-to-handle-net-unhandled-exceptions-gracefully
http://www.codeproject.com/Articles/7482/User-Friendly-Exception-Handling
https://msdn.microsoft.com/en-us/magazine/dd419661.aspx
*/
namespace ConsoleApplication1
{
  class unhandled_exceptions
  {
    static StreamWriter listenerWriter;
    static TextWriterTraceListener listener;

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public static void _Main(string[] args)
    {
      Trace.WriteLine("start");

      var logname = string.Format("{0}_{1}.{2}", "unhand", DateTime.Now.ToString("MMMdd-hhmmss-fffffff"), "log");
      var listenerFile = new System.IO.FileInfo(logname);
      listenerWriter = listenerFile.CreateText();
      listener = new TextWriterTraceListener(listenerWriter);
      Trace.Listeners.Add(listener);
      //Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
      Trace.AutoFlush = true;

      AppDomain currentDomain = AppDomain.CurrentDomain;
      currentDomain.UnhandledException += new UnhandledExceptionEventHandler(unhand_Handler);

      try
      {
        Trace.WriteLine("throw 1");
        throw new Exception("1");
      }
      catch (Exception e)
      {
        Trace.WriteLine($"Catch clause caught : {e.Message} \n");
      }
      Trace.WriteLine("throw 2");
      throw new Exception("2");
      //Trace.WriteLine("end");
    }

    static void unhand_Handler(object sender, UnhandledExceptionEventArgs args)
    {
      Exception e = (Exception)args.ExceptionObject;
      Trace.WriteLine("MyHandler caught : " + e.Message);
      Trace.WriteLine($"Runtime terminating: {args.IsTerminating}");
    }
  }
}