using System;
using System.Diagnostics;
using System.Reflection;
using Jint;
using Sancho.Client.Core;

namespace Sancho.Client.Plugin.Jint
{
    public class JintPlugin : IPlugin
    {
        Connection connection;

        public string Name => "plugin-jint";

        public Engine Engine { get; }

        public JintPlugin(Connection connection, params Assembly[] assemblies)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            this.connection = connection;
            Engine = new Engine(cfg => cfg.AllowClr(assemblies));
        }

        public void Recieve(Message message)
        {
            switch (message.command)
            {
                case "execute":
                    {
                        try
                        {
                            Engine.Execute(message.data as string);
                        }
                        catch (Exception ex)
                        {
                            connection.SendAsync(Name, "execute-error", new
                            {
                                error = ex.Message,
                                exception = ex
                            });

                            Debug.WriteLine($"Exception occurred: {ex}");
                        }
                        break;
                    }
                case "get-value":
                    {
                        try
                        {
                            var value = Engine.GetValue((message.data ?? string.Empty).ToString());
                            connection.SendAsync(Name, "value", value.ToString());
                        }
                        catch (Exception ex)
                        {
                            connection.SendAsync(Name, "execute-error", new
                            {
                                error = ex.Message,
                                exception = ex
                            });

                            Debug.WriteLine($"Exception occurred: {ex}");
                        }
                        break;
                    }
            }
        }
    }
}
