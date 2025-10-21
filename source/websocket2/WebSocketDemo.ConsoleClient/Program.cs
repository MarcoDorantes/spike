namespace WebSocketDemo.ConsoleClient;

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;

using static System.Console;
/*
orphan connection?
    HA: server db to track clients to restore connections
Each network touch should have retry/reconnect policy.
    no state change, at all.
    ping? verb support| ACK
evidence of contact to server or client
unplug cable

https://www.unixtimestamp.com
https://www.svix.com/resources/faq/websocket-vs-tcp

Are the websockets connections are as reliable as TCP sockets connections?

Summary
WebSockets use TCP under the hood, so they inherit TCP’s reliability guarantees (ordered, retransmitted, error-checked delivery). Differences in real-world reliability come from layers around TCP: framing, browser/runtime limits, intermediaries (proxies, load balancers), timeouts, and application-level heartbeats and reconnection logic.

How they compare technically
Transport layer WebSocket runs on top of TCP, so the core transport reliability (in-order delivery, retransmission on loss, checksum) is the same as a raw TCP socket.

Message model WebSocket provides a message/frame abstraction; raw TCP is a byte stream. Message framing means you get messages rather than a continuous byte stream, which changes how you detect and recover from partial data or framing errors.

Connection lifecycle WebSocket is established via an HTTP(S) handshake and subject to HTTP infrastructure behavior (upgrades, headers, timeouts); raw TCP connections don’t require an HTTP handshake and are less likely to be interrupted by HTTP-aware intermediaries.

Practical differences that affect reliability
Intermediaries and proxies HTTP proxies, corporate firewalls, and some load balancers may terminate or silently drop long-lived HTTP/WebSocket connections more aggressively than plain TCP connections between known endpoints.

Idle timeouts Many HTTP stacks and proxies enforce shorter idle timeouts for WebSocket connections; you typically need periodic pings (heartbeats) to keep the connection alive.

Browser and platform limits Browsers impose limits (concurrent connections per origin, resource constraints) and may suspend background tabs; native TCP sockets in server-to-server apps aren’t subject to those exact constraints.

TLS and handshake overhead WSS (WebSocket over TLS) and HTTPS upgrade handshakes add overhead and dependency on TLS session behavior; this can affect perceived availability but not TCP’s underlying delivery guarantees.

Reconnection and app-level handling WebSocket applications commonly rely on client-side reconnection, message buffering, and deduplication to handle interruptions; raw TCP apps often implement similar logic but have more control over lower-level socket options.

When to prefer one over the other
Use WebSockets when

You need real-time browser↔server bidirectional messaging.

You want message framing and a standardized API in browsers.

You accept HTTP-based deployment and can implement heartbeats and reconnection.

Use raw TCP when

You control both endpoints (server↔server or native client) and want maximal control over socket options, keepalive behavior, and lower overhead.

You need to avoid HTTP proxies or browser-imposed limits.

Recommendations to make WebSockets as reliable as possible
Implement application-level heartbeats/pings to prevent idle timeouts.

Add automatic reconnection with exponential backoff and resume or deduplicate messages.

Use TLS (WSS) and consider TLS session resumption to reduce handshake cost.

Monitor connection drops and intermediate devices (reverse proxies, load balancers) and tune their timeouts or use WebSocket-aware proxies.

Design messages to be idempotent or include sequence IDs so you can safely retry or resume.

Quick verdict
Transport reliability (packet delivery and ordering) is effectively the same because WebSocket uses TCP. Real-world reliability differences arise from the HTTP/handshake layer, intermediaries, client environment, and application-level handling. With proper heartbeats, reconnection, and infrastructure tuning, WebSockets can be made as reliable for production use as raw TCP connections for the typical browser-based real-time use cases.
-
Differences in Connection Management
While the data transport is equally reliable, the management of the connection can introduce differences in perceived reliability at the application level:

Message Orientation:

TCP Sockets offer a stream of bytes. Your application has to implement its own framing (message boundaries) to know where one message ends and the next begins. This adds complexity to the application layer.


WebSockets provide a stream of messages. The protocol handles the framing for you, ensuring that an entire, complete message is delivered to the application in a single event, which makes them easier to work with for higher-level applications.


Automatic Recovery: Neither a raw TCP socket nor a standard WebSocket connection automatically handles a broken connection (e.g., if a Wi-Fi connection is dropped).

If the underlying network connection fails, both will eventually time out and close.

You must implement application-level reconnection logic (e.g., using libraries like Socket.IO or custom code) for both to make them truly resilient in real-world scenarios.
*/

