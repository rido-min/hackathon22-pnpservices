using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingsApp.IoT_Platforms;

namespace ThingsApp.Models
{
    internal class MxChip : BaseDevice
    {
        public MxChip(string deviceId) : base(deviceId)
        {
            ModelId = "dtmi:azurertos:devkit:gsgmxchip;2";
        }
    }
}
