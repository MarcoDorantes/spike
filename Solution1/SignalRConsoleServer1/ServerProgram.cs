using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Microsoft.Owin.Hosting;

using static System.Console;
using Microsoft.AspNet.SignalR;

namespace SignalRConsoleServer1
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    class ServerProgram
    {
        static IDisposable SignalR;
        static void start(string[] args)
        {
            const string ServerURI = "http://localhost:8080";
            SignalR = WebApp.Start(ServerURI);
        }
        static void stop() { SignalR.Dispose(); }
        static void Main(string[] args)
        {
            try
            {
                start(args);
                WriteLine("Press ENTER to exit"); ReadLine();
                stop();
            }
            catch (Exception ex) { WriteLine($"{ex.GetType().FullName}: {ex.Message}"); }
        }
    }

    /// <summary> 
    /// Echoes messages sent using the Send message by calling the 
    /// addMessage method on the client. Also reports to the console 
    /// when clients connect and disconnect. 
    /// </summary> 
    public class MyHub : Hub
    {
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }
        public override Task OnConnected()
        {
            WriteLine("Client connected: " + Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            WriteLine("Client disconnected: " + Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}