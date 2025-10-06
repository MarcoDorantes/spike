using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketDemo.BackendService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Backend Service with WebSocket Client");
            
            // Start the background service
            await RunBackendServiceAsync();
        }

        static async Task RunBackendServiceAsync()
        {
            using var client = new ClientWebSocket();
            var serverUri = new Uri("ws://localhost:5297/ws");

            try
            {
                await client.ConnectAsync(serverUri, CancellationToken.None);
                Console.WriteLine("Connected to WebSocket server");

                // Create cancellation token for controlled shutdown
                using var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (s, e) => 
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                // Start receiving messages
                var receiveTask = ReceiveMessagesAsync(client, cts.Token);

                // Send periodic updates
                await SendPeriodicUpdatesAsync(client, cts.Token);

                await receiveTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            finally
            {
                if (client.State == WebSocketState.Open)
                {
                    await client.CloseAsync(
                        WebSocketCloseStatus.NormalClosure, 
                        "Backend service shutting down", 
                        CancellationToken.None);
                }
            }
        }

        static async Task SendPeriodicUpdatesAsync(ClientWebSocket client, CancellationToken cancellationToken)
        {
            var counter = 0;
            while (!cancellationToken.IsCancellationRequested && client.State == WebSocketState.Open)
            {
                counter++;
                var message = $"Backend service status update #{counter} at {DateTime.Now}";
                
                var bytes = Encoding.UTF8.GetBytes(message);
                await client.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    cancellationToken);

                Console.WriteLine($"Sent: {message}");
                
                // Wait before sending next update
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        static async Task ReceiveMessagesAsync(ClientWebSocket client, CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];

            while (!cancellationToken.IsCancellationRequested && client.State == WebSocketState.Open)
            {
                try
                {
                    var result = await client.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await client.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Server closed connection",
                            CancellationToken.None);
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {message}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }
}