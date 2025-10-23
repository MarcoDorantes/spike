using System.IO;
using System.Linq;
using System.Collections.Generic;

using static System.Console;

static class MessageProvider
{
    public static IEnumerable<string> GetResponseMessages(string[] args, string message)
    {
        List<string> result=[];
        var chat = args?.Any(x=>x=="chat");
        if(chat == false && message?.Contains("action") == true)
        {
            FileInfo payloads = new("payload.log");
            if(payloads.Exists) result.AddRange(System.IO.File.ReadAllLines(payloads.FullName));
            else WriteLine($"Payloads NOT FOUND ({payloads.FullName}).");
        }
        else
        {
            var responseMessage = $"Server echo: {message}";
            result.Add(responseMessage);
        }
        return result;
    }
}