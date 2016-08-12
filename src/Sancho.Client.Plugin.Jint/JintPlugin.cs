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

        public Action<Action> UIRunner { get; }

        public JintPlugin(Connection connection, Action<Action> uiRunner = null, params Assembly[] assemblies)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            UIRunner = uiRunner;

            this.connection = connection;
            Engine = new Engine(cfg => cfg.AllowClr(assemblies));
        }

        public void Recieve(Message message)
        {
            switch (message.command)
            {
                case "execute":
                    if (UIRunner != null)
                        UIRunner(() => OnExecute(message));
                    else
                        OnExecute(message);
                    break;

                case "get-value":
                    var value = Engine.GetValue((message.data ?? string.Empty).ToString());
                    connection.SendAsync(Name, "value", value.ToString());
                    break;
            }
        }

        void OnExecute(Message message)
        {
            try
            {
                Engine.Execute(message.data as string);
                var value = Engine.GetCompletionValue();
                if (!value.IsNull())
                {
                    connection.SendAsync(Name, "last-value", new
                    {
                        type = value.Type.ToString(),
                        value = value.ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                LogError(message, ex);
            }
        }

        void LogError(Message message, Exception ex)
        {
            connection.SendAsync(Name, "error", new
            {
                message = message,
                error = ex.Message,
                exception = ex
            });

            Debug.WriteLine($"Exception occurred: {ex}");
        }
    }
}
