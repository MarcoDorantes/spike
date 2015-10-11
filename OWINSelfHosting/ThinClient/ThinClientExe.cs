using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Xml.Linq;

namespace ThinClient
{
  class ThinClientExe
  {
    //http://MX-CB-MDORANTES:7000/api/time
    static void get1()
    {
      var client = new System.Net.Http.HttpClient();

      var baseAddress = GetBaseAddress();
      var response = client.GetAsync(baseAddress + "api/time").Result;

      Console.WriteLine(response);
      Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    }
    static void get2(int id)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var baseAddress = GetBaseAddress();
        var uri = baseAddress + "api/time/" + id;

        var response = client.GetAsync(uri).Result;

        Console.WriteLine(response);
        var json = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(json);
        var got = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
        Console.WriteLine("Thread: {0}\nRequest: {1}\nResponse:{2}", got.Thread, got.Request, got.Response);
      }
    }
    static void postJSON(int id)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var timepoint = new ContractLib.Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = id.ToString(), Response = DateTime.Now.ToString("o") };
        string posted = Newtonsoft.Json.JsonConvert.SerializeObject(timepoint);
        var content = new System.Net.Http.StringContent(posted);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var baseAddress = GetBaseAddress();
        var response = client.PostAsync(baseAddress + "api/time", content).Result;

        Console.WriteLine(response);
        var json = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(json);
        var got = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
        Console.WriteLine("Thread: {0}\nRequest: {1}\nResponse:{2}", got.Thread, got.Request, got.Response);
      }
    }
    static void postXML(int id)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var timepoint = new ContractLib.Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = id.ToString(), Response = DateTime.Now.ToString("o") };
        var baseAddress = GetBaseAddress();
        var url = baseAddress + "api/time";
        Console.WriteLine(url);
        var response = client.PostAsXmlAsync(url, timepoint).Result;

        Console.WriteLine(response);
        var xml = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(xml);
        //var got = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
        //Console.WriteLine("Thread: {0}\nRequest: {1}\nResponse:{2}", got.Thread, got.Request, got.Response);
      }
    }

    static void postVPN()
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var request_payload = new ContractLib.SendRequest() { VPNKey = "vpn11", Topic = "B/C", Message = "msg11", CorrelationID = 456 };
        var baseAddress = GetBaseAddress();
        var url = baseAddress + "api/vpn";
        Console.WriteLine(url);
        var response = client.PostAsXmlAsync(url, request_payload).Result;

        Console.WriteLine(response);
        var xml = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(xml);
      }
    }

    static void PostVPNAsXMLWithWebRequest()
    {
      var request_payload = new ContractLib.SendRequest() { VPNKey = "-vpn2", Topic = "B/C", Message = "msg2", CorrelationID = 456 };
      var baseAddress = GetBaseAddress();
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
      var payload = System.Text.Encoding.UTF8.GetBytes(payload_text);

      //Assert.AreEqual<string>("", payload_text);
      using (var bodystream = request.GetRequestStream())
      {
        bodystream.Write(payload, 0, payload.Length);
      }
      using (var response = request.GetResponse())
      {
        var http_response = response as System.Net.HttpWebResponse;
        Console.WriteLine("{0} {1}", http_response.StatusCode, http_response.StatusDescription);
        using (var response_stream = response.GetResponseStream())
        using (var reader = new StreamReader(response_stream))
        {
          string response_xml = reader.ReadToEnd();
          //Assert.AreEqual<string>("", response_xml);
          var response_doc = XDocument.Parse(response_xml);
          XNamespace ns = "http://schemas.datacontract.org/2004/07/ContractLib";

          //Assert.IsNotNull(response_doc.Root);
          //Assert.IsTrue(response_doc.Root.HasElements);
          //Assert.AreEqual<int>(3, response_doc.Root.Elements().Count());
          var correlationID = response_doc.Root.Element(ns + "CorrelationID");
          //Assert.IsNotNull(correlationID);
          var status = response_doc.Root.Element(ns + "Status");
          //Assert.IsNotNull(status);
          var description = response_doc.Root.Element(ns + "Description");
          //Assert.IsNotNull(description);
          Console.WriteLine("CorrelationID:{0}\nStatus:{1}\nDescription:{2}", correlationID.Value, status.Value, description.Value);
        }
      }
    }

    static ContractLib.Timepoint get_time(System.Net.Http.HttpClient client, string uri, int id)
    {
        var response = client.GetAsync(uri).Result;
        var json = response.Content.ReadAsStringAsync().Result;
        return Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
    }
    static IEnumerable<ContractLib.Timepoint> get_times(int from, int to)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var baseAddress = GetBaseAddress();
        for (int k = from; k < to; ++k)
        {
          var uri = baseAddress + "api/time/" + k;
          yield return get_time(client, uri, k);
        }
      }
    }
    static void get_many(int start, int size)
    {
      for (int k = 0; k < 3; ++k)
      {
        int from = k * size + 1;
        int to = from + size;
        foreach (var t in get_times(from + start - 1, to + start - 1))
        {
          Console.WriteLine("[{0,3}] {1,3}\t{2}", t.Thread, t.Request, t.Response);
        }
      }
    }

    #region format
    //http://www.asp.net/web-api/overview/formats-and-model-binding/json-and-xml-serialization
    [System.Runtime.Serialization.DataContract]
    public class Person {[System.Runtime.Serialization.DataMember]public string Name { get; set; }[System.Runtime.Serialization.DataMember]public int Age { get; set; } }
    static string Serialize<T>(System.Net.Http.Formatting.MediaTypeFormatter formatter, T value)
    {
      // Create a dummy HTTP Content.
      System.IO.Stream stream = new System.IO.MemoryStream();
      var content = new StreamContent(stream);
      /// Serialize the object.
      formatter.WriteToStreamAsync(typeof(T), value, stream, content, null).Wait();
      // Read the serialized string.
      stream.Position = 0;
      return content.ReadAsStringAsync().Result;
    }

    static T Deserialize<T>(System.Net.Http.Formatting.MediaTypeFormatter formatter, string str) where T : class
    {
      // Write the serialized string to a memory stream.
      System.IO.Stream stream = new System.IO.MemoryStream();
      var writer = new System.IO.StreamWriter(stream);
      writer.Write(str);
      writer.Flush();
      stream.Position = 0;
      // Deserialize to an object of type T
      return formatter.ReadFromStreamAsync(typeof(T), stream, null, null).Result as T;
    }

    // Example of use
    static void TestSerialization()
    {
      var value = new Person() { Name = "Alice", Age = 23 };

      var xmlformatter = new System.Net.Http.Formatting.XmlMediaTypeFormatter();
      string xml = Serialize(xmlformatter, value);
      Console.WriteLine("xml:{0}", xml);

      var jsonformatter = new System.Net.Http.Formatting.JsonMediaTypeFormatter();
      string json = Serialize(jsonformatter, value);
      Console.WriteLine("json:{0}", json);

      // Round trip
      Person person3 = Deserialize<Person>(xmlformatter, xml);
      Console.WriteLine("object from xml:{0}", person3 != null ? person3.Name : "(null)");
      Person person2 = Deserialize<Person>(jsonformatter, json);
      Console.WriteLine("object from json:{0}", person2 != null ? person2.Name : "(null)");

      var single_value = "aval1";
      xml = Serialize(xmlformatter, single_value);
      Console.WriteLine("\na value xml:{0}", xml);

      json = Serialize(jsonformatter, single_value);
      Console.WriteLine("a value json:{0}", json);

      string avalue_back = Deserialize<string>(xmlformatter, xml);
      Console.WriteLine("value from xml: {0}",avalue_back);
      string avalue_back2 = Deserialize<string>(jsonformatter, json);
      Console.WriteLine("value from json: {0}", avalue_back2);
    }
    static void format()
    {
      TestSerialization();
    }
    #endregion

    static string hostname;
    static string GetBaseAddress()
    {
      const string baseAddress_template = "http://{0}:7000/";
      return string.Format(baseAddress_template, hostname);
    }
    static void Main(string[] args)
    {
      try
      {
        hostname =
        null;
        //"otherhost";
        if (string.IsNullOrWhiteSpace(hostname))
        {
          hostname = Environment.MachineName;
        }
        //get1();
        //get2(321);
        //postJSON(456);
        //postXML(789);
        //format();
        postVPN();
        //PostVPNAsXMLWithWebRequest();
        //get_many(int.Parse(args[0]), int.Parse(args[1]));
      }
      catch (Exception ex)
      {
        while (ex != null)
        {
          Console.WriteLine("{0}: {1}", ex.GetType().FullName, ex.Message);
          ex = ex.InnerException;
        }
        /*
        System.AggregateException: One or more errors occurred.
        System.Net.Http.HttpRequestException: An error occurred while sending the request.
        System.Net.WebException: Unable to connect to the remote server
        System.Net.Sockets.SocketException: No connection could be made because the target machine actively refused it 192.168.1.72:7000
        */
      }
    }
  }
}