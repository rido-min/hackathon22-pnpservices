using Rido.IoTClient;

namespace smart_lightbulb;

public class DeviceRunner : BackgroundService
{
    private readonly ILogger<DeviceRunner> _logger;
    private readonly IConfiguration _configuration;

    public DeviceRunner(ILogger<DeviceRunner> logger, IConfiguration config)
    {
        _logger = logger;
        _configuration = config;
    }

    smartlightbulb? client;

    int currentBattery = 100;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var cs = ConnectionSettings.FromConnectionString(_configuration.GetConnectionString("cs"));
        client = await smartlightbulb.CreateClientAsync(cs, stoppingToken);
        Console.WriteLine(client.ConnectionSettings.HostName + " " + client.ConnectionSettings.DeviceId);

        client.Property_lightState.OnProperty_Updated = Property_lightState_UpdateHandler;

        await client.Property_lightState.InitPropertyAsync(client.InitialState, LightStateEnum.On, stoppingToken);

        client.Property_lastBatteryReplacement.PropertyValue = DateTime.Now;
        await client.Property_lastBatteryReplacement.ReportPropertyAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (currentBattery<1)
            {
                client.Property_lightState.PropertyValue = new PropertyAck<LightStateEnum>(client.Property_lightState.PropertyValue.Name)
                {
                    Value = LightStateEnum.Off,
                    Description = "battery ended",
                    Status = 203,
                    Version = 0
                };
                await client.Property_lightState.ReportPropertyAsync(stoppingToken);
            }

            if (client.Property_lightState.PropertyValue.Value.Equals(LightStateEnum.On))
            {
                await client.Telemetry_batteryLife.SendTelemetryAsync(currentBattery--, stoppingToken);
            }
            
            _logger.LogInformation($"Battery: {currentBattery} State: {client.Property_lightState.PropertyValue.Value}");
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task<PropertyAck<LightStateEnum>> Property_lightState_UpdateHandler(PropertyAck<LightStateEnum> p)
    {
        ArgumentNullException.ThrowIfNull(client);
        client.Property_lightState.PropertyValue.Value = p.Value;
        var ack = new PropertyAck<LightStateEnum>(p.Name)
        {
            Description = "light state Accepted",
            Status = 200,
            Value = p.Value,
            Version = p.Version
        };
        return await Task.FromResult(ack);
    }
}
