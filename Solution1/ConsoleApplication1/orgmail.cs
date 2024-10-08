// xcopy /v C:\design\github_spike\spike\Solution1\ConsoleApplication1\bin\Release\ConsoleApplication1.exe \tools\mail\orgmail.exe
// xcopy /v C:\design\github_spike\spike\Solution1\ConsoleApplication1\bin\Release\ConsoleApplication1.pdb \tools\mail\orgmail.pdb
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using static System.Console;

/*
Exchange Online and Exchange development
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-server-development
https://learn.microsoft.com/en-us/exchange/client-developer/migrating-to-exchange-online-and-exchange-2013-technologies

Overview of Microsoft Graph
https://docs.microsoft.com/en-us/graph/overview
https://docs.microsoft.com/en-us/learn/paths/m365-msgraph-fundamentals
https://graph.microsoft.com
https://graph.microsoft.com/v1.0/me/messages
http://aka.ms/graph
https://developer.microsoft.com/en-us/graph
https://developer.microsoft.com/graph/graph-explorer
https://docs.microsoft.com/en-us/graph/sdks/sdks-overview
https://github.com/microsoftgraph/aspnet-snippets-sample/tree/main
https://docs.microsoft.com/en-us/graph/api/resources/mail-api-overview?view=graph-rest-1.0
https://docs.microsoft.com/en-us/graph/auth/auth-concepts
https://docs.microsoft.com/en-us/graph/best-practices-concept
https://docs.microsoft.com/en-us/graph/auth-v2-service
https://learn.microsoft.com/en-us/graph/tutorials/dotnet-app-only?tabs=aad
https://learn.microsoft.com/en-us/graph/quick-start-faq
https://learn.microsoft.com/en-us/graph/api/overview?view=graph-rest-1.0
https://learn.microsoft.com/en-us/graph/sdks/create-requests

Migrate Exchange Web Services (EWS) apps to Microsoft Graph
https://learn.microsoft.com/en-us/graph/migrate-exchange-web-services-overview

Azure.Identity NuGet
https://www.nuget.org/packages/Azure.Identity
https://github.com/Azure/azure-sdk-for-net/blob/Azure.Identity_1.10.4/sdk/identity/Azure.Identity/README.md
https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow
https://learn.microsoft.com/en-us/entra/identity-platform/sample-v2-code?tabs=apptype#daemon-applications

Microsoft.Identity.Web.GraphServiceClient
https://learn.microsoft.com/en-us/entra/identity-platform/sample-v2-code?tabs=apptype#daemon-applications
https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/1-Call-MSGraph
https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/blob/master/1-Call-MSGraph/daemon-console/Program.cs

https://aka.ms/AAgzk1b | https://docs.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/deprecation-of-basic-authentication-exchange-online
https://developer.microsoft.com/en-us/graph/blogs/upcoming-changes-to-exchange-web-services-ews-api-for-office-365/
https://techcommunity.microsoft.com/t5/exchange-team-blog/upcoming-changes-to-exchange-web-services-ews-api-for-office-365/ba-p/608055
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-april-2020-update/ba-p/1275508
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-july-update/ba-p/1530163
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-february-2021-update/ba-p/2111904
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-and-exchange-online-september-2021-update/ba-p/2772210
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-deprecation-in-exchange-online-may-2022/ba-p/3301866
https://learn.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/deprecation-of-basic-authentication-exchange-online
https://techcommunity.microsoft.com/t5/exchange-team-blog/basic-authentication-deprecation-in-exchange-online-september/ba-p/3609437

https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/authentication-and-ews-in-exchange
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth
https://docs.microsoft.com/en-us/azure/active-directory/develop/security-tokens
https://aka.ms/redirectUriMismatchError
https://github.com/gscales/EWS-BasicToOAuth-Info/blob/main/What%20to%20do%20with%20EWS%20Managed%20API%20PowerShell%20scripts%20that%20use%20Basic%20Authentication.md
https://docs.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/deprecation-of-basic-authentication-exchange-online
https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-v2-libraries
https://docs.microsoft.com/en-us/azure/active-directory/develop/console-app-quickstart?pivots=devlang-dotnet-core
https://help.askcody.com/retirement-for-basic-auth-for-exchange-online
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-trace-requests-responses-to-troubleshoot-ews-managed-api-applications
https://aka.ms/msal-net-token-cache-serialization
https://docs.microsoft.com/en-us/exchange/troubleshoot/client-connectivity/http-clients-connect-diabled-account
https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/wiki/High-availability#pro-active-token-renewal
https://docs.microsoft.com/en-us/azure/active-directory/develop/msal-authentication-flows
https://testconnectivity.microsoft.com/

https://stackoverflow.com/questions/32735584/o365-ews-managed-api-and-exchangeversion
https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/ews-schema-versions-in-exchange?redirectedfrom=MSDN

Introduction to GraphQL
aka.ms/graphql
https://graphql.org/learn/

EWS Managed API reference
https://docs.microsoft.com/en-us/exchange/client-developer/web-service-reference/ews-managed-api-reference-for-exchange
https://docs.microsoft.com/en-us/dotnet/api/microsoft.exchange.webservices.data?view=exchange-ews-api

Office 365 Management APIs overview
https://docs.microsoft.com/en-us/office/office-365-management-api/office-365-management-apis-overview

EWS OAuth scope given mailbox only
https://learn.microsoft.com/en-us/answers/questions/293684/exchange-server-0auth-authorization-for-single-mai
https://techcommunity.microsoft.com/t5/exchange-team-blog/application-access-policy-support-in-ews/ba-p/2110361
https://stackoverflow.com/questions/64871100/how-can-i-access-a-mailbox-with-restricted-permissions-through-ews-without-inter

exchange.SubscribeToPushNotifications()
https://learn.microsoft.com/en-us/exchange/client-developer/exchange-web-services/notification-subscriptions-mailbox-events-and-ews-in-exchange

<Access-expired-exception NextExpires='2023-06-21 + 24 months = Saturday, June 21, 2025'>
Microsoft.Identity.Client.MsalServiceException: A configuration issue is preventing authentication
check the error message from the server for details.
You can modify the configuration in the application registration portal.
See https://aka.ms/msal-net-invalid-client for details.
Original exception: AADSTS7000222: The provided client secret keys for app '<uuid>' are expired.
Visit the Azure portal to create new keys for your app: https://aka.ms/NewClientSecret,
or consider using certificate credentials for added security: https://aka.ms/certCreds.
Trace ID: <uuid>
Correlation ID: <uuid>
Timestamp: 2023-06-21 13:58:16Z
</Access-expired-exception>

Issues net48 vs net. MSGraph is next on Office365.
https://github.com/OfficeDev/ews-managed-api/issues/96
https://github.com/OfficeDev/ews-managed-api/blob/master/README.md
*/
static class orgmail
{
  class Input
  {
    private const string To = "to";
    private const string Bcc = "bcc";

