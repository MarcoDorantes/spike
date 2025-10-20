namespace HttpSocket;

using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class HttpSocketClient : IDisposable
{
    public const string CheckStateDelayConfigKey = $"{nameof(CheckStateDelay)}";
    public const string BufferSizeConfigKey = $"{nameof(BufferSize)}";
    public const int CheckStateDelayDefault = 15_000;
    public const int BufferSizeDefault = 1_024 * 8;
    public const string MessagePayloadKey = $"{nameof(MessagePayloadKey)}";
    public const string DestinationNameKey = $"{nameof(DestinationNameKey)}";
    public const string MessageGuidKey = "GUID";
    public const string PayloadIdKey = "PayloadID";

    public ISourceProcessorHost SourceHost { get; set; }
    public IDictionary<string, object> Configuration { get; set; }
    public CancellationToken Cancellation { get; set; }
    public Action<IDictionary<string, object>> OnNext { get; set; }

    public HttpSocketClient()
    {
        encoding = Encoding.UTF8;
    }

    public void Setup()
    {
        ReceivedPayloadCount = 0UL;
        Previous_ReceivedPayloadCount = 0UL;
        ReceptionThroughputAvgCount = 0UL;
        received_count = 0U;
        ReceptionThroughputMin = ReceptionThroughputMax = ReceptionThroughputAvg = ReceptionThroughputSum = 0D;
        watch = null;
        http_responses = [];
        Address = $"{Configuration[nameof(Address)]}";
        InitialMessage = $"{Configuration[nameof(InitialMessage)]}";
        Topic = $"{Configuration["Topic"]}";
        SubscribePayload = $"{Configuration[nameof(SubscribePayload)]}";

        BufferSize = BufferSizeDefault;
        if (Configuration.TryGetValue(nameof(BufferSize), out object _size) && int.TryParse($"{_size}", out int size))
        {
            BufferSize = size;
        }
        SourceHost.Information($"{nameof(BufferSize)}: {BufferSize}");

        CheckStateDelay = CheckStateDelayDefault;
        if (Configuration.TryGetValue(nameof(CheckStateDelay), out object _delay) && int.TryParse($"{_delay}", out int delay) && delay > 0)
        {
            CheckStateDelay = delay;
        }
        SourceHost.Information($"{nameof(CheckStateDelay)}: {CheckStateDelay}");
    }
    public void Start()
    {
        _ = ConnectToServerAsyncGuarded(Address, InitialMessage, SubscribePayload);
        watch = Stopwatch.StartNew();
        transit_collection = CreateBlockingCollection();
        SourceHost.Information($"BlockingCollection created from type: [{GetType().FullName}]");
        Running = true;
        check_running("queuetask", Task.Run(() => ProcessInternalQueue()));
    }
    public void Stop()
    {
        if (!Running) return;
        try
        {
            _ = DisconnectFromServerAsync();
            if (Running) transit_collection.CompleteAdding();
            watch?.Stop();
            StringBuilder logline = new();
            logline.AppendLine($"\n{nameof(ReceivedPayloadCount)}:\t{ReceivedPayloadCount,9:N0} msgs");
            logline.AppendLine($"{nameof(ReceptionThroughputMin)}:\t{ReceptionThroughputMin,9:N2} msgs/s");
            logline.AppendLine($"{nameof(ReceptionThroughputAvg)}:\t{ReceptionThroughputAvg,9:N2} msgs/s");
            logline.AppendLine($"{nameof(ReceptionThroughputMax)}:\t{ReceptionThroughputMax,9:N2} msgs/s");
            logline.AppendLine($"Time elapsed:\t\t{watch?.Elapsed,9} ({watch?.ElapsedMilliseconds:N0}ms)");
            logline.AppendLine($"\nHTTP Responses:\n\t{string.Join("\n\t", http_responses.Select(k => $"{k.Value,4:N0} : {k.Key}"))}");
            SourceHost.Information($"{logline}");
        }
        finally
        {
            Running = false;
        }
    }
    protected virtual BlockingCollection<IDictionary<string, object>> CreateBlockingCollection() => [];
    protected IDictionary<string, object>[] deserialize(string message) => Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>[]>(message);

    #region WebSocket FeedHandler reception
    public string ID { get; set; }
    public bool Running { get; private set; }
    public WebSocketState? State { get => client?.State; }
    public ulong ReceivedPayloadCount { get; private set; }
    public int BufferSize { get; private set; }
    public int CheckStateDelay { get; private set; }

    private ClientWebSocket client;//https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketstate?view=net-8.0
    internal string Address, InitialMessage, Topic, SubscribePayload;
    internal double ReceptionThroughputMin, ReceptionThroughputMax, ReceptionThroughputAvg, ReceptionThroughputSum;
    internal Stopwatch watch;
    private ulong Previous_ReceivedPayloadCount, ReceptionThroughputAvgCount;
    private uint received_count;
    private BlockingCollection<IDictionary<string, object>> transit_collection;
    protected readonly Encoding encoding;
    internal Dictionary<string, uint> http_responses;

    private void OnPayload(byte[] payload)
    {
        Stopwatch elapsed = new();
        try
        {
            elapsed.Restart();
            ProcessPayload(payload);
        }
        finally
        {
            elapsed.Stop();
            SourceHost.Information($"Msg: {ReceivedPayloadCount} Wait: {elapsed.ElapsedTicks}");
        }
    }

    /*protected string getseqid(byte[] payload)
    {
        //[{"ev":"FMV","fmv":509.917,"sym":"MSFT","t":1760646477151120843}]
       //var map = deserialize(encoding.GetString(payload));
        var map = deserialize(payload);
        return $"{map["Sequence"]}/{map["Time"]}";
    }*/
    private void ProcessPayload(byte[] payload)
    {
        try
        {
           //++received_count;
            if (Cancellation.IsCancellationRequested)
            {
                SourceHost.Information("IsCancellationRequested is true");
                return;
            }
           //SourceHost.UpdateReceivedCount(received_count);

            Dictionary<string, object> payload_map = [];
            payload_map[MessagePayloadKey] = payload;

           //var seqid = getseqid(payload);//payload deserializacion is an array: //[{"ev":"FMV","fmv":509.917,"sym":"MSFT","t":1760646477151120843}]
           //var msg_id = $"{received_count}";//$"{received_count}/{seqid}";
           //SourceHost.Information($"{nameof(msg_id)}: {msg_id}");
           //message[MessageGuidKey] = msg_id;//.Substring(0, msg_id.Length > IDTrimLimit ? IDTrimLimit : msg_id.Length);
            payload_map["PossDup"] = false;
            payload_map[PayloadIdKey] = ReceivedPayloadCount;
            payload_map[DestinationNameKey] = "";

            ProcessDictionaryPayload(payload_map);
        }
        catch (Exception exception)
        {
            SourceHost.Error(exception, nameof(ProcessPayload));
        }
    }
    private void ProcessDictionaryPayload(IDictionary<string, object> payload)
    {
        ProcessPayloadWithInternalCollection(payload);
    }

    private void ProcessPayloadWithInternalCollection(IDictionary<string, object> payload)
    {
        transit_collection.Add(payload);
    }

    private async Task ConnectToServerAsyncGuarded(string address, string initialMessage, string subscription)
    {
        do
        {
            try
            {
                await ConnectToServerAsync(address, initialMessage, subscription);
                break;
            }
            catch (System.Threading.Tasks.TaskCanceledException exception)
            {
                SourceHost.Error($"{ID} Connect exception: {exception.Message}");
                /*
                Exception ex = exception;
                StringBuilder logline = new();
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
                }
                SourceHost.Error($"{ID} Connect exception:\n{logline}");
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
                SourceHost.Error($"{ID} Connect exception:\n{logline}");
            }
        } while (true);
    }
    private async Task ConnectToServerAsync(string address, string initialMessage, string subscription)
    {
        using CancellationTokenSource checking = new();
        try
        {
            client = new();
            Uri serverUri = new(address);
            await client.ConnectAsync(serverUri, Cancellation);
            SourceHost.Information($"{ID} {client.State} connection to WebSocket server ({address}) {nameof(client.Options.KeepAliveInterval)}: {client.Options.KeepAliveInterval}");
            _ = CheckState(checking.Token);
            Task receive = ReceivePayloadsAsync();
            await SendMessageAsync(initialMessage);
            await SendSubscribeMessageAsync(subscription);
            await receive;
        }
        finally
        {
            checking.Cancel();
            await LogFinalState();
        }
    }
    private async Task LogFinalState()
    {
        var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
        SourceHost.Information($"{DateTime.Now:o} {ID} {Topic} Msg#{ReceivedPayloadCount:N0} {client?.State} Connect task final {client?.HttpStatusCode}/{finalheads}");
        await Task.Delay(1, Cancellation);
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
        var bytes = encoding.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, Cancellation);
    }

    private async Task SendSubscribeMessageAsync(string subscription)
    {
        if (client?.State == WebSocketState.Open)
        {
            var subscribe_payload = subscription;
            SourceHost.Information($"\n{ID} Subscribing to {subscribe_payload}...");
            await SendMessageAsync(subscribe_payload);
        }
        else throw new Exception($"{ID} {nameof(SendSubscribeMessageAsync)} found invalid client state ({client?.State}).");
    }

    private async Task ReceivePayloadsAsync()
    {
        var buffer = new byte[BufferSize];
        try
        {
            while (!Cancellation.IsCancellationRequested)
            {
                if (!(client?.State == WebSocketState.Open))
                {
                    throw new Exception($"{ID} {nameof(ReceivePayloadsAsync)} found invalid connection state ({client?.State}).");
                }
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), Cancellation);
//https://learn.microsoft.com/en-us/dotnet/api/system.net.websockets.websocketclosestatus?view=net-8.0
                ++ReceivedPayloadCount;
                var heads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
                var http_response = $"[{result.CloseStatus}/{result.CloseStatusDescription}/{client?.HttpStatusCode}/{heads}]";
                if (!http_responses.ContainsKey(http_response)) http_responses[http_response] = 0U;
                ++http_responses[http_response];
                var header = $"{DateTime.Now:o} {ID} {Topic} Payload#{ReceivedPayloadCount} {client?.State} {result.MessageType} {result.Count} {result.EndOfMessage} {http_response}";
                string body = null;
                try
                {
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        body = " Server closed.";
                        await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server closed", Cancellation);
                        throw new Exception(body);
                    }
                   //var message = encoding.GetString(buffer, 0, result.Count);
                   //body = $" Received: {message}";
                    var message = new byte[result.Count];
                    Array.Copy(buffer, message, result.Count);
                    body = $" Received: {result.Count}";
                    OnPayload(message);//Ingest <message> into internal processing (where OnMessage/OnNext? is invoked)
                }
                finally
                {
                    var logline = $"{header}{body}";
                    SourceHost.Information(logline);
                }
            }
        }
        finally
        {
            var finalheads = $"{client?.HttpResponseHeaders?.Aggregate(new StringBuilder(), (whole, next) => whole.AppendFormat("{0}={1}|", next.Key, string.Join('\\', next.Value)))}";
            SourceHost.Information($"{DateTime.Now:o} {ID} {Topic} Payload#{ReceivedPayloadCount:N0} {client?.State} Receive task final {client?.HttpStatusCode}/{finalheads}");
        }
    }
    private async Task CheckState(CancellationToken checking)
    {
        var taskid = Guid.NewGuid();
        using var cancel = CancellationTokenSource.CreateLinkedTokenSource(Cancellation, checking);
        while (!cancel.IsCancellationRequested)
        {
            var current_ReceivedPayloadCount = ReceivedPayloadCount;
            double throughput_per_second = 0D;
            var dx = current_ReceivedPayloadCount - Previous_ReceivedPayloadCount;
            if (dx > 0)
            {
                throughput_per_second = (double)dx / ((double)CheckStateDelay / 1_000D);
                if (ReceptionThroughputMin == 0D) ReceptionThroughputMin = throughput_per_second;
                ReceptionThroughputMin = Math.MinMagnitude(throughput_per_second, ReceptionThroughputMin);
                ReceptionThroughputMax = Math.MaxMagnitude(throughput_per_second, ReceptionThroughputMax);
                ReceptionThroughputSum += throughput_per_second;
                ++ReceptionThroughputAvgCount;
                ReceptionThroughputAvg = ReceptionThroughputSum / ReceptionThroughputAvgCount;
            }
            SourceHost.Information($"{DateTime.Now:o} {ID} {Topic} T_{taskid} {nameof(WebSocketState)} = [{client?.State}] {Previous_ReceivedPayloadCount}/{current_ReceivedPayloadCount} {dx} [{throughput_per_second:N2} {ReceptionThroughputMin:N2} {ReceptionThroughputAvg:N2} {ReceptionThroughputMax:N2} msgs/s]");
            Previous_ReceivedPayloadCount = current_ReceivedPayloadCount;
            await Task.Delay(CheckStateDelay, cancel.Token);
        }
    }
    #endregion

    #region Internal handling to Observer (MessageWriter)
    private void ProcessInternalQueue()
    {
        SourceHost.Information($"{nameof(ProcessInternalQueue)} started.");
        if (transit_collection == null) { return; }
        foreach (var payload in transit_collection.GetConsumingEnumerable())
        {
            if (!Running)
            {
                break;
            }
            try
            {
                ProcessPayload(payload);
            }
            catch (Exception exception)
            {
                SourceHost.Error(exception, nameof(ProcessInternalQueue));
//Newtonsoft.Json.JsonReaderException: Unexpected character encountered while parsing value: S. Path '', line 0, position 0. (ProcessInternalQueue)
            }
        }
        SourceHost.Information($"{nameof(ProcessInternalQueue)} ended.");
        transit_collection.Dispose();
    }
    private void ProcessPayload(IDictionary<string, object> payload_map)
    {
        byte[] bytes = payload_map.TryGetValue(MessagePayloadKey, out object value) ? value as byte[] : null;
        if (bytes == null) throw new Exception($"Invalid null payload ({nameof(PayloadIdKey)}:{(payload_map.TryGetValue(PayloadIdKey, out object _id) ? _id : "")}).");
        string payload = encoding.GetString(bytes);
        var jsonarray = deserialize(payload);
        foreach (var jsonmap in jsonarray)
        {
            //message["GUID"] = msg_id;//.Substring(0, msg_id.Length > IDTrimLimit ? IDTrimLimit : msg_id.Length);
            //message["PossDup"] = false;
            //message[DestinationNameKey] = "";

            jsonmap[MessagePayloadKey] = payload_map[MessagePayloadKey];
            jsonmap[MessageGuidKey] = ++received_count;
            SourceHost.UpdateReceivedCount(received_count);
            OnNext(jsonmap);
        }
    }

    void check_running(string taskname, Task _t)
    {
        const int millisecondsTimeout = 100;
        while (_t.Status < TaskStatus.Running)
        {
            System.Threading.ThreadPool.GetMaxThreads(out int max_worker, out int max_io);
            System.Threading.ThreadPool.GetAvailableThreads(out int free_worker, out int free_io);
            SourceHost.Information($"Waiting {taskname} for TaskStatus.Running status ({_t.Status}). Max {max_worker}|{max_io} Available {free_worker}|{free_io}.");
            System.Threading.Thread.Sleep(millisecondsTimeout);
        }
        if (_t.Status != TaskStatus.Running)
        {
            throw new Exception($"TaskStatus of {taskname} must be {nameof(TaskStatus.Running)} ({_t.Status}).");
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