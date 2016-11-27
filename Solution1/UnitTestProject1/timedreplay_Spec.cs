using System;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;

namespace UnitTestProject1
{
  public class TimedReplay
  {
    private Action<DateTime> egress_action;
    public TimedReplay(Action<DateTime> egress_action)
    {
      this.egress_action = egress_action;
    }
    public IEnumerable<DateTime> GetReplayedFrom(DateTime start, IEnumerable<DateTime> captured)
    {
      var grouped = captured.OrderBy(t => t).GroupBy(t => t /*here goes the grouping of #msgs by elapsed time (half-second, second, etc.)*/);
      var egress_times = grouped.Select(g => g.Key);

      var intervals = new List<TimeSpan>();
      for (int k = 1; k < egress_times.Count(); ++k)
      {
        var dx = egress_times.ElementAt(k).Subtract(egress_times.ElementAt(k - 1));
        intervals.Add(dx);//change to yield return
      }

      var result = new List<DateTime> { start };
      DateTime replayed_time = start;
      foreach (var dx in intervals)
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
    IEnumerable<KeyValuePair<int, TimeSpan>> getpairs()
    {
      for (int k = 0; k < 24; ++k)
      {
        yield return new KeyValuePair<int, TimeSpan>(k, DateTime.Now.TimeOfDay);
      }
    }
    [TestMethod]
    public void grouping()
    {
      var groups = getpairs().GroupBy(p => p.Key % 5);
      Assert.AreEqual<int>(5, groups.Count());
      Assert.AreEqual<string>("0|1|2|3|4|", groups.ToList().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n.Key)).ToString());
      Assert.AreEqual<string>("5|5|5|5|4|", groups.ToList().Aggregate(new StringBuilder(), (w, n) => w.AppendFormat("{0}|", n.Count())).ToString());
    }
    [TestMethod]
    public void basic0_a()
    {
      TimeSpan fire = TimeSpan.Zero;
      var timer = new System.Timers.Timer(3000D);
      timer.Elapsed += (s, e) => fire = DateTime.Now.TimeOfDay;
      timer.Start();
      var now = DateTime.Now.TimeOfDay;
      Thread.Sleep(5000);
      timer.Stop();
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:03"), fire-now);
    }
    [TestMethod]
    public void basic0_b()
    {
      TimeSpan fire = TimeSpan.Zero;
      var timer = new System.Threading.Timer(x=> fire = DateTime.Now.TimeOfDay, null, TimeSpan.Parse("00:00:03"), TimeSpan.FromMilliseconds(-1D));
      var now = DateTime.Now.TimeOfDay;
      Thread.Sleep(5000);
      timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
      timer.Dispose();
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:03"), fire - now);
    }
    [TestMethod]
    public void basic1()
    {
      //Arrange
      var captured = new List<DateTime> { DateTime.Parse("2016-11-26T07:53:48"), DateTime.Parse("2016-11-26T07:53:49") };
      var processor = new TimedReplay(msg => { });
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

    [TestMethod]
    public void time_precision()
    {
      var fires = new List<DateTime>();
      var played_intervals = new List<TimeSpan>();

      var expected_fire_times = new List<TimeSpan> { TimeSpan.Parse("00:00:03"), TimeSpan.Parse("00:00:05") };

      var now = DateTime.Now.TimeOfDay;
      var fire_times = new List<TimeSpan>();
      TimeSpan fire_time = now;
      foreach (var dx in expected_fire_times)
      {
        fire_time = fire_time.Add(dx);
        fire_times.Add(fire_time);
      }

      using (var timer = new utility.RetryTimeOfDayTimer(fire_times, () => { var fire = DateTime.Now; fires.Add(fire); Trace.WriteLine($"->{fire}"); return true; }, id: "time1"))
      {
        Thread.Sleep(10000);

        for (int k = 1; k < fires.Count; ++k)
        {
          var dx = fires[k].Subtract(fires[k - 1]);
          played_intervals.Add(dx);
        }
      }

      Assert.AreEqual<int>(2, fires.Count);
      Assert.AreEqual<int>(1, played_intervals.Count);
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:05"), played_intervals[0]);
    }

    [TestMethod]
    public void time_precision2()
    {
      var fires = new List<DateTime>();
      var played_intervals = new List<TimeSpan>();

      var expected_fire_intervals = new List<TimeSpan> { TimeSpan.Parse("00:00:03"), TimeSpan.Parse("00:00:05") };

      var now = DateTime.Now.TimeOfDay;
      var fire_times = new List<TimeSpan>();
      TimeSpan fire_time = now;
      foreach (var dx in expected_fire_intervals)
      {
        fire_time = fire_time.Add(dx);
        fire_times.Add(fire_time);
        Trace.WriteLine($"expected_fire_interval: {fire_time}");
      }

      using (var timer = new utility.SequenceTimeOfDayTimer(fire_times, () => { var fire = DateTime.Now; fires.Add(fire); Trace.WriteLine($"->{fire}"); }, id: "time1"))
      {
        Thread.Sleep(10000);

        for (int k = 1; k < fires.Count; ++k)
        {
          var dx = fires[k].Subtract(fires[k - 1]);
          played_intervals.Add(dx);
        }
      }

      Assert.AreEqual<int>(2, fires.Count);
      Assert.AreEqual<int>(1, played_intervals.Count);
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:05"), played_intervals[0]);
    }

    [TestMethod]
    public void time_precision3()
    {
      var fires = new List<DateTime>();
      var played_intervals = new List<TimeSpan>();

      var expected_fire_intervals = new List<TimeSpan> { TimeSpan.Parse("00:00:03"), TimeSpan.Parse("00:00:05") };

      var now = DateTime.Now.TimeOfDay;
      var fire_times = new List<TimeSpan>();
      TimeSpan fire_time = now;
      foreach (var dx in expected_fire_intervals)
      {
        fire_time = fire_time.Add(dx);
        fire_times.Add(fire_time);
        Trace.WriteLine($"expected_fire_interval: {fire_time}");
      }

      using (var timer = new utility.SequenceTimeOfDayTimer2(fire_times, () => { var fire = DateTime.Now; fires.Add(fire); Trace.WriteLine($"->{fire}"); }, id: "time1"))
      {
        Thread.Sleep(10000);

        for (int k = 1; k < fires.Count; ++k)
        {
          var dx = fires[k].Subtract(fires[k - 1]);
          played_intervals.Add(dx);
        }
      }

      Assert.AreEqual<int>(2, fires.Count);
      Assert.AreEqual<int>(1, played_intervals.Count);
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:05"), played_intervals[0]);
    }

    class SequenceMember : utility.ISequenceMember
    {
      public int SequenceNumber { get; set; }
      public TimeSpan When { get; set; }
      public string OutboundMessage { get; set; }
    }

    IEnumerable<SequenceMember> getmembers(IEnumerable<TimeSpan> times)
    {
      int count = 0;
      foreach (var when in times)
      {
        yield return new SequenceMember { SequenceNumber = count++, When = when, OutboundMessage = null };
      }
    }
    [TestMethod]
    public void time_precision4()
    {
      var fires = new List<DateTime>();
      var played_intervals = new List<TimeSpan>();

      var expected_fire_intervals = new List<TimeSpan> { TimeSpan.Parse("00:00:03"), TimeSpan.Parse("00:00:05") };

      var now = DateTime.Now.TimeOfDay;
      var fire_times = new List<TimeSpan>();
      TimeSpan fire_time = now;
      foreach (var dx in expected_fire_intervals)
      {
        fire_time = fire_time.Add(dx);
        fire_times.Add(fire_time);
        Trace.WriteLine($"expected_fire_interval: {fire_time}");
      }

      using (var timer = new utility.SequencerTimeOfDayTimer<SequenceMember>(getmembers(fire_times), () => { var fire = DateTime.Now; fires.Add(fire); Trace.WriteLine($"->{fire}"); }, id: "time1"))
      {
        Thread.Sleep(10000);

        for (int k = 1; k < fires.Count; ++k)
        {
          var dx = fires[k].Subtract(fires[k - 1]);
          played_intervals.Add(dx);
        }
      }

      Assert.AreEqual<int>(2, fires.Count);
      Assert.AreEqual<int>(1, played_intervals.Count);
      Assert.AreEqual<TimeSpan>(TimeSpan.Parse("00:00:05"), played_intervals[0]);
    }
  }
}

