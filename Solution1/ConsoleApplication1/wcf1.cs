using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace ConsoleApplication1
{
  [ServiceContract]
  public interface IACKReceiver
  {
    [OperationContract] void ACK(string msg);
    [OperationContract] void NACK(string msg);
  }
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
  class ACKReceiver : IACKReceiver
  {
    public void ACK(string msg)
    {
      WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name}({msg})");
    }
    public void NACK(string msg)
    {
      WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name}({msg})");
    }
  }
  class ACKReceiverProxy : ClientBase<IACKReceiver>
  {
    public ACKReceiverProxy(Binding binding, string address) : base(binding, new EndpointAddress(address)) { }
    public void ACK(string msg)
    {
      Channel.ACK(msg);
    }
    public void NACK(string msg)
    {
      Channel.NACK(msg);
    }
    
  }
  class PipeServer
  {
    string pipename;
    public PipeServer(string pipename)
    {
      this.pipename = pipename;
    }

    ServiceHost host;
    public void Ack_Nack()
    {
      host = new ServiceHost(typeof(ACKReceiver));
      host.Faulted += (s, e) => WriteLine("Server Faulted");
      host.Opening += (s, e) => WriteLine("Server Opening");
      host.Opened += (s, e) => WriteLine("Server Opened");
      host.Closing += (s, e) => WriteLine("Server Closing");
      host.Closed += (s, e) => WriteLine("Server Closed");
      try
      {
        var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
        binding.ReceiveTimeout = TimeSpan.Parse("00:00:30");
        host.AddServiceEndpoint(typeof(IACKReceiver), binding, $"net.pipe://localhost/{pipename}");
        host.Open();
        //WriteLine("Press ENTER to exit"); ReadLine();
      }
      catch (Exception ex)
      {
        WriteLine($"{ex.GetType().FullName}: {ex.Message}");
      }
    }
    public void Stop() { host.Close(); }
  }
  class PipeClient
  {
    string host, pipename;
    public PipeClient(string host, string pipename)
    {
      this.host = host;
      this.pipename = pipename;
    }
    public ACKReceiverProxy proxy;
    public void Ack_Nack()
    {
      var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
      binding.ReceiveTimeout = TimeSpan.Parse("00:00:30");
      proxy = new ACKReceiverProxy(binding, $"net.pipe://{host}/{pipename}");
      var comm = proxy as ICommunicationObject;
      if (comm != null)
      {
        comm.Faulted += (s, e) => WriteLine("Client Faulted");
        comm.Opening += (s, e) => WriteLine("Client Opening");
        comm.Opened += (s, e) => WriteLine("Client Opened");
        comm.Closing += (s, e) => WriteLine("Client Closing");
        comm.Closed += (s, e) => WriteLine("Client Closed");
      }
      else WriteLine("No Client ICommunicationObject");
      try
      {
        proxy.ACK("ack1");
        //Task.Run(() => client.ACK("ack1"));
        WriteLine("Press ENTER to continue"); ReadLine();
        proxy.NACK("nack1");
        //Task.Run(() => { if (client.State == CommunicationState.Opened) client.NACK("nack1"); else WriteLine($"cannot call NACK ({client.State})"); });
        WriteLine("Press ENTER to exit"); ReadLine();
      }
      catch (Exception ex)
      {
        WriteLine($"{ex.GetType().FullName}: {ex.Message}");
      }
    }
    public void Ack_Nack_percall()
    {
      try
      {
        ACK_percall("ack1");
        //Task.Run(() => client.ACK("ack1"));
        WriteLine("Press ENTER to continue"); ReadLine();
        NACK_percall("nack1");
        //Task.Run(() => { if (client.State == CommunicationState.Opened) client.NACK("nack1"); else WriteLine($"cannot call NACK ({client.State})"); });
        WriteLine("Press ENTER to exit"); ReadLine();
      }
      catch (Exception ex)
      {
        WriteLine($"{ex.GetType().FullName}: {ex.Message}");
      }
    }
    public void Stop() { proxy.Close(); }
    private void ACK_percall(string msg)
    {
      var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
      binding.ReceiveTimeout = TimeSpan.Parse("00:00:30");
      using (var proxy = new ACKReceiverProxy(binding, $"net.pipe://{host}/{pipename}"))
      {
        var comm = proxy as ICommunicationObject;
        if (comm != null)
        {
          comm.Faulted += (s, e) => WriteLine("Client Faulted");
          comm.Opening += (s, e) => WriteLine("Client Opening");
          comm.Opened += (s, e) => WriteLine("Client Opened");
          comm.Closing += (s, e) => WriteLine("Client Closing");
          comm.Closed += (s, e) => WriteLine("Client Closed");
        }
        else WriteLine("No Client ICommunicationObject");
        proxy.ACK(msg);
      }
    }
    private void NACK_percall(string msg)
    {
      var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
      binding.ReceiveTimeout = TimeSpan.Parse("00:00:30");
      using (var proxy = new ACKReceiverProxy(binding, $"net.pipe://{host}/{pipename}"))
      {
        var comm = proxy as ICommunicationObject;
        if (comm != null)
        {
          comm.Faulted += (s, e) => WriteLine("Client Faulted");
          comm.Opening += (s, e) => WriteLine("Client Opening");
          comm.Opened += (s, e) => WriteLine("Client Opened");
          comm.Closing += (s, e) => WriteLine("Client Closing");
          comm.Closed += (s, e) => WriteLine("Client Closed");
        }
        else WriteLine("No Client ICommunicationObject");
        proxy.NACK(msg);
      }
    }
  }

  class wcf1
  {
    public static void _Main(string[] args)
    {
      _Main0(new string[] { "server", "default" });
      _Main0(new string[] { "client", "default" });
      WriteLine("Press ENTER to check Client"); ReadLine();
      //WriteLine($"client.State:{client.proxy.State}");
      WriteLine("Press ENTER to stop Server"); ReadLine();
      server.Stop();
      WriteLine("Server stopped. Press ENTER to check Client"); ReadLine();
      //WriteLine($"client.State:{client.proxy.State}");
      WriteLine("Press ENTER to stop Client"); ReadLine();
      //client.Stop();
      //WriteLine($"client.State:{client.proxy.State}");
      //WriteLine($"client.State:{(client.proxy.State != null ? client?.proxy.State.ToString() : "no proxy")}");
    }

    const string namedpipe = "wnpipe1";
    static PipeServer server;
    static PipeClient client;
    public static void _Main1(string[] args)
    {
      _Main0(new string[] { "server", "default" });
      _Main0(new string[] { "client", "default" });
      WriteLine("Press ENTER to stop Server"); ReadLine();
      server.Stop();
      WriteLine("Server Stopped. Press ENTER to stop Client"); ReadLine();
      client.Stop();
      WriteLine("Client Stopped. Press ENTER to stop Server"); ReadLine();
    }
    public static void _Main0(string[] args)
    {
      var usage = new Action(() =>
      {
        WriteLine("Server usage: server <named-pipe> | default");
        WriteLine("Client usage: client <host> <named-pipe> | default [default]");
      });
      if (args.Length <= 0 || args.Length > 3)
      {
        usage();
        return;
      }
      switch (args[0])
      {
        case "server":
          if (args.Length == 2)
          {
            WriteLine("Server mode");
            server = new PipeServer(args[1] == "default" ? namedpipe : args[1]);
            server.Ack_Nack();
          }
          else usage();
          break;
        case "client":
          if (args.Length >= 2)
          {
            WriteLine("Client mode");
            client = new PipeClient(args[1] == "default" ? Environment.MachineName : args[1], args.Length == 3 ? (args[2] == "default" ? namedpipe : args[2]) : namedpipe);
            //client.Ack_Nack();
            client.Ack_Nack_percall();
          }
          else usage();
          break;
        default:
          WriteLine($"Unknown mode: [{args[0]}]");
          break;
      }
    }
  }
}