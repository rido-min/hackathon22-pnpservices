using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace smart_lightbulb_winforms
{
    internal class PasswordForm : Form
    {
        private TextBox textBox1;
        private Button button1;
        private Label label1;


        public string X509Key {get; set;}
        string pfxPath;
        public X509Certificate2 Certificate { get; set; }
        public PasswordForm(string pfxPath)
        {
            this.pfxPath = pfxPath;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(21, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.PasswordChar = '*';
            this.textBox1.Size = new System.Drawing.Size(207, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "1234";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(248, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Accept";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "PFX Password";
            // 
            // PasswordForm
            // 
            this.ClientSize = new System.Drawing.Size(369, 67);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Name = "PasswordForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                X509Certificate2 cert = new X509Certificate2(this.pfxPath, textBox1.Text);
            
                if (cert.HasPrivateKey)
                {
                    X509Key = pfxPath + "|" + textBox1.Text;
                    Certificate = cert;
                }
                this.DialogResult = DialogResult.OK;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid PFX Password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.No;
            }
            
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                button1_Click(this, new EventArgs());
            }
        }
    }
}
