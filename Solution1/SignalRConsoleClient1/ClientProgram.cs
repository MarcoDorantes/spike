﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

using static System.Console;
using System.Net.Http;

/*
http://signalr.net/
https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-2.2
*/
namespace SignalRConsoleClient1
{
    class ClientProgram
    {
        const string ServerURI = "http://localhost:8080/signalr";
        static HubConnection Connection;
        static IHubProxy HubProxy;

        /// <summary> 
        /// Creates and connects the hub connection and hub proxy. This method 
        /// is called asynchronously from SignInButton_Click. 
        /// </summary> 
        private static async void ConnectAsync()
        {
            Connection = new HubConnection(ServerURI);
            Connection.Closed += () => WriteLine($"{getcontext()}You have been disconnected.");
            HubProxy = Connection.CreateHubProxy("MyHub");
            //Handle incoming event from server: use Invoke to write to console from SignalR's thread
            HubProxy.On<string, string>("AddMessage", (name, message) => WriteLine($"{getcontext()}Received from server: {name}: {message}"));
            try
            {
                await Connection.Start();
            }
            catch (HttpRequestException)
            {
                WriteLine($"{getcontext()}Unable to connect to server: Start server before connecting clients.");
                //No connection: Don't enable Send button or show chat UI 
                return;
            }
            WriteLine($"{getcontext()}Connected to server at " + ServerURI + "\r");
        }
        static void start(string[] args)
        {
            WriteLine($"{getcontext()}Connecting to server...");
            ConnectAsync();
        }
        static void send()
        {
            HubProxy.Invoke("Send", $"From this ({getcontext()}) context", $"{DateTime.Now.ToString("s")}");
        }
        static void stop()
        {
            Connection?.Stop();
            Connection?.Dispose();
        }
        static void Main(string[] args)
        {
            try
            {
                start(args);
                WriteLine($"{getcontext()}Press ENTER to send"); ReadLine();
                send();
                WriteLine($"{getcontext()}Press ENTER to exit"); ReadLine();
                stop();
            }
            catch (Exception ex) { WriteLine($"{getcontext()}{ex.GetType().FullName}: {ex.Message}"); }
        }
        static string getcontext() => $"[{System.Diagnostics.Process.GetCurrentProcess().Id}, {System.Threading.Thread.CurrentThread.ManagedThreadId}] ";
    }
}