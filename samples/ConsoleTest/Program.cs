using System;
using Sancho.Client.Core;
using Sancho.Client.Plugin.Jint;

namespace ConsoleTest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            DoStuff();

            Console.ReadKey();
        }

        async static void DoStuff()
        {
            Connection.ProtocolUrl = "http://sanchoprotocol-dev.azurewebsites.net/signalr";

            var connection = new Connection("00000000-0000-0000-0000-000000000000");
            connection.AddPlugin(new JintPlugin(connection, assemblies: typeof(object).Assembly));
            Console.WriteLine($"{connection.DeviceId} connecting!");

            var status = await connection.ConnectAsync();
            Console.WriteLine($"connected: {status}");
            Console.WriteLine($"transport: {connection.HubConnection.Transport}");

            connection.HubConnection.TraceLevel = Microsoft.AspNet.SignalR.Client.TraceLevels.All;
            connection.HubConnection.Error += HubConnection_Error;
            connection.HubConnection.StateChanged += HubConnection_StateChanged;
            connection.HubConnection.Closed += HubConnection_Closed;

            connection.On<Message>("Receive", m =>
            {
                Console.WriteLine("Got message from server");
            });

            Console.ReadKey();

            Console.WriteLine("Disconnecting...");
            connection.Disconnect();
            Console.WriteLine("Disconnected");
        }

        static void HubConnection_Error(Exception obj)
        => Write(ConsoleColor.Red, obj.ToString());

        static void HubConnection_StateChanged(Microsoft.AspNet.SignalR.Client.StateChange obj)
        => Write(ConsoleColor.Gray, obj.NewState.ToString());

        static void HubConnection_Closed()
        => Write(ConsoleColor.White, "Closed");

        static void Write(ConsoleColor color, string text)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = oldColor;
        }
    }
}
