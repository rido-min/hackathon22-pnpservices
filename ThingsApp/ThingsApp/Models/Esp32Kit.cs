using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingsApp.IoT_Platforms;

namespace ThingsApp.Models
{
    internal class Esp32Kit : BaseDevice
    {
        public Esp32Kit(string deviceId) : base(deviceId)
        {
            ModelId = "dtmi:Espressif:SensorController;2";
        }
    }
}
