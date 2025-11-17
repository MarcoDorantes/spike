namespace HttpPayloadRequest;

using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Specialized;

public static class MessageComposer
{
    static async Task<IEnumerable<string>> GetCurrentSymbols(string uri,string suffix, CancellationToken cancel)
    {
        return await Task.FromResult(new string[]{"A","B","C"});
    }
    static async Task<string> GetISIN(string symbol,string uri,CancellationToken cancel)=>await getquote(uri,cancel);
    static async Task<string> GetSymbolSuffix(string symbol,string uri,CancellationToken cancel)=>await getquote(uri,cancel);
    static async Task<string> GetSharesOutstanding(string symbol,string uri,CancellationToken cancel)=>await getquote(uri,cancel);
    static async Task<string> GetReferencePrice(string symbol,string uri,CancellationToken cancel)=>await getquote(uri,cancel);

    public static async IAsyncEnumerable<IDictionary<string,object>> SL_MessageComposer(ISourceProcessorHost SourceHost, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken Cancellation, NameValueCollection appsettings)
    {
        var uri = appsettings["uri"];
        var symbols = await GetCurrentSymbols(appsettings["list"], appsettings["ASuffix"], Cancellation);
        foreach(var symbol in symbols)
        {
            if (Cancellation.IsCancellationRequested) yield break;
            Dictionary<string,object> next=null;
            try
            {
                next=new()
                {
                    {"isin_code",await GetISIN(symbol,uri,Cancellation)},
                    {"issue",symbol},
                    {"series",await GetSymbolSuffix(symbol,uri,Cancellation)},
                    {"quantity_issued",await GetSharesOutstanding(symbol,uri,Cancellation)},
                    {"reference_price",await GetReferencePrice(symbol,uri,Cancellation)},
                    {"message_type","SecurityListUS"},
                    {"service_name",nameof(SL_MessageComposer)}
                };
            }catch(Exception ex){SourceHost.Error(ex,nameof(SL_MessageComposer));}
            if(next != null) yield return next;
        }
    }

    static async Task<string> getstring(string uri,CancellationToken cancel)=>await HttpPayloadRequest.HttpPayloadReader.GetString(uri, cancel);
    static async Task<string> getquote(string uri,CancellationToken cancel)
    {
        var result = await HttpPayloadRequest.HttpPayloadReader.GetObject<Dictionary<string, JsonElement>>(uri, cancel);
        var quote = JsonSerializer.Deserialize<Dictionary<string, object>>(result["results"]);
        return $"{quote["P"]}";
    }
}

