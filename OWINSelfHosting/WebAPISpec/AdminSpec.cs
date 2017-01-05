using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WebAPISpec
{
  public class AdminController : ApiController
  {
    // POST api/vpn
    public ContractLib.AdminResponse Post([FromBody]ContractLib.AdminRequest request)
    {
      return new ContractLib.AdminResponse { services = new string[] { "uno", "dos" } };
    }
  }
  public class AdminStartup
  {
    public void Configuration(IAppBuilder appBuilder)
    {
      var config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "DefaultApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      appBuilder.UseWebApi(config);
    }
  }

  [TestClass]
  public class AdminSpec
  {
    [TestMethod]
    public void services_OWIN_both_sides()
    {
      string baseAddress = "http://localhost:5000/";
      var request_payload = new ContractLib.AdminRequest() { cmd = "services" };
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.PostAsJsonAsync(baseAddress + "api/admin", request_payload).Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.OK, response.StatusCode);
          Assert.AreEqual<string>("OK", response.ReasonPhrase);
          string response_json = response.Content.ReadAsStringAsync().Result;
          var admin_response = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.AdminResponse>(response_json);
          Assert.IsNotNull(admin_response);
          Assert.IsNotNull(admin_response.services);
          Assert.AreEqual<int>(2, admin_response.services.Count());
          Assert.AreEqual<string>("uno", admin_response.services.ElementAt(0));
          Assert.AreEqual<string>("dos", admin_response.services.ElementAt(1));
        }
      }
    }
    [TestMethod]
    public void services_OWIN_service_side()
    {
      string baseAddress = "http://localhost:5001/";
      var request = new ContractLib.AdminRequest() { cmd = "services" };
      using (Microsoft.Owin.Hosting.WebApp.Start<ValuesStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          string url = baseAddress + "api/admin";
          var request_payload = Newtonsoft.Json.JsonConvert.SerializeObject(request);
          var payload = new System.Net.Http.StringContent(request_payload, Encoding.UTF8, "application/json");
          var response = client.PostAsync(url, payload).Result;

          //var response = client.PostAsJsonAsync(baseAddress + "api/admin", request_payload).Result;

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.OK, response.StatusCode);
          Assert.AreEqual<string>("OK", response.ReasonPhrase);
          string response_json = response.Content.ReadAsStringAsync().Result;
          var admin_response = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.AdminResponse>(response_json);
          Assert.IsNotNull(admin_response);
          Assert.IsNotNull(admin_response.services);
          Assert.AreEqual<int>(2, admin_response.services.Count());
          Assert.AreEqual<string>("uno", admin_response.services.ElementAt(0));
          Assert.AreEqual<string>("dos", admin_response.services.ElementAt(1));
        }
      }
    }
  }
}