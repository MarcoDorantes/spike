using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
  [TestClass]
  public class codingExercises //https://www.interviewbit.com/all-problem-list
  {
    [TestMethod]
    public void rotate_matrix()//https://www.interviewbit.com/problems/rotate-matrix
    {
      var rotate_90 = new Action<int[,]>(x =>
      {
        var n = x[0, 0];
        x[0, 0] = x[1, 0];
        x[1, 0] = x[1, 1];
        x[1, 1] = x[0, 1];
        x[0, 1] = n;
      });

      var arr1 = new int[,]
      {
        { 1, 2 },
        { 3, 4 }
      };
      var arr2 = new int[,]
      {
        { 4, 3 },
        { 2, 1 }
      };

      var arr3 = new int[,]
      {
        { 1, 2, 3 },
        { 4, 5, 6 },
        { 7, 8, 9 }
      };

      rotate_90(arr1);
      rotate_90(arr2);
      rotate_90(arr3);

      Assert.AreEqual(3, arr1[0, 0]);
      Assert.AreEqual(1, arr1[0, 1]);
      Assert.AreEqual(4, arr1[1, 0]);
      Assert.AreEqual(2, arr1[1, 1]);

      Assert.AreEqual(2, arr2[0, 0]);
      Assert.AreEqual(4, arr2[0, 1]);
      Assert.AreEqual(1, arr2[1, 0]);
      Assert.AreEqual(3, arr2[1, 1]);

      Assert.AreEqual(2, arr3.Rank);
      Assert.AreEqual(0, arr3.GetLowerBound(0));
      Assert.AreEqual(0, arr3.GetLowerBound(1));
      Assert.AreEqual(2, arr3.GetUpperBound(0));
      Assert.AreEqual(2, arr3.GetUpperBound(1));
    }
  }
}