using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ThingsApp.IoT_Platforms;

namespace ThingsApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static IIotPlatform IotPlatform = new AzureIotCentral(
        //    "", // base address
        //    "", // eventhub connection string
        //    "", // consumer group
        //    "" // API key
        //);

        //public static IIotPlatform IotPlatform = new AzureIotHub(
        //    "", // IoT Hub connection string
        //    "", // eventhub endpoint
        //    "", // consumer group
        //    "", // iothub name
        //    "" // service saskey
        //);
        //public static IIotPlatform IotPlatform = new AWSIotCore(
        //    "HostName=a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com;ClientId=servicecert;Auth=X509;X509Key=servicecert.pfx|1234"
        //    );
        public static IIotPlatform IotPlatform = new MqttBroker(
            "HostName=broker.azure-devices.net;DeviceId=service;SharedAccessKey=MDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDA="
            );
    }
}
