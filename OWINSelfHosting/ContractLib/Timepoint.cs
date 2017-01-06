using System.Collections.Generic;

namespace ContractLib
{
  public class Timepoint
  {
    public int Thread;
    public string Request;
    public string Response;
  }

  public class SendRequest
  {
    public string VPNKey;
    public string Topic;
    public string Message;
    public uint CorrelationID;
  }
  public class SendResponse
  {
    public uint CorrelationID;
    public byte Status;
    public string Description;
  }

  public class AdminRequest
  {
    public string cmd;
  }
//  public class ServiceList : List<string> { }
  public class AdminResponse
  {
    public string[] services;
  }

  public class AdminRequest2
  {
    public string Name;
    public Dictionary<string, string> Values;
  }
  public class AdminResponse2
  {
    public string Name;
    public Dictionary<string, string> Values;
  }

}