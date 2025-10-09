using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;

using static System.Console;

namespace WebSocketDemo.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            nutility.Switch opts = new(args);
            var batch = opts.Is("batch");
            if(!batch) WriteLine("Console WebSocket Client");
            await ConnectToServerAsync(opts, batch);
        }

        static async Task ConnectToServerAsync(nutility.Switch opts, bool batch)
        {
            var address = "";
            using var client = new ClientWebSocket();
            var serverUri = new Uri(address);
            try
            {
                await client.ConnectAsync(serverUri, CancellationToken.None);
                if(!batch) WriteLine($"Connected to WebSocket server ({address})");

                // Send initial message
                var msg = "";
                await SendMessageAsync(client, msg);

                // Start receiving messages
                _ = ReceiveMessagesAsync(client, opts, batch);

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
            int capture_lapse = 10000;
            if(int.TryParse(opts["lapse"],out int lapse)) capture_lapse = lapse;
            int cycle = 0;
            while (client.State == WebSocketState.Open)
            {
                if(batch) await System.Threading.Tasks.Task.Delay(1000);
                else WriteLine("Enter message (or 'exit' to quit): ");
                var message = "";
                if (batch)
                {
                    if (cycle > 0) message = "exit";
                    else if(cycle == 0)
                    {
                        ++cycle;
                        message = "sub";
                        await System.Threading.Tasks.Task.Delay(1000);
                    }
                }
                else message = ReadLine();

                if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                    break;

                if (string.IsNullOrEmpty(message) || message.ToLower() == "sub")
                {
                    var subscribe_payload = "";
                    var newsub = opts[0];
                    if (!string.IsNullOrWhiteSpace(newsub)) subscribe_payload = "";
                    if (!batch) WriteLine($"\nSubscribing to {subscribe_payload}...");
                    await SendMessageAsync(client, subscribe_payload);
                }
                else
                {
                    await SendMessageAsync(client, message);
                }
                if (batch) await System.Threading.Tasks.Task.Delay(capture_lapse);
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
    }
}