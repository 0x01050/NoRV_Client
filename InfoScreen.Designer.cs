namespace NoRV
{
    partial class InfoScreen
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
            this.lblWitness = new System.Windows.Forms.Label();
            this.lblCaseName = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblTimeZone = new System.Windows.Forms.Label();
            this.lblVideographer = new System.Windows.Forms.Label();
            this.Witness = new System.Windows.Forms.TextBox();
            this.CaseName = new System.Windows.Forms.TextBox();
            this.Address = new System.Windows.Forms.TextBox();
            this.Videographer = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.TimeZone = new System.Windows.Forms.ComboBox();
            this.Counsel = new System.Windows.Forms.ComboBox();
            this.lblCounsel = new System.Windows.Forms.Label();
            this.Commission = new System.Windows.Forms.TextBox();
            this.lblCommission = new System.Windows.Forms.Label();
            this.Template = new System.Windows.Forms.ComboBox();
            this.lblTemplate = new System.Windows.Forms.Label();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblNoRVMachineID = new System.Windows.Forms.Label();
            this.txtNoRVMachineID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lblWitness
            // 
            this.lblWitness.AutoSize = true;
            this.lblWitness.Location = new System.Drawing.Point(25, 81);
            this.lblWitness.Name = "lblWitness";
            this.lblWitness.Size = new System.Drawing.Size(111, 13);
            this.lblWitness.TabIndex = 0;
            this.lblWitness.Text = "Enter name of witness";
            // 
            // lblCaseName
            // 
            this.lblCaseName.AutoSize = true;
            this.lblCaseName.Location = new System.Drawing.Point(25, 106);
            this.lblCaseName.Name = "lblCaseName";
            this.lblCaseName.Size = new System.Drawing.Size(87, 13);
            this.lblCaseName.TabIndex = 0;
            this.lblCaseName.Text = "Enter case name";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(25, 181);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Address";
            // 
            // lblTimeZone
            // 
            this.lblTimeZone.AutoSize = true;
            this.lblTimeZone.Location = new System.Drawing.Point(25, 156);
            this.lblTimeZone.Name = "lblTimeZone";
            this.lblTimeZone.Size = new System.Drawing.Size(89, 13);
            this.lblTimeZone.TabIndex = 0;
            this.lblTimeZone.Text = "Time Zone (PDT)";
            // 
            // lblVideographer
            // 
            this.lblVideographer.AutoSize = true;
            this.lblVideographer.Location = new System.Drawing.Point(25, 206);
            this.lblVideographer.Name = "lblVideographer";
            this.lblVideographer.Size = new System.Drawing.Size(112, 13);
            this.lblVideographer.TabIndex = 0;
            this.lblVideographer.Text = "Name of videographer";
            // 
            // Witness
            // 
            this.Witness.Location = new System.Drawing.Point(162, 77);
            this.Witness.Name = "Witness";
            this.Witness.Size = new System.Drawing.Size(207, 20);
            this.Witness.TabIndex = 2;
            this.Witness.TextChanged += new System.EventHandler(this.Validate);
            // 
            // CaseName
            // 
            this.CaseName.Location = new System.Drawing.Point(162, 102);
            this.CaseName.Name = "CaseName";
            this.CaseName.Size = new System.Drawing.Size(207, 20);
            this.CaseName.TabIndex = 3;
            this.CaseName.TextChanged += new System.EventHandler(this.Validate);
            // 
            // Address
            // 
            this.Address.Location = new System.Drawing.Point(162, 179);
            this.Address.Name = "Address";
            this.Address.Size = new System.Drawing.Size(207, 20);
            this.Address.TabIndex = 7;
            this.Address.TextChanged += new System.EventHandler(this.Validate);
            // 
            // Videographer
            // 
            this.Videographer.Location = new System.Drawing.Point(162, 204);
            this.Videographer.Name = "Videographer";
            this.Videographer.Size = new System.Drawing.Size(207, 20);
            this.Videographer.TabIndex = 8;
            this.Videographer.TextChanged += new System.EventHandler(this.Validate);
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(385, 228);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 10;
            this.btnNext.Text = "NEXT";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // TimeZone
            // 
            this.TimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeZone.FormattingEnabled = true;
            this.TimeZone.Items.AddRange(new object[] {
            "Pacific Time (PDT)",
            "Eastern Time (EDT)",
            "Central Time (CDT)",
            "Mountain Time (MDT)",
            "Mountain Standard Time (MST)",
            "Alaska Time (ADT)",
            "Hawaii-Aleutian Standard Time (HAST)"});
            this.TimeZone.Location = new System.Drawing.Point(162, 153);
            this.TimeZone.Name = "TimeZone";
            this.TimeZone.Size = new System.Drawing.Size(207, 21);
            this.TimeZone.TabIndex = 6;
            this.TimeZone.SelectedIndexChanged += new System.EventHandler(this.Validate);
            // 
            // Counsel
            // 
            this.Counsel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Counsel.FormattingEnabled = true;
            this.Counsel.Items.AddRange(new object[] {
            "Plaintiffs",
            "Defendants"});
            this.Counsel.Location = new System.Drawing.Point(162, 127);
            this.Counsel.Name = "Counsel";
            this.Counsel.Size = new System.Drawing.Size(207, 21);
            this.Counsel.TabIndex = 5;
            this.Counsel.SelectedIndexChanged += new System.EventHandler(this.Validate);
            // 
            // lblCounsel
            // 
            this.lblCounsel.AutoSize = true;
            this.lblCounsel.Location = new System.Drawing.Point(25, 131);
            this.lblCounsel.Name = "lblCounsel";
            this.lblCounsel.Size = new System.Drawing.Size(60, 13);
            this.lblCounsel.TabIndex = 0;
            this.lblCounsel.Text = "Counsel for";
            // 
            // Commission
            // 
            this.Commission.Location = new System.Drawing.Point(162, 229);
            this.Commission.Name = "Commission";
            this.Commission.Size = new System.Drawing.Size(207, 20);
            this.Commission.TabIndex = 9;
            this.Commission.TextChanged += new System.EventHandler(this.Validate);
            // 
            // lblCommission
            // 
            this.lblCommission.AutoSize = true;
            this.lblCommission.Location = new System.Drawing.Point(25, 231);
            this.lblCommission.Name = "lblCommission";
            this.lblCommission.Size = new System.Drawing.Size(100, 13);
            this.lblCommission.TabIndex = 0;
            this.lblCommission.Text = "Commission number";
            // 
            // Template
            // 
            this.Template.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Template.FormattingEnabled = true;
            this.Template.Items.AddRange(new object[] {
            "Personal",
            "30B6"});
            this.Template.Location = new System.Drawing.Point(162, 51);
            this.Template.Name = "Template";
            this.Template.Size = new System.Drawing.Size(207, 21);
            this.Template.TabIndex = 1;
            this.Template.SelectedIndexChanged += new System.EventHandler(this.Validate);
            // 
            // lblTemplate
            // 
            this.lblTemplate.AutoSize = true;
            this.lblTemplate.Location = new System.Drawing.Point(25, 55);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(51, 13);
            this.lblTemplate.TabIndex = 0;
            this.lblTemplate.Text = "Template";
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(385, 51);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "LOAD";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // lblNoRVMachineID
            // 
            this.lblNoRVMachineID.AutoSize = true;
            this.lblNoRVMachineID.Location = new System.Drawing.Point(25, 16);
            this.lblNoRVMachineID.Name = "lblNoRVMachineID";
            this.lblNoRVMachineID.Size = new System.Drawing.Size(100, 13);
            this.lblNoRVMachineID.TabIndex = 0;
            this.lblNoRVMachineID.Text = "NoRV Machine ID :";
            // 
            // txtNoRVMachineID
            // 
            this.txtNoRVMachineID.Location = new System.Drawing.Point(160, 13);
            this.txtNoRVMachineID.Name = "txtNoRVMachineID";
            this.txtNoRVMachineID.ReadOnly = true;
            this.txtNoRVMachineID.Size = new System.Drawing.Size(300, 20);
            this.txtNoRVMachineID.TabIndex = 0;
            this.txtNoRVMachineID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // InfoScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(484, 261);
            this.Controls.Add(this.txtNoRVMachineID);
            this.Controls.Add(this.lblNoRVMachineID);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.Template);
            this.Controls.Add(this.lblTemplate);
            this.Controls.Add(this.Commission);
            this.Controls.Add(this.lblCommission);
            this.Controls.Add(this.Counsel);
            this.Controls.Add(this.lblCounsel);
            this.Controls.Add(this.TimeZone);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.Videographer);
            this.Controls.Add(this.Address);
            this.Controls.Add(this.CaseName);
            this.Controls.Add(this.Witness);
            this.Controls.Add(this.lblVideographer);
            this.Controls.Add(this.lblTimeZone);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblCaseName);
            this.Controls.Add(this.lblWitness);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 300);
            this.Name = "InfoScreen";
            this.Padding = new System.Windows.Forms.Padding(25);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoRV TTS - Step 1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InfoScreen_FormClosing);
            this.Load += new System.EventHandler(this.InfoScreen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWitness;
        private System.Windows.Forms.Label lblCaseName;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblTimeZone;
        private System.Windows.Forms.Label lblVideographer;
        private System.Windows.Forms.TextBox Witness;
        private System.Windows.Forms.TextBox CaseName;
        private System.Windows.Forms.TextBox Address;
        private System.Windows.Forms.TextBox Videographer;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ComboBox TimeZone;
        private System.Windows.Forms.ComboBox Counsel;
        private System.Windows.Forms.Label lblCounsel;
        private System.Windows.Forms.TextBox Commission;
        private System.Windows.Forms.Label lblCommission;
        private System.Windows.Forms.ComboBox Template;
        private System.Windows.Forms.Label lblTemplate;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblNoRVMachineID;
        private System.Windows.Forms.TextBox txtNoRVMachineID;
    }
}

