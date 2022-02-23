using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.AzIoTHub;
using Rido.IoTClient.AzIoTHub.TopicBindings;
using smart_lightbulb_winforms;

namespace smart_lightbulb_winforms_hub
{

    internal class smartlightbulb : IoTHubClient, Ismartlightbulb
    {
        const string modelId = "dtmi:pnd:demo:smartlightbulb;1";

        public ITelemetry<int> Telemetry_batteryLife { get; set; }
        public IReadOnlyProperty<DateTime> Property_lastBatteryReplacement { get; set; }
        public IWritableProperty<LightStateEnum> Property_lightState { get; set; }

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