public class SymbolCatalog(HttpPayloadRequest.ISourceProcessorHost host, CancellationToken cancel, NameValueCollection appsettings):IObserver<int>,IObserver<IDictionary<string,object>>
{
    public int SymbolCount, ExceptionCount;
    public void Start()
    {
        _ = RequestGuardedAsync();
        /*
        RequestGuardedAsync().ContinueWith(t=>host.Error(t.Exception,nameof(SymbolCatalog)),TaskContinuationOptions.OnlyOnFaulted);
.method public hidebysig instance void  Start() cil managed
{
  // Code size       46 (0x2e)
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  call       instance class [System.Runtime]System.Threading.Tasks.Task HttpPayloadRequest.SymbolCatalog::RequestGuardedAsync()
  IL_0006:  ldarg.0
  IL_0007:  ldftn      instance void HttpPayloadRequest.SymbolCatalog::'<Start>b__4_0'(class [System.Runtime]System.Threading.Tasks.Task)
  IL_000d:  newobj     instance void class [System.Runtime]System.Action`1<class [System.Runtime]System.Threading.Tasks.Task>::.ctor(object,
                                                                                                                                     native int)
  IL_0012:  ldc.i4     0x50000
  IL_0017:  callvirt   instance class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::ContinueWith(class [System.Runtime]System.Action`1<class [System.Runtime]System.Threading.Tasks.Task>,
                                                                                                                                            valuetype [System.Runtime]System.Threading.Tasks.TaskContinuationOptions)
  IL_001c:  pop
  IL_001d:  ldarg.0
  IL_001e:  ldfld      class HttpPayloadRequest.ISourceProcessorHost HttpPayloadRequest.SymbolCatalog::'<host>P'
  IL_0023:  ldstr      "Started"
  IL_0028:  callvirt   instance void HttpPayloadRequest.ISourceProcessorHost::Information(string)
  IL_002d:  ret
} // end of method SymbolCatalog::Start


        _ = RequestGuardedAsync();
.method public hidebysig instance void  Start() cil managed
{
  // Code size       24 (0x18)
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  call       instance class [System.Runtime]System.Threading.Tasks.Task HttpPayloadRequest.SymbolCatalog::RequestGuardedAsync()
  IL_0006:  pop
  IL_0007:  ldarg.0
  IL_0008:  ldfld      class HttpPayloadRequest.ISourceProcessorHost HttpPayloadRequest.SymbolCatalog::'<host>P'
  IL_000d:  ldstr      "Started"
  IL_0012:  callvirt   instance void HttpPayloadRequest.ISourceProcessorHost::Information(string)
  IL_0017:  ret
} // end of method SymbolCatalog::Start


        Task.Run(RequestGuardedAsync);
        Task back = Task.Run(() => RequestGuardedAsync());
.method public hidebysig instance void  Start() cil managed
{
  // Code size       35 (0x23)
  .maxstack  8
  IL_0000:  ldarg.0
  IL_0001:  ldftn      instance class [System.Runtime]System.Threading.Tasks.Task HttpPayloadRequest.SymbolCatalog::'<Start>b__4_0'()
  IL_0007:  newobj     instance void class [System.Runtime]System.Func`1<class [System.Runtime]System.Threading.Tasks.Task>::.ctor(object,
                                                                                                                                   native int)
  IL_000c:  call       class [System.Runtime]System.Threading.Tasks.Task [System.Runtime]System.Threading.Tasks.Task::Run(class [System.Runtime]System.Func`1<class [System.Runtime]System.Threading.Tasks.Task>)
  IL_0011:  pop
  IL_0012:  ldarg.0
  IL_0013:  ldfld      class HttpPayloadRequest.ISourceProcessorHost HttpPayloadRequest.SymbolCatalog::'<host>P'
  IL_0018:  ldstr      "Started"
  IL_001d:  callvirt   instance void HttpPayloadRequest.ISourceProcessorHost::Information(string)
  IL_0022:  ret
} // end of method SymbolCatalog::Start
        */
        /*var try_count = 0;
        do
        {
            ++try_count;
            var status = back.Status;
            host.Information($"back task {status}");
            if (status == TaskStatus.Created)
            {
                if (try_count > 5) throw new NotSupportedException($"Task did not run ({status}) after {try_count} status checks.");
                Thread.Sleep(1000);
            }
            else break;
        } while (true);*/
        host.Information("Started");
    }
    private async Task RequestGuardedAsync()
    {
        do
        {
            //try
            //{
                await RequestAsync();
                break;
            /*}
            catch(System.Threading.Tasks.TaskCanceledException){host.Information($"{nameof(RequestGuardedAsync)} task cancelled.");}
            catch(Exception exception)
            {
                ++ExceptionCount;
                Exception ex = exception;
                StringBuilder logline = new();
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
                }
                host.Error(exception, $"{nameof(RequestGuardedAsync)} Exception: {logline}");
            }*/
        } while(true);
        host.Information($"{nameof(RequestGuardedAsync)} ended.");
    }
    private async Task RequestAsync()
    {
        int count=0;
        await foreach (var next in GetSymbols(appsettings["list"], appsettings["ASuffix"],cancel))
        {
            try
            {
                if(cancel.IsCancellationRequested) break;
                ++SymbolCount;
                host.Information($"{++count,7:N0} {next["ticker"]}");
            }
            catch(System.Threading.Tasks.TaskCanceledException){host.Information($"{nameof(RequestGuardedAsync)} task cancelled.");}
            catch(Exception exception)
            {
                ++ExceptionCount;
                Exception ex = exception;
                StringBuilder logline = new();
                for (int level = 0; ex != null; ex = ex.InnerException, ++level)
                {
                    logline.AppendLine($"\t[Level {level}] {ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}");
                }
                host.Error(exception, $"{nameof(RequestGuardedAsync)} Exception: {logline}");
            }
        }
        /*
        IAsyncEnumerable<IDictionary<string,object>> collection = GetSymbols(appsettings["list"], appsettings["ASuffix"],cancel);
        IAsyncEnumerator<IDictionary<string,object>> iterator = collection.GetAsyncEnumerator(cancel);
        int count=0;
        while(!cancel.IsCancellationRequested)
        {
            bool is_next = await iterator.MoveNextAsync();
            if(!is_next) break;
            IDictionary<string,object> next = iterator.Current;
            ++SymbolCount;
            host.Information($"{++count,7:N0} {next["ticker"]}");
        }
        */
    }

