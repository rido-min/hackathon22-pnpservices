using Rido.IoTClient;

namespace smart_lightbulb_winforms
{
    internal enum LightStateEnum
    {
        On = 1,
        Off = 0
    }

    internal interface Ismartlightbulb : IBaseClient
    {
        IReadOnlyProperty<DateTime> Property_lastBatteryReplacement { get; set; }
        IWritableProperty<LightStateEnum> Property_lightState { get; set; }
        ITelemetry<int> Telemetry_batteryLife { get; set; }
    }
}