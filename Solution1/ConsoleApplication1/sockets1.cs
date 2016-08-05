using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
  class Feed
  {
    System.Net.Sockets.TcpListener server;
    List<System.Net.Sockets.TcpClient> subs;
    public void Start(int port)
    {
      //Task.Run(() => listen(port));
      listen(port);
    }
    void listen(int port)
    {
      const int buffer_size = 0xFF;
      var buffer = new byte[buffer_size];

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
    void newsubscriber(System.Net.Sockets.TcpClient sub) {}
  }

  class FeedHandler
  {
    public void Start(int port)
    {
      const int buffer_size = 0xFF;
      var buffer = new byte[buffer_size];

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
  }

  class sockets1
  {
    const int port = 13001;
    public static void _Main(string[] args)
    {
      if (args.Length > 0)
      {
        var feed = new Feed();
        feed.Start(port);
        /*do
        {
          var cmd = Console.ReadLine();
          if (string.Compare(cmd, "quit", true) == 0) break;
        } while (true);
        feed.Stop();*/
      }
      else
      {
        var client = new FeedHandler();
        client.Start(port);
      }
    }
  }
}