namespace HttpSocket;

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;

//public interface ISourceProcessorHost{}
public class HttpSocketClient : IDisposable
{
    public /*ISourceProcessorHost*/ System.IO.TextWriter SourceHost { get; set; }
    public IDictionary<string, object> Configuration { get; set; }
    public CancellationToken Cancellation { get; set; }
    public void Setup()
    {
        ReceivedMessageCount = 0UL;
        Address = $"{Configuration[nameof(Address)]}";
        InitialMessage = $"{Configuration[nameof(InitialMessage)]}";
        Topic = $"{Configuration["Topic"]}";
        SubscribePayload = $"{Configuration[nameof(SubscribePayload)]}";
    }
    public void Start()
    {
        _ = ConnectToServerAsync(Address, InitialMessage, SubscribePayload);
    }
    public void Stop()
    {
        _ = DisconnectFromServerAsync();
    }

    #region WebSocket
    public WebSocketState? State { get => client?.State; }
    public ulong ReceivedMessageCount { get; private set; }
    private ClientWebSocket client;//https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketstate?view=net-8.0
    private string Address, InitialMessage, Topic, SubscribePayload;

    private async Task ConnectToServerAsync(string address, string InitialMessage, string sub)
    {
        client = new();
        Task receive;
        Uri serverUri = new(address);
        try
        {
            await client.ConnectAsync(serverUri, Cancellation);
            SourceHost.WriteLine($"{client.State} connection to WebSocket server ({address})");
            SourceHost.WriteLine($"{nameof(client.Options.KeepAliveInterval)}: {client.Options.KeepAliveInterval}");

            // Send initial message
            await SendMessageAsync(InitialMessage);

            // Start receiving messages
            receive = ReceiveMessagesAsync();

            _ = CheckState();

            // Allow user to send messages
            await SendUserMessagesAsync(sub);
            await receive;
        }
        catch (Exception ex)
        {
            StringBuilder logline = new();
            for (int level = 0; ex != null; ex = ex.InnerException, ++level)
            {
                logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}");
            }
            SourceHost.WriteLine($"Connect exception:\n{logline}");
        }
        var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
        SourceHost.WriteLine($"#{ReceivedMessageCount:N0} {DateTime.Now:o} {Topic} {client?.State} Connect task final {client?.HttpStatusCode}/{finalheads}");
    }
    private async Task DisconnectFromServerAsync()
    {
        if (client?.State == WebSocketState.Open)
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", Cancellation);
        }
        client?.Dispose();
        client = null;
    }
    private async Task SendMessageAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, Cancellation);
    }

    private async Task SendUserMessagesAsync(string sub)
    {
        //while (client.State == WebSocketState.Open)
        if (client?.State == WebSocketState.Open)
        {
            var subscribe_payload = sub;
            SourceHost.WriteLine($"\nSubscribing to {subscribe_payload}...");
            await SendMessageAsync(subscribe_payload);
        }
        else throw new Exception($"{nameof(SendUserMessagesAsync)} found invalid client state ({client?.State}).");
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        while (client?.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), Cancellation);
//https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketclosestatus?view=net-8.0
            ++ReceivedMessageCount;
            var heads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
            SourceHost.WriteLine($"#{ReceivedMessageCount:N0} {DateTime.Now:o} {Topic} {client?.State} {result.MessageType} {result.Count} {result.EndOfMessage} [{result.CloseStatus}/{result.CloseStatusDescription}/{client?.HttpStatusCode}/{heads}]");
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", Cancellation);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var label = $"#{ReceivedMessageCount:N0} Received: ";
            var logline = $"{label}{message}";
            SourceHost.WriteLine(logline);
        }
        var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
        SourceHost.WriteLine($"#{ReceivedMessageCount:N0} {DateTime.Now:o} {Topic} {client?.State} Receive task final {client?.HttpStatusCode}/{finalheads}");
    }
    private async Task CheckState()
    {
        while (!Cancellation.IsCancellationRequested)
        {
            SourceHost.WriteLine($"{DateTime.Now:o} {Topic} {nameof(WebSocketState)} = [{client?.State}]");
            await Task.Delay(3_000);
        }
    }
    #endregion

    #region IDisposable support
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Stop();
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~HttpSocketClient()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}