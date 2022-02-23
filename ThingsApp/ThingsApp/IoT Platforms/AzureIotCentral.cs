using Azure.Messaging.EventHubs.Consumer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp.IoT_Platforms
{
    internal class AzureIotCentral : IIotPlatform
    {
        private EventHubConsumerClient eventHubConsumerClient;
        private Dictionary<string, TelemetryCallback> TelemetryCallbacks = new Dictionary<string, TelemetryCallback>();
        private Dictionary<string, PropertyChangedCallback> PropertyCallbacks = new Dictionary<string, PropertyChangedCallback>();

        private string appBaseAddress;
        private string eventHubConnectionString;
        private string consumerGroup;
        private string apiKey;
        public AzureIotCentral(string baseAddress, string ehConnectionString, string consumerGrp, string key)
        {
            appBaseAddress = baseAddress;
            eventHubConnectionString = ehConnectionString;
            consumerGroup = consumerGrp;
            apiKey = key;

            eventHubConsumerClient = new EventHubConsumerClient("$Default", eventHubConnectionString);
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
            Console.WriteLine($"Starting listener thread for partition: {partitionId}");
            while (true)
            {
                await foreach (PartitionEvent receivedEvent in eventHubConsumerClient.ReadEventsFromPartitionAsync(partitionId, EventPosition.Latest))
                {
                    string body = Encoding.UTF8.GetString(receivedEvent.Data.Body.ToArray());
                    JsonDocument doc = JsonDocument.Parse(body);
                    string? msgSource = doc.RootElement.GetProperty("messageSource").GetString();
                    string? deviceId = doc.RootElement.GetProperty("deviceId").GetString();
                    string? timeStamp = doc.RootElement.GetProperty("enqueuedTime").GetString();

                    switch (msgSource)
                    {
                        case "telemetry":
                            var telemetries = doc.RootElement.GetProperty("telemetry").EnumerateObject();
                            foreach (var telemetry in telemetries)
                            {
                                string callbackKey, component = "";
                                JsonElement componentElement;
                                if (doc.RootElement.TryGetProperty("component", out componentElement))
                                {
                                    component = componentElement.GetString();
                                    callbackKey = $"{deviceId}_{component}_{telemetry.Name}";
                                }
                                else
                                {
                                    callbackKey = $"{deviceId}_{telemetry.Name}";
                                }
                                if (TelemetryCallbacks.ContainsKey(callbackKey))
                                {
                                    TelemetryData data = new TelemetryData()
                                    {
                                        DeviceId = deviceId,
                                        TimeStamp = DateTime.Parse(timeStamp),
                                        Name = telemetry.Name,
                                        Component = component,
                                        Value = telemetry.Value
                                    };
                                    TelemetryCallbacks[callbackKey](data);
                                }
                            }
                            break;
                        case "properties":
                            string? msgType = doc.RootElement.GetProperty("messageType").GetString();
                            if (msgType == "devicePropertyReportedChange")
                            {
                                JsonElement reported = JsonDocument.Parse(body).RootElement.GetProperty("properties");
                                var properties = reported.EnumerateArray();
                                foreach (JsonElement property in properties)
                                {
                                    JsonProperty propName = property.EnumerateObject().ElementAt(0);
                                    JsonProperty propValue = property.EnumerateObject().ElementAt(1);

                                    string callbackKey, component = "";
                                    if (false /* TODO is component */)
                                    {
                                        component = "TODO";
                                        callbackKey = $"{deviceId}_{propName.Value}_{component}";
                                    }
                                    else
                                    {
                                        callbackKey = $"{deviceId}_{propName.Value}";
                                    }

                                    if (PropertyCallbacks.ContainsKey(callbackKey))
                                    {
                                        PropertyData data = new PropertyData()
                                        {
                                            DeviceId = deviceId,
                                            Name = propName.Value.GetString(),
                                            Component = component,
                                            Value = propValue.Value
                                        };
                                        PropertyCallbacks[callbackKey](data);
                                    }
                                }
                            }
                            break;
                        case "deviceLifecycle":
                            break;
                        case "deviceConnectivity":
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public Task<ObservableCollection<BaseDevice>> GetDevicesAsync()
        {
            //HttpClient client = new HttpClient();
            //HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "https://feiht.azureiotcentral.com/api/devices?api-version=1.1-preview");
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedAccessSignature", "sr=8ecc687c-738e-475a-8fdc-957e77e9ed24&sig=cyQIP7ceNmLN3bbk2rWqQESflR6iAeY5qr6j72qrfQw%3D&skn=lightbulb1&se=1676610577539");
            //HttpResponseMessage res = client.Send(req);
            //MemoryStream stream = (MemoryStream)res.Content.ReadAsStream();
            //string text = Encoding.UTF8.GetString(stream.ToArray());

            // TODO get device list from REST API

            ObservableCollection<BaseDevice> devices = new ObservableCollection<BaseDevice>();
            devices.Add(new LightFixture("bulb1"));
            devices.Add(new BaseDevice("device002"));
            return Task<ObservableCollection<BaseDevice>>.FromResult(devices);
        }

        public void SetDesiredProperty(string deviceId, string propertyName, object value)
        {
            HttpClient client = new HttpClient();
            string uri = $"https://feiht.azureiotcentral.com/api/devices/{deviceId}/properties?api-version=1.1-preview";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Patch, uri);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedAccessSignature", "sr=8ecc687c-738e-475a-8fdc-957e77e9ed24&sig=cyQIP7ceNmLN3bbk2rWqQESflR6iAeY5qr6j72qrfQw%3D&skn=lightbulb1&se=1676610577539");
            //string json = $"{{\"settings\":{{\"{propertyName}\": {value}}}}}";
            string json = $"{{\"{propertyName}\": {value}}}";
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage res = client.Send(req);
            MemoryStream stream = (MemoryStream)res.Content.ReadAsStream();
            string text = Encoding.UTF8.GetString(stream.ToArray());
        }

        public void SetDesiredProperty(string deviceId, string propertyName, string componentName, object value)
        {
            throw new NotImplementedException();
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, PropertyChangedCallback callback)
        {
            PropertyCallbacks.Add($"{deviceId}_{propertyName}", callback);
        }

        public void SubscribePropertyChange(string deviceId, string propertyName, string componentName, PropertyChangedCallback callback)
        {
            PropertyCallbacks.Add($"{deviceId}_{componentName}_{propertyName}", callback);
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, TelemetryCallback callback)
        {
            TelemetryCallbacks.Add($"{deviceId}_{telemetryName}", callback);
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, string componentName, TelemetryCallback callback)
        {
            TelemetryCallbacks.Add($"{deviceId}_{componentName}_{telemetryName}", callback);
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