    private static async IAsyncEnumerable<IDictionary<string, object>> GetSymbols(string uri, string suffix, [System.Runtime.CompilerServices.EnumeratorCancellation]CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            var response = await HttpPayloadRequest.HttpPayloadReader.GetObject<Dictionary<string, JsonElement>>(uri, cancel);
            var status = response["status"].GetString();
            var results = response["results"].EnumerateArray();
            foreach (var result in results)
            {
                var symbol = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
                if($"{symbol["ticker"]}"=="ACAD")throw new Exception("Faked");
                symbol.Add("status", status);
                yield return symbol;
            }
            if (response.TryGetValue("next_url", out JsonElement next_url) && next_url.ValueKind != JsonValueKind.Null)
            {
                uri = $"{next_url.GetString()}{suffix}";
            }
            else
            {
                yield break;
            }
        }
    }

    public void StartIterator()
    {
        IEnumerable<int> ints = getints();
        iter = ints.GetEnumerator();
        Task.Run(IterateGuarded);
        host.Information("Started");
    }
    IEnumerator<int> iter;
    private void IterateGuarded()
    {
        do
        {
            try
            {
                Iterate();
                break;
            }catch(Exception ex){ ++ExceptionCount; host.Error(ex,nameof(IterateGuarded));}
        }while(true);
    }
    private void Iterate()
    {
        host.Information($"{nameof(Iterate)} begin");
        while (!cancel.IsCancellationRequested)
        {
            host.Information($"{nameof(Iterate)} not cancelled");
            if(iter.MoveNext())
            {
                host.Information($"{iter.Current,-3:N0}");
                ++SymbolCount;
            } else break;
        }
        host.Information($"{nameof(Iterate)} end");
    }
    static IEnumerable<int> getints()
    {
        for(int k=0; k<10;++k)
        {
            if(k==4) throw new Exception("Faked");
            yield return k;
        }
    }
    IAsyncEnumerator<IDictionary<string,object>> async_iterator;
    public void StartIterator2()
    {
        host.Information(nameof(StartIterator2));
        IAsyncEnumerable<IDictionary<string,object>> collection = GetSymbols(appsettings["list"], appsettings["ASuffix"],cancel);
        async_iterator = collection.GetAsyncEnumerator(cancel);
        _ = Iterate2GuardedAsync();
        host.Information("Started");
    }
    private async Task Iterate2GuardedAsync()
    {
        do
        {
            try
            {
                await Iterate2Async();
                break;
            }catch(Exception ex){ ++ExceptionCount; host.Error(ex,nameof(IterateGuarded));}
        }while(true);
    }
    private async Task Iterate2Async()
    {
        host.Information($"{nameof(Iterate2Async)} begin");
        while (!cancel.IsCancellationRequested)
        {
            host.Information($"{nameof(Iterate2Async)} not cancelled");
            if(await async_iterator.MoveNextAsync())
            {
                var next = async_iterator.Current;
                ++SymbolCount;
                host.Information($"{SymbolCount,7:N0} {next["ticker"]}");
            } else break;
        }
        host.Information($"{nameof(Iterate2Async)} end");
    }
    class Numbers:IObservable<int>
    {
        CancellationToken cancel;
        public Numbers(int count, CancellationToken cancel)
        {
            observations = count;
            this.cancel=cancel;
        }
        class Unsubscriber(Numbers observable) : IDisposable
        {
            public void Dispose()
            {
                observable.observer=null;
            }
        }
        internal IObserver<int> observer;
        public IDisposable Subscribe(IObserver<int> observer)
        {
            this.observer = observer;
            return new Unsubscriber(this);
        }
        int observation_count,observations;
        public void StartObservableActivity()
        {
            while(!cancel.IsCancellationRequested)
            if(observation_count<observations)
            {
                try
                {
                    observer.OnNext(GetObservation());
                }
                catch(Exception ex){observer.OnError(ex);}
            }
            else {observer.OnCompleted();break;}
        }
        int GetObservation()
        {
            if(observation_count==4)
            {
                ++observation_count;
                throw new Exception("Faked");
            }
            return observation_count++;
        }
    }
    void IObserver<int>.OnNext(int n)
    {
        host.Information($"{n,-3:N0}");
        ++SymbolCount;
    }
    void IObserver<int>.OnError(Exception ex)
    {
        ++ExceptionCount;
        host.Error(ex,nameof(IObserver<int>.OnError));
    }
    void IObserver<int>.OnCompleted()
    {
        ints_unsubscription.Dispose();
        ints_unsubscription=null;
    }
    Numbers ints;
    IDisposable ints_unsubscription;
    public void StartObserver()
    {
        ints=new(10,cancel);
        ints_unsubscription=ints.Subscribe(this);
        Task.Run(ObserveGuarded);
        host.Information("Started");
    }
    private void ObserveGuarded()
    {
        do
        {
            try
            {
                Observe();
                break;
            }catch(Exception ex){ ++ExceptionCount; host.Error(ex,nameof(ObserveGuarded));}
        }while(true);
    }
    private void Observe()
    {
        ints.StartObservableActivity();
    }

    class QuoteProvider:IObservable<IDictionary<string,object>>
    {
        CancellationToken cancel;
        public QuoteProvider(CancellationToken cancel)
        {
            this.cancel=cancel;
        }
        class Unsubscriber(QuoteProvider observable) : IDisposable
        {
            public void Dispose()
            {
                observable.observer=null;
            }
        }
        internal IObserver<IDictionary<string,object>> observer;
        public IDisposable Subscribe(IObserver<IDictionary<string,object>> observer)
        {
            this.observer = observer;
            return new Unsubscriber(this);
        }
        public async Task StartObservableActivity(string uri,string suffix)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    var response = await HttpPayloadRequest.HttpPayloadReader.GetObject<Dictionary<string, JsonElement>>(uri, cancel);
                    var status = response["status"].GetString();
                    var results = response["results"].EnumerateArray();
                    foreach (var result in results)
                    {
                        try
                        {
                            var symbol = JsonSerializer.Deserialize<Dictionary<string, object>>(result);
                            if($"{symbol["ticker"]}"=="ACAD")throw new Exception("Faked");
                            symbol.Add("status", status);
                            observer.OnNext(symbol);
                        }
                        catch(Exception ex){observer.OnError(ex);}
                    }
                    if (response.TryGetValue("next_url", out JsonElement next_url) && next_url.ValueKind != JsonValueKind.Null)
                    {
                        uri = $"{next_url.GetString()}{suffix}";
                    }
                    else
                    {
                        break;
                    }
                }
                catch(Exception ex){observer.OnError(ex);}
            }
            observer.OnCompleted();
        }
    }
    void IObserver<IDictionary<string,object>>.OnNext(IDictionary<string,object> next)
    {
        ++SymbolCount;
        host.Information($"{SymbolCount,7:N0} {next["ticker"]}");
    }
    void IObserver<IDictionary<string,object>>.OnError(Exception ex)
    {
        ++ExceptionCount;
        host.Error(ex,nameof(IObserver<IDictionary<string,object>>.OnError));
    }
    void IObserver<IDictionary<string,object>>.OnCompleted()
    {
        quotes_unsubscription.Dispose();
        quotes_unsubscription=null;
    }
    QuoteProvider quote_provider;
    IDisposable quotes_unsubscription;
    public void StartObserver2()
    {
        quote_provider=new(cancel);
        quotes_unsubscription=quote_provider.Subscribe(this);
        _ = Observe2Guarded();
        host.Information("Started");
    }
    private async Task Observe2Guarded()
    {
        do
        {
            try
            {
                await Observe2();
                break;
            }catch(Exception ex){ ++ExceptionCount; host.Error(ex,nameof(ObserveGuarded));}
        }while(true);
    }
    private async Task Observe2()
    {
        await quote_provider.StartObservableActivity(appsettings["list"], appsettings["ASuffix"]);
    }
}