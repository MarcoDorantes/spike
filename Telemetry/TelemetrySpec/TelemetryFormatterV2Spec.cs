using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelemetrySpec
{
  public class TelemetryFormatterV2
  {
    public string NotifySharedState(Identity id, string shared_state = null)
    {
      //get headers=f(data_to_send)
      //formatter_key=f(headers)
      //formatter=formatters(formatter_key)
      //return formatter.Serialize(data_to_send)
      return "";
    }
  }

  [TestClass]
  public class TelemetryFormatterV2Spec
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

      var formatter = new TelemetryFormatterV2();

      //Act
      string payload = formatter.NotifySharedState(id, shared_state1);

      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }
  }
}