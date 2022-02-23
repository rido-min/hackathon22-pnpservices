using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp.IoT_Platforms
{
    internal class AWSIotCore : IIotPlatform
    {
        public Task<ObservableCollection<BaseDevice>> GetDevicesAsync()
        {
            throw new NotImplementedException();
        }

        public CommandResponse SendCommand(string deviceId, string commandName, object payload)
        {
            throw new NotImplementedException();
        }

        public CommandResponse SendCommand(string deviceId, string commandName, string componentName, object payload)
        {
            throw new NotImplementedException();
        }

        public void SetDesiredProperty(string deviceId, string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public void SetDesiredProperty(string deviceId, string propertyName, string componentName, object value)
        {
            throw new NotImplementedException();
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, PropertyChangedCallback callback)
        {
            throw new NotImplementedException();
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, string componentName, PropertyChangedCallback callback)
        {
            throw new NotImplementedException();
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, TelemetryCallback callback)
        {
            throw new NotImplementedException();
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, string componentName, TelemetryCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
