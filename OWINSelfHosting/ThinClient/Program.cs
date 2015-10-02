using System;

namespace ThinClient
{
  class Program
  {
    const string baseAddress = "http://localhost:7000/";

    static void get()
    {
      // Create HttpCient and make a request to api/time
      var client = new System.Net.Http.HttpClient();

      var response = client.GetAsync(baseAddress + "api/time").Result;

      Console.WriteLine(response);
      Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    }
    static void send()
    {
      // Create HttpCient and make a request to api/time
      var client = new System.Net.Http.HttpClient();

      //var response = client.PostAsync(baseAddress + "api/time").Result;

      //Console.WriteLine(response);
      //Console.WriteLine(response.Content.ReadAsStringAsync().Result);
    }

    static void Main(string[] args)
    {
      try
      {
        //get();
        send();
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