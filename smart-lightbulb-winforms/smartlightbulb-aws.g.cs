using MQTTnet.Client;
using Rido.IoTClient;
using Rido.IoTClient.Aws;
using Rido.IoTClient.Aws.TopicBindings;

namespace smart_lightbulb_winforms_aws
{
    internal enum LightStateEnum
    {
        On = 1,
        Off = 0
    }

    internal class smartlightbulb : AwsClient
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
            var connection = await AwsConnectionFactory.CreateAsync(cs, cancellationToken);
            var client = new smartlightbulb(connection) { ConnectionSettings = cs };
            client.InitialState = await client.GetShadowAsync(cancellationToken);
            return client;
        }

    }
}
