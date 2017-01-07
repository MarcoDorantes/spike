﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
  [TestClass]
  public class RX_Spec
  {
    /*
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
      //System.Reactive.dll
      //Observable.
      Assert.IsTrue(this.GetType().Assembly.GetReferencedAssemblies().Any(a => a.FullName.StartsWith("System.Reactive")));
    }
  }
}
/*
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
