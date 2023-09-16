using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using smart_lightbulb_sdk;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace smart_lightbulb_sdk;

public partial class LightbulbForm : Form
{
    const string cs = "HostName=rido.azure-devices.net;DeviceId=lightbulbsas;SharedAccessKey=jVDrKNC8LrX2aHpHIb+QgTQ6U7CddV3ryue0QcoO5mM=";
    //CloudSelecterForm cloudSelecterForm;

    //Ismartlightbulb? client;
    int currentBattery = 100;
    bool isConnected = false;
    LightStateEnum lightState = LightStateEnum.On;

    DeviceClient dc;

    public LightbulbForm()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await RunDevice(cs);
    }

    private async Task RunDevice(string connectionString)
    {
        dc = DeviceClient.CreateFromConnectionString(connectionString, new ClientOptions() { ModelId = "" });
        dc.SetConnectionStatusChangesHandler((d, r) => 
        {
            isConnected = d == ConnectionStatus.Connected;
            UpdateUI();
        });
        buttonConnectDisconnect.Text = "Disconnect";

        if (Properties.Settings.Default.battery > 0)
        {
            currentBattery = Properties.Settings.Default.battery;
        }

        currentBattery = 100;
        labelStatus.Text = cs;

        await dc.SetDesiredPropertyUpdateCallbackAsync(async (twin, ctx) =>
        {
            if (twin.Contains("lightState"))
            {
                var vv = Convert.ToString(twin["lightState"].Value);
                lightState = Enum.Parse<LightStateEnum>(vv);
                UpdateUI();
                var ack = new TwinCollection()
                {
                    ["lightState"] = new
                    {
                        av = twin.Version,
                        ad = "lightstate changed",
                        ac = 200
                    }
                };
                await dc.UpdateReportedPropertiesAsync(ack);
            }
        }, this);

        //client.Property_lightState.OnProperty_Updated = Property_lightState_UpdateHandler;

        //await client.Property_lightState.InitPropertyAsync(client.InitialState, LightStateEnum.On);

        //client.Property_lastBatteryReplacement.PropertyValue = DateTime.Now;
        //await client.Property_lastBatteryReplacement.ReportPropertyAsync();

        var lastBatteryReplacement = new TwinCollection()
        {
            ["lastBatteryReplacement"] = DateTime.Now
        };
        await dc.UpdateReportedPropertiesAsync (lastBatteryReplacement);

        UpdateUI();

        while (true)
        {
            if (currentBattery < 1)
            {
                var ack = new TwinCollection()
                {
                    ["lightState"] = new
                    {
                        av = 0,
                        ad = "battery ended",
                        ac = 203
                    }
                };
                await dc.UpdateReportedPropertiesAsync(ack);
                UpdateUI();
            }

            if (isConnected && lightState.Equals(LightStateEnum.On))
            {
                Microsoft.Azure.Devices.Client.Message msg = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(currentBattery--)));
                await dc.SendEventAsync(msg);
                progressBar1.Value = currentBattery;
            }
            await Task.Delay(1000);
        }
    }

    

    private async void ButtonOnOff_Click(object sender, EventArgs e)
    {
        
        Toggle();
        UpdateUI();
        var ack = new TwinCollection()
        { 
            ["lightState"] = new
            {
                ad = "Changed by user",
                ac = 203,
                value = lightState,
                av = 0
            }
        };

        await dc.UpdateReportedPropertiesAsync(ack);
    }

    private void Toggle()
    {
        ArgumentNullException.ThrowIfNull(dc);
        if (lightState == LightStateEnum.Off)
        {
            lightState = LightStateEnum.On;
        } 
        else
        {
            lightState = LightStateEnum.Off;
        }
         
    }

    private void UpdateUI()
    {
        
        ArgumentNullException.ThrowIfNull(dc);
        string selectedImg = string.Empty;
        string selectedText = string.Empty; 
        string connectedText = string.Empty;
        string buttonConnectText = string.Empty;
        if (isConnected)
        {
            buttonConnectText = "Disconnect";
            connectedText = $"connected"; ;
        }
        else
        {
            buttonConnectText = "Connect";
            connectedText = "Disconnected";
        }

        if (lightState == LightStateEnum.Off)
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
                buttonOnOff.Enabled = true;
                buttonReplaceBateries.Enabled = true;
                progressBar1.Enabled = true;
            });
        }
        else
        {
            labelStatus.Text = connectedText;
            buttonConnectDisconnect.Text = buttonConnectText;
            buttonOnOff.Text = selectedText;
            pictureBoxLightbulb.ImageLocation = selectedImg;
            buttonOnOff.Enabled = true;
            buttonReplaceBateries.Enabled = true;
            progressBar1.Enabled = true;
        }
    }

    //private async Task<PropertyAck<LightStateEnum>> Property_lightState_UpdateHandler(PropertyAck<LightStateEnum> p)
    //{
    //    ArgumentNullException.ThrowIfNull(client);
    //    client.Property_lightState.PropertyValue.Value = p.Value;
        
    //    UpdateUI();
    //    var ack = new PropertyAck<LightStateEnum>(p.Name)
    //    {
    //        Description = "light state Accepted",
    //        Status = 200,
    //        Value = p.Value,
    //        Version = p.Version
    //    };
    //    return await Task.FromResult(ack);
    //}

    private async void buttonReplaceBateries_Click(object sender, EventArgs e)
    {
        ArgumentNullException.ThrowIfNull(dc);
        currentBattery = 100;
        progressBar1.Value = currentBattery;
        var lastBatteryReplacement = new TwinCollection()
        {
            ["lastBatteryReplacement"] = DateTime.Now
        };
        await dc.UpdateReportedPropertiesAsync(lastBatteryReplacement);
    }

    private void LightbulbForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Properties.Settings.Default.battery = currentBattery;
        Properties.Settings.Default.Save();
    }

    private void labelStatus_Click(object sender, EventArgs e)
    {
        //Properties.Settings.Default.hostname = "";
        Properties.Settings.Default.battery = 100;
        currentBattery = 100;
        Properties.Settings.Default.Save();
    }

    private async void buttonConnectDisconnect_Click(object sender, EventArgs e)
    {
        if (isConnected)
        {
            await dc.CloseAsync();
            UpdateUI();
        }
        else
        {
            await RunDevice(cs);
            UpdateUI();
        }
    }

    //private async void buttonChangeCloud_Click(object sender, EventArgs e)
    //{
    //    await client.Connection.DisconnectAsync(
    //          new MqttClientDisconnectOptionsBuilder()
    //              .WithReason(MqttClientDisconnectReason.NormalDisconnection)
    //              .Build());
    //    UpdateUI();

    //    if (cloudSelecterForm.ShowDialog() == DialogResult.OK)
    //    {
    //        await RunDevice(cloudSelecterForm.connectionSettings, cloudSelecterForm.CloudType);
    //    }
    //    else
    //    {
    //        MessageBox.Show("Invalid Connection Settings", "InvalidConnectionSettings");
    //    }

    //}
}
