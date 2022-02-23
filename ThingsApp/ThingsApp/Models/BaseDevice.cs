using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ThingsApp.IoT_Platforms;

namespace ThingsApp.Models
{
    public class BaseDevice : INotifyPropertyChanged
    {
        private string? id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

        private string? modelId;
        public string ModelId
        {
            get { return modelId; }
            set
            {
                modelId = value;
                NotifyPropertyChanged();
            }
        }

        public BaseDevice(string deviceId)
        {
            Id = deviceId;
        }
        public BaseDevice(string deviceId, string dtmi)
        {
            Id = deviceId;
            ModelId = dtmi;
        }

        public static BaseDevice GetModeledDevice(string deviceId, string dtmi)
        {
            BaseDevice? device = null;
            switch (dtmi)
            {
                case "dtmi:pnd:demo:smartlightbulb;1":
                    device = new LightFixture(deviceId);
                    break;
                case "dtmi:azurertos:devkit:gsgmxchip;2":
                    device = new MxChip(deviceId);
                    break;
                case "dtmi:Espressif:SensorController;2":
                    device = new Esp32Kit(deviceId);
                    break;
                default:
                    device = new BaseDevice(deviceId);
                    break;
            }
            return device;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
