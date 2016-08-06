using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
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
        feed.Handshake();

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
        client.Handshake();
      }
    }
  }
}