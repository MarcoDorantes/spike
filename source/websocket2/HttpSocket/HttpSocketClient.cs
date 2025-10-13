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
        _ = ConnectToServerAsyncGuarded(Address, InitialMessage, SubscribePayload);
    }
    public void Stop()
    {
        _ = DisconnectFromServerAsync();
    }

    #region WebSocket
    public string ID { get; set; }
    public WebSocketState? State { get => client?.State; }
    public ulong ReceivedMessageCount { get; private set; }
    private ClientWebSocket client;//https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketstate?view=net-8.0
    internal string Address, InitialMessage, Topic, SubscribePayload;

    private async Task ConnectToServerAsyncGuarded(string address, string InitialMessage, string sub)
    {
        do
        {
            try
            {
                await ConnectToServerAsync(Address, InitialMessage, SubscribePayload);
                break;
            }
            catch (System.Threading.Tasks.TaskCanceledException exception)
            {
                SourceHost.WriteLine($"{ID} Connect exception: {exception.Message}");
/*
                Exception ex = exception;
                StringBuilder logline = new();
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
                }
                SourceHost.WriteLine($"{ID} Connect exception:\n{logline}");
*/
                break;
            }
            catch (Exception exception)
            {
                Exception ex = exception;
                StringBuilder logline = new();
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
                }
                SourceHost.WriteLine($"{ID} Connect exception:\n{logline}");
            }
        } while (true);
    }
    private async Task ConnectToServerAsync(string address, string InitialMessage, string sub)
    {
        try
        {
            client = new();
            Uri serverUri = new(address);
            await client.ConnectAsync(serverUri, Cancellation);
            SourceHost.WriteLine($"{ID} {client.State} connection to WebSocket server ({address}) {nameof(client.Options.KeepAliveInterval)}: {client.Options.KeepAliveInterval}");
            await SendMessageAsync(InitialMessage);
            Task receive = ReceiveMessagesAsync();
            _ = CheckState();
            await SendUserMessagesAsync(sub);
            await receive;
        }
        finally
        {
            await LogFinalState();
        }
    }
    private async Task LogFinalState()
    {
        var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
        SourceHost.WriteLine($"{DateTime.Now:o} {ID} {Topic} Msg#{ReceivedMessageCount:N0} {client?.State} Connect task final {client?.HttpStatusCode}/{finalheads}");
        await Task.Delay(1);
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
            SourceHost.WriteLine($"\n{ID} Subscribing to {subscribe_payload}...");
            await SendMessageAsync(subscribe_payload);
        }
        else throw new Exception($"{ID} {nameof(SendUserMessagesAsync)} found invalid client state ({client?.State}).");
    }

    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        try
        {
            while (!Cancellation.IsCancellationRequested)
            {
                if (!(client?.State == WebSocketState.Open))
                {
                    throw new Exception($"{ID} {nameof(ReceiveMessagesAsync)} found invalid connection state ({client?.State}).");
                }
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), Cancellation);
                //https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketclosestatus?view=net-8.0
                ++ReceivedMessageCount;
                var heads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
                var header = $"{DateTime.Now:o} {ID} {Topic} Msg#{ReceivedMessageCount} {client?.State} {result.MessageType} {result.Count} {result.EndOfMessage} [{result.CloseStatus}/{result.CloseStatusDescription}/{client?.HttpStatusCode}/{heads}]";
                string body = null;
                try
                {
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        body = " Server closed.";
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", Cancellation);
                        throw new Exception(body);
                    }
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    body = $" Received: {message}";
                }
                finally
                {
                    var logline = $"{header}{body}";
                    SourceHost.WriteLine(logline);
                }
            }
        }
        finally
        {
            var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
            SourceHost.WriteLine($"{DateTime.Now:o} {ID} {Topic} Msg#{ReceivedMessageCount:N0} {client?.State} Receive task final {client?.HttpStatusCode}/{finalheads}");
        }
    }
    private async Task CheckState()
    {
        while (!Cancellation.IsCancellationRequested)
        {
            SourceHost.WriteLine($"{DateTime.Now:o} {ID} {Topic} {nameof(WebSocketState)} = [{client?.State}]");
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