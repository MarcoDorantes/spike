using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace WebSocketDemo.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Console WebSocket Client");
            await ConnectToServerAsync(args);
        }

        static async Task ConnectToServerAsync(string[] args)
        {
            var address = "";
            using var client = new ClientWebSocket();
            var serverUri = new Uri(address);
            try
            {
                await client.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine($"Connected to WebSocket server ({address})");

                // Send initial message
                var msg = "";
                await SendMessageAsync(client, msg);

                // Start receiving messages
                _ = ReceiveMessagesAsync(client);

                // Allow user to send messages
                await SendUserMessagesAsync(client, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
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

        static async Task SendUserMessagesAsync(ClientWebSocket client, string[] args)
        {
            while (client.State == WebSocketState.Open)
            {
                Console.WriteLine("Enter message (or 'exit' to quit): ");
                var message = Console.ReadLine();
                
                if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                    break;

                if (string.IsNullOrEmpty(message) || message.ToLower() == "sub")
                {
                    var subscribe_payload = "";
                    var newsub = args.FirstOrDefault();
                    if(!string.IsNullOrWhiteSpace(newsub)) subscribe_payload = "";
                    Console.WriteLine($"\nSubscribing to {subscribe_payload}...");
                    await SendMessageAsync(client, subscribe_payload);
                }
                else
                {
                    await SendMessageAsync(client, message);
                }
            }

            if (client.State == WebSocketState.Open)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }
        }

        static async Task ReceiveMessagesAsync(ClientWebSocket client)
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
                Console.WriteLine($"Received: {message}");
            }
        }
    }
}