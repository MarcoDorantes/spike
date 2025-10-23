namespace HttpPayloadRequest;

using System.Net.Http;
using System.Threading;
//using System.Net.Http.Json;
using System.Threading.Tasks;

public class HttpPayloadReader
{
    public async Task<string> GetString(string uri, CancellationToken cancel)
    {
        using HttpClient client = new();
        //return await client.GetStringAsync(uri);

        var response = await client.GetAsync(uri, cancel);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancel);
    }
}