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

        public static IIotPlatform IotPlatform = new AzureIotHub(
            "", // IoT Hub connection string
            "", // eventhub endpoint
            "", // consumer group
            "", // iothub name
            "" // service saskey
        );
    }
}
