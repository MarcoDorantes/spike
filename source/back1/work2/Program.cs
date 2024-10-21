namespace work2;

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using static System.Console;

public interface IClass1{}
public class Class1:IClass1{}

public interface IFileLogger<Class2>: ILogger<Class2>{}
public class FileLogger<Class2> : IFileLogger<Class2>
{
    #region ILogger<T>
    public System.IDisposable BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception, System.Func<TState, System.Exception, string> formatter){}
    #endregion
}

public interface IClass2 {void f(string s);}
public class Class2(ILogger<Class2> logger1, IFileLogger<Class2> logger2):IClass2
{
    public void f(string s)
    {
        logger2.LogInformation("{what} f({s}) at: {time:yyyy-MM-dd HH:mm:ss.fffffff}", GetType().Name, s, DateTimeOffset.Now);
        logger1.LogInformation("{what} f({s}) at: {time:yyyy-MM-dd HH:mm:ss.fffffff}", GetType().Name, s, DateTimeOffset.Now);
    }
}

class Program
{
    static void ildasm(Type t)
    {
        WriteLine($"Type: {t.FullName}");
        WriteLine("\tProperties:");
        foreach(var p in t.GetProperties()) WriteLine($"\t\t{p.Name}:{p.PropertyType.FullName}");
        WriteLine("\tMethods:");
        foreach(var m in t.GetMethods()) WriteLine($"\t\t{m.Name}");
    }
    static void ildasm(object x)
    {
        ildasm(x.GetType());
    }
    static void Main(string[] args)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            //ildasm(builder);
            /*ildasm(builder.Environment);
            WriteLine(builder.Environment.EnvironmentName);
            WriteLine(builder.Environment.ApplicationName);
            WriteLine(builder.Environment.ContentRootPath);
            ildasm(builder.Environment.ContentRootFileProvider);
            WriteLine((builder.Environment.ContentRootFileProvider as Microsoft.Extensions.FileProviders.PhysicalFileProvider).Root);*/
            //ildasm(builder.Configuration);
            //ildasm(builder.Services);
            //foreach(var t in builder.Services.GetType().Assembly.GetTypes()) WriteLine(t.FullName);
            //var mm=builder.Services.GetType().Assembly.GetTypes().SelectMany(t=>t.GetMethods()).Where(m=>m.Name.StartsWith("Add"));
            //mm.ToList().ForEach(m=>WriteLine($"{m.Name}@{m.DeclaringType.FullName}"));
            //ildasm(mm.First().GetType());
            //ildasm(typeof(Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions));
            // var mm=typeof(Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions).GetMethods().Where(m=>m.Name=="Add");
            // mm.First().GetParameters().ToList().ForEach(p=>WriteLine($"{p.Name}"));
            //foreach(var t in builder.Services.GetType().Assembly.GetTypes()) WriteLine(t.FullName);
            //ildasm(typeof(Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions));
            //ildasm(typeof(Microsoft.Extensions.DependencyInjection.ObjectFactory));
            //return;

//          using ILoggerFactory logfactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

          //builder.Services.AddHostedService<Worker2>();
            builder.Services.AddSingleton<IClass1>(_=>new Class1());

            builder.Services.AddTransient<IClass2,Class2>();
            builder.Services.AddTransient<IFileLogger<Class2>,FileLogger<Class2>>();

          //builder.Services.AddTransient<lib2.IServiceProcessor>(provider => new lib2.ServiceProcessor(logfactory.CreateLogger<lib2.ServiceProcessor>()));
            builder.Services.AddTransient<lib2.IServiceProcessor, lib2.ServiceProcessor>();

            builder.Services.AddTransient<lib2.IEngineProcessor, lib2.EngineProcessor>();
            builder.Services.AddTransient<lib2.IEngineOperationalWindow, lib2.EngineOperationalWindow>();

            var host = builder.Build();
            host.Run();
            //host.Run();

            // var builder2 = Host.CreateApplicationBuilder(args);
            // builder2.Services.AddHostedService<Worker2>();

            // var host2 = builder2.Build();
            // host2.Run();            
        }
        catch(Exception ex)
        {
            for(int level=0; ex!=null;ex=ex.InnerException,++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}{ex.StackTrace}");
        }
    }
}