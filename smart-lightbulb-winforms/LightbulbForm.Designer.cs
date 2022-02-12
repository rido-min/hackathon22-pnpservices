namespace smart_lightbulb_winforms;

partial class LightbulbForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.pictureBoxLightbulb = new System.Windows.Forms.PictureBox();
            this.buttonOnOff = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.buttonReplaceBateries = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLightbulb)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxLightbulb
            // 
            this.pictureBoxLightbulb.Location = new System.Drawing.Point(38, 66);
            this.pictureBoxLightbulb.Name = "pictureBoxLightbulb";
            this.pictureBoxLightbulb.Size = new System.Drawing.Size(300, 363);
            this.pictureBoxLightbulb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLightbulb.TabIndex = 0;
            this.pictureBoxLightbulb.TabStop = false;
            // 
            // buttonOnOff
            // 
            this.buttonOnOff.Location = new System.Drawing.Point(38, 470);
            this.buttonOnOff.Name = "buttonOnOff";
            this.buttonOnOff.Size = new System.Drawing.Size(75, 23);
            this.buttonOnOff.TabIndex = 1;
            this.buttonOnOff.Text = "On/Off";
            this.buttonOnOff.UseVisualStyleBackColor = true;
            this.buttonOnOff.Click += new System.EventHandler(this.ButtonOnOff_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelStatus.Location = new System.Drawing.Point(38, 19);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(89, 21);
            this.labelStatus.TabIndex = 2;
            this.labelStatus.Text = "Connecting";
            this.labelStatus.Click += new System.EventHandler(this.labelStatus_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(38, 435);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(300, 30);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 3;
            // 
            // buttonReplaceBateries
            // 
            this.buttonReplaceBateries.Location = new System.Drawing.Point(205, 470);
            this.buttonReplaceBateries.Name = "buttonReplaceBateries";
            this.buttonReplaceBateries.Size = new System.Drawing.Size(133, 23);
            this.buttonReplaceBateries.TabIndex = 4;
            this.buttonReplaceBateries.Text = "Replace Bateries";
            this.buttonReplaceBateries.UseVisualStyleBackColor = true;
            this.buttonReplaceBateries.Click += new System.EventHandler(this.buttonReplaceBateries_Click);
            // 
            // LightbulbForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 555);
            this.Controls.Add(this.buttonReplaceBateries);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.buttonOnOff);
            this.Controls.Add(this.pictureBoxLightbulb);
            this.Name = "LightbulbForm";
            this.Text = "Smart Lightbulb";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LightbulbForm_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLightbulb)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private PictureBox pictureBoxLightbulb;
    private Button buttonOnOff;
    private Label labelStatus;
    private ProgressBar progressBar1;
    private Button buttonReplaceBateries;
}
