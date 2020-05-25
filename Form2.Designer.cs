namespace NoRV
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSource = new System.Windows.Forms.TextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.lblVoice = new System.Windows.Forms.Label();
            this.cbVoice = new System.Windows.Forms.ComboBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.slSpeed = new System.Windows.Forms.TrackBar();
            this.slPitch = new System.Windows.Forms.TrackBar();
            this.lblSpeedValue = new System.Windows.Forms.Label();
            this.lblPitchValue = new System.Windows.Forms.Label();
            this.lblPitch = new System.Windows.Forms.Label();
            this.btnSpeak = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblButtonStatus = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.slSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slPitch)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(12, 39);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(775, 248);
            this.txtSource.TabIndex = 0;
            this.txtSource.TabStop = false;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(75, 13);
            this.lblText.TabIndex = 1;
            this.lblText.Text = "Text to speak:";
            // 
            // lblVoice
            // 
            this.lblVoice.AutoSize = true;
            this.lblVoice.Location = new System.Drawing.Point(12, 316);
            this.lblVoice.Name = "lblVoice";
            this.lblVoice.Size = new System.Drawing.Size(65, 13);
            this.lblVoice.TabIndex = 2;
            this.lblVoice.Text = "Voice Name";
            // 
            // cbVoice
            // 
            this.cbVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVoice.FormattingEnabled = true;
            this.cbVoice.Location = new System.Drawing.Point(12, 346);
            this.cbVoice.Name = "cbVoice";
            this.cbVoice.Size = new System.Drawing.Size(186, 21);
            this.cbVoice.TabIndex = 3;
            // 
            // lblSpeed
            // 
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(293, 316);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(38, 13);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Speed";
            // 
            // slSpeed
            // 
            this.slSpeed.LargeChange = 3;
            this.slSpeed.Location = new System.Drawing.Point(293, 346);
            this.slSpeed.Maximum = 15;
            this.slSpeed.Minimum = 8;
            this.slSpeed.Name = "slSpeed";
            this.slSpeed.Size = new System.Drawing.Size(186, 45);
            this.slSpeed.TabIndex = 5;
            this.slSpeed.Value = 10;
            this.slSpeed.ValueChanged += new System.EventHandler(this.slSpeed_ValueChanged);
            // 
            // slPitch
            // 
            this.slPitch.LargeChange = 10;
            this.slPitch.Location = new System.Drawing.Point(574, 346);
            this.slPitch.Maximum = 50;
            this.slPitch.Minimum = -50;
            this.slPitch.Name = "slPitch";
            this.slPitch.Size = new System.Drawing.Size(213, 45);
            this.slPitch.SmallChange = 5;
            this.slPitch.TabIndex = 6;
            this.slPitch.TickFrequency = 5;
            this.slPitch.ValueChanged += new System.EventHandler(this.slPitch_ValueChanged);
            // 
            // lblSpeedValue
            // 
            this.lblSpeedValue.AutoSize = true;
            this.lblSpeedValue.Location = new System.Drawing.Point(444, 316);
            this.lblSpeedValue.Name = "lblSpeedValue";
            this.lblSpeedValue.Size = new System.Drawing.Size(28, 13);
            this.lblSpeedValue.TabIndex = 7;
            this.lblSpeedValue.Text = "1.00";
            // 
            // lblPitchValue
            // 
            this.lblPitchValue.AutoSize = true;
            this.lblPitchValue.Location = new System.Drawing.Point(752, 316);
            this.lblPitchValue.Name = "lblPitchValue";
            this.lblPitchValue.Size = new System.Drawing.Size(28, 13);
            this.lblPitchValue.TabIndex = 8;
            this.lblPitchValue.Text = "0.00";
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Location = new System.Drawing.Point(574, 316);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(31, 13);
            this.lblPitch.TabIndex = 9;
            this.lblPitch.Text = "Pitch";
            // 
            // btnSpeak
            // 
            this.btnSpeak.BackColor = System.Drawing.SystemColors.Control;
            this.btnSpeak.Image = global::NoRV.Properties.Resources.play;
            this.btnSpeak.Location = new System.Drawing.Point(646, 408);
            this.btnSpeak.Name = "btnSpeak";
            this.btnSpeak.Size = new System.Drawing.Size(141, 40);
            this.btnSpeak.TabIndex = 10;
            this.btnSpeak.Text = "SPEAK IT";
            this.btnSpeak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSpeak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSpeak.UseVisualStyleBackColor = false;
            this.btnSpeak.Click += new System.EventHandler(this.btnSpeak_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 425);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Simulate";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblButtonStatus
            // 
            this.lblButtonStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblButtonStatus.Location = new System.Drawing.Point(464, 408);
            this.lblButtonStatus.Name = "lblButtonStatus";
            this.lblButtonStatus.Size = new System.Drawing.Size(141, 40);
            this.lblButtonStatus.TabIndex = 12;
            this.lblButtonStatus.Text = "Inititing...";
            this.lblButtonStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 461);
            this.Controls.Add(this.lblButtonStatus);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSpeak);
            this.Controls.Add(this.lblPitch);
            this.Controls.Add(this.lblPitchValue);
            this.Controls.Add(this.lblSpeedValue);
            this.Controls.Add(this.slPitch);
            this.Controls.Add(this.slSpeed);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.cbVoice);
            this.Controls.Add(this.lblVoice);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.txtSource);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(825, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(825, 500);
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoRV TTS - Step 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form2_FormClosing);
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.slSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slPitch)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.Label lblVoice;
        private System.Windows.Forms.ComboBox cbVoice;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TrackBar slSpeed;
        private System.Windows.Forms.TrackBar slPitch;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.Label lblPitchValue;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.Button btnSpeak;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblButtonStatus;
    }
}