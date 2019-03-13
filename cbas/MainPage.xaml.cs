using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace cbas
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string deviceConnectionString = "HostName=kmithub.azure-devices.net;DeviceId=Device1;SharedAccessKey=Tgv78+M2f0C3GkNqzsuwun7E9dKmYUYoUGKgJyUZ8Y0=";
        public static async Task SendDeviceToCloudMessageAsync()
        {
            var deviceId = "Device1";
            DateTime dt = DateTime.Now;
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

#if WINDOWS_UWP
            var str = deviceId+","+dt;
#else
        var str = "Hello, Cloud from a C# app!";
#endif
            var message = new Message(Encoding.ASCII.GetBytes(str));

            await deviceClient.SendEventAsync(message);
        }
        public static async Task<string> ReceiveCloudToDeviceMessageAsync()
        {
            var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    await deviceClient.CompleteAsync(receivedMessage);
                    return messageData;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
            receive();
        }

        private void sendMsg(object sender, RoutedEventArgs e)
        {
            SendDeviceToCloudMessageAsync();
        }
         public async Task receive()
        {
            String s = await ReceiveCloudToDeviceMessageAsync();
            textBox.Text = s;
            await receive();
        }
    }
}
