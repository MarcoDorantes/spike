using System;

namespace ThinClient
{
  class ThinClientExe
  {
    const string baseAddress = "http://localhost:7000/";

    static void get1()
    {
      // Create HttpCient and make a request to api/time
      var client = new System.Net.Http.HttpClient();

      var response = client.GetAsync(baseAddress + "api/time").Result;

      Console.WriteLine(response);
      Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    }
    static void get2(int id)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var uri = baseAddress + "api/time/" + id;
        var response = client.GetAsync(uri).Result;

        Console.WriteLine(response);
        var json = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(json);
        var got = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
        Console.WriteLine("Thread: {0}\nRequest: {1}\nResponse:{2}", got.Thread, got.Request, got.Response);
      }
    }
    static void post(int id)
    {
      using (var client = new System.Net.Http.HttpClient())
      {
        var timepoint = new ContractLib.Timepoint() { Thread = Environment.CurrentManagedThreadId, Request = id.ToString(), Response = DateTime.Now.ToString("o") };
        string posted = Newtonsoft.Json.JsonConvert.SerializeObject(timepoint);
        var content = new System.Net.Http.StringContent(posted);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        var response = client.PostAsync(baseAddress + "api/time", content).Result;

        Console.WriteLine(response);
        var json = response.Content.ReadAsStringAsync().Result;
        Console.WriteLine(json);
        var got = Newtonsoft.Json.JsonConvert.DeserializeObject<ContractLib.Timepoint>(json);
        Console.WriteLine("Thread: {0}\nRequest: {1}\nResponse:{2}", got.Thread, got.Request, got.Response);
      }
    }

    static void Main(string[] args)
    {
      try
      {
        get1();
        //get2(321);
        //post(456);
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