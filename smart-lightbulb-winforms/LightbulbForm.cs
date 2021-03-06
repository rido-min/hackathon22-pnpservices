using MQTTnet.Client;
using Rido.IoTClient;
using System.Drawing.Design;

namespace smart_lightbulb_winforms;

public partial class LightbulbForm : Form
{
    //const string cs = "IdScope=0ne003861C6;Auth=X509;X509key=cert.pfx|1234";
    //const string cs = "HostName=a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com;ClientId=bulb1;Auth=X509;X509Key=cert.pfx|1234";
    const string cs = "IdScope=0ne004CB66B;Auth=X509;X509key=cert.pfx|1234";


    CloudSelecterForm cloudSelecterForm;

    Ismartlightbulb? client;
    int currentBattery = 100;

    public LightbulbForm()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        
        cloudSelecterForm = new CloudSelecterForm();
        if (cloudSelecterForm.ShowDialog() == DialogResult.OK)
        {
            await RunDevice(cloudSelecterForm.connectionSettings, cloudSelecterForm.CloudType);
        }
        else
        {
            MessageBox.Show("Invalid Connection Settings", "InvalidConnectionSettings");
        }    


    }

    private async Task RunDevice(ConnectionSettings cs, CloudType cloud)
    {
        switch (cloud)
        {
            case CloudType.IoTHub:
                client = await smart_lightbulb_winforms_hub.smartlightbulb.CreateClientAsync(cs) ;
                break;
            case CloudType.AWS:
                client = await smart_lightbulb_winforms_aws.smartlightbulb.CreateClientAsync(cs);
                break;
            case CloudType.IoTHubBroker:
                client = await smart_lightbulb_winforms_broker.smartlightbulb.CreateClientAsync(cs);
                break;
        }

        client.Connection.DisconnectedAsync += async d => UpdateUI();

        buttonConnectDisconnect.Text = "Disconnect";

        if (Properties.Settings.Default.battery > 0)
        {
            currentBattery = Properties.Settings.Default.battery;
        }

        labelStatus.Text = $"{client.ConnectionSettings.DeviceId} connected to {client.ConnectionSettings.HostName}";
        client.Property_lightState.OnProperty_Updated = Property_lightState_UpdateHandler;

        await client.Property_lightState.InitPropertyAsync(client.InitialState, LightStateEnum.On);

        client.Property_lastBatteryReplacement.PropertyValue = DateTime.Now;
        await client.Property_lastBatteryReplacement.ReportPropertyAsync();

        UpdateUI();

        while (client.Connection.IsConnected)
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
                await client.Property_lightState.ReportPropertyAsync();
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
        await client.Property_lightState.ReportPropertyAsync();
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
        string connectedText = string.Empty;
        string buttonConnectText = string.Empty;
        if (client.Connection.IsConnected)
        {
            buttonConnectText = "Disconnect";
            connectedText = $"{client.ConnectionSettings.ClientId} connected to {client.ConnectionSettings.HostName}"; ;
        } 
        else
        {
            buttonConnectText = "Connect";
            connectedText = "Disconnected";
        }

        if (client.Property_lightState.PropertyValue.Value == LightStateEnum.Off)
        {
            selectedText = "Turn On";
            selectedImg =  "Off.jpg";
            

        }
        else
        {
            selectedText = "Turn Off";
            selectedImg = "On.jpg";
        }

        if (this.InvokeRequired)
        {
            this.Invoke(() =>
            {
                labelStatus.Text = connectedText;
                buttonConnectDisconnect.Text = buttonConnectText;
                buttonOnOff.Text = selectedText;
                pictureBoxLightbulb.ImageLocation = selectedImg;
                buttonOnOff.Enabled = client.Connection.IsConnected;
                buttonReplaceBateries.Enabled = client.Connection.IsConnected;
                progressBar1.Enabled = client.Connection.IsConnected;
            });
        }
        else
        {
            labelStatus.Text = connectedText;
            buttonConnectDisconnect.Text = buttonConnectText;
            buttonOnOff.Text = selectedText;
            pictureBoxLightbulb.ImageLocation = selectedImg;
            buttonOnOff.Enabled = client.Connection.IsConnected;
            buttonReplaceBateries.Enabled = client.Connection.IsConnected;
            progressBar1.Enabled = client.Connection.IsConnected;
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

    private async void buttonConnectDisconnect_Click(object sender, EventArgs e)
    {
        if (client.Connection.IsConnected)
        {
            await client.Connection.DisconnectAsync(
                new MqttClientDisconnectOptionsBuilder()
                    .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                    .Build());
            UpdateUI();
        }
        else
        {
            await RunDevice(cloudSelecterForm.connectionSettings, cloudSelecterForm.CloudType);
            UpdateUI();
        }
    }

    private async void buttonChangeCloud_Click(object sender, EventArgs e)
    {
        await client.Connection.DisconnectAsync(
              new MqttClientDisconnectOptionsBuilder()
                  .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                  .Build());
        UpdateUI();

        if (cloudSelecterForm.ShowDialog() == DialogResult.OK)
        {
            await RunDevice(cloudSelecterForm.connectionSettings, cloudSelecterForm.CloudType);
        }
        else
        {
            MessageBox.Show("Invalid Connection Settings", "InvalidConnectionSettings");
        }

    }
}
