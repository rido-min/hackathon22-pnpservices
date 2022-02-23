using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.Aws;
using Rido.IoTClient.Aws.TopicBindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ThingsApp.Models;

namespace ThingsApp.IoT_Platforms
{
    internal class AWSIotCore : IIotPlatform
    {
        private Dictionary<string, TelemetryCallback> TelemetryCallbacks = new Dictionary<string, TelemetryCallback>();
        private Dictionary<string, PropertyChangedCallback> PropertyCallbacks = new Dictionary<string, PropertyChangedCallback>();

        IMqttClient mqtt;
        public AWSIotCore(string cs)
        {
           
            _ = ReceiveMessagesFromBrokerAsync(cs);
        }

        private (string deviceId, string feature) ParseTopic(string topic)
        {
            var segments = topic.Split('/');
            if (segments[0].StartsWith('$'))
            {
                return (segments[2], "props");
            }
            else
            {
                return (segments[1], segments[2]);
            }    
        }

        private async Task ReceiveMessagesFromBrokerAsync(string cs)
        {
            mqtt = await AwsConnectionFactory.CreateAsync(new ConnectionSettings(cs));
            mqtt.ApplicationMessageReceivedAsync += async m =>
            {
                (string deviceId, string feature) = ParseTopic(m.ApplicationMessage.Topic);
                string msg = Encoding.UTF8.GetString(m.ApplicationMessage.Payload ?? Array.Empty<byte>());
                switch (feature)
                {
                    case "telemetry":
                        Dictionary<string, JsonElement> telemetries = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(msg);
                        if (telemetries != null)
                        {
                            foreach (var telemetryName in telemetries.Keys)
                            {

                                TelemetryData data = new TelemetryData()
                                {
                                    DeviceId = deviceId,
                                    Name = telemetryName,
                                    Value = telemetries[telemetryName]
                                };
                                TelemetryCallbacks[$"{deviceId}_{telemetryName}"](data);
                            }
                        }
                        break;
                    case "props":
                        if (!string.IsNullOrEmpty(msg))
                        {
                            JsonElement properties = JsonDocument.Parse(msg).RootElement;
                            JsonElement state;
                            if (properties.TryGetProperty("state", out state))
                            {
                                JsonElement reported;
                                if (state.TryGetProperty("reported", out reported))
                                {
                                    var reportedProperties = reported.EnumerateObject();
                                    foreach (JsonProperty property in reportedProperties)
                                    {
                                        string callbackKey = $"{deviceId}_{property.Name}";
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
                            }
                        }
                        break;
                    default:
                        break;
                }
            };
            await mqtt.SubscribeAsync(
                new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("pnp/#")
                .WithTopicFilter("$aws/things/#")
                .Build());
        }


        public Task<ObservableCollection<BaseDevice>> GetDevicesAsync()
        {
            ObservableCollection<BaseDevice> devices = new ObservableCollection<BaseDevice>();
            devices.Add(new LightFixture("bulb1"));
            devices.Add(new LightFixture("bulb3"));
            return Task.FromResult(devices);
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
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "state", new Dictionary<string, object>()
                    {
                       { 
                            "desired", new Dictionary<string, object>
                            {
                                {
                                    propertyName, value
                                }
                            }
                       }
                    }
                }
            };
            var pubAck = mqtt.PublishAsync($"$aws/things/{deviceId}/shadow/update", JsonSerializer.Serialize(data)).Result;
            Console.WriteLine("desired puback " + pubAck.ReasonString);
            
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
            throw new NotImplementedException();
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, TelemetryCallback callback)
        {
            TelemetryCallbacks.Add($"{deviceId}_{telemetryName}", callback);
        }

        public void SubscribeTelemetry(string deviceId, string telemetryName, string componentName, TelemetryCallback callback)
        {
            throw new NotImplementedException();
        }
    }
}