#region Nuget nutility
namespace utility
{
  public class SequenceTimeOfDayTimer : IDisposable
  {
    private string ID;
    private System.Threading.Timer MainTimer;
    private Action Operation;
    private IEnumerator<TimeSpan> InvokeDayTimes;

    public SequenceTimeOfDayTimer(IEnumerable<TimeSpan> when, Action operation, string id = null)
    {
      this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("MMMdd-HHmmss-fffffff")) : id;
      Reset(when, operation);
    }

    ~SequenceTimeOfDayTimer()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Reset(IEnumerable<TimeSpan> when, Action operation)
    {
      if (operation == null)
      {
        throw new InvalidOperationException(ID + " Timer operation cannot be null.");
      }
      if (when == null || when.Count() <= 0)
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
      }
      //if (when.Distinct().Count() != when.Count())
      //{
      //  throw new System.Configuration.ConfigurationErrorsException(ID + " Duplicated daytimes are not supported");
      //}

      this.Operation = operation;
      this.InvokeDayTimes = when.GetEnumerator();
      this.InvokeDayTimes.MoveNext();
      StartNextInvokeTimer();
    }

    private void StartNextInvokeTimer()
    {
      if (this.MainTimer != null)
      {
        DisposeTimer();
      }
      TimeSpan diff = this.InvokeDayTimes.Current.Duration() - DateTime.Now.TimeOfDay;
      var nextinvoke = diff;
      this.MainTimer = new System.Threading.Timer(this.TimerInvoke, null, nextinvoke, TimeSpan.FromMilliseconds(-1D));
      ResultLogger.LogSuccess(ID + " Next invoke:" + nextinvoke);
    }

    private void TimerInvoke(object unused_state)
    {
      try
      {
        PauseTimer();
        this.Operation();
      }
      finally
      {
        if (this.InvokeDayTimes.MoveNext())
        {
          StartNextInvokeTimer();
        }
        else
        {
          DisposeTimer();
        }
      }
    }

    private void PauseTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
      }
    }

    private void DisposeTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        this.MainTimer.Dispose();
        this.MainTimer = null;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        DisposeTimer();
        ResultLogger.LogSuccess(ID + " PrecisionTimeOfDayTimer disposed.");
      }
    }
  }

  public class SequenceTimeOfDayTimer2 : IDisposable
  {
    private string ID;
    private System.Timers.Timer MainTimer;
    private Action Operation;
    private IEnumerator<TimeSpan> InvokeDayTimes;

    public SequenceTimeOfDayTimer2(IEnumerable<TimeSpan> when, Action operation, string id = null)
    {
      this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("MMMdd-HHmmss-fffffff")) : id;
      Reset(when, operation);
    }

    ~SequenceTimeOfDayTimer2()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Reset(IEnumerable<TimeSpan> when, Action operation)
    {
      if (operation == null)
      {
        throw new InvalidOperationException(ID + " Timer operation cannot be null.");
      }
      if (when == null || when.Count() <= 0)
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
      }
      //if (when.Distinct().Count() != when.Count())
      //{
      //  throw new System.Configuration.ConfigurationErrorsException(ID + " Duplicated daytimes are not supported");
      //}

      this.Operation = operation;
      this.InvokeDayTimes = when.GetEnumerator();
      this.InvokeDayTimes.MoveNext();
      StartNextInvokeTimer();
    }

    private void StartNextInvokeTimer()
    {
      if (this.MainTimer != null)
      {
        DisposeTimer();
      }
      TimeSpan diff = this.InvokeDayTimes.Current.Duration() - DateTime.Now.TimeOfDay;
      var nextinvoke = diff;
      this.MainTimer = new System.Timers.Timer(nextinvoke.TotalMilliseconds);
      this.MainTimer.Elapsed += (s, e) => { this.TimerInvoke(null); };
      this.MainTimer.Start();
      ResultLogger.LogSuccess(ID + " Next invoke:" + nextinvoke);
    }

    private void TimerInvoke(object unused_state)
    {
      try
      {
        PauseTimer();
        this.Operation();
      }
      finally
      {
        if (this.InvokeDayTimes.MoveNext())
        {
          StartNextInvokeTimer();
        }
        else
        {
          DisposeTimer();
        }
      }
    }

    private void PauseTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Stop();
      }
    }

    private void DisposeTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Stop();
        this.MainTimer.Dispose();
        this.MainTimer = null;
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        DisposeTimer();
        ResultLogger.LogSuccess(ID + " PrecisionTimeOfDayTimer disposed.");
      }
    }
  }

  public interface ISequenceMember
  {
    int SequenceNumber { get; }
    TimeSpan When { get; }
    string OutboundMessage { get; }
  }
  public class SequencerTimeOfDayTimer<T> : IDisposable where T : ISequenceMember
  {
    private string ID;
    private Action Operation;
    private List<IEnumerator<T>> InvokeDayTimesGroups;
    private List<SequenceTimeOfDayTimer2> Timers;

    public SequencerTimeOfDayTimer(IEnumerable<T> when, Action operation, string id = null)
    {
      this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("MMMdd-HHmmss-fffffff")) : id;
      Reset(when, operation);
    }

    private void Reset(IEnumerable<T> when, Action operation)
    {
      if (operation == null)
      {
        throw new InvalidOperationException(ID + " Timer operation cannot be null.");
      }
      if (when == null || when.Count() <= 0)
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
      }

      this.Operation = operation;
      var groups = when.GroupBy(p => p.SequenceNumber % 10);
      Trace.WriteLine($"groups: {groups.Count()}");
      this.InvokeDayTimesGroups = groups.Aggregate(new List<IEnumerator<T>>(), (whole, next) => { whole.Add(next.GetEnumerator()); return whole; });
      StartNextSetOfInvokeTimers();
    }

    private void StartNextSetOfInvokeTimers()
    {
      //IGrouping<int, T> next_group = this.InvokeDayTimesGroups.Current;
      this.Timers = new List<SequenceTimeOfDayTimer2>();
      foreach (var k in InvokeDayTimesGroups)
      {
        if(k.MoveNext())
        {
          Trace.WriteLine($"SequenceTimeOfDayTimer2: {k.Current.When}");
          this.Timers.Add(new SequenceTimeOfDayTimer2(new List<TimeSpan> { k.Current.When }, this.Operation, id: $"{k.Current.SequenceNumber}"));
        }
      }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // dispose managed state (managed objects).
        }

        // free unmanaged resources (unmanaged objects) and override a finalizer below.
        this.Timers.ForEach(t => t.Dispose());
        // set large fields to null.

        disposedValue = true;
      }
    }

    //override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    ~SequencerTimeOfDayTimer()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(false);
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // uncomment the following line if the finalizer is overridden above.
      GC.SuppressFinalize(this);
    }
    #endregion
  }

  public class RetryTimeOfDayTimer : IDisposable
  {
    public class RetryTimerConfiguration
    {
      public List<TimeSpan> InvokeDayTimes { get; set; }
      public int RetryLimit { get; set; }
      public int RetryMilliseconds { get; set; }
    }

    public static RetryTimerConfiguration ReadTimerConfiguration(NameValueCollection appSettings, string DayTimerSpansAppSetting, string RetryLimitAppSetting, string RetryMillisecondsAppSetting)
    {
      var result = new RetryTimerConfiguration();

      //App setting: DayTimerSpans
      result.InvokeDayTimes = utility.RetryTimeOfDayTimer.ParseInvokeDayTimes(appSettings[DayTimerSpansAppSetting], DayTimerSpansAppSetting);

      //App setting: RetryLimit
      int setting;
      if (int.TryParse(appSettings[RetryLimitAppSetting], out setting))
      {
        result.RetryLimit = setting;
      }
      else
      {
        string error_message = string.Format("Config value {0} ({1}) is invalid", RetryLimitAppSetting, setting);
        throw new System.Configuration.ConfigurationErrorsException(error_message);
      }

      //App setting: RetryMillisecondsAppSetting
      if (int.TryParse(appSettings[RetryMillisecondsAppSetting], out setting))
      {
        result.RetryMilliseconds = setting;
      }
      else
      {
        string error_message = string.Format("Config value {0} ({1}) is invalid", RetryMillisecondsAppSetting, setting);
        throw new System.Configuration.ConfigurationErrorsException(error_message);
      }
      return result;
    }
    public static List<TimeSpan> ParseInvokeDayTimes(string settingtext, string appSettingKey = "")
    {
      var invoketimes = new List<TimeSpan>();
      bool timespansOk = !string.IsNullOrWhiteSpace(settingtext);
      if (timespansOk == false)
      {
        string error_message = string.Format("Config value for day timer spans {0} ({1}) is empty", appSettingKey, settingtext);
        throw new System.Configuration.ConfigurationErrorsException(error_message);
      }
      else
      {
        string[] timespans = settingtext.Split('|');
        int k = -1;
        do
        {
          ++k;
          if (!timespansOk || k >= timespans.Length) break;
          if (string.IsNullOrWhiteSpace(timespans[k])) continue;

          TimeSpan timespan;
          timespansOk = TimeSpan.TryParse(timespans[k], out timespan);
          if (timespansOk)
          {
            invoketimes.Add(timespan);
          }
        } while (true);
      }
      if (timespansOk == false)
      {
        string error_message = string.Format("Config value for day timer spans {0} ({1}) is invalid", appSettingKey, settingtext);
        throw new System.Configuration.ConfigurationErrorsException(error_message);
      }
      return invoketimes;
    }

    private string ID;
    private List<TimeSpan> InvokeDayTimes;
    private Func<bool> Operation;
    private System.Threading.Timer MainTimer;
    private int MaxRetries;
    private int RetryMilliseconds;
    private bool WeekendDisable;
    private int retry_count;
    private TimeSpan nextinvoke;

    public RetryTimeOfDayTimer(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit = 3, int retry_milliseconds = 60000, bool weekend_disable = false, string id = null)
    {
      this.ID = string.IsNullOrWhiteSpace(id) ? string.Format("ID_{0}", DateTime.Now.ToString("MMMdd-HHmmss-fff")) : id;
      Reset(when, operation, retry_limit, retry_milliseconds, weekend_disable);
    }

    ~RetryTimeOfDayTimer()
    {
      Dispose(false);
    }

    public TimeSpan NextInvokeTime { get { return nextinvoke; } }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private void Reset(IEnumerable<TimeSpan> when, Func<bool> operation, int retry_limit, int retry_milliseconds, bool weekend_disable)
    {
      this.MaxRetries = retry_limit;
      this.RetryMilliseconds = retry_milliseconds;
      this.WeekendDisable = weekend_disable;

      if (operation == null)
      {
        throw new InvalidOperationException(ID + " Timer operation cannot be null.");
      }
      if (when == null || when.Count() <= 0)
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Daytimes are not configured");
      }
      if (when.Distinct().Count() != when.Count())
      {
        throw new System.Configuration.ConfigurationErrorsException(ID + " Duplicated daytimes are not supported");
      }

      this.Operation = operation;
      var positivewhen = when.Aggregate(new List<TimeSpan>(), (whole, next) => { whole.Add(next.Duration()); return whole; });
      this.InvokeDayTimes = new List<TimeSpan>();
      foreach (TimeSpan daytime in positivewhen.OrderBy(time => time))
      {
        this.InvokeDayTimes.Add(daytime);
      }
      ResultLogger.LogSuccess(string.Format(ID + " InvokeDayTime: {0}", this.InvokeDayTimes.Aggregate(new StringBuilder(), (whole, next) => { whole.AppendFormat("{0},", next); return whole; })));
      StartNextInvokeTimer();
    }

    private void StartNextInvokeTimer(int retry = 0)
    {
      if (this.MainTimer != null)
      {
        DisposeTimer();
      }
      nextinvoke = retry == 0 ? GetNextInvokeTimeSpan() : TimeSpan.FromMilliseconds(retry);
      this.MainTimer = new System.Threading.Timer(this.TimerInvoke, null, nextinvoke, TimeSpan.FromMilliseconds(-1D));
      ResultLogger.LogSuccess(ID + " Next invoke:" + nextinvoke);
    }

    private void TimerInvoke(object unused_state)
    {
      bool result = false;
      try
      {
        PauseTimer();
        DayOfWeek dayname = DateTime.Now.DayOfWeek;
        if (WeekendDisable && (dayname == DayOfWeek.Saturday || dayname == DayOfWeek.Sunday))
        {
          ResultLogger.LogSuccess(string.Format("{0} Today is {1} and the execution has been disable for today.", ID, dayname));
          result = true;
        }
        else
        {
          result = this.Operation();
        }
      }
      finally
      {
        if (result)
        {
          StartNextInvokeTimer();
        }
        else
        {
          if (this.retry_count < this.MaxRetries)
          {
            StartNextInvokeTimer(this.RetryMilliseconds);
            ++this.retry_count;
          }
          else
          {
            this.retry_count = 0;
            ResultLogger.LogSuccess(ID + " Retry limit");
            StartNextInvokeTimer();
          }
        }
      }
    }

    private void PauseTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
      }
    }

    private void DisposeTimer()
    {
      if (this.MainTimer != null)
      {
        this.MainTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        this.MainTimer.Dispose();
        this.MainTimer = null;
        ResultLogger.LogSuccess(ID + " RetryTimeOfDayTimer.DisposeMainTimer");
      }
    }

    public TimeSpan GetNextInvokeTimeSpan()
    {
      var result = TimeSpan.Zero;
      var now = DateTime.Now.TimeOfDay;
      var nowAndFutures = this.InvokeDayTimes.Where(time => time.CompareTo(now) >= 0);
      //ResultLogger.LogSuccess(ID + " Now.TimeOfDay:" + now);
      if (nowAndFutures.Count() > 0)
      {
        TimeSpan diff = nowAndFutures.First() - now;
        if (diff == TimeSpan.Zero)
        {
          diff = TimeSpan.Parse("00:00:03");
        }
        result = diff;
      }
      else
      {
        var nextday = DateTime.Today.AddDays(1);
        TimeSpan nextspan = this.InvokeDayTimes[0];
        var nexttime = new DateTime(nextday.Year, nextday.Month, nextday.Day, nextspan.Hours, nextspan.Minutes, nextspan.Seconds);
        result = nexttime.Subtract(DateTime.Now);
      }
      return result;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        DisposeTimer();
        ResultLogger.LogSuccess(ID + " RetryTimeOfDayTimer disposed.");
      }
    }
  }

  static class ResultLogger
  {
    private const int InformationEventID = 1000;

    public static void LogSuccess(string message)
    {
      AddToLog(message);
    }

    /*public static void LogServiceGenericFault(System.ServiceModel.FaultException exception)
    {
      AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
    }

    public static void LogCommunicationIssue(System.ServiceModel.CommunicationException exception)
    {
      AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
    }*/

    public static void LogGenericException(Exception exception)
    {
      if (exception.Data == null)
      {
        Environment.FailFast("Process is corrupt: exception.Data is null.", exception);
      }
      else
      {
        AddToLog(System.Reflection.MethodBase.GetCurrentMethod().Name, exception);
      }
    }

    private static void AddToLog(string message)
    {
      string logtext = string.Format("{0:s} [{1}] {2}", DateTime.Now, System.Threading.Thread.CurrentThread.ManagedThreadId, message);
      //GBM.Instrumentation.LogManager.LogInformation(InformationEventID, logtext);
      System.Diagnostics.Trace.WriteLine(logtext);
    }

    private static void AddToLog(string method, Exception exception)
    {
      int eventId = 0; //GBM.Instrumentation.LogManager.LogExceptionWithRandomEventId(exception);
      System.Diagnostics.Trace.WriteLine(string.Format("Log method: {0} EventID: {1}. Message: {2}", method, eventId, exception.Message));
    }
  }
}
#endregion