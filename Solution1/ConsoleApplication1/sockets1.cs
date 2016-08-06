using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
  class MsgSource
  {
    const int buffer_size = 0xFF;

    int port;
    System.Net.Sockets.TcpListener server;
    System.Net.Sockets.TcpClient serverside_client;
    System.Net.Sockets.NetworkStream stream;
    public MsgSource(int port)
    {
      this.port = port;
      Console.Write("Listening...");
      server = System.Net.Sockets.TcpListener.Create(port);
      server.Start();
      serverside_client = server.AcceptTcpClient();
      Console.WriteLine("\nClient connected");
      stream = serverside_client.GetStream();
    }
    public void Start()
    {
      do
      {
        Console.Write("> ");
        var cmd = Console.ReadLine();
        if (cmd == "quit") break;
        switch (cmd)
        {
          case "wait":
            wait();
            break;

          case "send":
            send();
            break;

          case "read":
            read();
            break;

          default:
            Console.WriteLine($"What is [{cmd}]?");
            break;
        }
      } while (true);
    }
    void wait()
    {
      var buffer = new byte[buffer_size];
      do
      {
        int read = stream.Read(buffer, 0, buffer_size);
        if (read == 0) break;
        string handshake = Encoding.UTF8.GetString(buffer, 0, read);
        if (string.Compare(handshake, "GO", true) != 0)
        {
          Console.WriteLine($"No session for [{handshake}]");
          continue;
        }
        Console.WriteLine("Session started");break;
      } while (true);
      Console.WriteLine("wait done");
    }
    void send()
    {
      Console.Write("Send:");
      var app = Console.ReadLine();
      var msg = Encoding.UTF8.GetBytes(app);
      stream.Write(msg, 0, msg.Length);
      //System.IO.IOException: Unable to write data to the transport connection: An established connection was aborted by the software in your host machine.
      //at System.Net.Sockets.NetworkStream.Write(Byte[] buffer, Int32 offset, Int32 size)
    }
    void read()
    {
      var buffer = new byte[buffer_size];
      int read = stream.Read(buffer, 0, buffer_size);
      if (read != 0)
      {
        var received_text = Encoding.UTF8.GetString(buffer, 0, read);
        foreach (var msg in received_text.Split('|'))
        {
          Console.WriteLine($"Received:[{msg}]");
        }
      }
      else Console.WriteLine($"No bytes read");
    }
    public void Stop()
    {
      stream.Dispose();
      serverside_client.Dispose();
      server.Stop();
    }
  }
  class AppMessage
  {
    public string data;
  }
  class ClientAccepter : IDisposable
  {
    public class MsgSender : IDisposable
    {
      const int buffer_size = 0xFF;

      System.Net.Sockets.TcpClient serverside_client;
      System.Net.Sockets.NetworkStream stream;
      System.Collections.Concurrent.BlockingCollection<AppMessage> inbound;
      System.Collections.Concurrent.BlockingCollection<AppMessage> outbound;
      Task read_task, send_task, mex_task, app_read_task;
      bool connection_lost;
      public MsgSender(System.Net.Sockets.TcpClient client)
      {
        connection_lost = false;
        serverside_client = client;
        stream = serverside_client.GetStream();
        inbound = new System.Collections.Concurrent.BlockingCollection<AppMessage>();
        outbound = new System.Collections.Concurrent.BlockingCollection<AppMessage>();
        read_task = Task.Run(() => read());
        send_task = Task.Run(() => send());
      }
      public void Start()
      {
        mex_task = Task.Run(() => mex());
      }
      public void Stop()
      {
        inbound?.CompleteAdding();
        outbound?.CompleteAdding();

        read_task.Wait(1000);
        send_task.Wait(1000);
        mex_task.Wait(1000);
        app_read_task.Wait(1000);

        stream?.Dispose();
        serverside_client?.Dispose();

        //inbound?.Dispose();
        //outbound?.Dispose();
        //inbound = null;
        //outbound = null;

        stream = null;
        serverside_client = null;
      }

      void mex()
      {
        try
        {
          app_read_task = Task.Run(() => app_read());
          foreach (var msg in inbound.GetConsumingEnumerable())
          {
            if (msg.data == "GO") break;
          }
          Console.WriteLine("Session started");
          int k;
          for (k = 0; k < 5; ++k)
          {
            outbound.Add(new AppMessage { data = $"msg{k}" });
          }
          Console.WriteLine($"Sent msg count: {k}");
        }
        catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
      }
      void app_read()
      {
        try
        {
          Console.WriteLine("app_read start");
          var acks = new List<string>();
          foreach (var msg in inbound.GetConsumingEnumerable())
          {
            acks.Add(msg.data);
          }
          Console.WriteLine($"Received ACK count: {acks.Count}");
        }
        catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
      }
      void read()
      {
        try
        {
          Console.WriteLine("read start");
          var buffer = new byte[buffer_size];
          do
          {
            if (connection_lost) continue;
            int read = stream.Read(buffer, 0, buffer_size);
            if (read != 0)
            {
              var received_text = Encoding.UTF8.GetString(buffer, 0, read);
              foreach (var msg in received_text.Split('|'))
              {
                inbound.Add(new AppMessage { data = msg });
              }
            }
            else
            {
              connection_lost = true;
              //Console.WriteLine($"No bytes read");
            }
          } while (inbound?.IsAddingCompleted == false);
          Console.WriteLine("read stop");
        }
        catch (System.IO.IOException) { connection_lost = true; }
        catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
      }
      void send()
      {
        try
        {
          Console.WriteLine("send start");
          foreach (var app in outbound.GetConsumingEnumerable())
          {
            var msg = Encoding.UTF8.GetBytes(app.data);
            stream.Write(msg, 0, msg.Length);
          }
          Console.WriteLine("send stop");
        }
        catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
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
            Stop();
          }

          // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
          // TODO: set large fields to null.

          disposedValue = true;
        }
      }

      // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
      // ~MsgProvider() {
      //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      //   Dispose(false);
      // }

      // This code added to correctly implement the disposable pattern.
      public void Dispose()
      {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
        // TODO: uncomment the following line if the finalizer is overridden above.
        // GC.SuppressFinalize(this);
      }
      #endregion
    }

    int port;
    System.Net.Sockets.TcpListener server;
    List<MsgSender> providers;
    public ClientAccepter(int port)
    {
      this.port = port;
      providers = new List<MsgSender>();
    }
    public void Start()
    {
      Task.Run(() => listen());
    }
    public void Stop()
    {
      providers?.ForEach(p => p.Stop());
      server?.Stop();
      providers = null;
      server = null;
    }
    void listen()
    {
      server = System.Net.Sockets.TcpListener.Create(port);
      server.Start();
      Console.Write("Listening...");
      do
      {
        var serverside_client = server.AcceptTcpClient();
        var provider = new MsgSender(serverside_client);
        providers.Add(provider);
        provider.Start();
        Console.WriteLine("\nClient connected");
      } while (true);
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
          Stop();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~ClientAccepter() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion
  }
  class Feed
  {
    const int buffer_size = 0xFF;
    byte[] buffer;
    int port;
    int ack = 0, nack = 0;
    System.Net.Sockets.TcpListener server;
    public Feed(int port)
    {
      this.port = port;
      buffer = new byte[buffer_size];
    }

    public void Echo()
    {
      server = System.Net.Sockets.TcpListener.Create(port);
      server.Start();
      using (var serverside_client = server.AcceptTcpClient())
      using (var serverside_stream = serverside_client.GetStream())
      {
        do
        {
          int read = serverside_stream.Read(buffer, 0, buffer_size);
          if (read == 0) break;
          string msg = Encoding.UTF8.GetString(buffer, 0, read);
          var reply = Encoding.UTF8.GetBytes($"[{DateTime.Now.ToString("s")}] Received: [{msg}]");
          serverside_stream.Write(reply, 0, reply.Length);
        } while (true);
      }
      server.Stop();
    }

    private string app(int n) => $"{new string('X', (n + 1) * 5)}\n";
    public void Handshake()
    {
      server = System.Net.Sockets.TcpListener.Create(port);
      server.Start();
      using (var serverside_client = server.AcceptTcpClient())
      using (var stream = serverside_client.GetStream())
      {
        do
        {
          int read = stream.Read(buffer, 0, buffer_size);
          if (read == 0) break;
          string handshake = Encoding.UTF8.GetString(buffer, 0, read);
          if (string.Compare(handshake, "GO", true) != 0) continue;
          Console.WriteLine("Session started");

          Task.Run(() => ack_nack(stream));

          for (int k = 0; k < 0xF; ++k)
          {
            var msg = Encoding.UTF8.GetBytes($"{app(k)}");
            stream.Write(msg, 0, msg.Length);
            System.Threading.Thread.Sleep(50);
          }
          break;
        } while (true);
        Console.WriteLine($"{nameof(ack_nack)}: acks:{ack} nacks:{nack}");
      }
      server.Stop();
    }
    private void ack_nack(System.IO.Stream stream)
    {
      var buffer = new byte[buffer_size];
      do
      {
        int read = stream.Read(buffer, 0, buffer_size);
        if (read == 0) break;
        var block = Encoding.UTF8.GetString(buffer, 0, read);
        foreach (var msg in block.Split('\n'))
        {
          Console.WriteLine($"Received:[{msg}] ");
          if (msg.StartsWith("NACK")) ++nack; else ++ack;
        }
      } while (true);
    }

    public void GD1()
    {
      var server = new MsgSource(port);
      server.Start();
      Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
      server.Stop();
    }
    public void GD2()
    {
      var server = new ClientAccepter(port);
      server.Start();
      Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
      server.Stop();
    }
  }

  class MsgTarget
  {
    const int buffer_size = 0xFF;

    int port;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    public MsgTarget(int port)
    {
      this.port = port;
      client = new System.Net.Sockets.TcpClient();
    }
    public void Start()
    {
      client.Connect(Environment.MachineName, port);
      stream = client.GetStream();

      do
      {
        Console.Write("> ");
        var cmd = Console.ReadLine();
        if (cmd == "quit") break;
        switch (cmd)
        {
          case "open":
            open();
            break;

          case "read":
            read();
            break;

          case "send":
            send();
            break;

          default:
            Console.WriteLine($"What is [{cmd}]?");
            break;
        }
      } while (true);
    }
    void open()
    {
      do
      {
        Console.Write("Handshake:");
        var input = Console.ReadLine();
        if (input == "done") break;
        var handshake = Encoding.UTF8.GetBytes(input);
        stream.Write(handshake, 0, handshake.Length);
      } while (true);
    }
    void read()
    {
      var buffer = new byte[buffer_size];
      int read = stream.Read(buffer, 0, buffer_size);
      if (read != 0)
      {
        var received_text = Encoding.UTF8.GetString(buffer, 0, read);
        foreach (var msg in received_text.Split('|'))
        {
          Console.WriteLine($"Received:[{msg}]");
        }
      }
      else Console.WriteLine($"No bytes read");
    }
    void send()
    {
      Console.Write("Send:");
      var app = Console.ReadLine();
      var msg = Encoding.UTF8.GetBytes(app);
      stream.Write(msg, 0, msg.Length);
      //System.IO.IOException: Unable to write data to the transport connection: An established connection was aborted by the software in your host machine.
      //at System.Net.Sockets.NetworkStream.Write(Byte[] buffer, Int32 offset, Int32 size)
    }
    public void Stop()
    {
      stream.Dispose();
      client.Dispose();
    }
  }

  class MsgReceiver : IDisposable
  {
    const int buffer_size = 0xFF;

    int port;
    System.Net.Sockets.TcpClient client;
    System.Net.Sockets.NetworkStream stream;
    System.Collections.Concurrent.BlockingCollection<AppMessage> inbound;
    System.Collections.Concurrent.BlockingCollection<AppMessage> outbound;
    Task read_task, send_task, mex_task, app_read_task;
    bool connection_lost;

    List<string> msgs;
    public MsgReceiver(int port)
    {
      connection_lost = false;
      this.port = port;
      client = new System.Net.Sockets.TcpClient();
      inbound = new System.Collections.Concurrent.BlockingCollection<AppMessage>();
      outbound = new System.Collections.Concurrent.BlockingCollection<AppMessage>();

      msgs = new List<string>();
    }

    public void Start()
    {
      client.Connect(Environment.MachineName, port);
      Console.WriteLine("Connected");
      stream = client.GetStream();

      read_task = Task.Run(() => read());
      send_task = Task.Run(() => send());
      mex_task =Task.Run(() => mex());
    }
    public void Stop()
    {
      inbound?.CompleteAdding();
      outbound?.CompleteAdding();

      read_task.Wait(1000);
      send_task.Wait(1000);
      mex_task.Wait(1000);
      app_read_task.Wait(1000);

      stream?.Dispose();
      client?.Dispose();

      //inbound?.Dispose();
      //outbound?.Dispose();
      //inbound = null;
      //outbound = null;

      stream = null;
      client = null;

      Console.WriteLine($"Received msg count: {msgs.Count}");
    }

    void mex()
    {
      try
      {
        app_read_task = Task.Run(() => app_read());
        outbound.Add(new AppMessage { data = "GO" });
      }
      catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
    }
    void app_read()
    {
      try
      {
        Console.WriteLine("app_read start");
        int count = 0;
        foreach (var msg in inbound.GetConsumingEnumerable())
        {
          msgs.Add(msg.data);
          Console.WriteLine("app_msg received & ack sent");
          outbound.Add(new AppMessage { data = $"ack{++count}" });
        }
        Console.WriteLine("app_read stop");
      }
      catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
    }
    void read()
    {
      try
      {
        Console.WriteLine("read start");
        var buffer = new byte[buffer_size];
        do
        {
          if (connection_lost) continue;
          int read = stream.Read(buffer, 0, buffer_size);
          if (read != 0)
          {
            var received_text = Encoding.UTF8.GetString(buffer, 0, read);
            foreach (var msg in received_text.Split('|'))
            {
              inbound.Add(new AppMessage { data = msg });
            }
          }
          else
          {
            connection_lost = true;
            //Console.WriteLine($"No bytes read");
          }
        } while (inbound?.IsAddingCompleted == false);
        Console.WriteLine("read stop");
      }
      //The underlying Socket is closed. -> System.IO.IOException: Unable to read data from the transport connection: A blocking operation was interrupted by a call to WSACancelBlockingCall.
      catch (System.IO.IOException) { connection_lost = true; }
      catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
    }
    void send()
    {
      try
      {
        Console.WriteLine("send start");
        foreach (var app in outbound.GetConsumingEnumerable())
        {
          var msg = Encoding.UTF8.GetBytes(app.data);
          stream.Write(msg, 0, msg.Length);
        }
        Console.WriteLine("send stop");
      }
      catch (Exception ex) { Console.WriteLine($"->{System.Reflection.MethodBase.GetCurrentMethod().Name} {ex.GetType().FullName}: {ex.Message}"); }
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
          Stop();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~MsgAccepter() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion
  }

  class FeedHandler
  {
    const int buffer_size = 0xFF;
    byte[] buffer;
    int port;
    int received;
    System.Threading.CancellationTokenSource source;
    System.Threading.CancellationToken cancel;
    public FeedHandler(int port)
    {
      this.port = port;
      buffer = new byte[buffer_size];
    }

    public void Echo()
    {
      using (var client = new System.Net.Sockets.TcpClient())
      {
        client.Connect(Environment.MachineName, port);
        using (var stream = client.GetStream())
        {
          for (int k = 0; k < 10; ++k)
          {
            var msg = Encoding.UTF8.GetBytes($"msg{k}");
            stream.Write(msg, 0, msg.Length);
            int read = stream.Read(buffer, 0, buffer_size);
            if (read == 0) break;
            Console.WriteLine($"Reply: {Encoding.UTF8.GetString(buffer, 0, read)}");
          }
        }
      }
    }

    public void Handshake()
    {
      received = 0;
      source = new System.Threading.CancellationTokenSource();
      cancel = source.Token;

      using (var client = new System.Net.Sockets.TcpClient())
      {
        client.Connect(Environment.MachineName, port);
        using (var stream = client.GetStream())
        {
          var handshake = Encoding.UTF8.GetBytes("GO");
          stream.Write(handshake, 0, handshake.Length);

          Task.Run(() => ack_nack(stream));

          do
          {
            int read = stream.Read(buffer, 0, buffer_size);
            //System.IO.IOException: Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host.
            //at System.Net.Sockets.NetworkStream.Read(Byte[] buffer, Int32 offset, Int32 size)
            if (read == 0) break;
            var block = Encoding.UTF8.GetString(buffer, 0, read);
            foreach (var msg in block.Split('\n'))
            {
              Console.WriteLine($"Received:[{msg}]");
              ++received;
            }
          } while (true);
          source.Cancel();
        }
      }
    }
    private void ack_nack(System.IO.Stream stream)
    {
      System.Threading.Thread.Sleep(25);
      while (!cancel.IsCancellationRequested)
      {
        var ack = $"{(received % 2 == 0 ? "NACK" : "ACK")}\n";
        var msg = Encoding.UTF8.GetBytes(ack);
        stream.Write(msg, 0, msg.Length);
      }
    }

    public void GD1()
    {
      var client = new MsgTarget(port);
      client.Start();
      Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
      client.Stop();
    }
    public void GD2()
    {
      var client = new MsgReceiver(port);
      client.Start();
      Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
      client.Stop();
    }
  }

  class sockets1
  {
    const int port = 13001;
    public static void _Main(string[] args)
    {
      if (args.Length > 0)
      {
        var feed = new Feed(port);
        //feed.Echo();
        //feed.Handshake();
        //feed.GD1();
        feed.GD2();

        /*do
        {
          var cmd = Console.ReadLine();
          if (string.Compare(cmd, "quit", true) == 0) break;
        } while (true);
        feed.Stop();*/
      }
      else
      {
        var client = new FeedHandler(port);
        //client.Echo();
        //client.Handshake();
        //client.GD1();
        client.GD2();
      }
    }
  }
}