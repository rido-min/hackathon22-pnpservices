using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ThingsApp.IoT_Platforms;

namespace ThingsApp.Models
{
    public class LightFixture : BaseDevice
    {
        private int batteryLevel = 0;
        public int BatteryLevel
        {
            get { return batteryLevel; }
            set
            {
                batteryLevel = value;
                NotifyPropertyChanged();
            }
        }

        private bool desiredState;
        public bool DesiredState
        {
            get { return desiredState; }
            set
            {
                desiredState = value;
                App.IotPlatform.SetDesiredProperty(this.Id, "lightState", desiredState ? 1 : 0);
                NotifyPropertyChanged();
            }
        }

        private bool reportedState;
        public bool ReportedState
        {
            get { return reportedState; }
            set
            {
                reportedState = value;
                NotifyPropertyChanged();
            }
        }
        public LightFixture(string deviceId) : base(deviceId)
        {
            ModelId = "dtmi:pnd:demo:smartlightbulb;1";
            App.IotPlatform.SubscribePropertyChange(this.Id, "lightState", LightStateChanged);
            App.IotPlatform.SubscribeTelemetry(this.Id, "batteryLife", BatteryLifeReceived);
        }

        private void BatteryLifeReceived(TelemetryData data)
        {
            int bl = data.Value.GetInt32();
            BatteryLevel = bl;
        }

        private void LightStateChanged(PropertyData data)
        {
            int state;
            if (data.Value.ValueKind == JsonValueKind.Object)
            {
                state = data.Value.GetProperty("value").GetInt32();
            }
            else
            {
                state = data.Value.GetInt32();
            }
            
            ReportedState = (state == 1) ? true : false;
            
            // reset the UI for desired without sending a request to the device
            desiredState = ReportedState;
            NotifyPropertyChanged("DesiredState");
        }
    }
}