    public bool excep_only, needs, html, confirm, expand, restart, soft, deleteditems, exact;
    public int count, pageSize, offset, read;
    public Microsoft.Exchange.WebServices.Data.OffsetBasePoint offsetBasePoint;
    public string subject, body, folder;
    public DateTime? received_begin, received_end;
    public FileInfo file, subjectfile, pack;
    public List<FileInfo> attachs;
    public bool ascii, utf7, utf8, utf32, unicode, latin1;
    public bool? allPages;

    private Microsoft.Exchange.WebServices.Data.ItemView view;
    private Microsoft.Exchange.WebServices.Data.SearchFilter filter;
    private Microsoft.Exchange.WebServices.Data.DeleteMode delete_mode;
    private string informative_subject;

    public void latest()
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Inbox";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      WriteLine($"subject{(exact?" (exact)":"")}: [{subject}]");

      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = string.IsNullOrWhiteSpace(subject) ? exchange.FindItems(target_folder.Id, view) : exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          ++count;
          if (count == read)
          {
            item.Load();
            WriteLine(item.Body.Text);
            return;
          }
          else if (read == 0)
          {
            WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{count}");
          }
        }
      }
    }
    public void excep()
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Clutter";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      if (subject != null)
      {
        WriteLine($"subject{(exact?" (exact)":"")}: [{subject}]");
      }
      if (informative_subject != null)
      {
        WriteLine($"informative subject: [{informative_subject}]");
      }

      int count = 0;
      bool moreItems = true;
      while (moreItems)
      {
        var found = exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{++count}");
        }
      }
    }
    public void clean()// -clean [-pageSize=200] [-restart] [-soft|-deleteditems] ["-subject=touch DC1 monitor" -received_begin=2023-07-14T14:00:00 -received_end=2023-07-15T00:00:00]
    {
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? true;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Clutter";

      if (soft) delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.SoftDelete;
      else if (deleteditems) delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.MoveToDeletedItems;
      else delete_mode = Microsoft.Exchange.WebServices.Data.DeleteMode.HardDelete;

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      var rand = new Random();
      SetView();
      SetFilter();
      SetReceivedFilter();
      WriteLine($"Folder: [{folder}]");
      if (subject != null)
      {
        WriteLine($"subject{(exact?" (exact)":"")}: [{subject}]");
      }
      if (informative_subject != null)
      {
        WriteLine($"informative subject: [{informative_subject}]");
      }
      WriteLine($"DeleteMode: {delete_mode}");
      if(received_begin.HasValue && received_end.HasValue)
      {
        WriteLine($"{nameof(received_begin)}: {received_begin:s}");
        WriteLine($"{nameof(received_end)}  : {received_end:s}");
      }
      if (confirm)
      {
        Write("Type 'YES' to DeleteItems: ");
        if (ReadLine() != "YES")
        {
          WriteLine("\nCancelled by the user: DeleteItems was not executed.");
          return;
        }
      }

      do
      {
        bool moreItems = true;
        while (moreItems)
        {
          var found = exchange.FindItems(target_folder.Id, filter, view);
          if (found.Any() == false) break;
          moreItems = found.MoreAvailable && _allpages;
          if (moreItems)
          {
            view.Offset += pageSize;
          }

          var responses = exchange.DeleteItems(found.Select(msg => msg.Id), delete_mode, Microsoft.Exchange.WebServices.Data.SendCancellationsMode.SendToNone, Microsoft.Exchange.WebServices.Data.AffectedTaskOccurrence.AllOccurrences);
          foreach (var g in responses.GroupBy(resp => resp.Result))
          {
            WriteLine($"{g.Key}: {g.Count()}");
          }
        }
        if (restart)
        {
          var sleep = rand.Next(30000, 60000);
          Write($"Waiting for {sleep / 1000:N0}s...");
          System.Threading.Thread.Sleep(sleep);
          WriteLine($"\nRestarting...");
          SetView();
        }
      } while (restart);
      WriteLine("Done.");
    }

    //.\tools\mail\orgmail.exe -send -subject="the here4" -file="doc111.htm"
    //.\tools\mail\orgmail.exe -send -subject="the here5" -pack="doc111.htm"
    private Encoding encoding;
    public void send()
    {
      if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[To])) throw new ArgumentException("There is no configured recipients.");
      var msg = new Microsoft.Exchange.WebServices.Data.EmailMessage(GetExchangeService());
      SetEncoding();
      WriteLine($"Encoding: {encoding}");
      msg.Subject = GetSubject();
      WriteLine($"Subject: [{msg.Subject}]");
      msg.Body = GetBody();
      if (expand)
      {
        WriteLine($"Body: [{msg.Body}]");
      }
      SetRecipients(msg);
      SetAttachments(msg);
      if (confirm)
      {
        Write("Type 'YES' to Send: ");
        if (ReadLine() != "YES")
        {
          WriteLine("\nCancelled by the user: Send was not executed.");
          return;
        }
      }
      msg.Send();
      WriteLine($"Send(): Done.");
    }
    public DirectoryInfo outdir;
    public void download()
    {
      if (outdir == null) outdir = new DirectoryInfo(Environment.CurrentDirectory);
      if (pageSize == 0) pageSize = 10;
      var _allpages = allPages ?? false;
      if (string.IsNullOrWhiteSpace(folder)) folder = "Inbox";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      SetView();
      SetFilter();

      WriteLine($"Folder: [{folder}]");
      WriteLine($"subject: [{subject}]");
      WriteLine($"outdir: [{outdir.FullName}]");

      bool moreItems = true;
      int count = 0;
      while (moreItems)
      {
        var found = string.IsNullOrWhiteSpace(subject) ? exchange.FindItems(target_folder.Id, view) : exchange.FindItems(target_folder.Id, filter, view);
        if (found.Any() == false) break;
        moreItems = found.MoreAvailable && _allpages;
        if (moreItems)
        {
          view.Offset += pageSize;
        }
        foreach (Microsoft.Exchange.WebServices.Data.EmailMessage item in found.OrderBy(i => i.DateTimeReceived))
        {
          ++count;
          item.Load();
          WriteLine($"\nIsRead:\t{item.IsRead}\nFrom:\t{item.From.Name}\nSubject:\t{item.Subject}\nReceived:\t{item.DateTimeReceived:s}\nCount:\t{count}");
          WriteLine($"Attachments:\t{item.Attachments.Count}");
          Dictionary<string, int> attach_names = [];
          foreach (var _attach in item.Attachments)
          {
            var attach = _attach as Microsoft.Exchange.WebServices.Data.FileAttachment;
            if (attach == null)
            {
              WriteLine($"Not supported attachment type ({_attach.GetType().FullName}");
              continue;
            }
            int attach_number = attach_names.Count + 1;
            var attach_name = attach_names.ContainsKey(attach.Name) ? $"{Path.GetFileNameWithoutExtension(attach.Name)}_{attach_number}{Path.GetExtension(attach.Name)}" : attach.Name;
            var downfile = Path.Combine(outdir.FullName, attach_name);
            Write($"DownloadTo: {downfile}");
            attach.Load(downfile);
            WriteLine();
            attach_names[attach_name] = 1;
          }
        }
      }
    }
    public void subs()
    {
      if (string.IsNullOrWhiteSpace(folder)) folder = "Clutter";

      var exchange = GetExchangeService();
      var target_folder = GetTargetFolder(exchange, folder);
      //Microsoft.Exchange.WebServices.Data.FolderId[] targetfolder = new[] { new Microsoft.Exchange.WebServices.Data.FolderId(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.Inbox) };
      Microsoft.Exchange.WebServices.Data.FolderId[] targetfolderid = new[] { target_folder.Id };
      var StreamingSubscription = exchange.SubscribeToStreamingNotifications(targetfolderid, Microsoft.Exchange.WebServices.Data.EventType.NewMail);
// Create a streaming connection to the service object, over which events are returned to the client.
// Keep the streaming connection open for 30 minutes.
      Microsoft.Exchange.WebServices.Data.StreamingSubscriptionConnection connection = new(exchange, 30);
      connection.AddSubscription(StreamingSubscription);
      connection.OnNotificationEvent += OnNotificationEvent;
      connection.OnDisconnect += OnDisconnect;
      connection.OnSubscriptionError += OnSubscriptionError;
      connection.Open();
      WriteLine($"{DateTime.Now:s} Press ENTER to end");ReadLine();
      void OnNotificationEvent(object sender, Microsoft.Exchange.WebServices.Data.NotificationEventArgs e)
      {
        int thread = System.Threading.Thread.CurrentThread.ManagedThreadId;
        int _count = System.Threading.Interlocked.Increment(ref count);
        var grouped = string.Join(" ",e.Events?.GroupBy(x=>$"[{x.EventType}|{x.GetType().FullName}]").Select(g=>$"{g.Key}:{g.Count()}"));
        var msgs = string.Join("\n\t",e.Events.OfType<Microsoft.Exchange.WebServices.Data.ItemEvent>().Select(item_event => {
            var msg = Microsoft.Exchange.WebServices.Data.EmailMessage.Bind(exchange, item_event.ItemId.UniqueId);
            var fileattachs = msg.Attachments.OfType<Microsoft.Exchange.WebServices.Data.FileAttachment>();
            var attachs = fileattachs?.Count() > 0? "\n\t\t"+string.Join("\n\t\t", fileattachs.Select(a=>$"{a.Name} ({getcontent(a).Length},{a.Size})")) : "";
            return $"({msg.Attachments.Count} attachs) {msg.Subject}{attachs}";
        }));
        WriteLine($"\n[{thread}/{_count}] {DateTime.Now:s} {nameof(OnNotificationEvent)} ({e.Events?.Count()}): {grouped}\n\t{msgs}");
      }
      static void OnDisconnect(object sender, Microsoft.Exchange.WebServices.Data.SubscriptionErrorEventArgs e){WriteLine($"{DateTime.Now:s} {nameof(OnDisconnect)}");}
      static void OnSubscriptionError(object sender, Microsoft.Exchange.WebServices.Data.SubscriptionErrorEventArgs e){WriteLine($"{DateTime.Now:s} {nameof(OnSubscriptionError)}");}
      static string getcontent(Microsoft.Exchange.WebServices.Data.FileAttachment attach)
      {
          using MemoryStream stream = new();
          attach.Load(stream);
          stream.Position = 0;
          using StreamReader reader = new(stream, System.Text.Encoding.UTF8);
          return reader.ReadToEnd();
      }
    }
    void SetEncoding()
    {
      if(ascii) encoding = Encoding.ASCII;
      else if (utf7) encoding = Encoding.UTF7;
      else if (utf8) encoding = Encoding.UTF8;
      else if (utf32) encoding = Encoding.UTF32;
      else if (unicode) encoding = Encoding.Unicode;
      else if (latin1) encoding = Encoding.GetEncoding("ISO-8859-1");
      else encoding = Encoding.UTF8;
    }
    string GetSubject()
    {
      var subjectline = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(subject)) subjectline.Append($"{subject}");
      if (pack?.Exists == true)
      {
        if (subjectline.Length > 0) subjectline.Append(" ");
        subjectline.Append($"{GetSubjectFromPackFile()}");
      }
      if (subjectfile?.Exists == true)
      {
        if (subjectline.Length > 0) subjectline.Append(" ");
        subjectline.Append($"{File.ReadAllText(subjectfile.FullName)}");
      }
      return $"{subjectline}";
    }
    Microsoft.Exchange.WebServices.Data.MessageBody GetBody()
    {
      var result = new Microsoft.Exchange.WebServices.Data.MessageBody();
      result.BodyType = html ? Microsoft.Exchange.WebServices.Data.BodyType.HTML : Microsoft.Exchange.WebServices.Data.BodyType.Text;
      WriteLine($"BodyType: {result.BodyType}");
      var payload = new StringBuilder();
      if (!string.IsNullOrWhiteSpace(body)) payload.AppendLine(body);
      if (pack?.Exists == true) payload.Append($"{GetBodyFromPackFile()}");
      if (file?.Exists == true) payload.Append(File.ReadAllText(file.FullName, encoding));
      result.Text = $"{payload}";
      WriteLine($"Payload length: {result.Text.Length}");
      return result;
    }
    string GetSubjectFromPackFile()
    {
      if (pack?.Exists == false) return string.Empty;
      var subjectpack = new StringBuilder();
      var after_first_line = false;
      using (var reader = new StreamReader(pack.FullName, encoding))
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          line = line.Trim();
          if (string.IsNullOrWhiteSpace(line)) break;
          if (after_first_line) subjectpack.AppendLine();
          subjectpack.Append(line);
          after_first_line = true;
        } while (true);
      return $"{subjectpack}";
    }
    string GetBodyFromPackFile()
    {
      const string attach_directive = "@attach=";

      if (pack?.Exists == false) return string.Empty;
      var bodypack = new StringBuilder();
      var isBody = false;
      using (var reader = new StreamReader(pack.FullName, encoding))
        do
        {
          var line = reader.ReadLine();
          if (line == null) break;
          if (!(isBody || string.IsNullOrWhiteSpace(line))) continue;
          if (!isBody && string.IsNullOrWhiteSpace(line)) { isBody = true; continue; };
          if (line.StartsWith(attach_directive))
          {
            var attach_file = new FileInfo(line.Substring(attach_directive.Length));
            if (attach_file.Exists)
            {
              if (attachs == null) attachs = new List<FileInfo>();
              attachs.Add(attach_file);
            }
            else bodypack.AppendLine($"[not found] {line}");
          }
          else bodypack.AppendLine(line);
        } while (true);
      return $"{bodypack}";
    }
    void SetRecipients(Microsoft.Exchange.WebServices.Data.EmailMessage msg)
    {
      ConfigurationManager.AppSettings[To]?.Trim().Split('|').ToList().ForEach(t => msg.ToRecipients.Add(t.Trim()));
      ConfigurationManager.AppSettings[Bcc]?.Trim().Split('|').ToList().ForEach(t => msg.BccRecipients.Add(t.Trim()));
    }
    void SetAttachments(Microsoft.Exchange.WebServices.Data.EmailMessage msg)
    {
      if (attachs?.Count > 0) attachs.ForEach(attach =>
      {
        if (attach.Exists)
        {
          Write($"\tAttaching {attach.FullName}...");
          msg.Attachments.AddFileAttachment(attach.FullName);
          WriteLine($"Done.");
        }
        else
        {
          WriteLine($"\tAttach not found: {attach.FullName}");
        }
      });
      WriteLine($"Attachments count: {msg.Attachments.Count}");
    }
    Microsoft.Exchange.WebServices.Data.ExchangeService GetExchangeService()
    {
      var ews_url = ConfigurationManager.AppSettings["EWS"];
      var address = ConfigurationManager.AppSettings["emailaddress"];

      var appId = GetAccess(ConfigurationManager.AppSettings["appId"]);
      var clientSecret = GetAccess(ConfigurationManager.AppSettings["clientSecret"]);
      var tenantId = GetAccess(ConfigurationManager.AppSettings["tenantId"]);

      /*var access = GetAccess();
      var exchange = new Microsoft.Exchange.WebServices.Data.ExchangeService(Microsoft.Exchange.WebServices.Data.ExchangeVersion.Exchange2013);
      exchange.Credentials = new Microsoft.Exchange.WebServices.Data.WebCredentials(address, access);
      exchange.Url = new Uri(ews_url);
      return exchange;*/

      Microsoft.Identity.Client.AuthenticationResult authResult = Get_a_token_with_apponly_auth(appId, clientSecret, tenantId).Result;
      //Microsoft.Identity.Client.AuthenticationResult authResult = Get_a_token_with_delegated_auth().Result;
      return Add_an_authentication_token_to_EWS_requests(authResult, ews_url, address, true);
    }
    string GetAccess(string key)
    {
      return
      //ReadLine();
      //System.Configuration.ConfigurationManager.AppSettings["access"];
      //StringCipher.Decrypt(System.Configuration.ConfigurationManager.AppSettings["access"], System.Configuration.ConfigurationManager.AppSettings["key"]);
      GetSecret(key);
    }
    string GetSecret(string key)
    {
      var x = new encod.sencod();
      return x.Open(key);
    }
    async Task<Microsoft.Identity.Client.AuthenticationResult> Get_a_token_with_delegated_auth()//https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#get-a-token-with-delegated-auth
    {
      Microsoft.Identity.Client.AuthenticationResult result=null;
      // Using Microsoft.Identity.Client 4.22.0

      // Configure the MSAL client to get tokens
      var pcaOptions = new Microsoft.Identity.Client.PublicClientApplicationOptions
      {
          ClientId = ConfigurationManager.AppSettings["appId"],
          TenantId = ConfigurationManager.AppSettings["tenantId"]
      };

      var pca = Microsoft.Identity.Client.PublicClientApplicationBuilder.CreateWithApplicationOptions(pcaOptions).Build();

      // The permission scope required for EWS access
      var ewsScopes = new string[] { "https://outlook.office365.com/EWS.AccessAsUser.All" };

      // Make the interactive token request
      var authResult = await pca.AcquireTokenInteractive(ewsScopes).ExecuteAsync();
      result=authResult;
      return result;
    }
    async Task<Microsoft.Identity.Client.AuthenticationResult> Get_a_token_with_apponly_auth(string appId, string clientSecret, string tenantId)//https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#get-a-token-with-app-only-auth
    {
      Microsoft.Identity.Client.AuthenticationResult result=null;
      // Using Microsoft.Identity.Client 4.22.0
      var cca = Microsoft.Identity.Client.ConfidentialClientApplicationBuilder
          .Create(appId)
          .WithClientSecret(clientSecret)
          .WithTenantId(tenantId)
          .Build();

      // The permission scope required for EWS access
      var ewsScopes = new string[] { "https://outlook.office365.com/.default" };

      //Make the token request
      var authResult = await cca.AcquireTokenForClient(ewsScopes).ExecuteAsync();
      result=authResult;
      return result;
    }
    Microsoft.Exchange.WebServices.Data.ExchangeService Add_an_authentication_token_to_EWS_requests(Microsoft.Identity.Client.AuthenticationResult authResult, string ews_url, string emailaddress, bool impersonation = false)//https://docs.microsoft.com/en-us/exchange/client-developer/exchange-web-services/how-to-authenticate-an-ews-application-by-using-oauth#add-an-authentication-token-to-ews-requests
    {
      // Configure the ExchangeService with the access token
      var ewsClient = new Microsoft.Exchange.WebServices.Data.ExchangeService();
      ewsClient.Url = new Uri(ews_url);
      ewsClient.Credentials = new Microsoft.Exchange.WebServices.Data.OAuthCredentials(authResult.AccessToken);

      if(!impersonation) return ewsClient;

      //Impersonate the mailbox you'd like to access.
      ewsClient.ImpersonatedUserId = new Microsoft.Exchange.WebServices.Data.ImpersonatedUserId(Microsoft.Exchange.WebServices.Data.ConnectingIdType.SmtpAddress, emailaddress);

      //Include x-anchormailbox header
      ewsClient.HttpHeaders.Add("X-AnchorMailbox", emailaddress);

      return ewsClient;
    }
    Microsoft.Exchange.WebServices.Data.Folder GetTargetFolder(Microsoft.Exchange.WebServices.Data.ExchangeService exchange, string foldername)
    {
      if (string.IsNullOrWhiteSpace(foldername))
      {
        throw new ArgumentNullException($"{nameof(foldername)}", "Folder name cannot be null or empty.");
      }
      Microsoft.Exchange.WebServices.Data.Folder target_folder = null;
      var stepline = foldername.Split('\\', '/', '|').Select(n => n.Trim()).Where(fname => string.IsNullOrWhiteSpace(fname) == false);
      Microsoft.Exchange.WebServices.Data.FolderId parentfolder = null;
      foreach (var step in stepline)
      {
        target_folder = listfolders(exchange, step, parentfolder);
        if (target_folder == null) throw new Exception($"Folder '{foldername}' not found.");
        parentfolder = target_folder.Id;
      }
      return target_folder;
    }
    Microsoft.Exchange.WebServices.Data.Folder listfolders(Microsoft.Exchange.WebServices.Data.ExchangeService exchange, string foldername, Microsoft.Exchange.WebServices.Data.FolderId parentfolder = null)
    {
      WriteLine($"\n{nameof(listfolders)}:");

      var view = new Microsoft.Exchange.WebServices.Data.FolderView(50);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.FolderSchema.Id,
          Microsoft.Exchange.WebServices.Data.FolderSchema.ChildFolderCount,
          Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName);

      var filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.IsEqualTo(Microsoft.Exchange.WebServices.Data.FolderSchema.DisplayName, foldername);

      var found = parentfolder == null ? exchange.FindFolders(Microsoft.Exchange.WebServices.Data.WellKnownFolderName.MsgFolderRoot, filter, view) : exchange.FindFolders(parentfolder, filter, view);
      foreach (Microsoft.Exchange.WebServices.Data.Folder f in found.Folders)
      {
        WriteLine($"[{f.DisplayName,-30}]");//\t{f.Id}
        if (f.DisplayName == foldername) return f;
      }
      return null;
    }
    void SetView()
    {
      view = new Microsoft.Exchange.WebServices.Data.ItemView(pageSize, offset, offsetBasePoint);
      view.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Id,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.From,
          Microsoft.Exchange.WebServices.Data.ItemSchema.Subject,
          Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived,
          Microsoft.Exchange.WebServices.Data.EmailMessageSchema.IsRead);
      view.OrderBy.Add(Microsoft.Exchange.WebServices.Data.ItemSchema.DateTimeReceived, Microsoft.Exchange.WebServices.Data.SortDirection.Descending);
    }
    void SetFilter()
    {
      informative_subject = null;
      if (string.IsNullOrWhiteSpace(subject) == false)
      {
        var containment_mode = exact ?
        Microsoft.Exchange.WebServices.Data.ContainmentMode.FullString :
        Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring;
        var subject_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, subject, containment_mode, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        filter = subject_filter;
      }
      else
      {
        var exceptionText_in_subject = "Exception";
        var needsText_in_subject = "needs help";
        var exception_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, exceptionText_in_subject, Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        var needs_help_filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.ContainsSubstring(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.Subject, needsText_in_subject, Microsoft.Exchange.WebServices.Data.ContainmentMode.Substring, Microsoft.Exchange.WebServices.Data.ComparisonMode.IgnoreCaseAndNonSpacingCharacters);
        var both = new Microsoft.Exchange.WebServices.Data.SearchFilter.SearchFilterCollection
        (
          Microsoft.Exchange.WebServices.Data.LogicalOperator.Or,
          exception_filter,
          needs_help_filter
        );
        filter = null;
        var visible_subject_exceptionFilter = $"'{exceptionText_in_subject}'";
        var visible_subject_needsFilter = $"'{needsText_in_subject}'";
        var default_subject_filter_label = "Default subject filter";
        if (excep_only && !needs)
        {
          filter = exception_filter;
          informative_subject = $"({default_subject_filter_label}: {visible_subject_exceptionFilter})";
        }
        else if (!excep_only && needs)
        {
          filter = needs_help_filter;
          informative_subject = $"({default_subject_filter_label}): {visible_subject_needsFilter})";
        }
        else
        {
          filter = both;
          informative_subject = $"({default_subject_filter_label}: {visible_subject_exceptionFilter} OR {visible_subject_needsFilter})";
        }
      }
    }
    void SetReceivedFilter()
    {
      if(!(received_begin.HasValue && received_end.HasValue)) return;
      Microsoft.Exchange.WebServices.Data.SearchFilter.IsGreaterThanOrEqualTo received_begin_filter = new(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.DateTimeReceived, received_begin.Value);
      Microsoft.Exchange.WebServices.Data.SearchFilter.IsLessThanOrEqualTo received_end_filter = new(Microsoft.Exchange.WebServices.Data.EmailMessageSchema.DateTimeReceived, received_end.Value);
      Microsoft.Exchange.WebServices.Data.SearchFilter.SearchFilterCollection received_filter = new(
        Microsoft.Exchange.WebServices.Data.LogicalOperator.And,
        received_begin_filter,
        received_end_filter
      );
      if(filter == null)
      {
        filter = received_filter;
      }
      else
      {
        filter = new Microsoft.Exchange.WebServices.Data.SearchFilter.SearchFilterCollection(Microsoft.Exchange.WebServices.Data.LogicalOperator.And, filter, received_filter);
      }
    }

    #region call StringCipher
    public string plain, secret, key;
    public void close()
    {
      WriteLine(StringCipher.Encrypt(plain, key));
    }
    public void open()
    {
      WriteLine(StringCipher.Decrypt(secret, key));
    }
    #endregion
  }

  /*
https://stackoverflow.com/questions/10168240/encrypting-decrypting-a-string-in-c-sharp
https://tekeye.uk/visual_studio/encrypt-decrypt-c-sharp-string
https://docs.microsoft.com/en-us/dotnet/standard/security/walkthrough-creating-a-cryptographic-application
  */
  public static class StringCipher
  {
    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    public static string Encrypt(string plainText, string passPhrase)
    {
      // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
      // so that the same Salt and IV values can be used when decrypting.
      var saltStringBytes = Generate256BitsOfRandomEntropy();
      var ivStringBytes = Generate256BitsOfRandomEntropy();
      var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
      using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
      {
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream())
            {
              using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
              {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                var cipherTextBytes = saltStringBytes;
                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                memoryStream.Close();
                cryptoStream.Close();
                return Convert.ToBase64String(cipherTextBytes);
              }
            }
          }
        }
      }
    }

    public static string Decrypt(string cipherText, string passPhrase)
    {
      // Get the complete stream of bytes that represent:
      // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
      var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
      // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
      var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
      // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
      var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
      // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
      var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

      using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
      {
        var keyBytes = password.GetBytes(Keysize / 8);
        using (var symmetricKey = new RijndaelManaged())
        {
          symmetricKey.BlockSize = 256;
          symmetricKey.Mode = CipherMode.CBC;
          symmetricKey.Padding = PaddingMode.PKCS7;
          using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
          {
            using (var memoryStream = new MemoryStream(cipherTextBytes))
            {
              using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
              {
                var plainTextBytes = new byte[cipherTextBytes.Length];
                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
              }
            }
          }
        }
      }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
      var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
      using (var rngCsp = new RNGCryptoServiceProvider())
      {
        // Fill the array with cryptographically secure random bytes.
        rngCsp.GetBytes(randomBytes);
      }
      return randomBytes;
    }
  }

  public static int _Main(string[] args)
  {
    int result = 1;
    if (args?.Length == 0) nutility.Switch.ShowUsage(typeof(Input));
    else {nutility.Switch.AsType<Input>(args); result = 0;}
    return result;
  }
}