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

    public void Ack_Nack()
    {
      using (var host = new ServiceHost(typeof(ACKReceiver)))
      {
        try
        {
          host.AddServiceEndpoint(typeof(IACKReceiver), new NetNamedPipeBinding(NetNamedPipeSecurityMode.None), $"net.pipe://localhost/{pipename}");
          host.Open();
          WriteLine("Press ENTER to exit"); ReadLine();
        }
        catch (Exception ex)
        {
          WriteLine($"{ex.GetType().FullName}: {ex.Message}");
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
    public void Ack_Nack()
    {
      using (var client = new ACKReceiverProxy(new NetNamedPipeBinding(NetNamedPipeSecurityMode.None),$"net.pipe://{host}/{pipename}"))
      {
        try
        {
          client.ACK("ack1");
          WriteLine("Press ENTER to continue");ReadLine();
          client.NACK("nack1");
          WriteLine("Press ENTER to exit");ReadLine();
        }
        catch (Exception ex)
        {
          WriteLine($"{ex.GetType().FullName}: {ex.Message}");
        }
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
            var server = new PipeServer(args[1] == "default" ? namedpipe : args[1]);
            server.Ack_Nack();
          }
          else usage();
          break;
        case "client":
          if (args.Length >= 2)
          {
            WriteLine("Client mode");
            var client = new PipeClient(args[1] == "default" ? Environment.MachineName : args[1], args.Length == 3 ? (args[2] == "default" ? namedpipe : args[2]) : namedpipe);
            client.Ack_Nack();
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