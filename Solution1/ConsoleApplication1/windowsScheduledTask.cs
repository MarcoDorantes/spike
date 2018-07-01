using System;
using System.Linq;
using static System.Console;

namespace ConsoleApplication1
{
  class windowsTask
  {
    public static void _Main(string[] args)
    {
      //foreach (var t in Microsoft.Win32.TaskScheduler.TaskService.Instance.AllTasks) WriteLine(t.Name);
      //foreach (var t in Microsoft.Win32.TaskScheduler.TaskService.Instance.AllTasks.Where(t=>t.Name.Contains("arr"))) WriteLine($"{t?.Enabled} | {t?.Name} | {t?.Path}");

      var server = Environment.MachineName;
      using (var tasker = new Microsoft.Win32.TaskScheduler.TaskService(server))
      {
        using (var task = tasker.FindTask("arr1"))
        {
          WriteLine($"{task?.Enabled} | {task?.Name} | {task?.Path} | Folder {task?.Folder.Name} {task.Folder.Path}");
          var tr = task?.Definition.Triggers.Count > 0 ? task?.Definition.Triggers[0] : null;
          if (tr != null)
            using (tr)
            {
              WriteLine($"\t{tr.Id} | {tr.Enabled} | {tr.StartBoundary}");
              tr.Enabled = true;
              var prev = tr.StartBoundary;
              var today = DateTime.Today.AddDays(10D);
              var start = new DateTime(today.Year, today.Month, today.Day, prev.Hour, prev.Minute, prev.Second);
              tr.StartBoundary = start;
              task.Definition.Settings.Enabled = true;
              task.RegisterChanges();
              WriteLine($"{task?.Enabled} | {task?.Name} | {task?.Path} | Folder {task?.Folder.Name} {task.Folder.Path}");
              WriteLine($"\t{tr.Id} | {tr.Enabled} | {tr.StartBoundary}");
            }
        }
      }
    }
  }
}