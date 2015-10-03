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
      var result = string.Format("[{0}] {1}", Environment.CurrentManagedThreadId, DateTime.Now.ToString("o"));
      Console.WriteLine(result);
      return result;
    }

    // GET api/time/5
    public Timepoint Get(int id)
    {
      var result= new Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = id.ToString(), Response = DateTime.Now.ToString("o") };
      Console.WriteLine("\nThread: {0}\nRequest: {1}\nResponse:{2}", result.Thread, result.Request, result.Response);
      return result;
    }

    // POST api/time
    //public void Post([FromBody]string value)
    public Timepoint Post(Timepoint value)
    {
      //Console.WriteLine("Request: {0}",value);
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
        string baseAddress = "http://localhost:7000/";

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          Console.WriteLine("[{0}] Listening at: {1}", Environment.CurrentManagedThreadId, baseAddress);
          Console.WriteLine("Press ENTER to exit");
          Console.ReadLine();
        }
      }
      catch (Exception ex) { Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message); }
    }
  }
}