internal static partial class Config
{
    public static string Address;
    public static string InitialMessage;
}
class Input { public string[] Topics { get; set; } }
class ConsoleHost(System.IO.TextWriter Writer) : HttpSocket.ISourceProcessorHost
{
    private HttpSocket.HttpSocketClient.UpdateReceivedOnType updateReceivedOn;
    public HttpSocket.HttpSocketClient.UpdateReceivedOnType UpdateReceivedOn
    {
        get => updateReceivedOn;
        set {updateReceivedOn=value; UpdateReceivedCountLabel = " " + (value == HttpSocket.HttpSocketClient.UpdateReceivedOnType.Message ? "app-level-msgs" : "payloads");}
    }
    void HttpSocket.ISourceProcessorHost.Information(string information) => Writer.WriteLine(information);
    void HttpSocket.ISourceProcessorHost.Warning(string details_for_diagnostic) => Writer.WriteLine(details_for_diagnostic);
    void HttpSocket.ISourceProcessorHost.Error(string details_for_diagnostic) => Writer.WriteLine(details_for_diagnostic);
    void HttpSocket.ISourceProcessorHost.Error(Exception exception, string details_for_diagnostic) => Writer.WriteLine($"{exception.GetType().FullName}: {exception.Message} ({details_for_diagnostic})");
    void HttpSocket.ISourceProcessorHost.NotifyState(string state) => Writer.WriteLine(state);
    void HttpSocket.ISourceProcessorHost.StartTopicSubscription(string name, string vpnName, string host, string userName, string password, string sourceTopicPath, Action<IDictionary<string, object>> onmessage, string payloadFormat /*= "JSON"*/) => throw new NotImplementedException();
    void HttpSocket.ISourceProcessorHost.SendNotification(string subject, string[] lines, IList<KeyValuePair<string, string>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/, System.Text.Encoding encoding /*= null*/) => throw new NotImplementedException();
    void HttpSocket.ISourceProcessorHost.SendNotification(string subject, string[] lines, IList<KeyValuePair<string, byte[]>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/) => throw new NotImplementedException();
    void HttpSocket.ISourceProcessorHost.UpdateReceivedCount(uint received_count) => Writer.WriteLine($"{nameof(HttpSocket.ISourceProcessorHost.UpdateReceivedCount)}: {received_count}{UpdateReceivedCountLabel}");
    internal string UpdateReceivedCountLabel;
}
class Program
{
    static ulong received_onnext_count;
    static async Task Main(string[] args)
    {
        nutility.Switch opts = new(args);
        if (opts.Is("client")) { LaunchClients(opts); }
        else
        {
            var batch = opts.Is("batch");
            if (!batch) WriteLine("Console WebSocket Client");
            await ConnectToServerAsync(opts, batch);
        }
    }
    static void LaunchClients(nutility.Switch opts)
    {
        received_onnext_count = 0UL;
        int nclients = opts.Is("count") ? int.Parse(opts["count"]) : 3;
        if (!(nclients > 0)) return;
        using CancellationTokenSource cancel = new();
        List<HttpSocket.HttpSocketClient> clients = [];
        string[] topics = [Config.DefaultTopic, "FMV.AAPL", "FMV.ORCL"];
        if (opts.Is("topics"))
        {
            var input = nutility.Switch.AsType<Input>(opts);
            if (input?.Topics?.Length > 0) topics = input.Topics;
        }
        try
        {
            _ = Enumerable.Range(0, nclients).Aggregate(clients, (whole, next) =>
            {
                var id = whole.Count;
                var topic = topics[id % topics.Length];
                whole.Add(LaunchClient($"{id}", cancel.Token, topic, opts));
                return whole;
            });
            ReadLine();
            cancel.Cancel();
            clients.ForEach(c => c.Stop());
            clients.ForEach(c => c.Dispose());
            Thread.Sleep(3_000);
        }
        finally
        {
            clients.ForEach(c => c.Dispose());
        }
    }
    static HttpSocket.HttpSocketClient LaunchClient(string id, CancellationToken cancel, string topic, nutility.Switch opts)
    {
        //Add all these to the AppSettingsKey? No: the host must arrange these from hosting AppSettings environment.
        Dictionary<string, object> config = new()
        {
            {nameof(Config.Address),Config.Address}
            ,{nameof(Config.InitialMessage),Config.InitialMessage}
            ,{"Topic",topic}
            ,{"SubscribePayload",Config.GetSubscribePayload(topic)}
           //,{HttpSocket.HttpSocketClient.UpdateReceivedOnKey,HttpSocket.HttpSocketClient.UpdateReceivedOnType.Payload}
        };
        if (opts.Is(HttpSocket.HttpSocketClient.CheckStateDelayConfigKey)) config[HttpSocket.HttpSocketClient.CheckStateDelayConfigKey] = opts[HttpSocket.HttpSocketClient.CheckStateDelayConfigKey];
        ConsoleHost host = new(Out);
        HttpSocket.HttpSocketClient client = new()
        {
            ID = id,
            SourceHost = host,
            Configuration = config,
            Cancellation = cancel,
            OnNext = OnNext
        };
        client.Setup();
        host.UpdateReceivedOn = client.UpdateReceivedOn;
        client.Start();
        return client;
    }
    static void OnNext(IDictionary<string, object> message) //? => foreach(app in Parsed-array-in-message) Observer?.OnNext(app);
    {
        ++received_onnext_count;
        var payload = message[HttpSocket.HttpSocketClient.MessagePayloadKey] as byte[];
        var keys = $"{string.Join('|', message.Where(k=>k.Key!=HttpSocket.HttpSocketClient.MessagePayloadKey).Select(p => $"{p.Key}={p.Value}"))}";
        WriteLine($"Msg#{received_onnext_count} ({payload.GetType().Name}):{Encoding.UTF8.GetString(payload)}|{keys}");
    }

