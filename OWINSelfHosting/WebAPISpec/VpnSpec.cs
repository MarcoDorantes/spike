using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using Owin;
using ContractLib;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Text;

/*
*** Reliable WebAPI ***

POST REQUEST:
	POST api/reliable
	<SendRequest>
		<vpn>registeredVPN</vpn>
		<topic>topic</topic>
		<message>text-message</message>
	</SendRequest>

POST RESPONSE: 204 NoContent

*** Guaranteed WebAPI ***

POST REQUEST:
	POST api/guaranteed
	<SendRequest>
		<vpn>registeredVPN</vpn>
		<destination_type>topic-or-queue</destination_type>
		<destination>topicpath-or-queue</destination>
		<correlationid>integer</correlationid>
		<message>text-message</message>
	</SendRequest>

POST RESPONSE:
	<SendResponse>
		<correlationid>integer</correlationid>
		<status>string</status>
		<description>string</description>
	</SendResponse>

Ejemplos:
A tópico:
	URL	: POST VPN_1/topic
	Payload	: 
		<destination>topicpathj</destination>
		<message>8=FIX ...</message>


A queue:
	URL	: PUT VPN_2/queue
	Payload	: 
		<destination>QUEUE1</destination>
		<message>8=FIX ...</message>


  */

namespace WebAPISpec
{
  public class VpnController : ApiController
  {
    public static SendRequest received_request;
    // POST api/vpn
    public SendResponse Post([FromBody]SendRequest request)
    {
      //async System.Threading.Tasks.Task<
      //var payload = new MemoryStream();
      //System.Threading.Tasks.Task task = Request.Content.CopyToAsync(payload);

      //var line = Encoding.UTF8.GetString(payload.GetBuffer());
      //var line = Request.Content.ReadAsStringAsync().Result;
      //var line = Request.ToString();
      //Request.Content.LoadIntoBufferAsync()

      //System.Diagnostics.Trace.WriteLine("Request Payload:");
      //System.Diagnostics.Trace.WriteLine(string.Format("[{0}]",line));

      received_request = request;
      return new SendResponse() { CorrelationID = request.CorrelationID, Status = 102, Description = request.VPNKey + "|" + request.Topic + "|" + request.Message };
      //return new SendResponse() { CorrelationID = request.CorrelationID, Status = 102, Description = request.Topic };
    }
  }

  public class VpnStartup
  {
    public void Configuration(IAppBuilder appBuilder)
    {
      HttpConfiguration config = new HttpConfiguration();
      config.Routes.MapHttpRoute(
          name: "VpnApi",
          routeTemplate: "api/{controller}/{id}",
          defaults: new { id = RouteParameter.Optional }
      );
      appBuilder.UseWebApi(config);
    }
  }

  [TestClass]
  public class VpnSpec
  {
    [TestMethod, TestCategory("VPN")]
    public void PostVPN()
    {
      string baseAddress = "http://localhost:6000/";
      var request_payload = new SendRequest() { VPNKey = "vpn1", Topic = "A/B", Message = "msg1", CorrelationID = 123 };
      using (Microsoft.Owin.Hosting.WebApp.Start<VpnStartup>(url: baseAddress))
      {
        using (var client = new System.Net.Http.HttpClient())
        {
          var response = client.PostAsXmlAsync(baseAddress + "api/vpn", request_payload).Result;

          Assert.AreEqual<string>("vpn1", VpnController.received_request.VPNKey);
          Assert.AreEqual<string>("A/B", VpnController.received_request.Topic);
          Assert.AreEqual<string>("msg1", VpnController.received_request.Message);
          Assert.AreEqual<uint>(123, VpnController.received_request.CorrelationID );

          Assert.IsTrue(response.IsSuccessStatusCode);
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.OK, response.StatusCode);
          Assert.AreEqual<string>("OK", response.ReasonPhrase);
          string response_xml = response.Content.ReadAsStringAsync().Result;
          Assert.IsTrue(response_xml.Length > 0);
          var response_doc = XDocument.Parse(response_xml);

          XNamespace ns = "http://schemas.datacontract.org/2004/07/ContractLib";

          Assert.IsNotNull(response_doc.Root);
          Assert.IsTrue(response_doc.Root.HasElements);
          Assert.AreEqual<int>(3, response_doc.Root.Elements().Count());
          var correlationID = response_doc.Root.Element(ns + "CorrelationID");
          Assert.IsNotNull(correlationID);
          var status = response_doc.Root.Element(ns + "Status");
          Assert.IsNotNull(status);
          var description = response_doc.Root.Element(ns + "Description");
          Assert.IsNotNull(description);

          Assert.AreEqual<string>("123", correlationID.Value);
          Assert.AreEqual<string>("102", status.Value);
          Assert.AreEqual<string>(string.Format("{0}|{1}|{2}", request_payload.VPNKey, request_payload.Topic, request_payload.Message), description.Value);
        }
      }
    }

