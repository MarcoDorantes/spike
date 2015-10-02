using System;
using System.Collections.Generic;
using Owin;
using System.Web.Http;

namespace RedundantQuasiService
{
  public class TimeController : ApiController
  {
    // GET api/time
    public string Get()
    {
      return DateTime.Now.ToString("o");
    }

    // GET api/time/5 
    public string Get(int id)
    {
      return "value";
    }

    // POST api/time
    public void Post([FromBody]string value)
    {
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
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        string baseAddress = "http://localhost:7000/";

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          Console.WriteLine("Listening at: {0}",baseAddress);
          Console.WriteLine("Press ENTER to exit");
          Console.ReadLine();
        }
      }
      catch (Exception ex) { Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message); }
    }
  }
}