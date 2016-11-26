using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestProject1
{
  public class TimedReplay
  {
    private Action<DateTime> efferent_action;
    public TimedReplay(Action<DateTime> efferent_action)
    {
      this.efferent_action = efferent_action;
    }
    public IEnumerable<DateTime> GetReplayedFrom(DateTime start, IEnumerable<DateTime> captured)
    {
      var expected_intervals = new List<TimeSpan>();
      for (int k = 1; k < captured.Count(); ++k)
      {
        var dx = captured.ElementAt(k).Subtract(captured.ElementAt(k - 1));
        expected_intervals.Add(dx);
      }

      var result = new List<DateTime> { start };
      DateTime replayed_time = start;
      foreach (var dx in expected_intervals)
      {
        replayed_time = replayed_time.Add(dx);
        result.Add(replayed_time);
      }

      return result;
    }
  }

  [TestClass]
  public class timedreplay_Spec
  {
    [TestMethod]
    public void basic1()
    {
      //Arrange
      var captured = new List<DateTime> { DateTime.Parse("2016-11-26T07:53:48"), DateTime.Parse("2016-11-26T07:53:49") };
      var processor = new TimedReplay(msg=> { });
      var start = DateTime.Parse("2016-11-27T01:00:00");

      //Act
      var replayed = processor.GetReplayedFrom(start, captured);

      //Assert
      Assert.AreEqual<int>(2, replayed.Count());


      var expected_intervals = new List<TimeSpan>();
      for (int k = 1; k < captured.Count(); ++k)
      {
        var dx = captured.ElementAt(k).Subtract(captured.ElementAt(k - 1));
        expected_intervals.Add(dx);
      }

      var replayed_intervals = new List<TimeSpan>();
      for (int k = 1; k < replayed.Count(); ++k)
      {
        var dx = replayed.ElementAt(k).Subtract(replayed.ElementAt(k - 1));
        replayed_intervals.Add(dx);
      }
      CollectionAssert.AreEqual(expected_intervals, replayed_intervals);


      var expected = new List<DateTime> { start };
      DateTime replayed_time = start;
      foreach (var dx in expected_intervals)
      {
        replayed_time = replayed_time.Add(dx);
        expected.Add(replayed_time);
      }
      CollectionAssert.AreEqual(expected, replayed.ToList());
    }
  }
}