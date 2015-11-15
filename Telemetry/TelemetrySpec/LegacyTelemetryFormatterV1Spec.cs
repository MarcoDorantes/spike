using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelemetrySpec
{
  /*public class LegacyTelemetryFormatter
  {
    public string Serialize(string data_to_send)
    {
      //get headers=f(data_to_send)
      //formatter_key=f(headers)
      //formatter=formatters(formatter_key)
      //return formatter.Serialize(data_to_send)
      return data_to_send;
    }
  }*/

  public static class LegacyTelemetryFormatterV1
  {
    public static string NotifySharedState(Identity id, string shared_state = null)
    {
      return LegacySerializeStateNotification(id, shared_state);
    }

    public static string NotifyState(Identity id)
    {
      return LegacySerializeStateNotification(id);
    }

    public static string NotifyThroughput(Identity id, uint diachronicCount, decimal diachronicElapsed_s, decimal synchronicThroughputMin, decimal synchronicThroughput, decimal synchronicThroughputMax, decimal diachronicThroughput, uint success_count, uint error_count, uint received, uint queued_count, uint queued_maxcount)
    {
      return LegacyNotifyThroughputSerialization(id, diachronicCount, diachronicElapsed_s, synchronicThroughputMin, synchronicThroughput, synchronicThroughputMax, diachronicThroughput, success_count, error_count, received, queued_count, queued_maxcount);
    }

    public static string NotifyReceivedCounts(Identity id, uint receivedCount, uint queuedCount, uint queued_maxcount)
    {
      return LegacyNotifyReceivedCountsSerialization(id, receivedCount, queuedCount, queued_maxcount);
    }

    public static string NotifyBurnException(Identity id, PersistResult exception_category, string log)
    {
      return LegacyNotifyExceptionSerialization(id, exception_category, log);
    }

    public static string NotifyBurnException(Identity id, PersistResult exception_category, string Timepoint, string WriterName, string MessageCount, string Description, string InboundMessage, string SQL_Parameters, string QuasiTSL)
    {
      return LegacyNotifyBurnExceptionSerialization(id, exception_category, Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);
    }

    public static string NotifyException(Identity id, PersistResult exception_category, string log)
    {
      return LegacyTelemetryFormatterV1.LegacyNotifyExceptionSerialization(id, exception_category, log);

    }
    //
    private static string LegacySerializeStateNotification(Identity id, string shared_state = null)
    {
      return string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}",
          NotificationType.Status,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          shared_state == null ? id.State : shared_state
      );
    }

    private static string LegacyNotifyThroughputSerialization(Identity id, uint diachronicCount, decimal diachronicElapsed_s, decimal synchronicThroughputMin, decimal synchronicThroughput, decimal synchronicThroughputMax, decimal diachronicThroughput, uint success_count, uint error_count, uint received, uint queued_count, uint queued_maxcount)
    {
      return string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}\x0{10}\x0{11}\x0{12}\x0{13}\x0{14}\x0{15}\x0{16}\x0{17}\x0{18}",
          NotificationType.MessagePersistance,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          id.State,
          diachronicCount,
          diachronicElapsed_s,
          synchronicThroughputMin,
          synchronicThroughput,
          synchronicThroughputMax,
          diachronicThroughput,
          success_count,
          error_count,
          received,
          queued_count,
          queued_maxcount
      );
    }

    private static string LegacyNotifyReceivedCountsSerialization(Identity id, uint receivedCount, uint queuedCount, uint queued_maxcount)
    {
      return string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}\x0{10}",
          NotificationType.MessageReception,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          id.State,
          receivedCount,
          queuedCount,
          queued_maxcount
      );
    }

    private static string ToStringSentToMonitorUI(string Timepoint, string WriterName, string MessageCount, string Description, string InboundMessage, string SQL_Parameters, string QuasiTSL)
    {
      return string.Format("{0}\x1{1}\x1{2}\x1{3}\x1{4}\x1{5}\x1{6}", Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);
    }

    private static string LegacyNotifyBurnExceptionSerialization(Identity id, PersistResult exception_category, string Timepoint, string WriterName, string MessageCount, string Description, string InboundMessage, string SQL_Parameters, string QuasiTSL)
    {
      string log_to_send = ToStringSentToMonitorUI(Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);
      return LegacyNotifyExceptionSerialization(id, exception_category, log_to_send);
    }

    private static string LegacyNotifyExceptionSerialization(Identity id, PersistResult exception_category, string log)
    {
      return string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}",
        NotificationType.MessagePersistanceException,
        id.ID,
        id.Host,
        id.Service,
        id.Name,
        id.SourceName,
        id.TargetName,
        id.State,
        log,
        exception_category
     );
    }
  }

  public enum Notification
  {
    MessageWriterStatus,
    UnrecoverSqlConnection,
    Alert,
    WriterReplaced
  }
  public static class NotificationType
  {
    public const string MessagePersistance = "P";
    public const string MessagePersistanceException = "X";
    public const string MessageReception = "R";
    public const string Status = "S";
  }
  public enum PersistResult
  {
    OK,
    SystemException,
    ApplicationException,
    BusinessException
  }

  public class Identity
  {
    public string ID { get; set; }
    public string Host { get; set; }
    public string Service { get; set; }
    public string Name { get; set; }
    public string SourceName { get; set; }
    public string TargetName { get; set; }
    public string State { get; set; }
  }

  [TestClass]
  public class LegacyTelemetryFormatterV1Spec
  {
    [TestMethod]
    public void NotifySharedState()
    {
      //Arrange
      string shared_state1 = "shared_state1";
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}",
          NotificationType.Status,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          shared_state1 == null ? id.State : shared_state1
        );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifySharedState(id, shared_state1);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyEventid_SerializeStateNotification()
    {
      //Arrange
      string shared_state1 = null;
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}",
          NotificationType.Status,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          shared_state1 == null ? id.State : shared_state1
        );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyState(id);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyThroughput()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      uint
        diachronicCount = 1,
        success_count = 2,
        error_count = 3,
        received = 4,
        queued_count = 5,
        queued_maxcount = 6;
      decimal
        diachronicElapsed_s = 12.5M,
        synchronicThroughputMin = 1.1M,
        synchronicThroughput = 2.2M,
        synchronicThroughputMax = 3.3M,
        diachronicThroughput = 4.4M;

      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}\x0{10}\x0{11}\x0{12}\x0{13}\x0{14}\x0{15}\x0{16}\x0{17}\x0{18}",
          NotificationType.MessagePersistance,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          id.State,
          diachronicCount,
          diachronicElapsed_s,
          synchronicThroughputMin,
          synchronicThroughput,
          synchronicThroughputMax,
          diachronicThroughput,
          success_count,
          error_count,
          received,
          queued_count,
          queued_maxcount
      );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyThroughput(
        id,
        diachronicCount,
        diachronicElapsed_s,
        synchronicThroughputMin,
        synchronicThroughput,
        synchronicThroughputMax,
        diachronicThroughput,
        success_count,
        error_count,
        received,
        queued_count,
        queued_maxcount
      );

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyReceivedCounts()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      uint receivedCount = 2, queuedCount = 3, queued_maxcount = 4;
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}\x0{10}",
          NotificationType.MessageReception,
          id.ID,
          id.Host,
          id.Service,
          id.Name,
          id.SourceName,
          id.TargetName,
          id.State,
          receivedCount,
          queuedCount,
          queued_maxcount
      );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyReceivedCounts(id, receivedCount, queuedCount, queued_maxcount);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyBurnException()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      PersistResult exception_category = PersistResult.SystemException;
      string log_to_send = "logline1";
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}",
         NotificationType.MessagePersistanceException,
         id.ID,
         id.Host,
         id.Service,
         id.Name,
         id.SourceName,
         id.TargetName,
         id.State,
         log_to_send,
         exception_category
        );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyBurnException(id, exception_category, log_to_send);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyBurnExceptionWithQuasiTSQL()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string Timepoint = "Timepoint", WriterName = "WriterName", MessageCount = "MessageCount", Description = "Description", InboundMessage = "InboundMessage", SQL_Parameters = "SQL_Parameters", QuasiTSL = "QuasiTSL";
      PersistResult exception_category = PersistResult.SystemException;
      string log_to_send = string.Format("{0}\x1{1}\x1{2}\x1{3}\x1{4}\x1{5}\x1{6}", Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}",
         NotificationType.MessagePersistanceException,
         id.ID,
         id.Host,
         id.Service,
         id.Name,
         id.SourceName,
         id.TargetName,
         id.State,
         log_to_send,
         exception_category
        );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyBurnException(id, exception_category, Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }

    [TestMethod]
    public void NotifyException()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(LegacyTelemetryFormatterV1Spec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      PersistResult exception_category = PersistResult.SystemException;
      string log = "logline1";
      string expected_payload =
        string.Format("{0}\x0{1}\x0{2}\x0{3}\x0{4}\x0{5}\x0{6}\x0{7}\x0{8}\x0{9}",
         NotificationType.MessagePersistanceException,
         id.ID,
         id.Host,
         id.Service,
         id.Name,
         id.SourceName,
         id.TargetName,
         id.State,
         log,
         exception_category
       );

      //Act
      string payload = LegacyTelemetryFormatterV1.NotifyException(id, exception_category, log);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }
  }
}