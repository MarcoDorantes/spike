namespace HttpPayloadRequest;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;

public static class MessageComposer
{
    static async Task<IEnumerable<string>> GetCurrentSymbols(NameValueCollection appsettings)=>await Task.FromResult(new string[]{""});
    static async Task<string> GetISIN(string symbol)=>await Task.FromResult("");
    static async Task<string> GetSymbolSuffix(string symbol)=>await Task.FromResult("");
    static async Task<long> GetSharesOutstanding(string symbol)=>await Task.FromResult(0L);
    static async Task<double> GetReferencePrice(string symbol)=>await Task.FromResult(0D);

    public static async IAsyncEnumerable<IDictionary<string,object>> SL_MessageComposer(ISourceProcessorHost SourceHost, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken Cancellation, NameValueCollection appsettings)
    {
        var symbols = await GetCurrentSymbols(appsettings);
        foreach(var symbol in symbols)
        {
            if (Cancellation.IsCancellationRequested) yield break;
            Dictionary<string,object> next=null;
            try
            {
                next=new()
                {
                    {"isin_code",await GetISIN(symbol)},
                    {"issue",symbol},
                    {"series",await GetSymbolSuffix(symbol)},
                    {"quantity_issued",GetSharesOutstanding(symbol)},
                    {"reference_price",GetReferencePrice(symbol)},
                    {"message_type","SecurityListUS"},
                    {"service_name",nameof(SL_MessageComposer)}
                };
            }catch(Exception ex){SourceHost.Error(ex,nameof(SL_MessageComposer));}
            if(next != null) yield return next;
        }
    }
}