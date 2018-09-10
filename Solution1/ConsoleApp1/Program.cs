using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine($"here: {libSvs.Class1.f()}");

            /*
2>------ Build started: Project: ConsoleApp1, Configuration: Release Any CPU ------
2>C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\Microsoft.Common.CurrentVersion.targets(1657,5): error : Project '..\libCoreVS\libCoreVS.csproj' targets 'netcoreapp2.1'. It cannot be referenced by a project that targets '.NETFramework,Version=v4.7.2'.
             */
            //WriteLine($"here: {libCoreVS.Class1.g()}");
        }
    }
}