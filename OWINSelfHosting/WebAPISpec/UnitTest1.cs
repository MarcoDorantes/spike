using System;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPISpec
{
  public class ValuesController : ApiController
  {
    public static System.Net.Http.HttpRequestMessage GetRequest;
    public static System.Net.Http.HttpRequestMessage GetXRequest;

    // GET api/values
    public IEnumerable<string> Get()
    {
      GetRequest = Request;
      return new string[] { "value 1", "value 2" };
    }

    // GET api/values/5
    public string Get(int id)
    {
      GetXRequest = Request;
      return "value " + id;
    }

    // POST api/values
    public void Post([FromBody]string value)
    {
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

  public class ValuesStartup
  {
    public void Configuration(IAppBuilder appBuilder)
    {
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      appBuilder.UseWebApi(config);
    }
  }

  [TestClass]
  public class ValuesSpec
  {
    [TestMethod]
    public void GetValues()
    {
      string baseAddress = "http://localhost:9000/";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var client = new System.Net.Http.HttpClient();
        var response = client.GetAsync(baseAddress + "api/values").Result;

        Assert.IsNotNull(ValuesController.GetRequest);
        Assert.IsNotNull(ValuesController.GetRequest.Content);
        Assert.IsNotNull(ValuesController.GetRequest.Content.Headers);
        Assert.IsNull(ValuesController.GetRequest.Content.Headers.ContentType);

        Assert.AreEqual<string>("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.AreEqual<string>("[\"value 1\",\"value 2\"]", response.Content.ReadAsStringAsync().Result);
      }
    }

    [TestMethod]
    public void GetValue()
    {
      string baseAddress = "http://localhost:9001/";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var client = new System.Net.Http.HttpClient();
        var response = client.GetAsync(baseAddress + "api/values/2").Result;

        Assert.IsNotNull(ValuesController.GetXRequest);
        Assert.IsNotNull(ValuesController.GetXRequest.Content);
        Assert.IsNotNull(ValuesController.GetXRequest.Content.Headers);
        Assert.IsNull(ValuesController.GetXRequest.Content.Headers.ContentType);

        Assert.AreEqual<string>("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.AreEqual<string>("\"value 2\"", response.Content.ReadAsStringAsync().Result);
      }
    }

    /*[TestMethod]
    public void PostValue()
    {
      string baseAddress = "http://localhost:9001/";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var client = new System.Net.Http.HttpClient();
        var response = client.PostAsync(baseAddress + "api/values","posted").Result;

        Assert.AreEqual<string>("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        Assert.AreEqual<string>("\"value 2\"", response.Content.ReadAsStringAsync().Result);
      }
    }*/
  }
}