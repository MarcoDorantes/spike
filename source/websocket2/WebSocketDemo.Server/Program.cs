using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

using static System.Console;

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
    var buffer = new byte[1024 * 8];
    var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    var chat = args?.Any(x => x == "chat");
    uint sent_count = 0U;

    while (!receiveResult.CloseStatus.HasValue)
    {
        var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
        WriteLine($"Server received: {message}");

        // Echo the message back
        var responseMessages = MessageProvider.GetResponseMessages(args, message);
        foreach(var responseMessage in responseMessages)
        {
            var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
            await webSocket.SendAsync(
                new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            WriteLine($"Server sent ({++sent_count}): {responseMessage}");
            await Task.Delay(3_000);
        }
        receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if(chat == false) break;
    }
    while (chat == false && !receiveResult.CloseStatus.HasValue)
    {
        await Task.Delay(3_000);
    }
    await webSocket.CloseAsync(
        receiveResult.CloseStatus.Value,
        receiveResult.CloseStatusDescription,
        CancellationToken.None);
}