using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Collections.Generic;
using Owin;
using System.Text;
using System.IO;

namespace WebAPISpec
{
  public class ValueController : ApiController
  {
    public static Dictionary<string, System.Net.Http.HttpRequestMessage> PostRequest;
    public static List<string> PostRequestValue;

    // POST api/value
    public string Post([FromBody]string value)
    {
      if (PostRequestValue == null)
      {
        PostRequestValue = new List<string>();
      }
      PostRequestValue.Add(value);

      if (PostRequest == null)
      {
        PostRequest = new Dictionary<string, System.Net.Http.HttpRequestMessage>();
      }
      PostRequest[value] = Request;

      return "received " + value;
    }

    // PUT api/value/5
    public void Put(int id, [FromBody]string value)
    {
    }
  }

  public class ValueStartup
  {
    public void Configuration(IAppBuilder appBuilder)
    {
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "ValueApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      appBuilder.UseWebApi(config);
    }
  }

  [TestClass]
  public class ValueSpec
  {
    [TestMethod, TestCategory("Value")]
    public void PostValue()
    {
      string baseAddress = "http://localhost:8000/";
      const string posted_datakey = "posted";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValueStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/value");
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        var payload = System.Text.Encoding.UTF8.GetBytes("\"" + posted_datakey + "\"");
        using (var bodystream = request.GetRequestStream())
        {
          bodystream.Write(payload, 0, payload.Length);
        }
        using (var response = request.GetResponse())
        {
          var http_response = response as System.Net.HttpWebResponse;
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.OK, http_response.StatusCode);
          Assert.AreEqual<string>("OK", http_response.StatusDescription);
          using (var reader = new StreamReader(response.GetResponseStream()))
          {
            string response_payload = reader.ReadToEnd();
            Assert.IsFalse(string.IsNullOrWhiteSpace(response_payload));
            Assert.AreEqual<string>("\"received posted\"", response_payload);
          }

          Assert.IsNotNull(ValueController.PostRequest);
          Assert.IsTrue(ValueController.PostRequest.ContainsKey(posted_datakey));
          Assert.AreEqual<string>("application/json; charset=utf-8", ValueController.PostRequest[posted_datakey].Content.Headers.ContentType.ToString());
          Assert.IsNotNull(ValueController.PostRequestValue);
          Assert.IsTrue(ValueController.PostRequestValue.Contains(posted_datakey));
        }
      }
    }

    /*
TODO Post/Put with response, not just response status
TODO text/plain

[TestMethod]
public void PostValue()
{
  string baseAddress = "http://localhost:8010/";
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
      Assert.IsTrue(ValuesController.PostRequest.ContainsKey("posted"));
      Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PostRequest["posted"].Content.Headers.ContentType.ToString());
      Assert.IsNotNull(ValuesController.PostRequestValue);
      Assert.IsTrue(ValuesController.PostRequestValue.Contains("posted"));
    }
  }
}
*/
  }
}