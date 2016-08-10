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
            connection.AddPlugin(new JintPlugin(connection, typeof(object).Assembly));
            Console.WriteLine($"{connection.DeviceId} connecting!");

            var status = await connection.ConnectAsync();
            Console.WriteLine($"status: {status}");

            connection.On<Message>("Receive", m =>
            {
                Console.WriteLine("Got message from server");
            });

            Console.ReadKey();

            connection.Disconnect();
        }
    }
}
