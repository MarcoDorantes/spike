namespace HttpSocket;

using System;
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
        Address = $"{Configuration[nameof(Address)]}";
        InitialMessage = $"{Configuration[nameof(InitialMessage)]}";
        SubscribePayload = $"{Configuration[nameof(SubscribePayload)]}";
    }
    public void Start()
    {
        _ = ConnectToServerAsync(Address, InitialMessage, SubscribePayload);
        //Task.Run(() => ConnectToServerAsync(Address, InitialMessage, SubscribePayload));
    }
    public void Stop()
    {
        _ = DisconnectFromServerAsync();
        //Task.Run(() => DisconnectFromServerAsync());
    }

    #region WebSocket
    private ClientWebSocket client;
    private string Address, InitialMessage, SubscribePayload;
    private async Task ConnectToServerAsync(string address, string InitialMessage, string sub)
    {
        client = new();
        Uri serverUri = new(address);
        try
        {
            await client.ConnectAsync(serverUri, CancellationToken.None);
            SourceHost.WriteLine($"{client.State} connection to WebSocket server ({address})");
            SourceHost.WriteLine($"{nameof(client.Options.KeepAliveInterval)}: {client.Options.KeepAliveInterval}");

            // Send initial message
            await SendMessageAsync(client, InitialMessage);

            // Start receiving messages
            _ = ReceiveMessagesAsync(client);
            _ = CheckState(client);

            // Allow user to send messages
            await SendUserMessagesAsync(client, sub);
        }
        catch (Exception ex)
        {
            SourceHost.WriteLine($"Exception: {ex.Message}");
        }
    }
    private async Task DisconnectFromServerAsync()
    {
        if (client.State == WebSocketState.Open)
        {
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
        }
        client.Dispose();
        client = null;
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

    private async Task SendUserMessagesAsync(ClientWebSocket client, string sub)
    {
        //while (client.State == WebSocketState.Open)
        if (client.State == WebSocketState.Open)
        {
            var subscribe_payload = sub;
            SourceHost.WriteLine($"\nSubscribing to {subscribe_payload}...");
            await SendMessageAsync(client, subscribe_payload);
        }
    }

    private async Task ReceiveMessagesAsync(ClientWebSocket client)
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
            var label = "Received: ";
            var logline = $"{label}{message}";
            SourceHost.WriteLine(logline);
        }
    }
    private async Task CheckState(ClientWebSocket client)
    {
        while (!Cancellation.IsCancellationRequested)
        {
            SourceHost.WriteLine($"{DateTime.Now:o} {nameof(WebSocketState)} = [{client?.State}]");
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