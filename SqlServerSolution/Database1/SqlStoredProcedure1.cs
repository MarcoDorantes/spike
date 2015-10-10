using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

/*
Getting Started with CLR Integration
https://msdn.microsoft.com/en-us/library/ms131052(v=sql.110).aspx
*/
public partial class StoredProcedures
{
  [Microsoft.SqlServer.Server.SqlProcedure]
  public static void WriteMessageToQueue(string vpn, string queuename, string message, int correlationID)
  {
    //SqlContext.Pipe.ExecuteAndSend(cmd);

    string baseAddress = GetConfiguredSchemaAndHostNameOrIPAndPort(vpn);
    const string posted_datakey = "posted2";
    var request = System.Net.WebRequest.Create(baseAddress + "api/values") as System.Net.HttpWebRequest;
    request.Method = "POST";
    request.ContentType = "application/xml; charset=utf-8";
    var payload = System.Text.Encoding.UTF8.GetBytes("<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">" + posted_datakey + "</string>");
    using (var bodystream = request.GetRequestStream())
    {
      bodystream.Write(payload, 0, payload.Length);
    }
    using (var response = request.GetResponse())
    {
      System.Net.HttpWebResponse http_response = response as System.Net.HttpWebResponse;
      System.Net.HttpStatusCode status = http_response.StatusCode;
      using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
      {
        string response_payload = reader.ReadToEnd();
        if (response_payload.Length <= 0)
        {
          throw new Exception("no response");
        }
      }
    }
  }

  private static string GetConfiguredSchemaAndHostNameOrIPAndPort(string vpn)
  {
    //TODO static cache | each hit | cancelable/unloadable static cache

    string result = "";//https://hostname:7000/

    //result = f(vpn); //https://msdn.microsoft.com/es-es/library/ms186755(v=sql.110).aspx

    return result;
  }

  /*private static string GetBaseAddress(string hostname_or_IP_and_port)
  {
    const string baseAddress_template = "http://{0}/";
    return string.Format(baseAddress_template, hostname_or_IP_and_port);
  }*/
}