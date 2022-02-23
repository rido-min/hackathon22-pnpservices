using Azure.Messaging.EventHubs.Consumer;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp.IoT_Platforms
{
    internal class AzureIotHub : IIotPlatform
    {
        private ServiceClient? serviceClient = null;
        private RegistryManager? registryManager = null;
        private EventHubConsumerClient? eventHubConsumerClient = null;

        private Dictionary<string, TelemetryCallback> TelemetryCallbacks = new Dictionary<string, TelemetryCallback>();
        private Dictionary<string, PropertyChangedCallback> PropertyCallbacks = new Dictionary<string, PropertyChangedCallback>();

        public AzureIotHub(string iotHubConnectionString, string eventHubsCompatibleEndpoint, string consumerGroup, string eventHubsCompatiblePath, string iotHubSasKey)
        {
            serviceClient = ServiceClient.CreateFromConnectionString(iotHubConnectionString);
            registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

            string eventHubConnectionString = $"Endpoint={eventHubsCompatibleEndpoint.Replace("sb://", "amqps://")};EntityPath={eventHubsCompatiblePath};SharedAccessKeyName=service;SharedAccessKey={iotHubSasKey};";
            eventHubConsumerClient = new EventHubConsumerClient(consumerGroup, eventHubConnectionString);

            _ = InitialyzeAsync();
        }

        private async Task InitialyzeAsync()
        { 
            var tasks = new List<Task>();
            var partitions = await eventHubConsumerClient.GetPartitionIdsAsync();
            foreach (string partition in partitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition));
            }
        }

        private async Task ReceiveMessagesFromDeviceAsync(string partitionId)
        {            
            while (true)
            {
                try
                {
                    await foreach (PartitionEvent receivedEvent in eventHubConsumerClient.ReadEventsFromPartitionAsync(partitionId, EventPosition.Latest))
                    {
                        string body = Encoding.UTF8.GetString(receivedEvent.Data.Body.ToArray());
                        if (receivedEvent.Data.SystemProperties.ContainsKey("iothub-message-source"))
                        {
                            string msgSource = (string)receivedEvent.Data.SystemProperties["iothub-message-source"];
                            string deviceId = (string)receivedEvent.Data.SystemProperties["iothub-connection-device-id"];
                            var appProperties = receivedEvent.Data.Properties;
                            var systemProperties = receivedEvent.Data.SystemProperties;
                            Debug.WriteLine($"{partitionId} {msgSource} {body}");
                            switch (msgSource)
                            {
                                case "Telemetry":
                                    Dictionary<string, JsonElement> telemetries = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(body);
                                    if (telemetries != null)
                                    {
                                        foreach (string telemetryName in telemetries.Keys)
                                        {
                                            string callbackKey, component = "";
                                            if (receivedEvent.Data.SystemProperties.ContainsKey("dt-subject"))
                                            {
                                                component = (string)receivedEvent.Data.SystemProperties["dt-subject"];
                                                callbackKey = $"{deviceId}_{component}_{telemetryName}";
                                            }
                                            else
                                            {
                                                callbackKey = $"{deviceId}_{telemetryName}";
                                            }
                                            if (TelemetryCallbacks.ContainsKey(callbackKey))
                                            {
                                                TelemetryData data = new TelemetryData()
                                                {
                                                    DeviceId = deviceId,
                                                    Name = telemetryName,
                                                    Component = component,
                                                    Value = telemetries[telemetryName]
                                                };
                                                TelemetryCallbacks[callbackKey](data);
                                            }
                                        }
                                    }
                                    break;
                                case "twinChangeEvents":
                                    JsonElement properties = JsonDocument.Parse(body).RootElement.GetProperty("properties");
                                    JsonElement reported;
                                    if (properties.TryGetProperty("reported", out reported))
                                    {
                                        var reportedProperties = reported.EnumerateObject();
                                        foreach (JsonProperty property in reportedProperties)
                                        {
                                            string callbackKey;
                                            JsonElement child;
                                            if (property.Value.ValueKind == JsonValueKind.Object && property.Value.TryGetProperty("__t", out child))
                                            {
                                                var componentProperties = property.Value.EnumerateObject();
                                                foreach (JsonProperty componentProperty in componentProperties)
                                                {
                                                    callbackKey = $"{deviceId}_{property.Name}_{componentProperty.Name}";
                                                    if (PropertyCallbacks.ContainsKey(callbackKey))
                                                    {
                                                        PropertyData data = new PropertyData()
                                                        {
                                                            DeviceId = deviceId,
                                                            Name = componentProperty.Name,
                                                            Component = property.Name,
                                                            Value = componentProperty.Value
                                                        };
                                                        PropertyCallbacks[callbackKey](data);
                                                    }
                                                }
                                            }
                                            callbackKey = $"{deviceId}_{property.Name}";
                                            if (PropertyCallbacks.ContainsKey(callbackKey))
                                            {
                                                PropertyData data = new PropertyData()
                                                {
                                                    DeviceId = deviceId,
                                                    Name = property.Name,
                                                    Component = "",
                                                    Value = property.Value
                                                };
                                                PropertyCallbacks[callbackKey](data);
                                            }
                                        }
                                    }
                                    break;
                                case "digitalTwinChangeEvents":
                                    break;
                                case "deviceConnectionStateEvents":
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            Debug.WriteLine($"{partitionId} Unknown message: {body}");
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.ToString());
                }
            }
        }

        public async Task<ObservableCollection<BaseDevice>> GetDevicesAsync()
        {
            ObservableCollection<BaseDevice> deviceList = new ObservableCollection<BaseDevice>();
            if (registryManager != null)
            {
                IQuery query = registryManager.CreateQuery("select * from devices");
                while (query.HasMoreResults)
                {
                    IEnumerable<Twin> twins = await query.GetNextAsTwinAsync();
                    foreach (Twin twin in twins)
                    {
                        deviceList.Add(BaseDevice.GetModeledDevice(twin.DeviceId, twin.ModelId));
                    }
                }
            }
            return deviceList;
        }

        public async void SetDesiredProperty(string deviceId, string propertyName, object value)
        {
            Twin twin = await registryManager.GetTwinAsync(deviceId);
            var twinPatch = new Twin();
            twinPatch.Properties.Desired[propertyName] = value;
            await registryManager.UpdateTwinAsync(deviceId, twinPatch, twin.ETag);
        }

        public void SetDesiredProperty(string deviceId, string propertyName, string componentName, object value)
        {
            throw new NotImplementedException();
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, TelemetryCallback callback)
        {
            TelemetryCallbacks.Add($"{deviceId}_{telemetryName}", callback);
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, string componentName, TelemetryCallback callback)
        {
            TelemetryCallbacks.Add($"{deviceId}_{componentName}_{telemetryName}", callback);
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, PropertyChangedCallback callback)
        {
            PropertyCallbacks.Add($"{deviceId}_{propertyName}", callback);
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, string componentName, PropertyChangedCallback callback)
        {
            PropertyCallbacks.Add($"{deviceId}_{componentName}_{propertyName}", callback);
        }

        public CommandResponse SendCommand(string deviceId, string commandName, object payload)
        {
            throw new NotImplementedException();
        }

        public CommandResponse SendCommand(string deviceId, string commandName, string componentName, object payload)
        {
            throw new NotImplementedException();
        }
    }
}
