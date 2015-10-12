using System;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using ContractLib;

namespace RedundantQuasiService
{
  public class TimeController : ApiController
  {
    // GET api/time
    public string Get()
    {
      Console.WriteLine("Request: {0}\n", Request);
      var result = string.Format("[{0}] {1}", Environment.CurrentManagedThreadId, DateTime.Now.ToString("o"));
      Console.WriteLine(result);
      return result;
    }

    // GET api/time/5
    public Timepoint Get(int id)
    {
      Console.WriteLine("Request: {0}\n", Request);
      string request = GetRequestID(id);
      var result = new Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = request, Response = DateTime.Now.ToString("o") };
      Console.WriteLine("\nThread: {0}\nRequest: {1}\nResponse:{2}", result.Thread, result.Request, result.Response);
      return result;
    }
    private string GetRequestID(int id)
    {
      string result = id.ToString();
      if (id % 8 == 0)
      {
        System.Threading.Thread.Sleep(5000);
        result = "0xFF";
      }
      return result;
    }

    // POST api/time
    //public void Post([FromBody]string value)
    public Timepoint Post(Timepoint value)
    {
      //Console.WriteLine("Request: {0}\n", Request.Content.ReadAsStringAsync().Result);
      Console.WriteLine("Request: {0}\n", Request);
      Console.WriteLine("\nThread: {0}\nRequest: {1}\nResponse:{2}", value.Thread, value.Request, value.Response);
      var result = new Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = value.Request, Response = DateTime.Now.ToString("o") };
      return result;
    }

    // PUT api/time/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/time/5
    public void Delete(int id)
    {
    }

    /*public IDictionary<string, object> PostMap(IDictionary<string, object> request)
    {
      return null;
    }*/
  }

  public class VpnController : ApiController
  {
    // POST api/vpn
    public SendResponse Post([FromBody]SendRequest request)
    {
      Console.WriteLine("Request: {0}\n", Request);
      return new SendResponse() { CorrelationID = request.CorrelationID, Status = 102, Description = request.VPNKey + "|" + request.Topic + "|" + request.Message };
    }
  }

  static class Correlation
  {
    internal static readonly System.Collections.Concurrent.ConcurrentDictionary<uint, System.Threading.ManualResetEvent> syncs;
    static Correlation()
    {
      syncs = new System.Collections.Concurrent.ConcurrentDictionary<uint, System.Threading.ManualResetEvent>();
    }
  }
  public class TrackController : ApiController
  {
    // POST api/track
    public SendResponse Post([FromBody]SendRequest request)
    {
      //Console.WriteLine("Request: {0}\n", Request);
      var result = new SendResponse() { CorrelationID = request.CorrelationID, Status = 103, Description = request.VPNKey + "|" + request.Topic + "|" + request.Message };
      result.Status = GetStatus(request.CorrelationID);
      return result;
    }
    private byte GetStatus(uint id)
    {
      try
      {
        Correlation.syncs[id] = new System.Threading.ManualResetEvent(false);
        System.Threading.Tasks.Task.Run(() => callback(id));
        return (byte)(Correlation.syncs[id].WaitOne(10000) ? 101 : 0xFF);
      }
      finally
      {
        System.Threading.ManualResetEvent sync = null;
        if (Correlation.syncs.TryRemove(id, out sync))
        {
          sync.Dispose();
        }
      }
    }
    private void callback(uint id)
    {
      System.Threading.Thread.Sleep(5000);
      System.Threading.EventWaitHandle sync= Correlation.syncs[id];
      sync.Set();
    }
  }

  public class Startup
  {
    // This code configures Web API. The Startup class is specified as a type
    // parameter in the WebApp.Start method.
    public void Configuration(Owin.IAppBuilder appBuilder)
    {
      // Configure Web API for self-host. 
      var config = new System.Web.Http.HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = System.Web.Http.RouteParameter.Optional }
      );

      appBuilder.UseWebApi(config);
    }
  }
  class RedundantQuasiServiceExe
  {
    static void Main(string[] args)
    {
      try
      {
        string baseAddress = string.Format("http://{0}:7000/", Environment.MachineName);

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          Console.WriteLine("[{0}] Listening at: {1}", Environment.CurrentManagedThreadId, baseAddress);
          Console.WriteLine("Press ENTER to exit");
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);
          ex = ex.InnerException;
        }
      }
    }
  }
}