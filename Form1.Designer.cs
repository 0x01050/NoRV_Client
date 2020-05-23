namespace NoRV
{
    partial class Form1
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
            this.lblCase1 = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblTimeZone = new System.Windows.Forms.Label();
            this.lblVideographer = new System.Windows.Forms.Label();
            this.Witness = new System.Windows.Forms.TextBox();
            this.Case1 = new System.Windows.Forms.TextBox();
            this.Address = new System.Windows.Forms.TextBox();
            this.Videographer = new System.Windows.Forms.TextBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.TimeZone = new System.Windows.Forms.ComboBox();
            this.Counsel = new System.Windows.Forms.ComboBox();
            this.lblCounsel = new System.Windows.Forms.Label();
            this.Commission = new System.Windows.Forms.TextBox();
            this.lblCommission = new System.Windows.Forms.Label();
            this.Case2 = new System.Windows.Forms.TextBox();
            this.lblCase2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblWitness
            // 
            this.lblWitness.AutoSize = true;
            this.lblWitness.Location = new System.Drawing.Point(25, 29);
            this.lblWitness.Name = "lblWitness";
            this.lblWitness.Size = new System.Drawing.Size(111, 13);
            this.lblWitness.TabIndex = 0;
            this.lblWitness.Text = "Enter name of witness";
            // 
            // lblCase1
            // 
            this.lblCase1.AutoSize = true;
            this.lblCase1.Location = new System.Drawing.Point(25, 54);
            this.lblCase1.Name = "lblCase1";
            this.lblCase1.Size = new System.Drawing.Size(67, 13);
            this.lblCase1.TabIndex = 0;
            this.lblCase1.Text = "Enter case 1";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(25, 154);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Address";
            // 
            // lblTimeZone
            // 
            this.lblTimeZone.AutoSize = true;
            this.lblTimeZone.Location = new System.Drawing.Point(25, 129);
            this.lblTimeZone.Name = "lblTimeZone";
            this.lblTimeZone.Size = new System.Drawing.Size(89, 13);
            this.lblTimeZone.TabIndex = 0;
            this.lblTimeZone.Text = "Time Zone (PDT)";
            // 
            // lblVideographer
            // 
            this.lblVideographer.AutoSize = true;
            this.lblVideographer.Location = new System.Drawing.Point(25, 179);
            this.lblVideographer.Name = "lblVideographer";
            this.lblVideographer.Size = new System.Drawing.Size(112, 13);
            this.lblVideographer.TabIndex = 0;
            this.lblVideographer.Text = "Name of videographer";
            // 
            // Witness
            // 
            this.Witness.Location = new System.Drawing.Point(162, 25);
            this.Witness.Name = "Witness";
            this.Witness.Size = new System.Drawing.Size(110, 20);
            this.Witness.TabIndex = 1;
            this.Witness.TextChanged += new System.EventHandler(this.Validate);
            // 
            // Case1
            // 
            this.Case1.Location = new System.Drawing.Point(162, 50);
            this.Case1.Name = "Case1";
            this.Case1.Size = new System.Drawing.Size(110, 20);
            this.Case1.TabIndex = 2;
            this.Case1.TextChanged += new System.EventHandler(this.Validate);
            // 
            // Address
            // 
            this.Address.Location = new System.Drawing.Point(162, 152);
            this.Address.Name = "Address";
            this.Address.Size = new System.Drawing.Size(110, 20);
            this.Address.TabIndex = 6;
            this.Address.TextChanged += new System.EventHandler(this.Validate);
            // 
            // Videographer
            // 
            this.Videographer.Location = new System.Drawing.Point(162, 177);
            this.Videographer.Name = "Videographer";
            this.Videographer.Size = new System.Drawing.Size(110, 20);
            this.Videographer.TabIndex = 7;
            this.Videographer.TextChanged += new System.EventHandler(this.Validate);
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(294, 201);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 9;
            this.btnNext.Text = "NEXT";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // TimeZone
            // 
            this.TimeZone.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeZone.FormattingEnabled = true;
            this.TimeZone.Items.AddRange(new object[] {
            "PDT (GMT -7)",
            "EDT (GMT -4)",
            "CDT (GMT -5)",
            "MDT (GMT -6)",
            "MST (GMT -7)",
            "ADT (GMT -8)",
            "HAST (GMT -10)"});
            this.TimeZone.Location = new System.Drawing.Point(162, 126);
            this.TimeZone.Name = "TimeZone";
            this.TimeZone.Size = new System.Drawing.Size(110, 21);
            this.TimeZone.TabIndex = 5;
            this.TimeZone.SelectedIndexChanged += new System.EventHandler(this.Validate);
            // 
            // Counsel
            // 
            this.Counsel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Counsel.FormattingEnabled = true;
            this.Counsel.Items.AddRange(new object[] {
            "Plaintiffs",
            "Defendents"});
            this.Counsel.Location = new System.Drawing.Point(162, 100);
            this.Counsel.Name = "Counsel";
            this.Counsel.Size = new System.Drawing.Size(110, 21);
            this.Counsel.TabIndex = 4;
            this.Counsel.SelectedIndexChanged += new System.EventHandler(this.Validate);
            // 
            // lblCounsel
            // 
            this.lblCounsel.AutoSize = true;
            this.lblCounsel.Location = new System.Drawing.Point(25, 104);
            this.lblCounsel.Name = "lblCounsel";
            this.lblCounsel.Size = new System.Drawing.Size(60, 13);
            this.lblCounsel.TabIndex = 0;
            this.lblCounsel.Text = "Counsel for";
            // 
            // Commission
            // 
            this.Commission.Location = new System.Drawing.Point(162, 202);
            this.Commission.Name = "Commission";
            this.Commission.Size = new System.Drawing.Size(110, 20);
            this.Commission.TabIndex = 8;
            this.Commission.TextChanged += new System.EventHandler(this.Validate);
            // 
            // lblCommission
            // 
            this.lblCommission.AutoSize = true;
            this.lblCommission.Location = new System.Drawing.Point(25, 204);
            this.lblCommission.Name = "lblCommission";
            this.lblCommission.Size = new System.Drawing.Size(100, 13);
            this.lblCommission.TabIndex = 0;
            this.lblCommission.Text = "Commission number";
            // 
            // Case2
            // 
            this.Case2.Location = new System.Drawing.Point(162, 75);
            this.Case2.Name = "Case2";
            this.Case2.Size = new System.Drawing.Size(110, 20);
            this.Case2.TabIndex = 3;
            // 
            // lblCase2
            // 
            this.lblCase2.AutoSize = true;
            this.lblCase2.Location = new System.Drawing.Point(25, 79);
            this.lblCase2.Name = "lblCase2";
            this.lblCase2.Size = new System.Drawing.Size(67, 13);
            this.lblCase2.TabIndex = 0;
            this.lblCase2.Text = "Enter case 2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(384, 236);
            this.Controls.Add(this.Case2);
            this.Controls.Add(this.lblCase2);
            this.Controls.Add(this.Commission);
            this.Controls.Add(this.lblCommission);
            this.Controls.Add(this.Counsel);
            this.Controls.Add(this.lblCounsel);
            this.Controls.Add(this.TimeZone);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.Videographer);
            this.Controls.Add(this.Address);
            this.Controls.Add(this.Case1);
            this.Controls.Add(this.Witness);
            this.Controls.Add(this.lblVideographer);
            this.Controls.Add(this.lblTimeZone);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.lblCase1);
            this.Controls.Add(this.lblWitness);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 275);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 275);
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(25);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoRV TTS - Step 1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWitness;
        private System.Windows.Forms.Label lblCase1;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblTimeZone;
        private System.Windows.Forms.Label lblVideographer;
        private System.Windows.Forms.TextBox Witness;
        private System.Windows.Forms.TextBox Case1;
        private System.Windows.Forms.TextBox Address;
        private System.Windows.Forms.TextBox Videographer;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.ComboBox TimeZone;
        private System.Windows.Forms.ComboBox Counsel;
        private System.Windows.Forms.Label lblCounsel;
        private System.Windows.Forms.TextBox Commission;
        private System.Windows.Forms.Label lblCommission;
        private System.Windows.Forms.TextBox Case2;
        private System.Windows.Forms.Label lblCase2;
    }
}

