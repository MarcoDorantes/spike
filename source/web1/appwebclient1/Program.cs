// See https://aka.ms/new-console-template for more information
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using static System.Console;

class ConsoleHost(System.IO.TextWriter Writer) : HttpPayloadRequest.ISourceProcessorHost
{
    void HttpPayloadRequest.ISourceProcessorHost.Information(string information) => Writer.WriteLine(information);
    void HttpPayloadRequest.ISourceProcessorHost.Warning(string details_for_diagnostic) => Writer.WriteLine(details_for_diagnostic);
    void HttpPayloadRequest.ISourceProcessorHost.Error(string details_for_diagnostic) => Writer.WriteLine(details_for_diagnostic);
    void HttpPayloadRequest.ISourceProcessorHost.Error(Exception exception, string details_for_diagnostic) => Writer.WriteLine($"{exception.GetType().FullName}: {exception.Message} ({details_for_diagnostic})");
    void HttpPayloadRequest.ISourceProcessorHost.NotifyState(string state) => Writer.WriteLine(state);
    void HttpPayloadRequest.ISourceProcessorHost.StartTopicSubscription(string name, string vpnName, string host, string userName, string password, string sourceTopicPath, Action<IDictionary<string, object>> onmessage, string payloadFormat /*= "JSON"*/) => throw new NotImplementedException();
    void HttpPayloadRequest.ISourceProcessorHost.SendNotification(string subject, string[] lines, IList<KeyValuePair<string, string>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/, System.Text.Encoding encoding /*= null*/) => throw new NotImplementedException();
    void HttpPayloadRequest.ISourceProcessorHost.SendNotification(string subject, string[] lines, IList<KeyValuePair<string, byte[]>> attachs /*= null*/, bool error /*= false*/, string[] to /*= null*/) => throw new NotImplementedException();
    void HttpPayloadRequest.ISourceProcessorHost.UpdateReceivedCount(uint received_count) => Writer.WriteLine($"{nameof(HttpPayloadRequest.ISourceProcessorHost.UpdateReceivedCount)}: {received_count}");
}

