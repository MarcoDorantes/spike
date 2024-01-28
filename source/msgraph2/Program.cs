// See https://aka.ms/new-console-template for more information Console.WriteLine("Hello, World!");

/*
https://stackoverflow.com/questions/68633304/ms-graph-daemon-app-obtaining-bearer-token
https://github.com/Azure-Samples/active-directory-dotnetcore-daemon-v2/tree/master/1-Call-MSGraph
https://stackoverflow.com/questions/59985143/sending-mail-using-a-daemon-application
*/
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using System;
using System.Threading.Tasks;

using static System.Console;

class Exe
{/*
    static async Task GetUserInfo(GraphServiceClient office365client)
    {
        UserCollectionResponse userPage = await office365client.Users.GetAsync(config => {
            // Only request specific properties
            config.QueryParameters.Select = new[] { "displayName", "id", "mail" };
            // Get at most 25 results
            config.QueryParameters.Top = 25;
            // Sort by display name
            config.QueryParameters.Orderby = new[] { "displayName" };
        });
        
        foreach (var user in userPage.Value)
        {
            WriteLine($"User: {user.DisplayName ?? "NO NAME"}");
            WriteLine($"  ID: {user.Id}");
            WriteLine($"  Email: {user.Mail ?? "NO EMAIL"}");
        }

        // If NextPageRequest is not null, there are more users available on the server
        // Access the next page like:
        // var nextPageRequest = new UsersRequestBuilder(userPage.OdataNextLink, _appClient.RequestAdapter);
        // var nextPage = await nextPageRequest.GetAsync();
        var moreAvailable = !string.IsNullOrEmpty(userPage.OdataNextLink);
        WriteLine($"\nMore users available? {moreAvailable}");
    }

    static async Task GetCurrentUserInfo(GraphServiceClient office365client)
    {
        // GET https://graph.microsoft.com/v1.0/me
        var user_ = await office365client.Me.GetAsync();
        WriteLine(user_.DisplayName);

        // GET https://graph.microsoft.com/v1.0/me?$select=displayName,jobTitle
        var user = await office365client.Me.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["displayName", "jobTitle"];
        });
    }

    static async Task GetMail(GraphServiceClient office365client)
    {
        //https://learn.microsoft.com/en-us/graph/sdks/create-requests
        //https://learn.microsoft.com/en-us/graph/api/user-list-messages?view=graph-rest-1.0&tabs=http

        // GET https://graph.microsoft.com/v1.0/me/messages?$select=subject,sender&$filter=subject eq 'Hello world'
        var messages = await office365client.Me.Messages.GetAsync(requestConfig =>
        {
            requestConfig.QueryParameters.Select =
                ["subject", "sender"];
            requestConfig.QueryParameters.Filter =
                "subject eq 'Hello world'";
        });
    }
*/
    /*static async Task SendMail(GraphServiceClient office365client)
    {
        //office365client.Me.Messages.GetAsync()
        //office365client.Me.SendMail()
    }*/

