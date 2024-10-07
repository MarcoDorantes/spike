namespace work2;

using System;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using static System.Console;

public interface IClass1{}
public class Class1:IClass1{}
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
            //builder.Services.AddHostedService<Worker2>();
            builder.Services.AddSingleton<IClass1>(_=>new Class1());
            builder.Services.AddTransient<lib2.IServiceProcessor>(_=>new lib2.ServiceProcessor());
            builder.Services.AddTransient<lib2.IEngineProcessor>(_=>new lib2.EngineProcessor());
            builder.Services.AddTransient<lib2.IEngineOperationalWindowCycle>(_=>new lib2.EngineOperationalWindowCycle());

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