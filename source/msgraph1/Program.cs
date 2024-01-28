// See https://aka.ms/new-console-template for more information Console.WriteLine("Hello, World!");

// https://learn.microsoft.com/en-us/graph/tutorials

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

using static System.Console;

record Config
{
    static Config()
    {
        var location = typeof(Exe).Assembly.Location;
        var config_builder = new ConfigurationBuilder();
        config_builder.SetBasePath(System.IO.Path.GetDirectoryName(location));
        config_builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        var config = config_builder.Build();
        var config_section = config.GetRequiredSection("Config");
        TenantId = config_section?.GetValue<string>("TenantId");
        ClientId = config_section?.GetValue<string>("ClientId");
        ClientSecret = config_section?.GetValue<string>("ClientSecret");
    }

    public static string ClientId { get; }
    public static string ClientSecret { get; }
    public static string TenantId { get; }
}

class Exe
{
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
        /*var user = await office365client.Me.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = ["displayName", "jobTitle"];
        });*/
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

    /*static async Task SendMail(GraphServiceClient office365client)
    {
        //office365client.Me.Messages.GetAsync()
        //office365client.Me.SendMail()
    }*/

    static async Task Main()
    //static void Main()
    {
        try
        {
            //ClientSecretCredential credential = new(Config.TenantId, Config.ClientId, Config.ClientSecret);
            AuthorizationCodeCredential credential = new(Config.TenantId, Config.ClientId, Config.ClientSecret, ""); //[Level 0] Azure.Identity.AuthenticationFailedException: AuthorizationCodeCredential authentication failed: AuthorizationCode can not be null or whitespace (Parameter 'AuthorizationCode')

            //EnvironmentCredential credential = new();//[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: /me request is only valid with delegated authentication flow.
            //[Level 0] Azure.Identity.CredentialUnavailableException: EnvironmentCredential authentication unavailable. Environment variables are not fully configured. See the troubleshooting guide for more information. https://aka.ms/azsdk/net/identity/environmentcredential/troubleshoot
            
            GraphServiceClient office365client = new(credential, ["https://graph.microsoft.com/.default"]);

            //await GetUserInfo(office365client);//[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: Insufficient privileges to complete the operation.
            await GetCurrentUserInfo(office365client);//[Level 0] Microsoft.Graph.Models.ODataErrors.ODataError: /me request is only valid with delegated authentication flow.
            //await GetMail(office365client);
            //await SendMail(office365client);
        }
        catch(Exception ex){for(int level=0;ex!=null;ex=ex.InnerException,++level)WriteLine($"[Level {level}] {ex.GetType().FullName}: {ex.Message}\n");}
    }
}