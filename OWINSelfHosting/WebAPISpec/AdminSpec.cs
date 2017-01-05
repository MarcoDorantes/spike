using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Owin;
using System.Web.Http;
using System.Net.Http;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebAPISpec
{
  public class AdminController : ApiController
  {
    // POST api/vpn
    public ContractLib.AdminResponse Post([FromBody]ContractLib.AdminRequest request)
    {
      return new ContractLib.AdminResponse { services = new string[] { "uno", "dos" } };
      //var result = new ContractLib.AdminResponse();
      //result.services = new ContractLib.ServiceList();
      //result.services.AddRange(new string[] { "uno", "dos" });
      //return result;
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
    public void services()
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
        }
      }
    }
  }
}