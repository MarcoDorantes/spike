/*
GraphServiceClient graphClient = new GraphServiceClient( authProvider );

var messages = await graphClient.Me.Messages
	.Request()
	.Top(4)
	.GetAsync();

Build .NET Core apps with Microsoft Graph
https://docs.microsoft.com/en-us/graph/tutorials/dotnet-core
https://docs.microsoft.com/en-us/graph/tutorials/dotnet-core?tutorial-step=1
https://docs.microsoft.com/en-us/graph/tutorials/dotnet-core?tutorial-step=2

https://login.microsoftonline.com/common/oauth2/nativeclient

https://docs.microsoft.com/en-us/azure/active-directory/develop/reference-aadsts-error-codes
Unhandled exception. System.AggregateException: One or more errors occurred. (DeviceCodeCredential authentication failed: AADSTS50059: No tenant-identifying information found in either the request or implied by any provided credentials.
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GraphTutorial
{
    class Program
    {
        static IConfigurationRoot LoadAppSettings()
        {
            var appConfig = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            // Check for required settings
            if (string.IsNullOrEmpty(appConfig["appId"]) ||
                string.IsNullOrEmpty(appConfig["scopes"]))
            {
                return null;
            }

            return appConfig;
        }
        static void Main(string[] args)
        {
            Console.WriteLine(".NET Core Graph Tutorial\n");

            var appConfig = LoadAppSettings();

            if (appConfig == null)
            {
                Console.WriteLine("Missing or invalid appsettings.json...exiting");
                return;
            }

            var appId = appConfig["appId"];
            var scopesString = appConfig["scopes"];
            var scopes = scopesString.Split(';');

            // Initialize Graph client
            GraphHelper.Initialize(appId, scopes, (code, cancellation) => {
                Console.WriteLine($"Initialize Graph client code.Message: {code.Message}");
                return Task.FromResult(0);
            });

            var accessToken = GraphHelper.GetAccessTokenAsync(scopes).Result;

            int choice = -1;

            while (choice != 0) {
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Display access token");
                Console.WriteLine("2. View this week's calendar");
                Console.WriteLine("3. Add an event");

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }
                catch (System.FormatException)
                {
                    // Set to invalid value
                    choice = -1;
                }

                switch(choice)
                {
                    case 0:
                        // Exit the program
                        Console.WriteLine("Goodbye...");
                        break;
                    case 1:
                        // Display access token
                        Console.WriteLine($"Access token: {accessToken}\n");
                        break;
                    case 2:
                        // List the calendar
                        break;
                    case 3:
                        // Create a new event
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Please try again.");
                        break;
                }
            }
        }
    }
}