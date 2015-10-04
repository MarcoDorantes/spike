using System;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPISpec
{
  public class ValuesController : ApiController
  {
    public static System.Net.Http.HttpRequestMessage GetRequest;
    public static System.Net.Http.HttpRequestMessage GetXRequest;
    public static System.Net.Http.HttpRequestMessage PostRequest;

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
    //public void Post(string value)
    {
      PostRequest = Request;
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
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.GetAsync(baseAddress + "api/values").Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<string>("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
          Assert.AreEqual<string>("[\"value 1\",\"value 2\"]", response.Content.ReadAsStringAsync().Result);

          Assert.IsNotNull(ValuesController.GetRequest);
          Assert.IsNotNull(ValuesController.GetRequest.Content);
          Assert.IsNotNull(ValuesController.GetRequest.Content.Headers);
          Assert.IsNull(ValuesController.GetRequest.Content.Headers.ContentType);
        }
      }
    }

    [TestMethod]
    public void GetValue()
    {
      string baseAddress = "http://localhost:9001/";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.GetAsync(baseAddress + "api/values/2").Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<string>("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
          Assert.AreEqual<string>("\"value 2\"", response.Content.ReadAsStringAsync().Result);

          Assert.IsNotNull(ValuesController.GetXRequest);
          Assert.IsNotNull(ValuesController.GetXRequest.Content);
          Assert.IsNotNull(ValuesController.GetXRequest.Content.Headers);
          Assert.IsNull(ValuesController.GetXRequest.Content.Headers.ContentType);
        }
      }
    }

    [TestMethod]
    public void PostValue()
    {
      string baseAddress = "http://localhost:9002/";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          //var content = new System.Net.Http.StringContent("posted");
          //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
          //var response = client.PostAsync(baseAddress + "api/values", content).Result;
          var response = client.PostAsJsonAsync(baseAddress + "api/values", "posted").Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, response.StatusCode);
          Assert.AreEqual<string>("No Content", response.ReasonPhrase);

          Assert.IsNotNull(ValuesController.PostRequest);
          //Assert.AreEqual<string>("", ValuesController.PostRequest.ToString());
          Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PostRequest.Content.Headers.ContentType.ToString());
        }
      }
    }
  }
}