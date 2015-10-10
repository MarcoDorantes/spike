using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Collections.Generic;
using Owin;
using System.IO;

namespace WebAPISpec
{
  public class FaultController : ApiController
  {
    // GET api/fault
    public IEnumerable<string> Get()
    {
      throw new Exception("top",new Exception("inner"));
    }

    // POST api/fault
    public void Post([FromBody]string value)
    {
      throw new Exception("top", new Exception("inner"));
    }
  }
  public class FaultStartup
  {
    public void Configuration(IAppBuilder appBuilder)
    {
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "FaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      appBuilder.UseWebApi(config);
    }
  }

  [TestClass]
  public class FaultSpec //http://www.asp.net/web-api/overview/error-handling/exception-handling
  {
    [TestMethod]
    public void PostFaultAsXML()
    {
      string baseAddress = "http://localhost:7000/";
      const string posted_datakey = "posted";
      using (Microsoft.Owin.Hosting.WebApp.Start<FaultStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/fault") as System.Net.HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/xml; charset=utf-8";
        var payload = System.Text.Encoding.UTF8.GetBytes("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + posted_datakey + "</string>");
        using (var bodystream = request.GetRequestStream())
        {
          bodystream.Write(payload, 0, payload.Length);
        }
        try
        {
          request.GetResponse();//https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.getresponse(v=vs.100).aspx
        }
        catch (System.Net.WebException ex)
        {
          using (ex.Response)
          using (var reader = new StreamReader(ex.Response.GetResponseStream()))
          {
            Assert.AreEqual<string>("", reader.ReadToEnd());
          }
        }
        catch (Exception ex)
        {
          Assert.Fail($"{ex.GetType().FullName}: {ex.Message}");
        }
      }
    }
  }
}