using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

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
      Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name}({msg})");
    }
    public void NACK(string msg)
    {
      Console.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name}({msg})");
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

    public void Echo()
    {
      using (var host = new ServiceHost(typeof(ACKReceiver)))
      {
        try
        {
          host.AddServiceEndpoint(typeof(IACKReceiver), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), $"net.pipe://localhost/{pipename}");
          host.Open();
          Console.WriteLine("Press ENTER to exit"); Console.ReadLine();
        }
        catch (Exception ex)
        {
          Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
        }
      }
    }
  }
  class PipeClient
  {
    string host, pipename;
    public PipeClient(string host, string pipename)
    {
      this.host = host;
      this.pipename = pipename;
    }
    public void Echo()
    {
      using (var client = new ACKReceiverProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),$"net.pipe://{host}/{pipename}"))
      {
        client.ACK("ack1");
        client.NACK("nack1");
      }
    }
  }

  class wcf1
  {
    const string namedpipe = "wnpipe1";
    public static void _Main(string[] args)
    {
      var usage = new Action(() =>
      {
        Console.WriteLine("Server usage: server <named-pipe> | default");
        Console.WriteLine("Client usage: client <host> <named-pipe> | default [default]");
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
            Console.WriteLine("Server mode");
            var pipeserver = new PipeServer(args[1] == "default" ? namedpipe : args[1]);
            pipeserver.Echo();
          }
          else usage();
          break;
        case "client":
          if (args.Length >= 2)
          {
            Console.WriteLine("Client mode");
            var client = new PipeClient(args[1] == "default" ? Environment.MachineName : args[1], args.Length == 3 ? (args[2] == "default" ? namedpipe : args[2]) : namedpipe);
            client.Echo();
          }
          else usage();
          break;
        default:
          Console.WriteLine($"Unknown mode: [{args[0]}]");
          break;
      }
    }
  }
}