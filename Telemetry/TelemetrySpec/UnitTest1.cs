using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace TelemetrySpec
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod, TestCategory("BackwardCompatibility"), Ignore]
    public void expectation()
    {
      //Arrange

      string payload = "RawStringPayload-without-ContentType-for-Content";

      //Act

      //Assert
    }

    delegate void F(int index, out string name, out object value);
    class ExecutionStateSender
    {
      public void Send(int count, F f)
      {
        if (count < 0)
        {
          throw new ArgumentException("count cannot be negative.",nameof(count));
        }
        string name;
        object value;
        var values = new StringBuilder();
        for (int k = 0; k < count; ++k)
        {
          f(k, out name, out value);
          values.AppendFormat("{0}:{1}", name, value);
        }
        string payload = values.ToString();
      }

      public void Send(string message_type, params object[] values)
      { }
    }
    class ExecutionStateReceiver { }

    void f1(int index, out string name, out object value)
    {
      name = "name1";
      value = 12.5;
    }

    [TestMethod]
    public void TestMethod1()
    {
      //Arrange
      //string payload = "StringPayload-with-ContentType-for-Content";
      var content = new { name1 = "value1", name2 = 12.5 };
      var state_sender = new ExecutionStateSender();
      var state_receiver = new ExecutionStateReceiver();

      //var metric_provider = new Action(()=> { "name1" = content.name1 });

      string name1 = "name1";
      int count = 2;
      decimal metric1 = 12.5M; ;

      //Act
      state_sender.Send("P", nameof(name1), name1, nameof(count), count, nameof(metric1), metric1);

      //Assert
      //metric_receiver
    }
  }
}