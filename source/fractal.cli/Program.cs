namespace fractal.cli
{
  class Program
  {
    static void Main(string[] args) => fractal.lib.CLI.MainEntryPoint(args, System.AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName);
  }
}