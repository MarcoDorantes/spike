using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Text;

namespace UnitTestProject1
{
  [TestClass]
  public class RX_Spec
  {
    /*

http://reactivex.io
http://reactivex.io/intro.html

https://github.com/Reactive-Extensions/Rx.NET
https://msdn.microsoft.com/en-us/library/hh242985(v=vs.103).aspx
https://msdn.microsoft.com/en-us/library/system.reactive.linq.observable(v=vs.103).aspx

Reactive Extensions (Rx) v2.0
https://www.microsoft.com/en-us/download/details.aspx?id=30708

Reactive Extensions (Rx) - Main Library 3.1.1
https://www.nuget.org/packages/System.Reactive/
     */
    [TestMethod]
    public void TestMethod1()
    {
      //System.Reactive.dll ?

      //System.Reactive.Linq.Observable.
      Assert.AreEqual<string>("System.Reactive.Linq.Observable", typeof(System.Reactive.Linq.Observable).FullName);
      Trace.WriteLine(this.GetType().Assembly.GetReferencedAssemblies().Aggregate(new StringBuilder(),(w,n)=>w.AppendFormat("{0}\n",n.FullName)).ToString());
      Assert.IsTrue(this.GetType().Assembly.GetReferencedAssemblies().Any(a => a.FullName.StartsWith("System.Reactive")));
    }
  }
}
/*
Each package is licensed to you by its owner. NuGet is not responsible for, nor does it grant any licenses to, third-party packages. Some packages may include dependencies which are governed by additional licenses. Follow the package source (feed) URL to determine any dependencies.

Package Manager Console Host Version 3.4.4.1321

Type 'get-help NuGet' to see all available NuGet commands.

PM> Install-Package System.Reactive
Attempting to gather dependency information for package 'System.Reactive.3.1.1' with respect to project 'UnitTestProject1', targeting '.NETFramework,Version=v4.6.1'
Attempting to resolve dependencies for package 'System.Reactive.3.1.1' with DependencyBehavior 'Lowest'
Resolving actions to install package 'System.Reactive.3.1.1'
Resolved actions to install package 'System.Reactive.3.1.1'
Adding package 'System.Reactive.Interfaces.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Interfaces.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Interfaces.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Interfaces 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Core.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Core.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Core.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Core 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Linq.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Linq.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Linq.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Linq 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.PlatformServices.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.PlatformServices.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.PlatformServices.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.PlatformServices 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.Windows.Threading.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Windows.Threading.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.Windows.Threading.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive.Windows.Threading 3.1.1' to UnitTestProject1
Adding package 'System.Reactive.3.1.1', which only has dependencies, to project 'UnitTestProject1'.
Adding package 'System.Reactive.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.3.1.1' to folder 'C:\design\github_spike\spike\Solution1\packages'
Added package 'System.Reactive.3.1.1' to 'packages.config'
Successfully installed 'System.Reactive 3.1.1' to UnitTestProject1
*/
