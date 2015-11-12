using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TelemetrySpec
{
  public static class aTelemetryFormatter
  {
  }

  [TestClass]
  public class aTelemetryFormatterSpec
  {
    [TestMethod]
    public void NotifySharedState()
    {
      //Arrange
      string data_to_send;

      //Act
      //string payload = aTelemetryFormatter.Serialize(data_to_send);

      //Assert
      //expected payload
    }
    [TestMethod]
    public void NotifyEventid_SerializeStateNotification()
    {
      //Arrange
      //TelemetryFormatter

      //Act

      //Assert
    }

    [TestMethod]
    public void NotifyThroughput()
    {
      //Arrange
      //TelemetryFormatter

      //Act

      //Assert
    }

    [TestMethod]
    public void NotifyReceivedCounts()
    {
      //Arrange
      //TelemetryFormatter

      //Act

      //Assert
    }

    [TestMethod]
    public void NotifyBurnException()
    {
      //Arrange
      //TelemetryFormatter

      //Act

      //Assert
    }

    [TestMethod]
    public void NotifyException()
    {
      //Arrange
      //TelemetryFormatter

      //Act

      //Assert
    }
  }
}