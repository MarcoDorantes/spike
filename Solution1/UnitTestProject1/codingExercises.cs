using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
  [TestClass]
  public class codingExercises
  {
    [TestMethod]
    public void rotate_matrix()
    {
      var rotate_90 = new Action<int[,]>(x =>
      {
        var n = x[0, 0];
        x[0, 0] = x[1, 0];
        x[1, 0] = x[1, 1];
        x[1, 1] = x[0, 1];
        x[0, 1] = n;
      });
      var arr = new int[,] { { 1, 2 }, { 3, 4 } };
      Assert.AreEqual(1, arr[0, 0]);
      Assert.AreEqual(2, arr[0, 1]);
      Assert.AreEqual(3, arr[1, 0]);
      Assert.AreEqual(4, arr[1, 1]);
      rotate_90(arr);
      Assert.AreEqual(3, arr[0, 0]);
      Assert.AreEqual(1, arr[0, 1]);
      Assert.AreEqual(4, arr[1, 0]);
      Assert.AreEqual(2, arr[1, 1]);
    }
  }
}