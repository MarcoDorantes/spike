using System;
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