    static async Task AsFound()
    {
        // Get the Token acquirer factory instance. By default it reads an appsettings.json
        // file if it exists in the same folder as the app (make sure that the 
        // "Copy to Output Directory" property of the appsettings.json file is "Copy if newer").
        TokenAcquirerFactory tokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance();

        // Configure the application options to be read from the configuration
        // and add the services you need (Graph, token cache)
        IServiceCollection services = tokenAcquirerFactory.Services;
        services.AddMicrosoftGraph();
        // By default, you get an in-memory token cache.
        // For more token cache serialization options, see https://aka.ms/msal-net-token-cache-serialization

        // Resolve the dependency injection.
        var serviceProvider = tokenAcquirerFactory.Build();

        // Call Microsoft Graph using the Graph SDK
        try
        {
            GraphServiceClient graphServiceClient = serviceProvider.GetRequiredService<GraphServiceClient>();
            var users = await graphServiceClient.Users
                .GetAsync(r => r.Options.WithAppOnly());
            Console.WriteLine($"{users.Value.Count} users");
        }
        catch (ServiceException e)
        {
            Console.WriteLine("We could not retrieve the user's list: " + $"{e}");

            // If you get the following exception, here is what you need to do
            // ---------------------------------------------------------------
            //  IDW10503: Cannot determine the cloud Instance.
            //    Provide the configuration (appsettings.json with an "AzureAd" section, and "Instance" set,
            //    the project needs to be this way)
            // <ItemGroup>
            //  < None Update = "appsettings.json" >
            //    < CopyToOutputDirectory > PreserveNewest </ CopyToOutputDirectory >
            //  </ None >
            // </ ItemGroup >
            // System.ArgumentNullException: Value cannot be null. (Parameter 'tenantId')
            //    Provide the TenantId in the configuration
            // Microsoft.Identity.Client.MsalClientException: No ClientId was specified.
            //    Provide the ClientId in the configuration
            // ErrorCode: Client_Credentials_Required_In_Confidential_Client_Application
            //    Provide a ClientCredentials section containing either a client secret, or a certificate
            //    or workload identity federation for Kubernates if your app runs in AKS
        }
    }
    static async Task Main()
    //static void Main()
    {
        try
        {
            //await AsFound()//[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: Insufficient privileges to complete the operation.


            // Get the Token acquirer factory instance. By default it reads an appsettings.json
            // file if it exists in the same folder as the app (make sure that the 
            // "Copy to Output Directory" property of the appsettings.json file is "Copy if newer").
            TokenAcquirerFactory tokenAcquirerFactory = TokenAcquirerFactory.GetDefaultInstance();

            // Configure the application options to be read from the configuration
            // and add the services you need (Graph, token cache)
            IServiceCollection services = tokenAcquirerFactory.Services;
            services.AddMicrosoftGraph();
            // By default, you get an in-memory token cache.
            // For more token cache serialization options, see https://aka.ms/msal-net-token-cache-serialization

            // Resolve the dependency injection.
            var serviceProvider = tokenAcquirerFactory.Build();

            // Call Microsoft Graph using the Graph SDK
            GraphServiceClient office365client = serviceProvider.GetRequiredService<GraphServiceClient>();
            var users = await office365client.Users.GetAsync(r => r.Options.WithAppOnly());//[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: Insufficient privileges to complete the operation.
            WriteLine(users.OdataCount);
          //var user_ = await office365client.Me.GetAsync(r => r.Options.WithAppOnly()); //[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: /me request is only valid with delegated authentication flow.
          //WriteLine(user_.DisplayName);
        }
        catch(Exception ex){for(int level=0;ex!=null;ex=ex.InnerException,++level)WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}\n");}
    }
}/*
https://stackoverflow.com/questions/62697391/can-a-ms-graph-background-daemon-app-impersonate-a-user-account-without-user-int
https://learn.microsoft.com/en-us/entra/identity-platform/sample-v2-code?tabs=apptype
.NET Core	Call Microsoft Graph by signing in users using username/password	MSAL.NET	Resource owner password credentials
https://github.com/azure-samples/active-directory-dotnetcore-console-up-v2

send email
https://learn.microsoft.com/en-us/graph/api/user-sendmail?view=graph-rest-1.0&tabs=csharp
https://learn.microsoft.com/en-us/answers/questions/43724/sending-emails-from-daemon-app-using-graph-api-on

https://learn.microsoft.com/en-us/answers/questions/869759/background-mail-sending-service-using-microsoft-ac

https://frankchen2016.medium.com/how-to-allow-aad-app-with-application-permissions-to-access-specific-email-boxes-ce4552fb7f5c
https://www.linkedin.com/pulse/reading-o365-emails-from-daemon-process-using-graph-bhattacharya

$url = 'https://login.microsoftonline.com/$($tenantid)/oauth2/v2.0/token'
$body = @{client_id = ''; scope = 'https://graph.microsoft.com/.default'; client_secret = ''; grant_type = 'client_credentials' }
$response = Invoke-RestMethod $url -Method 'POST' -Body $body -SkipHttpErrorCheck -StatusCodeVariable http_code
$response.access_token

https://graph.microsoft.com/v1.0/users/{object id}/messages?$filter=isRead ne true&$count=true
https://graph.microsoft.com/v1.0/users/{object id}/messages?$filter=isRead ne true&$count=true

https://www.youtube.com/watch?v=C0hjEja-vPA
*/