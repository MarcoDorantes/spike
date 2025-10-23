namespace HttpPayloadRequest;

using System.Net.Http;
using System.Threading;
//using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class HttpPayloadReader
{
    public async Task<string> GetString(string uri, CancellationToken cancel)
    {
        using HttpClient client = new();
        var response = await client.GetAsync(uri, cancel);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancel);
    }

    public async Task<T> GetObject<T>(string uri, CancellationToken cancel)
    {
        using HttpClient client = new();
        /*var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = false,
            WriteIndented = false
        };*/
        return await client.GetFromJsonAsync<T>(uri, /*jsonOptions,*/ cancel);
    }
}