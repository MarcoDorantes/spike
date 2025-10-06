using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    await HandleWebSocketAsync(webSocket);
});

app.MapGet("/", () => "WebSocket Server is running. Connect to /ws endpoint");

app.Run();

async Task HandleWebSocketAsync(WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    var receiveResult = await webSocket.ReceiveAsync(
        new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!receiveResult.CloseStatus.HasValue)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
        Console.WriteLine($"Server received: {message}");

        // Echo the message back
        var responseMessage = $"Server echo: {message}";
        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
        
        await webSocket.SendAsync(
            new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        receiveResult = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(
        receiveResult.CloseStatus.Value,
        receiveResult.CloseStatusDescription,
        CancellationToken.None);
}