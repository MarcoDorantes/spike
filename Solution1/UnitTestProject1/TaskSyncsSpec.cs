using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace UnitTestProject1
{
  [TestClass]
  public class TaskSyncsSpec
  {
    [TestMethod, TestCategory("Task")]
    public void DetachedParentChild()
    {
      //Arrange
      var sequence = new BlockingCollection<int>();
      var child1 = new Action(() => { sequence.Add(2); Thread.Sleep(5000); sequence.Add(4); });
      var parent = new Action(() => { sequence.Add(1); Task.Factory.StartNew(child1); Thread.Sleep(2500); sequence.Add(3); });

      //Act
      var parent_task = Task.Factory.StartNew(parent);
      parent_task.Wait();

      //Assert
      Assert.IsTrue(Enumerable.SequenceEqual(new int[] { 1, 2, 3 }, sequence), sequence.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}|", next)).ToString());
      CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, sequence.ToList());
    }
    [TestMethod, TestCategory("Task")]
    public void AttachedParentChild()
    {
      //Arrange
      var sequence = new BlockingCollection<int>();
      var child1 = new Action(() => { sequence.Add(2); Thread.Sleep(5000); sequence.Add(4); });
      var parent = new Action(() => { sequence.Add(1); Task.Factory.StartNew(child1, TaskCreationOptions.AttachedToParent); Thread.Sleep(2500); sequence.Add(3); });

      //Act
      var parent_task = Task.Factory.StartNew(parent);
      parent_task.Wait();

      //Assert
      Assert.IsTrue(Enumerable.SequenceEqual(new int[] { 1, 2, 3, 4 }, sequence), sequence.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}|", next)).ToString());
      CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4 }, sequence.ToList());
    }
  }
}