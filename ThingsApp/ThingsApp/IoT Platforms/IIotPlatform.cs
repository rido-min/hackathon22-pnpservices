using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp.IoT_Platforms
{
    public delegate void TelemetryCallback(TelemetryData data);
    public delegate void PropertyChangedCallback(PropertyData data);
    public interface IIotPlatform
    {
        abstract Task<ObservableCollection<BaseDevice>> GetDevicesAsync();
        abstract void SubscribeTelemetry(string deviceId, string telemetryName, TelemetryCallback callback);
        abstract void SubscribeTelemetry(string deviceId, string telemetryName, string componentName, TelemetryCallback callback);
        abstract void SubscribePropertyChange(string deviceId, string propertyName, PropertyChangedCallback callback);
        abstract void SubscribePropertyChange(string deviceId, string propertyName, string componentName, PropertyChangedCallback callback);
        abstract void SetDesiredProperty(string deviceId, string propertyName, object value);
        abstract void SetDesiredProperty(string deviceId, string propertyName, string componentName, object value);
        abstract CommandResponse SendCommand(string deviceId, string commandName, object payload);
        abstract CommandResponse SendCommand(string deviceId, string commandName, string componentName, object payload);
    }

    public class TelemetryData
    {
        public string? DeviceId { get; set; }
        public string? Name { get; set; }
        public string? Component { get; set; }
        public DateTime TimeStamp { get; set; }
        public JsonElement Value { get; set; }
    }

    public class PropertyData
    {
        public string? DeviceId { get; set; }
        public string? Name { get; set; }
        public string? Component { get; set; }
        public JsonElement Value { get; set; }
    }
    public class CommandResponse
    {
        object Response;
    }
}
