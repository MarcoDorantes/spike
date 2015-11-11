using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace TelemetrySpec
{
  [TestClass]
  public class BasicCases
  {
    interface IPayloadFormatter
    {
      string Serialize(string message_type, object[] values);
      object Deserialize(string message_type, string payload);
    }
    interface ITelemetryService
    {
      IExecutionStateTransport GetExecutionStateTransport();
      IDictionary<string, IPayloadFormatter> GetFormatters();
    }
    class ExecutionStateSender
    {
      private IExecutionStateTransport transport;
      private ITransportPublisher publisher;
      private IDictionary<string, IPayloadFormatter> formatters;

      public ExecutionStateSender(ITelemetryService service)
      {
        transport = service.GetExecutionStateTransport();
        formatters = service.GetFormatters();
      }

      public void Start()
      {
        publisher = transport.GetPublisher();
      }

      public void Stop() { }

      public void Send(string message_type, params object[] values)
      {
        IPayloadFormatter formatter = formatters[message_type];
        string payload = formatter.Serialize(message_type, values);
        publisher.Send(payload);
      }
      public void Send(string message_type, System.Runtime.Serialization.ISerializable value)
      {
        IPayloadFormatter formatter = formatters[message_type];
        string payload = formatter.Serialize(message_type,new object[] { value });
        publisher.Send(payload);
      }
    }
    class ExecutionStateReceiver : IExecutionStateReceiver, IObserver<string>
    {
      private IExecutionStateTransport transport;
      private ITransportReceiver receiver;
      private IDictionary<string, IPayloadFormatter> formatters;
      private System.Collections.Concurrent.ConcurrentDictionary<IObserver<string>, bool> observers;
      public ExecutionStateReceiver(ITelemetryService service)
      {
        transport = service.GetExecutionStateTransport();
        formatters = service.GetFormatters();
        observers = new System.Collections.Concurrent.ConcurrentDictionary<IObserver<string>, bool>();
      }
      public void Start()
      {
        receiver = transport.GetReceiver();
        receiver.Subscribe(this);
      }

      public void Stop() { }

      #region IObservable<string> by monitoring application
      public IDisposable Subscribe(IObserver<string> observer)
      {
        if(!observers.TryAdd(observer, true))
        {
          throw new Exception("Cannot add observer");
        }
        return new Subscription(this, observer);
      }
      private void RemoveObserver(IObserver<string> observer)
      {
        bool unused;
        if (!observers.TryRemove(observer, out unused))
        {
          throw new Exception("Cannot remove observer");
        }
      }
      internal class Subscription: IDisposable
      {
        private ExecutionStateReceiver observable;
        private IObserver<string> observer;
        public Subscription(ExecutionStateReceiver observable, IObserver<string> observer)
        {
          this.observable = observable;
          this.observer = observer;
        }
        public void Dispose()
        {
          observable.RemoveObserver(observer);
        }
      }
      #endregion

      #region IObserver<string> of transport-level messages
      public void OnNext(string value)
      {
        foreach (var observer in observers.Keys)
        {
          observer.OnNext(value);
        }
      }

      public void OnError(Exception error)
      {
        throw new NotImplementedException();
      }

      public void OnCompleted()
      {
        throw new NotImplementedException();
      }
      #endregion
    }

    interface ITransportPublisher
    {
      void Send(string payload);
    }
    interface ITransportReceiver : IObservable<string> { }
    interface IExecutionStateTransport
    {
      ITransportPublisher GetPublisher();
      ITransportReceiver GetReceiver();
    }
    interface IExecutionStatePublisher
    {
      void Send(string payload);
    }
    interface IExecutionStateReceiver : IObservable<string>
    {
    }

    class PairsFormatter : IPayloadFormatter
    {
      public object Deserialize(string message_type, string payload)
      {
        throw new NotImplementedException();
      }

      public string Serialize(string message_type, object[] values)
      {
        var payload = new StringBuilder($"{message_type}");
        for (int k = 1; k < values.Length; k += 2)
        {
          payload.AppendFormat("|{0}|{1}", values[k - 1], values[k]);
        }
        return payload.ToString();
      }
    }
    class ArrayFormatter : IPayloadFormatter
    {
      public object Deserialize(string message_type, string payload)
      {
        throw new NotImplementedException();
      }

      public string Serialize(string message_type, object[] values)
      {
        var payload = new StringBuilder($"{message_type}");
        for (int k = 0; k < values.Length; ++k)
        {
          payload.AppendFormat("|{0}", values[k]);
        }
        return payload.ToString();
      }
    }
    class ObjectBinaryFormatter : IPayloadFormatter
    {
      private System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer;

      public ObjectBinaryFormatter()
      {
        serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      }

      public object Deserialize(string message_type, string payload)
      {
        var stream = new System.IO.MemoryStream(Convert.FromBase64String(payload));
        return serializer.Deserialize(stream);
      }

      public string Serialize(string message_type, object[] values)
      {
        var stream = new System.IO.MemoryStream();
        serializer.Serialize(stream, values[0]);
        return Convert.ToBase64String(stream.GetBuffer());
      }
    }
    class TelemetryService : ITelemetryService
    {
      private IDictionary<string, IPayloadFormatter> formatters;
      private IExecutionStateTransport transport;
      public TelemetryService()
      {
        transport = new MemoryTransport();
        formatters = new Dictionary<string, IPayloadFormatter>
        {
          {"X1", new PairsFormatter() },
          {"Pa", new ArrayFormatter() },
          {"X2", new ObjectBinaryFormatter() }
          //https://msdn.microsoft.com/en-us/library/182eeyhh(v=vs.110).aspx
        };
      }
      public IExecutionStateTransport GetExecutionStateTransport()
      {
        return transport;
      }

      public IDictionary<string, IPayloadFormatter> GetFormatters()
      {
        return formatters;
      }
    }
    class MemoryTransport : IExecutionStateTransport, ITransportPublisher, ITransportReceiver
    {
      private IObserver<string> receive_side_transport_observer;
      public ITransportPublisher GetPublisher()
      {
        return this;
      }
      public ITransportReceiver GetReceiver()
      {
        return this;
      }
      public void Send(string payload)
      {
        //as ITransportReceiver invoke callback
        if (receive_side_transport_observer == null)
        {
          throw new Exception($"{nameof(receive_side_transport_observer)} is null");
        }
        receive_side_transport_observer.OnNext(payload);
      }

      public IDisposable Subscribe(IObserver<string> observer)
      {
        receive_side_transport_observer = observer;
        return new Subscription(this);
      }

      class Subscription : IDisposable
      {
        private MemoryTransport owner;
        public Subscription(MemoryTransport owner)
        {
          this.owner = owner;
        }
        public void Dispose()
        {
          owner.receive_side_transport_observer = null;
        }
      }
    }
    class MonitorApp : IObserver<string>
    {
      internal List<string> Messages;
      public MonitorApp()
      {
        Messages = new List<string>();
      }
      public void OnCompleted() {throw new NotImplementedException();}

      public void OnError(Exception error){throw new NotImplementedException();}

      public void OnNext(string value)
      {
        Messages.Add(value);
      }
    }

    [TestMethod, Description("StringPayload-with-ContentType-for-ArrayContent")]
    public void send_array()
    {
      //Arrange
      var service = new TelemetryService();
      var state_receiver = new ExecutionStateReceiver(service);
      var state_sender = new ExecutionStateSender(service);
      var monitor_app = new MonitorApp();

      state_receiver.Start();
      state_receiver.Subscribe(monitor_app);
      state_sender.Start();

      string name1 = "value1";
      int count = 2;
      decimal metric1 = 12.5M;

      //Act
      state_sender.Send("Pa", name1, count, metric1);

      //Assert
      Assert.AreEqual<int>(1, monitor_app.Messages.Count);
      Assert.AreEqual<string>("Pa|value1|2|12.5", monitor_app.Messages[0]);
    }

    [TestMethod, Description("StringPayload-with-ContentType-for-PairsContent")]
    public void send_pairs()
    {
      //Arrange
      var service = new TelemetryService();
      var state_receiver = new ExecutionStateReceiver(service);
      var state_sender = new ExecutionStateSender(service);
      var monitor_app = new MonitorApp();

      state_receiver.Start();
      state_receiver.Subscribe(monitor_app);
      state_sender.Start();

      string name1 = "value1";
      int count = 2;
      decimal metric1 = 12.5M;

      //Act
      state_sender.Send("X1", nameof(name1), name1, nameof(count), count, nameof(metric1), metric1);

      //Assert
      Assert.AreEqual<int>(1, monitor_app.Messages.Count);
      Assert.AreEqual<string>("X1|name1|value1|count|2|metric1|12.5", monitor_app.Messages[0]);
    }

    [Serializable]
    class RunState //: System.Runtime.Serialization.ISerializable
    {
      public string ID;
      public string Name;
      public string State;
      public string Count;
      public string Time;

      //public void GetObjectData(SerializationInfo info, StreamingContext context) {throw new NotImplementedException();}
    }

    [TestMethod, Description("BinaryPayload-with-ContentType-for-ComplexTypeContent")]
    public void send_object()
    {
      //Arrange
      var service = new TelemetryService();
      var state_receiver = new ExecutionStateReceiver(service);
      var state_sender = new ExecutionStateSender(service);
      var monitor_app = new MonitorApp();

      state_receiver.Start();
      state_receiver.Subscribe(monitor_app);
      state_sender.Start();

      var runstate = new RunState() { ID = "id2", Name = "name1", State = "OK", Count = "1,234", Time = "8:40:23" };

      //Act
      state_sender.Send("X2", runstate);

      //Assert
      Assert.AreEqual<int>(1, monitor_app.Messages.Count);
      Assert.IsTrue(monitor_app.Messages[0].Length > 0);
    }

    [TestMethod, Description("Headers/Payload-with-ContentType-for-ComplexTypeContent"),Ignore]
    public void send_with_headers()
    {
      //Arrange
      var service = new TelemetryService();
      var state_receiver = new ExecutionStateReceiver(service);
      var state_sender = new ExecutionStateSender(service);
      var monitor_app = new MonitorApp();

      state_receiver.Start();
      state_receiver.Subscribe(monitor_app);
      state_sender.Start();

      var runstate = new RunState() { ID = "id2", Name = "name1", State = "OK", Count = "1,234", Time = "8:40:23" };

      //Act
      //state_sender.Post(runstate);//, "PayloadType", "aType", );

      //Assert
      Assert.AreEqual<int>(1, monitor_app.Messages.Count);
      Assert.IsTrue(monitor_app.Messages[0].Length > 0);
    }
  }
}