class Program
{
    static void log(Exception ex) { for (int level = 0; ex != null; ex = ex.InnerException, ++level) WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}"); }
    static ulong received_onnext_count;
    static async Task Main(string[] args)
    {
        nutility.Switch opts = new(args);
        if (opts.Is("reader")) { LaunchReaders(opts); }
        if (opts.Is("getstring")) await getstring();
        else if (opts.Is("getquotes")) await getquotes();
        else if (opts.Is("getcatalog")) try { await getcatalog(); } catch (Exception ex) { log(ex); }
    }
    static void LaunchReaders(nutility.Switch opts)
    {
        using CancellationTokenSource cancel = new();
        List<HttpPayloadRequest.HttpPayloadReader> readers = [];
        try
        {
            readers.Add(LaunchReader("1", cancel.Token, opts));
            ReadLine();
            cancel.Cancel();
            readers.ForEach(c => c.Stop());
            readers.ForEach(c => c.Dispose());
            Thread.Sleep(3_000);
        }
        finally
        {
            readers.ForEach(c => c.Dispose());
        }
    }
    static HttpPayloadRequest.HttpPayloadReader LaunchReader(string id, CancellationToken cancel, nutility.Switch opts)
    {
        //Add all configuration keys to the AppSettingsKey? No: the host must arrange these from hosting AppSettings environment.
        Dictionary<string, object> config = new()
        {
        };
        ConsoleHost host = new(Out);
        HttpPayloadRequest.HttpPayloadReader reader = new()
        {
            ID = id,
            SourceHost = host,
            Configuration = config,
            Cancellation = cancel,
            OnNext = OnNext
        };
        reader.Setup();
        reader.Start();
        return reader;
    }
    static void OnNext(IDictionary<string, object> message)
    {
        ++received_onnext_count;
        // var payload = message[HttpSocket.HttpSocketClient.MessagePayloadKey] as byte[];
        // var keys = $"{string.Join('|', message.Where(k => k.Key != HttpSocket.HttpSocketClient.MessagePayloadKey).Select(p => $"{p.Key}={p.Value}"))}";
        // var payload_detail = Config.PayloadLogEnabled ? $" ({payload.GetType().Name}):{Encoding.UTF8.GetString(payload)}|{keys}" : "";
        var payload_detail = "";
        WriteLine($"Msg#{received_onnext_count}{payload_detail}");
    }
    static async Task getstring()
    {
        var appsettings = System.Configuration.ConfigurationManager.AppSettings;
        HttpPayloadRequest.HttpPayloadReader reader = new();
        var uri = appsettings["uri"];
        var result = await reader.GetString(uri, CancellationToken.None);
        WriteLine(result);
    }
    static async Task getquote()
    {
        var appsettings = System.Configuration.ConfigurationManager.AppSettings;
        HttpPayloadRequest.HttpPayloadReader reader = new();
        var uri = appsettings["uri"];
        var result = await reader.GetObject<Dictionary<string, JsonElement>>(uri, CancellationToken.None);
        var status = result["status"].GetString();
        var quote = JsonSerializer.Deserialize<Dictionary<string, object>>(result["results"]);
        var t = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse($"{quote["t"]}") / 1_000_000L).ToLocalTime();
        WriteLine($"{nameof(status)}:{status} {string.Join('|', quote.Select(k => $"{k.Key}:{k.Value}"))} {t:o}");
    }
    static async Task poll_quotes(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
            try { await getquote(); await Task.Delay(1000); } catch (Exception ex) { log(ex); }
    }
    static async Task getquotes()
    {
        //try { await getquote(); } catch (Exception ex) { log(ex); }
        CancellationTokenSource cancel = new();
        _ = poll_quotes(cancel.Token);
        ReadLine();
        cancel.Cancel();
        await Task.Delay(50);
    }
    static async Task getcatalog()
    {
        var appsettings = System.Configuration.ConfigurationManager.AppSettings;
        HttpPayloadRequest.HttpPayloadReader reader = new();
        var prefix = appsettings["prefix"];
        var suffix = appsettings["suffix"];
        List<Dictionary<string, JsonElement>> results = [];
        do
        {
            var uri = prefix + suffix;
            var result = await reader.GetObject<Dictionary<string, JsonElement>>(uri, CancellationToken.None);
            results.Add(result);
            WriteLine(string.Join('|', result.Select(k => k.Key == "status" || k.Key == "count" ? $"{k.Key}={k.Value}" : k.Key)));
            if (result.ContainsKey("next_url"))
            {
                prefix = result["next_url"].GetString();
                //WriteLine(uri);
            }
            else { WriteLine("no next_url"); break; }
        } while (true);
        var ticker_results = results.Aggregate(new List<Dictionary<string, object>[]>(), (whole, next) => { whole.Add(JsonSerializer.Deserialize<Dictionary<string, object>[]>(next["results"])); return whole; });
        var tickers = ticker_results.SelectMany(ticket_result => ticket_result.Select(ticker => ticker));
        var found_keys = tickers.SelectMany(t => t.Keys).Distinct();

        WriteLine(string.Join(',', found_keys));
        tickers.Aggregate(Out, (whole, next) => { whole.WriteLine(string.Join(',', next.Values)); return whole; });

        WriteLine($"\nresults = {results.Count}\nstatus = {string.Join('|', results.Select(k => k["status"].GetString()).Distinct())}");

        var r = results.First();
        //WriteLine(r["results"].ValueKind);
        var rr = JsonSerializer.Deserialize<Dictionary<string, object>[]>(r["results"]);
        var map = rr.First();
        WriteLine(string.Join('|', map.Select(k => $"{k.Key}:{k.Value}")));

        WriteLine($"Keys found ({map.Keys.Count}:{found_keys.Count()}): {string.Join('|', found_keys)}");

        WriteLine(ticker_results.Count);
        WriteLine(ticker_results.Sum(a => a.Length));
        WriteLine(tickers.Count());
    }
}
/*
https://devblogs.microsoft.com/dotnet/dotnet9-openapi
How to process output from a REST API in net9.0?

namespace pg;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Generic REST API client for making HTTP requests and processing responses in .NET 9
/// </summary>
public class RestApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposedValue;

    public RestApiClient(string baseUrl = null, TimeSpan? timeout = null)
    {
        _httpClient = new HttpClient();
        
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
        
        if (timeout.HasValue)
        {
            _httpClient.Timeout = timeout.Value;
        }
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    /// <summary>
    /// Adds default headers to the HTTP client
    /// </summary>
    public void AddDefaultHeaders(IDictionary<string, string> headers)
    {
        if (headers == null) return;
        
        foreach (var header in headers)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    /// <summary>
    /// Sets the authorization header with a bearer token
    /// </summary>
    public void SetBearerToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Makes a GET request to the specified endpoint and returns the response as a string
    /// </summary>
    public async Task<string> GetStringAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    /// <summary>
    /// Makes a GET request to the specified endpoint and returns the response as a stream
    /// </summary>
    public async Task<Stream> GetStreamAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetStreamAsync(endpoint, cancellationToken);
    }

    /// <summary>
    /// Makes a GET request to the specified endpoint and deserializes the response to the specified type
    /// </summary>
    public async Task<T> GetJsonAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<T>(endpoint, _jsonOptions, cancellationToken);
    }

    /// <summary>
    /// Makes a POST request with JSON content to the specified endpoint and returns the response as the specified type
    /// </summary>
    public async Task<TResponse> PostJsonAsync<TRequest, TResponse>(
        string endpoint, 
        TRequest content, 
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, content, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions, cancellationToken);
    }

    /// <summary>
    /// Makes a POST request with JSON content to the specified endpoint
    /// </summary>
    public async Task<HttpResponseMessage> PostJsonAsync<TRequest>(
        string endpoint, 
        TRequest content, 
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, content, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Makes a PUT request with JSON content to the specified endpoint
    /// </summary>
    public async Task<HttpResponseMessage> PutJsonAsync<TRequest>(
        string endpoint, 
        TRequest content, 
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync(endpoint, content, _jsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Makes a DELETE request to the specified endpoint
    /// </summary>
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response;
    }

    /// <summary>
    /// Processes a JSON response stream and returns a strongly typed object
    /// </summary>
    public async Task<T> ProcessJsonResponseAsync<T>(Stream responseStream, CancellationToken cancellationToken = default)
    {
        return await JsonSerializer.DeserializeAsync<T>(responseStream, _jsonOptions, cancellationToken);
    }

    /// <summary>
    /// Processes a JSON response string and returns a strongly typed object
    /// </summary>
    public T ProcessJsonResponse<T>(string jsonResponse)
    {
        return JsonSerializer.Deserialize<T>(jsonResponse, _jsonOptions);
    }

    /// <summary>
    /// Processes a JSON response string into a dictionary
    /// </summary>
    public Dictionary<string, object> ProcessJsonResponseToDictionary(string jsonResponse)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponse, _jsonOptions);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
--
namespace pg;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using static System.Console;

/// <summary>
/// Example class for processing REST API responses
/// </summary>
public class RestApiProcessor
{
    private readonly RestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken;

    public RestApiProcessor(string baseUrl, CancellationToken cancellationToken = default)
    {
        _apiClient = new RestApiClient(baseUrl);
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// Processes a REST API endpoint and writes the output to the console
    /// </summary>
    public async Task ProcessApiEndpointAsync(string endpoint)
    {
        try
        {
            WriteLine($"Fetching data from endpoint: {endpoint}");
            string jsonResponse = await _apiClient.GetStringAsync(endpoint, _cancellationToken);
            
            // Process the raw JSON string
            WriteLine($"\nRaw JSON response:\n{jsonResponse}\n");
            
            // Convert to dictionary for flexible access
            var responseDictionary = _apiClient.ProcessJsonResponseToDictionary(jsonResponse);
            WriteLine("Response as dictionary:");
            foreach (var item in responseDictionary)
            {
                WriteLine($"{item.Key}: {item.Value}");
            }
        }
        catch (HttpRequestException ex)
        {
            WriteLine($"HTTP request error: {ex.Message}");
            if (ex.InnerException != null)
            {
                WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
        catch (TaskCanceledException)
        {
            WriteLine("Request was canceled");
        }
        catch (Exception ex)
        {
            WriteLine($"Error processing API response: {ex.Message}");
        }
    }

    /// <summary>
    /// Processes a REST API endpoint and returns a strongly typed response
    /// </summary>
    public async Task<T> ProcessApiEndpointAsync<T>(string endpoint)
    {
        try
        {
            return await _apiClient.GetJsonAsync<T>(endpoint, _cancellationToken);
        }
        catch (Exception ex)
        {
            WriteLine($"Error processing typed API response: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Posts data to an API endpoint and processes the response
    /// </summary>
    public async Task<TResponse> PostToApiEndpointAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
    {
        try
        {
            WriteLine($"Posting data to endpoint: {endpoint}");
            return await _apiClient.PostJsonAsync<TRequest, TResponse>(endpoint, requestData, _cancellationToken);
        }
        catch (Exception ex)
        {
            WriteLine($"Error posting to API: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Streams large API responses to avoid memory issues
    /// </summary>
    public async Task StreamApiResponseAsync(string endpoint, string outputFilePath)
    {
        try
        {
            WriteLine($"Streaming data from endpoint: {endpoint}");
            
            using var responseStream = await _apiClient.GetStreamAsync(endpoint, _cancellationToken);
            using var fileStream = File.Create(outputFilePath);
            
            await responseStream.CopyToAsync(fileStream, _cancellationToken);
            
            WriteLine($"Response saved to file: {outputFilePath}");
        }
        catch (Exception ex)
        {
            WriteLine($"Error streaming API response: {ex.Message}");
        }
    }
}
--
RestExtensions.cs
namespace pg;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using static System.Console;

/// <summary>
/// Extensions to the Input class for REST API processing
/// </summary>
public partial class Input
{
    public string apiUrl;
    public string endpoint;
    public string outputFile;
    public bool pretty;
    
    /// <summary>
    /// Process a REST API response
    /// </summary>
    public async Task rest_process()
    {
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            WriteLine("API URL is required. Please specify using -apiUrl parameter.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            WriteLine("API endpoint is required. Please specify using -endpoint parameter.");
            return;
        }
        
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) => 
        {
            WriteLine("Canceling request...");
            cts.Cancel();
            e.Cancel = true;
        };
        
        var processor = new RestApiProcessor(apiUrl, cts.Token);
        
        if (!string.IsNullOrWhiteSpace(outputFile))
        {
            await processor.StreamApiResponseAsync(endpoint, outputFile);
        }
        else
        {
            await processor.ProcessApiEndpointAsync(endpoint);
        }
    }
    
    /// <summary>
    /// Process a REST API response as a typed object
    /// </summary>
    public async Task rest_typed<T>() where T : class
    {
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            WriteLine("API URL is required. Please specify using -apiUrl parameter.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            WriteLine("API endpoint is required. Please specify using -endpoint parameter.");
            return;
        }
        
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) => 
        {
            WriteLine("Canceling request...");
            cts.Cancel();
            e.Cancel = true;
        };
        
        var processor = new RestApiProcessor(apiUrl, cts.Token);
        
        try
        {
            T result = await processor.ProcessApiEndpointAsync<T>(endpoint);
            
            string json = pretty 
                ? JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }) 
                : JsonSerializer.Serialize(result);
            
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                await File.WriteAllTextAsync(outputFile, json);
                WriteLine($"Response saved to file: {outputFile}");
            }
            else
            {
                WriteLine(json);
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Post data to a REST API endpoint
    /// </summary>
    public async Task rest_post()
    {
        if (string.IsNullOrWhiteSpace(apiUrl))
        {
            WriteLine("API URL is required. Please specify using -apiUrl parameter.");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            WriteLine("API endpoint is required. Please specify using -endpoint parameter.");
            return;
        }
        
        if (file == null || !file.Exists)
        {
            WriteLine("Input file is required. Please specify using -file parameter.");
            return;
        }
        
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) => 
        {
            WriteLine("Canceling request...");
            cts.Cancel();
            e.Cancel = true;
        };
        
        var processor = new RestApiProcessor(apiUrl, cts.Token);
        
        try
        {
            string jsonRequest = await File.ReadAllTextAsync(file.FullName);
            var requestData = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonRequest);
            
            var response = await processor.PostToApiEndpointAsync<Dictionary<string, object>, Dictionary<string, object>>(
                endpoint, requestData);
                
            string responseJson = pretty 
                ? JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true }) 
                : JsonSerializer.Serialize(response);
                
            if (!string.IsNullOrWhiteSpace(outputFile))
            {
                await File.WriteAllTextAsync(outputFile, responseJson);
                WriteLine($"Response saved to file: {outputFile}");
            }
            else
            {
                WriteLine(responseJson);
            }
        }
        catch (Exception ex)
        {
            WriteLine($"Error: {ex.Message}");
        }
    }
}
--
namespace pg.Models;

using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;

/// <summary>
/// Base class for API responses with pagination
/// </summary>
public class PaginatedResponse<T>
{
    [JsonPropertyName("page")]
    public int Page { get; set; }
    
    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }
    
    [JsonPropertyName("total")]
    public int Total { get; set; }
    
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
    
    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
}

/// <summary>
/// Example model for a user in an API response
/// </summary>
public class UserModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("email")]
    public string Email { get; set; }
    
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }
    
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }
    
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; }
}

/// <summary>
/// Example model for financial market data
/// </summary>
public class MarketDataModel
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }
    
    [JsonPropertyName("volume")]
    public long Volume { get; set; }
    
    [JsonPropertyName("change")]
    public decimal Change { get; set; }
    
    [JsonPropertyName("change_percent")]
    public decimal ChangePercent { get; set; }
}
--
// Create an instance of RestApiProcessor with base URL
var processor = new RestApiProcessor("https://api.example.com");

// Process a simple endpoint
await processor.ProcessApiEndpointAsync("/data");
-
// For a paginated list of users
var userResponse = await processor.ProcessApiEndpointAsync<PaginatedResponse<UserModel>>("/users?page=1");

// Process the typed response
foreach (var user in userResponse.Data)
{
    Console.WriteLine($"User: {user.FirstName} {user.LastName} ({user.Email})");
}
-
// Create request data
var newUser = new UserModel
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com"
};

// Post the data and get the response
var createdUser = await processor.PostToApiEndpointAsync<UserModel, UserModel>("/users", newUser);
-
// Stream a large dataset to a file
await processor.StreamApiResponseAsync("/large-dataset", "output.json");
-
dotnet run -rest_process -apiUrl="https://api.example.com" -endpoint="/data"
dotnet run -rest_process -apiUrl="https://api.example.com" -endpoint="/large-dataset" -outputFile="output.json"
*/