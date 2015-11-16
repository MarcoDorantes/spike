using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Collections.Generic;

namespace TelemetrySpec
{
  public interface ITelemetryFormatter
  {
    string Serialize(object x);
  }
  class Formatter1 : ITelemetryFormatter
  {
    public string Serialize(object x)
    {
      var payload = new StringBuilder($"{message_type}");
      for (int k = 0; k < values.Length; ++k)
      {
        payload.AppendFormat("|{0}", values[k]);
      }
      return payload.ToString();
    }
  }
  public class TelemetryFormatterV2
  {
    private Dictionary<string, ITelemetryFormatter> formatters;
    public TelemetryFormatterV2()
    {
      formatters = new Dictionary<string, ITelemetryFormatter>();
      formatters.Add("S",new Formatter1());
    }
    public string NotifyState(object payload, params string[] headers)
    {
      //get headers=f(data_to_send)
      //formatter_key=f(headers)
      //formatter=formatters(formatter_key)
      //return formatter.Serialize(data_to_send)

      string formatter_key = get_key(headers);
      ITelemetryFormatter formatter = formatters[formatter_key];
      return formatter.Serialize(payload);
    }

    private string get_key(string[] headers)
    {
      return headers.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}", next)).ToString();
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
      string payload = formatter.NotifyState(id, NotificationType.Status);
      //
      //Assert
      Assert.AreEqual<string>(expected_payload, payload);
    }
  }
}