    [TestMethod, TestCategory("VPN")]
    public void PostVPNAsXMLWithWebRequest()
    {
      string baseAddress = "http://localhost:6001/";
      var request_payload = new SendRequest() { VPNKey = "vpn2", Topic = "B/C", Message = "msg2", CorrelationID = 456 };
      using (Microsoft.Owin.Hosting.WebApp.Start<VpnStartup>(url: baseAddress))
      {
        var request = System.Net.WebRequest.Create(baseAddress + "api/vpn") as System.Net.HttpWebRequest;
        request.Method = "POST";
        request.ContentType = "application/xml; charset=utf-8";

        string payload_template = @"<SendRequest xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://schemas.datacontract.org/2004/07/ContractLib'>
<VPNKey>{0}</VPNKey>
<Topic>{1}</Topic>
<Message>{2}</Message>
<CorrelationID>{3}</CorrelationID>
</SendRequest>";
        var payload_text = string.Format(payload_template, request_payload.VPNKey, request_payload.Topic, request_payload.Message, request_payload.CorrelationID);

        //var serial = new System.Xml.Serialization.XmlSerializer(typeof(SendRequest));
        //var w = new StringWriter();
        //serial.Serialize(w, request_payload); w.Flush();
        //var payload_text = w.GetStringBuilder().ToString();

        //var serial = new System.Runtime.Serialization.DataContractSerializer(typeof(SendRequest));
        //var w = new StringWriter();
        //var ww = System.Xml.XmlWriter.Create(w);
        //serial.WriteObject(ww, request_payload); w.Flush();
        //var payload_text = w.GetStringBuilder().ToString();

        var payload = System.Text.Encoding.UTF8.GetBytes(payload_text);

        //Assert.AreEqual<string>("", payload_text);
        using (var bodystream = request.GetRequestStream())
        {
          bodystream.Write(payload, 0, payload.Length);
        }
        using (var response = request.GetResponse())
        {
          //Assert.AreEqual<string>("vpn2", VpnController.received_request.VPNKey);
          //Assert.AreEqual<string>("B/C", VpnController.received_request.Topic);
          //Assert.AreEqual<string>("msg2", VpnController.received_request.Message);
          //Assert.AreEqual<uint>(456, VpnController.received_request.CorrelationID);

          var http_response = response as System.Net.HttpWebResponse;
          Assert.AreEqual<System.Net.HttpStatusCode>(System.Net.HttpStatusCode.OK, http_response.StatusCode);
          Assert.AreEqual<string>("OK", http_response.StatusDescription);
          using (var response_stream = response.GetResponseStream())
          using (var reader = new StreamReader(response_stream))
          {
            string response_xml = reader.ReadToEnd();
            //Assert.AreEqual<string>("", response_xml);
            var response_doc = XDocument.Parse(response_xml);

            XNamespace ns = "http://schemas.datacontract.org/2004/07/ContractLib";

            Assert.IsNotNull(response_doc.Root);
            Assert.IsTrue(response_doc.Root.HasElements);
            Assert.AreEqual<int>(3, response_doc.Root.Elements().Count());
            var correlationID = response_doc.Root.Element(ns + "CorrelationID");
            Assert.IsNotNull(correlationID);
            var status = response_doc.Root.Element(ns + "Status");
            Assert.IsNotNull(status);
            var description = response_doc.Root.Element(ns + "Description");
            Assert.IsNotNull(description);

            //Assert.AreEqual<string>(request_payload.CorrelationID.ToString(), correlationID.Value);
            //Assert.AreEqual<string>("102", status.Value);
            //Assert.AreEqual<string>(request_payload.VPNKey, description.Value);
            //Assert.AreEqual<string>(request_payload.Topic, description.Value);
            //Assert.IsTrue(description.Value.StartsWith("--"));
            //Assert.AreEqual<string>(string.Format("{0}|{1}|{2}", request_payload.VPNKey, request_payload.Topic, request_payload.Message), description.Value);
          }
        }
      }
    }
  }
}