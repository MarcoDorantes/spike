using System;
using System.Linq;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Console;

//http://www.asp.net/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api
namespace ConsoleApplication1
{
  public class ValuesController : ApiController
  {
    // GET api/values 
    public IEnumerable<string> Get()
    {
      return new string[] { $"thread: {Environment.CurrentManagedThreadId}", $"instance: {this.GetHashCode()}" };
    }

    // GET api/values/5 
    public string Get(int id)
    {
      return "value";
    }

    // POST api/values 
    public void Post([FromBody]string value)
    {
      var s = new System.IO.MemoryStream();
      Request.Content.CopyToAsync(s);
      s.Flush();
      WriteLine("payload:[{0}]", System.Text.Encoding.UTF8.GetString(s.ToArray()));
      WriteLine(value);
    }

    // PUT api/values/5 
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/values/5 
    public void Delete(int id)
    {
    }
  }

  public class Startup
  {
    // This code configures Web API. The Startup class is specified as a type
    // parameter in the WebApp.Start method.
    public void Configuration(IAppBuilder appBuilder)
    {
      // Configure Web API for self-host. 
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );

      appBuilder.UseWebApi(config);
    }
  }

  class SelfHostTry1
  {
    public static void FullClientCall(string baseAddress)
    {
      // Create HttpClient and make a request to api/values
      var client = new System.Net.Http.HttpClient();

      string url = baseAddress + "api/values";
      //WriteLine("HTTP to: {0}", url);
      var response = client.GetAsync(url).Result;

      //WriteLine(response);
      WriteLine(response.Content.ReadAsStringAsync().Result);
    }

    public static void SingleRequest(string[] args)
    {
      try
      {
        string baseAddress = "http://localhost:9000/";

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          FullClientCall(baseAddress);
        }

        ReadLine();
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);
          ex = ex.InnerException;
        }
      }
    }
    public static void MultiRequest(string[] args)
    {
      try
      {
        int max = args.Length == 1 ? int.Parse(args[0]) : 10;
        string baseAddress = "http://localhost:9000/";

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          Parallel.ForEach(Enumerable.Range(1, max), n => FullClientCall(baseAddress));
        }

        ReadLine();
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);
          ex = ex.InnerException;
        }
      }
    }

    public static void Run(string[] args)
    {
      //SingleRequest(args);
      MultiRequest(args);
    }
  }
}