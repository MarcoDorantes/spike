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
}