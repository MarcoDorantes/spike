namespace HttpSocket;

using System;
using System.Collections.Generic;

public interface ISourceProcessorHost
{
    //IProcessorHost
    void Information(string information);
    void Warning(string details_for_diagnostic);
    void Error(string details_for_diagnostic);
    void Error(Exception exception, string details_for_diagnostic);
    void NotifyState(string state);
    void StartTopicSubscription(string name, string vpnName, string host, string userName, string password, string sourceTopicPath, Action<IDictionary<string, object>> onmessage, string payloadFormat /*= "JSON"*/);
    void SendNotification(string subject, string[] lines, IList<KeyValuePair<string, string>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/, System.Text.Encoding encoding /*= null*/);
    void SendNotification(string subject, string[] lines, IList<KeyValuePair<string, byte[]>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/);
    //ISourceProcessorHost
    void UpdateReceivedCount(uint received_count);
}