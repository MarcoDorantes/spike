namespace HttpPayloadRequest;

using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HttpPayloadReader : IDisposable
{
    public const string MessagePayloadKey = nameof(MessagePayloadKey);
    public const string DestinationNameKey = nameof(DestinationNameKey);
    public const string URLKey = nameof(URLValue);
    public const string DestinationNamePrefixKey = nameof(DestinationNamePrefixValue);
    public const string BusinessEntityIDTagKey = nameof(BusinessEntityIDTagValue);//"T"
    public const string BusinessEntityIDTagValueDefault = nameof(BusinessEntityIDTagValueDefault);

    public ISourceProcessorHost SourceHost { get; set; }
    public IDictionary<string, object> Configuration { get; set; }
    public CancellationToken Cancellation { get; set; }
    public Action<IDictionary<string, object>> OnNext { get; set; }
    protected readonly Encoding encoding;
    public string ID { get; set; }
    public bool Running { get; private set; }
    public uint ReceivedPayloadCount { get; private set; }
    public string URLValue { get; private set; }
    public string DestinationNamePrefixValue { get; private set; }// = "dat/GBM/R/L2/MEX/E/BMV/";
    public string BusinessEntityIDTagValue { get; private set; }

    public HttpPayloadReader()
    {
        encoding = Encoding.UTF8;
    }

    public void Setup()
    {
        ReceivedPayloadCount = 0U;

        URLValue = null;
        if (Configuration.TryGetValue(URLKey, out object _url) && !string.IsNullOrWhiteSpace($"{_url}"))
        {
            URLValue = $"{_url}";
        }
        SourceHost.Information($"{URLKey}: {URLValue}");

        DestinationNamePrefixValue = null;
        if (Configuration.TryGetValue(DestinationNamePrefixKey, out object _prefix) && !string.IsNullOrWhiteSpace($"{_prefix}"))
        {
            DestinationNamePrefixValue = $"{_prefix}";
        }
        SourceHost.Information($"{DestinationNamePrefixKey}: {DestinationNamePrefixValue}");

        BusinessEntityIDTagValue = null;
        if (Configuration.TryGetValue(BusinessEntityIDTagKey, out object _symboltag) && !string.IsNullOrWhiteSpace($"{_symboltag}"))
        {
            BusinessEntityIDTagValue = $"{_symboltag}";
        }
        SourceHost.Information($"{BusinessEntityIDTagKey}: {BusinessEntityIDTagValue}");

        /*Previous_ReceivedPayloadCount = 0U;
        ReceivedThroughputAvgCount = 0U;
        received_count = 0U;
        ReceivedThroughputMin = ReceivedThroughputMax = ReceivedThroughputAvg = ReceivedThroughputSum = 0D;
        watch = null;
        http_status_code = [];
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

        UpdateReceivedOn = UpdateReceivedOnDefault;
        if (Configuration.TryGetValue(UpdateReceivedOnKey, out object _updatetype) && Enum.TryParse<UpdateReceivedOnType>($"{_updatetype}", out UpdateReceivedOnType updatetype))
        {
            UpdateReceivedOn = updatetype;
        }
        SourceHost.Information($"{nameof(UpdateReceivedOn)}: {UpdateReceivedOn}");

        KeepAliveInterval = WebSocket.DefaultKeepAliveInterval;
        if (Configuration.TryGetValue(nameof(KeepAliveInterval), out object _interval) && TimeSpan.TryParse($"{_interval}", out TimeSpan interval))
        {
            KeepAliveInterval = interval;
        }
        SourceHost.Information($"{nameof(KeepAliveInterval)}: {KeepAliveInterval}");

        KeepAliveTimeout = WebSocket.DefaultKeepAliveInterval;
        if (Configuration.TryGetValue(nameof(KeepAliveTimeout), out object _timeout) && TimeSpan.TryParse($"{_timeout}", out TimeSpan timeout))
        {
            KeepAliveTimeout = timeout;
        }
        SourceHost.Information($"{nameof(KeepAliveTimeout)}: {KeepAliveTimeout}");

        CollectHttpResponseDetails = false;
        if (Configuration.TryGetValue(nameof(CollectHttpResponseDetails), out object _details) && bool.TryParse($"{_details}", out bool details) && details)
        {
            CollectHttpResponseDetails = details;
        }
        SourceHost.Information($"{nameof(CollectHttpResponseDetails)}: {CollectHttpResponseDetails}");

        SlowSubscriber = false;
        if (Configuration.TryGetValue(nameof(SlowSubscriber), out object _slow) && bool.TryParse($"{_slow}", out bool slow) && slow)
        {
            SlowSubscriber = slow;
        }
        SourceHost.Information($"{nameof(SlowSubscriber)}: {SlowSubscriber}");*/
    }
    public void Start()
    {
        _ = InvokeHttpRequestAsyncGuarded();
        /*_ = ConnectToServerAsyncGuarded(Address, InitialMessage, SubscribePayload);
        watch = Stopwatch.StartNew();
        transit_collection = CreateBlockingCollection();
        SourceHost.Information($"BlockingCollection created from type: [{GetType().FullName}]");
        check_running("queuetask", Task.Run(() => ProcessInternalQueue()));*/
        Running = true;
    }
    public void Stop()
    {
        if (!Running) return;
        try
        {
            StringBuilder logline = new();
            logline.AppendLine($"\n{nameof(ReceivedPayloadCount)}:\t{ReceivedPayloadCount,9:N0} msgs");
            SourceHost.Information($"{logline}");

            /*_ = DisconnectFromServerAsync();
            if (Running) transit_collection.CompleteAdding();
            watch?.Stop();
            StringBuilder logline = new();
            logline.AppendLine($"\n{nameof(ReceivedPayloadCount)}:\t{ReceivedPayloadCount,9:N0} msgs");
            logline.AppendLine($"{nameof(ReceivedThroughputMin)}:\t{ReceivedThroughputMin,9:N2} msgs/s");
            logline.AppendLine($"{nameof(ReceivedThroughputAvg)}:\t{ReceivedThroughputAvg,9:N2} msgs/s");
            logline.AppendLine($"{nameof(ReceivedThroughputMax)}:\t{ReceivedThroughputMax,9:N2} msgs/s");
            logline.AppendLine($"Time elapsed:\t\t{watch?.Elapsed,9} ({watch?.ElapsedMilliseconds:N0}ms)");
            logline.AppendLine($"\nHTTP status codes:\n\t{string.Join("\n\t", http_status_code.Select(k => $"{k.Value,4:N0} : {k.Key}"))}");
            SourceHost.Information($"{logline}");*/
        }
        finally
        {
            Running = false;
        }
    }

    private async Task InvokeHttpRequestAsyncGuarded()
    {
        do
        {
            try
            {
                await InvokeHttpRequestAsync();
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
    private async Task InvokeHttpRequestAsync()
    {
        while (!Cancellation.IsCancellationRequested)
        {
            var next = await getquote(URLValue);
            //var next = await getsnap(URL);

            var symbol = next.TryGetValue(BusinessEntityIDTagValue, out object _value) ? $"{_value}" : BusinessEntityIDTagValueDefault;
            /*add prefix and symbol-key to config*/next.Add(DestinationNameKey, $"{DestinationNamePrefixValue}{symbol}");

            SourceHost.UpdateReceivedCount(++ReceivedPayloadCount);
            OnNext(next);
          //OnNext?.Invoke(next);
            await Task.Delay(2000);
        }
    }
    private async Task<IDictionary<string, object>> getquote(string uri)
    {
        var result = await GetObject<Dictionary<string, JsonElement>>(uri, Cancellation);
        var status = result["status"].GetString();
        var quote = JsonSerializer.Deserialize<Dictionary<string, object>>(result["results"]);
        var t = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse($"{quote["t"]}") / 1_000_000L).ToLocalTime();
        quote.Add("status", status);
        quote.Add("LOCALTIMESTAMP", t);
        return quote;
    }
    private async Task<IDictionary<string, object>> getsnap(string uri)
    {
        var result = await GetObject<Dictionary<string, JsonElement>>(uri, CancellationToken.None);
        var status = result["status"].GetString();
        Dictionary<string, object> flat = result["ticker"].EnumerateObject().Aggregate(new Dictionary<string, object>(), (whole, next) =>
        {
            switch (next.Value.ValueKind)
            {
                case JsonValueKind.Array:
                    whole[next.Name] = string.Join(' ', next.Value.EnumerateArray());
                    break;
                case JsonValueKind.Object:
                    next.Value.EnumerateObject().Aggregate(whole, (w, n) =>
                    {
                        w[$"{next.Name} ({n.Name})"] = n.Value.ValueKind == JsonValueKind.Array ? string.Join(' ', n.Value.EnumerateArray()) : $"{n.Value}";
                        return w;
                    });
                    break;
                default:
                    whole[next.Name] = $"{next.Value}";
                    break;
            }
            return whole;
        });
        //updated:1761350400003149384
        var updated = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse($"{flat["updated"]}") / 1_000_000L).ToLocalTime();
        flat.Add("status", status);
        flat.Add("LOCALTIMESTAMP", updated);
        return flat;
    }

    #region Access to HttpClient
    public static async Task<string> GetString(string uri, CancellationToken cancel)
    {
        using HttpClient client = new();
        var response = await client.GetAsync(uri, cancel);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancel);
    }

    public static async Task<T> GetObject<T>(string uri, CancellationToken cancel)
    {
        using HttpClient client = new();
        /*var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            WriteIndented = false
        };*/
        return await client.GetFromJsonAsync<T>(uri, /*jsonOptions,*/ cancel);
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
    // ~HttpPayloadReader()
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