    static async Task ConnectToServerAsync(nutility.Switch opts, bool batch)
    {
        var address = Config.Address;
        using var client = new ClientWebSocket();
        var serverUri = new Uri(address);
        try
        {
            await client.ConnectAsync(serverUri, CancellationToken.None);
            if (!batch)
            {
                WriteLine($"{client.State} connection to WebSocket server ({address})");
                WriteLine($"{nameof(client.Options.KeepAliveInterval)}: {client.Options.KeepAliveInterval}");
            }

            // Send initial message
            await SendMessageAsync(client, Config.InitialMessage);

            // Start receiving messages
            _ = ReceiveMessagesAsync(client, opts, batch);
            _ = CheckState(client, opts, batch);

            // Allow user to send messages
            await SendUserMessagesAsync(client, opts, batch);
        }
        catch (Exception ex)
        {
            WriteLine($"Exception: {ex.Message}");
        }
    }

    static async Task SendMessageAsync(ClientWebSocket client, string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    static async Task SendUserMessagesAsync(ClientWebSocket client, nutility.Switch opts, bool batch)
    {
        int capture_lapse = 10_000;
        if (int.TryParse(opts["lapse"], out int lapse)) capture_lapse = lapse;
        int cycle = 0;
        while (client.State == WebSocketState.Open)
        {
            if (batch) await Task.Delay(1_000);
            else WriteLine("Enter message (or 'exit' to quit): ");
            var message = "";
            if (batch)
            {
                if (cycle > 0) message = "exit";
                else if (cycle == 0)
                {
                    ++cycle;
                    message = "sub";
                    await Task.Delay(1_000);
                }
            }
            else message = ReadLine();

            if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                break;

            if (string.IsNullOrEmpty(message) || message.ToLower() == "sub")
            {
                var subscribe_payload = Config.GetSubscribePayload();
                var newsub = opts[0];
                if (!string.IsNullOrWhiteSpace(newsub)) subscribe_payload = Config.GetSubscribePayload(newsub);
                if (!batch) WriteLine($"\nSubscribing to {subscribe_payload}...");
                await SendMessageAsync(client, subscribe_payload);
            }
            else
            {
                await SendMessageAsync(client, message);
            }
            if (batch) await Task.Delay(capture_lapse);
        }

        if (client.State == WebSocketState.Open)
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
    }

    static async Task ReceiveMessagesAsync(ClientWebSocket client, nutility.Switch opts, bool batch)
    {
        var buffer = new byte[1024 * 4];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", CancellationToken.None);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var label = opts.Is("recv") ? "Received: " : "";
            var logline = $"{label}{message}";
            WriteLine(logline);
        }
    }
    static async Task CheckState(ClientWebSocket client, nutility.Switch opts, bool batch)
    {
        if (batch) return;
        do
        {
            WriteLine($"{DateTime.Now:o} {nameof(WebSocketState)} = [{client?.State}]");
            await Task.Delay(3_000);
        } while (true);
    }
}