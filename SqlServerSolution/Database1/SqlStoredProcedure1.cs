using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Concurrent;

/*
Getting Started with CLR Integration
https://msdn.microsoft.com/en-us/library/ms131052(v=sql.110).aspx
*/
//public partial class StoredProcedures
public static class SolSenderClientStoredProcedures
{
  private static readonly ConcurrentDictionary<string,string> CachedConfiguredSchemaAndHostNameOrIPAndPort;

  static SolSenderClientStoredProcedures()
  {
    CachedConfiguredSchemaAndHostNameOrIPAndPort = new ConcurrentDictionary<string, string>();
  }

  [Microsoft.SqlServer.Server.SqlProcedure]
  public static void Reset()
  {
    if (CachedConfiguredSchemaAndHostNameOrIPAndPort == null)
    {
      throw new InvalidOperationException("Internal cache was not properly created.");
    }
    CachedConfiguredSchemaAndHostNameOrIPAndPort.Clear();
  }

  [Microsoft.SqlServer.Server.SqlProcedure]
  public static string SendMessage(string VPNkey, string topicpath, string message, int correlationID)
  {
    #region Parameter validation
    if (string.IsNullOrWhiteSpace(VPNkey))
    {
      throw new ArgumentNullException("VPNkey", "VPN cannot be empty or null.");
    }
    else
    {
      VPNkey = VPNkey.Trim();
    }

    if (string.IsNullOrWhiteSpace(topicpath))
    {
      throw new ArgumentNullException("topicpath", "topicpath cannot be empty or null.");
    }
    else
    {
      topicpath = topicpath.Trim();
    }
    #endregion

    //SqlContext.Pipe.ExecuteAndSend(cmd);?

    string result = "";
    string base_address = GetConfiguredSchemaAndHostNameOrIPAndPort(VPNkey);

    const string posted_datakey = "posted2";
    var request = System.Net.WebRequest.Create(base_address + "api/values") as System.Net.HttpWebRequest;
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
          throw new Exception("no response" + "<correlationid> + <status> + <description>"); //exception flow
        }
        result = "<correlationid> + <status> + <description>"; //normal flow
      }
      return result;
    }
  }

  private static string GetConfiguredSchemaAndHostNameOrIPAndPort(string VPNkey)
  {
    string result = GetCachedConfiguredSchemaAndHostNameOrIPAndPort(VPNkey);
    if (string.IsNullOrWhiteSpace(result))
    {
      string url = QueryForConfiguredSchemaAndHostNameOrIPAndPort(VPNkey);
      SetCachedConfiguredSchemaAndHostNameOrIPAndPort(VPNkey, url);
      result = url;
    }
    return result;
  }

  private static string GetCachedConfiguredSchemaAndHostNameOrIPAndPort(string VPNkey)
  {
    //static reset-able cache | -- each hit | cancelable/unloadable static cache
    string result = null;
    if (CachedConfiguredSchemaAndHostNameOrIPAndPort != null)
    {
      if (!CachedConfiguredSchemaAndHostNameOrIPAndPort.TryGetValue(VPNkey, out result))
      {
        result = null;
      }
    }
    else
    {
      throw new InvalidOperationException("Internal cache was not properly created.");
    }
    return result;
  }
  private static void SetCachedConfiguredSchemaAndHostNameOrIPAndPort(string VPNkey, string url)
  {
    if (CachedConfiguredSchemaAndHostNameOrIPAndPort == null)
    {
      throw new InvalidOperationException("Internal cache was not properly created.");
    }
    if (!CachedConfiguredSchemaAndHostNameOrIPAndPort.TryAdd(VPNkey, url))
    {
      throw new Exception("Target URL cannot be added to internal cache.");
    }
  }

  /// <summary>
  /// Get URL to send requests.  In the exact format: https://hostname:7000/
  /// https://msdn.microsoft.com/es-es/library/ms186755(v=sql.110).aspx
  /// https://msdn.microsoft.com/es-es/library/ms131043(v=sql.110).aspx
  /// </summary>
  /// <param name="VPNkey">Key into the index of registered VPNs at SolSenderService-side.</param>
  /// <returns>Target URL for requests.</returns>
  private static string QueryForConfiguredSchemaAndHostNameOrIPAndPort(string VPNkey)
  {
    if (string.IsNullOrWhiteSpace(VPNkey))
    {
      throw new ArgumentException("VPNkey", "QueryForConfiguredSchemaAndHostNameOrIPAndPort was called with an empty or null argument value.");
    }

    string result = "";
    using (var connection = new SqlConnection("context connection=true"))
    {
      connection.Open();
      var command = new SqlCommand(string.Format("SELECT dbo.DatabaseScalarFunction1('{0}') AS RequestAddress", VPNkey), connection);
      object value = command.ExecuteScalar();
      if (value == null)
      {
        throw new Exception("dbo.DatabaseScalarFunction1 returned an empty or null value.");
      }
      result = value.ToString();
    }
    return result;
  }
}