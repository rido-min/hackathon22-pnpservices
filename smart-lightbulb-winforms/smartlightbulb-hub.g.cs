using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;

namespace smart_lightbulb_winforms_hub
{
    internal enum LightStateEnum
    {
        On = 1,
        Off = 0
    }

    internal class smartlightbulb : IoTHubClient
    {
        const string modelId = "dtmi:pnd:demo:smartlightbulb;1";

        public Telemetry<int> Telemetry_batteryLife;
        public ReadOnlyProperty<DateTime> Property_lastBatteryReplacement;
        public WritableProperty<LightStateEnum> Property_lightState;

        public smartlightbulb(IMqttClient connection) : base(connection)
        {
            Telemetry_batteryLife = new Telemetry<int>(connection, "batteryLife");
            Property_lastBatteryReplacement = new ReadOnlyProperty<DateTime>(connection, "lastBatteryReplacement");
            Property_lightState = new WritableProperty<LightStateEnum>(connection, "lightState");
        }

        public static async Task<smartlightbulb> CreateClientAsync(ConnectionSettings cs, CancellationToken cancellationToken = default)
        {
            cs.ModelId = modelId;
            var client = new smartlightbulb(await IoTHubConnectionFactory.CreateAsync(cs, cancellationToken)) { ConnectionSettings = cs };
            client.InitialState = await client.GetTwinAsync(cancellationToken);
            return client;
        }

    }
}
