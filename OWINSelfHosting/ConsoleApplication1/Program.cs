using System;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using System.Net.Http;

//http://www.asp.net/web-api/overview/hosting-aspnet-web-api/use-owin-to-self-host-web-api
namespace ConsoleApplication1
{
  public class ValuesController : ApiController
  {
    // GET api/values 
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
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
      Console.WriteLine("payload:[{0}]", System.Text.Encoding.UTF8.GetString(s.ToArray()));
      Console.WriteLine(value);
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
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        string baseAddress = "http://localhost:9000/";

        // Start OWIN host
        using (Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress))
        {
          // Create HttpCient and make a request to api/values
          var client = new System.Net.Http.HttpClient();

          string url = baseAddress + "api/values";
          Console.WriteLine("HTTP to: {0}", url);
          var response = client.GetAsync(url).Result;

          Console.WriteLine(response);
          Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

        Console.ReadLine();
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