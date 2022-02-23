using Rido.IoTClient;
using Rido.IoTClient.AzDps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace smart_lightbulb_winforms
{
    public enum CloudType
    {
        IoTHub,
        AWS,
        IoTHubBroker
    }

    internal class CloudSelecterForm : Form
    {
        private GroupBox groupBox1;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private Label label1;
        private Button buttonAccept;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private OpenFileDialog openFileDialog1;
        private Label label2;
        private Button button2;

        public ConnectionSettings connectionSettings { get; set; } = new ConnectionSettings() { Auth = "X509"};
        public CloudType CloudType = CloudType.IoTHub;
        public string pfxPath { get; set; } = "cert.pfx";
        PasswordForm pwdForm;

        public CloudSelecterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.buttonAccept = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(75, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(461, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select Broker";
            // 
            // textBox3
            // 
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(115, 80);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(280, 23);
            this.textBox3.TabIndex = 5;
            this.textBox3.Text = "0ne004CB66B";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(114, 47);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(279, 23);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "a38jrw6jte2l2x-ats.iot.us-west-1.amazonaws.com";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(115, 18);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(279, 23);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "0ne003861C6";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(13, 84);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(101, 19);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "IoTHub Broker";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(13, 53);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(96, 19);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "AWS IoT Core";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(12, 22);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(96, 19);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "IoTHub / DPS";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "<select certificate>";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(75, 50);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(181, 29);
            this.button2.TabIndex = 3;
            this.button2.Text = "Select Cetificate";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonAccept
            // 
            this.buttonAccept.Location = new System.Drawing.Point(405, 241);
            this.buttonAccept.Name = "buttonAccept";
            this.buttonAccept.Size = new System.Drawing.Size(131, 32);
            this.buttonAccept.TabIndex = 4;
            this.buttonAccept.Text = "Accept";
            this.buttonAccept.UseVisualStyleBackColor = true;
            this.buttonAccept.Click += new System.EventHandler(this.buttonAccept_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 293);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 15);
            this.label2.TabIndex = 5;
            // 
            // CloudSelecterForm
            // 
            this.ClientSize = new System.Drawing.Size(698, 359);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonAccept);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Name = "CloudSelecterForm";
            this.Text = "Device Identity Selector";
            this.Load += new System.EventHandler(this.CloudSelecterForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                CloudType = CloudType.IoTHub;
                textBox1.Enabled = true;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
            }
            if (radioButton2.Checked)
            {
                CloudType = CloudType.AWS;
                textBox1.Enabled = false;
                textBox2.Enabled = true;
                textBox3.Enabled = false;
            }
            if (radioButton3.Checked)
            {
                CloudType = CloudType.IoTHubBroker;
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = true;
            }

        }

        private async void buttonAccept_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label2.Text = "Provisioning...";
                if (textBox1.Text.StartsWith("0ne"))
                {
                    connectionSettings.IdScope = textBox1.Text.Trim();
                    await DpsClient.ProvisionIfNeededAsync(connectionSettings);
                    connectionSettings.IdScope = String.Empty;
                }
                else
                {
                    connectionSettings.HostName = textBox1.Text.Trim();
                }
            }
            if (radioButton2.Checked)
            {
                connectionSettings.ClientId = GetCNFromCertSubject(pwdForm.Certificate.SubjectName.Name);
                connectionSettings.HostName = textBox2.Text.Trim();
            }
            if (radioButton3.Checked)
            {
                label2.Text = "Provisioning...";
                if (textBox3.Text.StartsWith("0ne"))
                {
                    connectionSettings.IdScope = textBox3.Text.Trim();
                    await DpsClient.ProvisionIfNeededAsync(connectionSettings);
                    connectionSettings.IdScope = String.Empty;
                }
                else
                {
                    connectionSettings.HostName = textBox3.Text.Trim();
                }
            }
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.pfx";
            if ( openFileDialog1.ShowDialog() == DialogResult.OK )
            {
                pwdForm = new PasswordForm(openFileDialog1.FileName);
                if (pwdForm.ShowDialog()==DialogResult.OK)
                {
                    label1.Text = pwdForm.Certificate.SubjectName.Name + " Issuer:" + pwdForm.Certificate.IssuerName.Name;
                    connectionSettings.X509Key = pwdForm.X509Key;
                }
                else
                {
                    label1.Text = "Private Key not found";
                }
            }
        }

        private void CloudSelecterForm_Load(object sender, EventArgs e)
        {
            X509Certificate2 cert = new X509Certificate2(this.pfxPath,"1234");

            pwdForm = new PasswordForm("cert.pfx");
            if (cert.HasPrivateKey)
            {
                pwdForm.Certificate = cert;
                label1.Text = cert.SubjectName.Name + " Issuer:" + cert.IssuerName.Name;
                connectionSettings.X509Key = pfxPath + "|" + "1234";
            }
        }

        internal static string GetCNFromCertSubject(string subject)
        {
            var result = subject[3..];
            if (subject.Contains(','))
            {
                var posComma = result.IndexOf(',');
                result = result[..posComma];
            }
            return result.Replace(" ", "");
        }
    }
}
