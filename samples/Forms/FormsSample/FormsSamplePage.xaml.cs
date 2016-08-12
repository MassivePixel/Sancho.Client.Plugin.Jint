using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Sancho.Client.Core;
using Sancho.Client.Plugin.Jint;
using Xamarin.Forms;

namespace FormsSample
{
    public partial class FormsSamplePage : ContentPage
    {
        Connection connection;

        public FormsSamplePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            Connection.ProtocolUrl = "http://sanchoprotocol-dev.azurewebsites.net/signalr";

            connection = new Connection("00000000-0000-0000-0000-000000000000");
            var jint = new JintPlugin(connection,
                                      HandleAction,
                                      assemblies: typeof(Color).GetTypeInfo().Assembly
                                     );

            jint.Engine.SetValue("ContentRoot", ContentRoot);
            jint.Engine.SetValue("Info", Info);
            jint.Engine.SetValue("log", new Action<object>(o => Debug.WriteLine(o.ToString())));
            connection.AddPlugin(jint);
            WriteLine($"{connection.DeviceId} connecting!");

            var status = await connection.ConnectAsync();
            WriteLine($"connected: {status}");

            //connection.On<Message>("Receive", m =>
            //{
            //    WriteLine(m.command);
            //});
        }

        void HandleAction(Action obj)
        {
            Device.BeginInvokeOnMainThread(obj);
        }

        void WriteLine(string text)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Info.Text = text;
            });
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            ContentRoot.BackgroundColor = Color.Red;
            Info.Text = "hello";
        }
    }
}
