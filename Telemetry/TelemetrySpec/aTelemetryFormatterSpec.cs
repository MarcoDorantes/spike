using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelemetrySpec
{
  public static class aTelemetryFormatter
  {
    public static string Serialize(string data_to_send)
    {
      //get headers=f(data_to_send)
      //formatter_key=f(headers)
      //formatter=formatters(formatter_key)
      //return formatter.Serialize(data_to_send)
      return data_to_send;
    }
  }

  [TestClass]
  public class aTelemetryFormatterSpec
  {
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
    private string LegacySerializeStateNotification(Identity id, string shared_state = null)
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

    public string LegacyNotifyThroughputSerialization(Identity id, uint diachronicCount, decimal diachronicElapsed_s, decimal synchronicThroughputMin, decimal synchronicThroughput, decimal synchronicThroughputMax, decimal diachronicThroughput, uint success_count, uint error_count, uint received, uint queued_count, uint queued_maxcount)
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

    public string LegacyNotifyReceivedCountsSerialization(Identity id, uint receivedCount, uint queuedCount, uint queued_maxcount)
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

    public string ToStringSentToMonitorUI(string Timepoint, string WriterName, string MessageCount, string Description, string InboundMessage, string SQL_Parameters, string QuasiTSL)
    {
      return string.Format("{0}\x1{1}\x1{2}\x1{3}\x1{4}\x1{5}\x1{6}", Timepoint, WriterName, MessageCount, Description, InboundMessage, SQL_Parameters, QuasiTSL);
    }

    public string LegacyNotifyBurnExceptionSerialization(Identity id, PersistResult exception_category, string log_to_send)
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
       log_to_send,
       exception_category
      );
    }

    public string LegacyNotifyExceptionSerialization(Identity id, PersistResult exception_category, string log)
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

    [TestMethod]
    public void NotifySharedState()
    {
      //Arrange
      string shared_state1 = "shared_state1";
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacySerializeStateNotification(id, shared_state1);

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }
    [TestMethod]
    public void NotifyEventid_SerializeStateNotification()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacySerializeStateNotification(id);

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }

    [TestMethod]
    public void NotifyThroughput()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacyNotifyThroughputSerialization(
        id,
        diachronicCount: 1,
        diachronicElapsed_s: 12.5M,
        synchronicThroughputMin: 1.1M,
        synchronicThroughput: 2.2M,
        synchronicThroughputMax: 3.3M,
        diachronicThroughput: 4.4M,
        success_count: 2,
        error_count: 3,
        received: 4,
        queued_count: 5,
        queued_maxcount: 6
      );

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }

    [TestMethod]
    public void NotifyReceivedCounts()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacyNotifyReceivedCountsSerialization(
        id,
        receivedCount:2,
        queuedCount:3,
        queued_maxcount:4
      );

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }

    [TestMethod]
    public void NotifyBurnException()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacyNotifyBurnExceptionSerialization(id, PersistResult.SystemException, "logline1");

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }

    [TestMethod]
    public void NotifyBurnExceptionWithQuasiTSQL()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string sqlcall = ToStringSentToMonitorUI("Timepoint", "WriterName", "MessageCount", "Description", "InboundMessage", "SQL_Parameters", "QuasiTSL");
      string data_to_send = LegacyNotifyBurnExceptionSerialization(id, PersistResult.SystemException, sqlcall);

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }

    [TestMethod]
    public void NotifyException()
    {
      //Arrange
      var id = new Identity { ID = Guid.NewGuid().ToString(), Host = Environment.MachineName, Service = nameof(aTelemetryFormatterSpec), Name = nameof(NotifySharedState), SourceName = nameof(NotifySharedState), TargetName = nameof(Assert), State = "Arranged" };
      string data_to_send = LegacyNotifyExceptionSerialization(id, PersistResult.SystemException, "logline1");

      //Act
      string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      Assert.AreEqual<string>(payload, data_to_send);
    }
  }
}