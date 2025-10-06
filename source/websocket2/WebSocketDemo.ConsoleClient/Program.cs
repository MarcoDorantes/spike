using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketDemo.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Console WebSocket Client");
            await ConnectToServerAsync();
        }

        static async Task ConnectToServerAsync()
        {
            using var client = new ClientWebSocket();
            var serverUri = new Uri("ws://localhost:5297/ws");

            try
            {
                await client.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine("Connected to WebSocket server");

                // Send initial message
                await SendMessageAsync(client, "Hello from console client!");

                // Start receiving messages
                _ = ReceiveMessagesAsync(client);

                // Allow user to send messages
                await SendUserMessagesAsync(client);
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

        static async Task SendUserMessagesAsync(ClientWebSocket client)
        {
            while (client.State == WebSocketState.Open)
            {
                Console.Write("Enter message (or 'exit' to quit): ");
                var message = Console.ReadLine();
                
                if (string.IsNullOrEmpty(message) || message.ToLower() == "exit")
                    break;

                await SendMessageAsync(client, message);
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