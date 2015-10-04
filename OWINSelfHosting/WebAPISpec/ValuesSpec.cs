using System;
using System.Linq;
using System.Collections.Generic;
using Owin;
using System.Web.Http;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebAPISpec
{
  public class ValuesController : ApiController
  {
    public static System.Net.Http.HttpRequestMessage GetRequest;//Single because there is just one GET test.
    public static System.Net.Http.HttpRequestMessage GetXRequest;//Single because there is just one GET X test.
    public static Dictionary<string,System.Net.Http.HttpRequestMessage> PostRequest;
    public static List<string> PostRequestValue;
    public static Dictionary<string, System.Net.Http.HttpRequestMessage> PutRequest;
    public static List<KeyValuePair<int, string>> PutRequestValue;
    public static Dictionary<int, System.Net.Http.HttpRequestMessage> DeleteRequest;
    public static List<int> DeleteRequestValue;

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
      if (PostRequestValue == null)
      {
        PostRequestValue = new List<string>();
      }
      PostRequestValue.Add(value);

      if (PostRequest == null)
      {
        PostRequest = new Dictionary<string, HttpRequestMessage>();
      }
      PostRequest[value] = Request;
    }

    // PUT api/values/5
    public void Put(int id, [FromBody]string value)
    {
      if (PutRequestValue == null)
      {
        PutRequestValue = new List<KeyValuePair<int, string>>();
      }
      PutRequestValue.Add(new KeyValuePair<int, string>(id, value));

      if (PutRequest == null)
      {
        PutRequest = new Dictionary<string, HttpRequestMessage>();
      }
      PutRequest[value] = Request;
    }

    // DELETE api/values/5
    public void Delete(int id)
    {
      if (DeleteRequestValue == null)
      {
        DeleteRequestValue = new List<int>();
      }
      DeleteRequestValue.Add(id);

      if (DeleteRequest == null)
      {
        DeleteRequest = new Dictionary<int, HttpRequestMessage>();
      }
      DeleteRequest[id] = Request;
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
      const string posted_datakey = "posted";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.PostAsJsonAsync(baseAddress + "api/values", posted_datakey).Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, response.StatusCode);
          Assert.AreEqual<string>("No Content", response.ReasonPhrase);

          Assert.IsNotNull(ValuesController.PostRequest);
          Assert.IsTrue(ValuesController.PostRequest.ContainsKey(posted_datakey));
          Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PostRequest[posted_datakey].Content.Headers.ContentType.ToString());
          Assert.IsNotNull(ValuesController.PostRequestValue);
          Assert.IsTrue(ValuesController.PostRequestValue.Contains(posted_datakey));
        }
      }
    }

    [TestMethod]
    public void PostValueWithWebRequest()
    {
      string baseAddress = "http://localhost:9003/";
      const string posted_datakey = "posted2";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/values");
        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        var payload = System.Text.Encoding.UTF8.GetBytes("\""+ posted_datakey + "\"");
        using (var bodystream = request.GetRequestStream())
        {
          bodystream.Write(payload, 0, payload.Length);
        }
        using (var response = request.GetResponse())
        {
          var http_response = response as System.Net.HttpWebResponse;
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, http_response.StatusCode);
          Assert.AreEqual<string>("No Content", http_response.StatusDescription);
          using (var response_stream = response.GetResponseStream())
          {
            var buffer = new byte[1];
            Assert.AreEqual<int>(0, response_stream.Read(buffer, 0, 1));
          }

          Assert.IsNotNull(ValuesController.PostRequest);
          Assert.IsTrue(ValuesController.PostRequest.ContainsKey(posted_datakey));
          Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PostRequest[posted_datakey].Content.Headers.ContentType.ToString());
          Assert.IsNotNull(ValuesController.PostRequestValue);
          Assert.IsTrue(ValuesController.PostRequestValue.Contains(posted_datakey));
        }
      }
    }

    [TestMethod]
    public void PutValue()
    {
      string baseAddress = "http://localhost:9004/";
      const string posted_datakey = "posted3";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.PutAsJsonAsync(baseAddress + "api/values/3", posted_datakey).Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, response.StatusCode);
          Assert.AreEqual<string>("No Content", response.ReasonPhrase);

          Assert.IsNotNull(ValuesController.PutRequest);
          Assert.IsTrue(ValuesController.PutRequest.ContainsKey(posted_datakey));
          Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PutRequest[posted_datakey].Content.Headers.ContentType.ToString());
          Assert.IsNotNull(ValuesController.PutRequestValue);
          Assert.AreEqual<int>(1, ValuesController.PutRequestValue.Count(v => v.Value == posted_datakey));
          Assert.AreEqual<char>(ValuesController.PutRequest[posted_datakey].RequestUri.OriginalString.Last(), ValuesController.PutRequestValue.First(v => v.Value == posted_datakey).Key.ToString().First());
        }
      }
    }

    [TestMethod]
    public void PutValueWithWebRequest()
    {
      string baseAddress = "http://localhost:9005/";
      const string posted_datakey = "posted4";
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/values/4");
        request.Method = "PUT";
        request.ContentType = "application/json; charset=utf-8";
        var payload = System.Text.Encoding.UTF8.GetBytes("\"" + posted_datakey + "\"");
        using (var bodystream = request.GetRequestStream())
        {
          bodystream.Write(payload, 0, payload.Length);
        }
        using (var response = request.GetResponse())
        {
          var http_response = response as System.Net.HttpWebResponse;
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, http_response.StatusCode);
          Assert.AreEqual<string>("No Content", http_response.StatusDescription);
          using (var response_stream = response.GetResponseStream())
          {
            var buffer = new byte[1];
            Assert.AreEqual<int>(0, response_stream.Read(buffer, 0, 1));
          }

          Assert.IsNotNull(ValuesController.PutRequest);
          Assert.IsTrue(ValuesController.PutRequest.ContainsKey(posted_datakey));
          Assert.AreEqual<string>("application/json; charset=utf-8", ValuesController.PutRequest[posted_datakey].Content.Headers.ContentType.ToString());
          Assert.IsNotNull(ValuesController.PutRequestValue);
          Assert.AreEqual<int>(1, ValuesController.PutRequestValue.Count(v => v.Value == posted_datakey));
          Assert.AreEqual<char>(ValuesController.PutRequest[posted_datakey].RequestUri.OriginalString.Last(), ValuesController.PutRequestValue.First(v => v.Value == posted_datakey).Key.ToString().First());
        }
      }
    }

    [TestMethod]
    public void DeleteValue()
    {
      string baseAddress = "http://localhost:9006/";
      const int posted_datakey = 5;
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.DeleteAsync(baseAddress + "api/values/5").Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, response.StatusCode);
          Assert.AreEqual<string>("No Content", response.ReasonPhrase);

          Assert.IsNotNull(ValuesController.DeleteRequest);
          Assert.IsTrue(ValuesController.DeleteRequest.ContainsKey(posted_datakey));
          Assert.IsNotNull(ValuesController.DeleteRequestValue);
          Assert.AreEqual<int>(1, ValuesController.DeleteRequestValue.Count(v => v == posted_datakey));
          Assert.AreEqual<char>(ValuesController.DeleteRequest[posted_datakey].RequestUri.OriginalString.Last(), ValuesController.DeleteRequestValue.First(v => v == posted_datakey).ToString().First());
        }
      }
    }

    [TestMethod]
    public void DeleteValueWithWebRequest()
    {
      string baseAddress = "http://localhost:9007/";
      const int posted_datakey = 6;
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/values/6");
        request.Method = "DELETE";
        using (var response = request.GetResponse())
        {
          var http_response = response as System.Net.HttpWebResponse;
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.NoContent, http_response.StatusCode);
          Assert.AreEqual<string>("No Content", http_response.StatusDescription);
          using (var response_stream = response.GetResponseStream())
          {
            var buffer = new byte[1];
            Assert.AreEqual<int>(0, response_stream.Read(buffer, 0, 1));
          }

          Assert.IsNotNull(ValuesController.DeleteRequest);
          Assert.IsTrue(ValuesController.DeleteRequest.ContainsKey(posted_datakey));
          Assert.IsNotNull(ValuesController.DeleteRequestValue);
          Assert.AreEqual<int>(1, ValuesController.DeleteRequestValue.Count(v => v == posted_datakey));
          Assert.AreEqual<char>(ValuesController.DeleteRequest[posted_datakey].RequestUri.OriginalString.Last(), ValuesController.DeleteRequestValue.First(v => v == posted_datakey).ToString().First());
        }
      }
    }

    /*
    TODO Post/Put with response, not just response status
    TODO text/plain

    [TestMethod]
    public void PostValue()
    {
      string baseAddress = "http://localhost:9010/";
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