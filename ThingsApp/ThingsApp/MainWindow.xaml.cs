using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThingsApp.IoT_Platforms;

namespace ThingsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem? item = (sender as ComboBox).SelectedItem as ComboBoxItem;
            switch (item.Name)
            {
                case "cbCentral":
                    App.IotPlatform = new AzureIotCentral(
                        "", // base address
                        "", // eventhub connection string
                        "", // consumer group
                        "" // API key
                        );
                    break;
                case "cbHub":
                    App.IotPlatform = new AzureIotHub(
                        "", // IoT Hub connection string
                        "", // eventhub endpoint
                        "", // consumer group
                        "", // iothub name
                        "" // service saskey
                        );
                    break;
                case "cbAWS":
                    App.IotPlatform = new AWSIotCore("HostName=a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com;ClientId=servicecert;Auth=X509;X509Key=servicecert.pfx|1234");
                    break;
                case "cbMQTT":
                    App.IotPlatform = new MqttBroker(
                        "HostName=broker.azure-devices.net;DeviceId=service;SharedAccessKey=MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA="
                        );
                    break;
                case "cbHive":
                    App.IotPlatform = null; // TODO
                    break;
            }

            await (this.DataContext as MainWindowViewModel).InitializeAsync();
        }
    }
}
