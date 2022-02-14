using Rido.IoTClient;
using System.Drawing.Design;

namespace smart_lightbulb_winforms;

public partial class LightbulbForm : Form
{
    const string cs = "HostName=a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com;ClientId=bulb1;Auth=X509;X509Key=cert.pfx|1234";
    smartlightbulb? client;
    int currentBattery = 100;

    public LightbulbForm()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(Properties.Settings.Default.hostname))
        {
            client = await smartlightbulb.CreateClientAsync(new ConnectionSettings(cs));
            Properties.Settings.Default.hostname = client.ConnectionSettings.HostName;
        }
        else
        {
            string host = Properties.Settings.Default.hostname;
            client = await smartlightbulb.CreateClientAsync(new ConnectionSettings(cs) { HostName = host, IdScope = null});
        }
        
        if (Properties.Settings.Default.battery>0)
        {
            currentBattery = Properties.Settings.Default.battery;
        }

        labelStatus.Text = $"{client.ConnectionSettings.ClientId} connected to {client.ConnectionSettings.HostName}";
        client.Property_lightState.OnProperty_Updated = Property_lightState_UpdateHandler;

        await client.Property_lightState.InitPropertyAsync(client.InitialState, LightStateEnum.On);

        client.Property_lastBatteryReplacement.PropertyValue = DateTime.Now;
        await client.Property_lastBatteryReplacement.ReportPropertyAsync();

        UpdateUI();

        while (true)
        {
            if (currentBattery < 1)
            {
                client.Property_lightState.PropertyValue = new PropertyAck<LightStateEnum>(client.Property_lightState.PropertyValue.Name)
                {
                    Value = LightStateEnum.Off,
                    Description = "battery ended",
                    Status = 203,
                    Version = 0
                };
                await client.Property_lightState.UpdatePropertyAsync();
                UpdateUI();
                
            }

            if (client.Property_lightState.PropertyValue.Value.Equals(LightStateEnum.On))
            {
                await client.Telemetry_batteryLife.SendTelemetryAsync(currentBattery--);
                progressBar1.Value = currentBattery;
            }
            await Task.Delay(1000);
        }

    }

    private async void ButtonOnOff_Click(object sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(client);
        Toggle();
        UpdateUI();
        var ack = new PropertyAck<LightStateEnum>(client.Property_lightState.PropertyValue.Name)
        {
            Description = "Changed by user",
            Status = 203,
            Value = client.Property_lightState.PropertyValue.Value,
            Version = 0
        };
        client.Property_lightState.PropertyValue = ack;
        await client.Property_lightState.UpdatePropertyAsync();
    }

    private void Toggle()
    {
        ArgumentNullException.ThrowIfNull(client);
        if (client.Property_lightState.PropertyValue.Value == LightStateEnum.Off)
        {
            client.Property_lightState.PropertyValue.Value = LightStateEnum.On;
        } 
        else
        {
            client.Property_lightState.PropertyValue.Value = LightStateEnum.Off;
        }
         
    }

    private void UpdateUI()
    {
        ArgumentNullException.ThrowIfNull(client);
        string selectedImg = string.Empty;
        string selectedText = string.Empty; 
        if (client.Property_lightState.PropertyValue.Value == LightStateEnum.Off)
        {
            selectedText = "Turn On";
            selectedImg = @"..\..\..\Off.jpg";
        }
        else
        {
            selectedText = "Turn Off";
            selectedImg = @"..\..\..\On.jpg";
        }

        if (pictureBoxLightbulb.InvokeRequired)
        {
            pictureBoxLightbulb.Invoke(() =>
            {
                buttonOnOff.Text = selectedText;
                pictureBoxLightbulb.ImageLocation = selectedImg;
            });
        }
        else
        {
            buttonOnOff.Text = selectedText;
            pictureBoxLightbulb.ImageLocation = selectedImg;
        }
    }

    private async Task<PropertyAck<LightStateEnum>> Property_lightState_UpdateHandler(PropertyAck<LightStateEnum> p)
    {
        ArgumentNullException.ThrowIfNull(client);
        client.Property_lightState.PropertyValue.Value = p.Value;
        
        UpdateUI();
        var ack = new PropertyAck<LightStateEnum>(p.Name)
        {
            Description = "light state Accepted",
            Status = 200,
            Value = p.Value,
            Version = p.Version
        };
        return await Task.FromResult(ack);
    }

    private async void buttonReplaceBateries_Click(object sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(client);
        currentBattery = 100;
        progressBar1.Value = currentBattery;
        client.Property_lastBatteryReplacement.PropertyValue = DateTime.Now;
        await client.Property_lastBatteryReplacement.ReportPropertyAsync();
    }

    private void LightbulbForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Properties.Settings.Default.battery = currentBattery;
        Properties.Settings.Default.Save();
    }

    private void labelStatus_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.hostname = "";
        Properties.Settings.Default.battery = 100;
        currentBattery = 100;
        Properties.Settings.Default.Save();
    }
}
