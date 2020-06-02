namespace NoRV
{
    partial class MainScreen
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
            this.mainSplitter = new System.Windows.Forms.SplitContainer();
            this.splitter1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.speedTable = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.pitchTable = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblButtonStatus = new System.Windows.Forms.Label();
            this.btnSpeak = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.slSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.slPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).BeginInit();
            this.mainSplitter.Panel1.SuspendLayout();
            this.mainSplitter.Panel2.SuspendLayout();
            this.mainSplitter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitter1)).BeginInit();
            this.splitter1.Panel1.SuspendLayout();
            this.splitter1.Panel2.SuspendLayout();
            this.splitter1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.speedTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            this.pitchTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(0, 0);
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ReadOnly = true;
            this.txtSource.Size = new System.Drawing.Size(544, 326);
            this.txtSource.TabIndex = 0;
            this.txtSource.TabStop = false;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblText.Location = new System.Drawing.Point(0, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(75, 13);
            this.lblText.TabIndex = 1;
            this.lblText.Text = "Text to speak:";
            // 
            // lblVoice
            // 
            this.lblVoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblVoice.Location = new System.Drawing.Point(0, 0);
            this.lblVoice.Name = "lblVoice";
            this.lblVoice.Size = new System.Drawing.Size(175, 25);
            this.lblVoice.TabIndex = 2;
            this.lblVoice.Text = "Voice Name";
            this.lblVoice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbVoice
            // 
            this.cbVoice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVoice.FormattingEnabled = true;
            this.cbVoice.Location = new System.Drawing.Point(0, 0);
            this.cbVoice.Name = "cbVoice";
            this.cbVoice.Size = new System.Drawing.Size(175, 21);
            this.cbVoice.TabIndex = 3;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpeed.Location = new System.Drawing.Point(3, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(81, 25);
            this.lblSpeed.TabIndex = 4;
            this.lblSpeed.Text = "Speed";
            this.lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // slSpeed
            // 
            this.slSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slSpeed.LargeChange = 3;
            this.slSpeed.Location = new System.Drawing.Point(0, 0);
            this.slSpeed.Maximum = 15;
            this.slSpeed.Minimum = 8;
            this.slSpeed.Name = "slSpeed";
            this.slSpeed.Size = new System.Drawing.Size(175, 45);
            this.slSpeed.TabIndex = 5;
            this.slSpeed.Value = 10;
            this.slSpeed.ValueChanged += new System.EventHandler(this.slSpeed_ValueChanged);
            // 
            // slPitch
            // 
            this.slPitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slPitch.LargeChange = 10;
            this.slPitch.Location = new System.Drawing.Point(0, 0);
            this.slPitch.Maximum = 50;
            this.slPitch.Minimum = -50;
            this.slPitch.Name = "slPitch";
            this.slPitch.Size = new System.Drawing.Size(176, 45);
            this.slPitch.SmallChange = 5;
            this.slPitch.TabIndex = 6;
            this.slPitch.TickFrequency = 5;
            this.slPitch.ValueChanged += new System.EventHandler(this.slPitch_ValueChanged);
            // 
            // lblSpeedValue
            // 
            this.lblSpeedValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSpeedValue.Location = new System.Drawing.Point(90, 0);
            this.lblSpeedValue.Name = "lblSpeedValue";
            this.lblSpeedValue.Size = new System.Drawing.Size(82, 25);
            this.lblSpeedValue.TabIndex = 7;
            this.lblSpeedValue.Text = "1.00";
            this.lblSpeedValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPitchValue
            // 
            this.lblPitchValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPitchValue.Location = new System.Drawing.Point(91, 0);
            this.lblPitchValue.Name = "lblPitchValue";
            this.lblPitchValue.Size = new System.Drawing.Size(82, 25);
            this.lblPitchValue.TabIndex = 8;
            this.lblPitchValue.Text = "0.00";
            this.lblPitchValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPitch
            // 
            this.lblPitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPitch.Location = new System.Drawing.Point(3, 0);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(82, 25);
            this.lblPitch.TabIndex = 9;
            this.lblPitch.Text = "Pitch";
            this.lblPitch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // mainSplitter
            // 
            this.mainSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.mainSplitter.IsSplitterFixed = true;
            this.mainSplitter.Location = new System.Drawing.Point(20, 20);
            this.mainSplitter.Margin = new System.Windows.Forms.Padding(0);
            this.mainSplitter.Name = "mainSplitter";
            this.mainSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitter.Panel1
            // 
            this.mainSplitter.Panel1.Controls.Add(this.splitter1);
            // 
            // mainSplitter.Panel2
            // 
            this.mainSplitter.Panel2.Controls.Add(this.splitContainer1);
            this.mainSplitter.Panel2MinSize = 112;
            this.mainSplitter.Size = new System.Drawing.Size(544, 471);
            this.mainSplitter.SplitterDistance = 355;
            this.mainSplitter.TabIndex = 14;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitter1.IsSplitterFixed = true;
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter1.Panel1
            // 
            this.splitter1.Panel1.Controls.Add(this.lblText);
            this.splitter1.Panel1MinSize = 13;
            // 
            // splitter1.Panel2
            // 
            this.splitter1.Panel2.Controls.Add(this.txtSource);
            this.splitter1.Size = new System.Drawing.Size(544, 355);
            this.splitter1.SplitterDistance = 25;
            this.splitter1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Panel1MinSize = 68;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2MinSize = 40;
            this.splitContainer1.Size = new System.Drawing.Size(544, 112);
            this.splitContainer1.SplitterDistance = 68;
            this.splitContainer1.TabIndex = 15;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.splitContainer3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.splitContainer4, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.splitContainer5, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 68F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(544, 68);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.lblVoice);
            this.splitContainer3.Panel1MinSize = 13;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.cbVoice);
            this.splitContainer3.Panel2MinSize = 45;
            this.splitContainer3.Size = new System.Drawing.Size(175, 62);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(184, 3);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.speedTable);
            this.splitContainer4.Panel1MinSize = 13;
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.slSpeed);
            this.splitContainer4.Panel2MinSize = 45;
            this.splitContainer4.Size = new System.Drawing.Size(175, 62);
            this.splitContainer4.SplitterDistance = 25;
            this.splitContainer4.TabIndex = 1;
            // 
            // speedTable
            // 
            this.speedTable.ColumnCount = 2;
            this.speedTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.speedTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.speedTable.Controls.Add(this.lblSpeed, 0, 0);
            this.speedTable.Controls.Add(this.lblSpeedValue, 1, 0);
            this.speedTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedTable.Location = new System.Drawing.Point(0, 0);
            this.speedTable.Name = "speedTable";
            this.speedTable.RowCount = 1;
            this.speedTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.speedTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.speedTable.Size = new System.Drawing.Size(175, 25);
            this.speedTable.TabIndex = 0;
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(365, 3);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.pitchTable);
            this.splitContainer5.Panel1MinSize = 13;
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.slPitch);
            this.splitContainer5.Panel2MinSize = 45;
            this.splitContainer5.Size = new System.Drawing.Size(176, 62);
            this.splitContainer5.SplitterDistance = 25;
            this.splitContainer5.TabIndex = 2;
            // 
            // pitchTable
            // 
            this.pitchTable.ColumnCount = 2;
            this.pitchTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pitchTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pitchTable.Controls.Add(this.lblPitchValue, 1, 0);
            this.pitchTable.Controls.Add(this.lblPitch, 0, 0);
            this.pitchTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pitchTable.Location = new System.Drawing.Point(0, 0);
            this.pitchTable.Name = "pitchTable";
            this.pitchTable.RowCount = 1;
            this.pitchTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pitchTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.pitchTable.Size = new System.Drawing.Size(176, 25);
            this.pitchTable.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Panel1MinSize = 0;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer2.Panel2MinSize = 304;
            this.splitContainer2.Size = new System.Drawing.Size(544, 40);
            this.splitContainer2.SplitterDistance = 236;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.lblButtonStatus, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSpeak, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(304, 40);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblButtonStatus
            // 
            this.lblButtonStatus.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lblButtonStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblButtonStatus.Location = new System.Drawing.Point(3, 0);
            this.lblButtonStatus.Name = "lblButtonStatus";
            this.lblButtonStatus.Size = new System.Drawing.Size(146, 40);
            this.lblButtonStatus.TabIndex = 12;
            this.lblButtonStatus.Text = "Inititing...";
            this.lblButtonStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSpeak
            // 
            this.btnSpeak.BackColor = System.Drawing.SystemColors.Control;
            this.btnSpeak.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSpeak.Image = global::NoRV.Properties.Resources.play;
            this.btnSpeak.Location = new System.Drawing.Point(155, 0);
            this.btnSpeak.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.btnSpeak.Name = "btnSpeak";
            this.btnSpeak.Size = new System.Drawing.Size(146, 40);
            this.btnSpeak.TabIndex = 10;
            this.btnSpeak.Text = "SPEAK IT";
            this.btnSpeak.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSpeak.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSpeak.UseVisualStyleBackColor = false;
            this.btnSpeak.Click += new System.EventHandler(this.btnSpeak_Click);
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 511);
            this.Controls.Add(this.mainSplitter);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "MainScreen";
            this.Padding = new System.Windows.Forms.Padding(20);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoRV TTS - Step 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainScreen_FormClosing);
            this.Load += new System.EventHandler(this.MainScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.slSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.slPitch)).EndInit();
            this.mainSplitter.Panel1.ResumeLayout(false);
            this.mainSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitter)).EndInit();
            this.mainSplitter.ResumeLayout(false);
            this.splitter1.Panel1.ResumeLayout(false);
            this.splitter1.Panel1.PerformLayout();
            this.splitter1.Panel2.ResumeLayout(false);
            this.splitter1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitter1)).EndInit();
            this.splitter1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.speedTable.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            this.pitchTable.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.SplitContainer mainSplitter;
        private System.Windows.Forms.SplitContainer splitter1;
        private System.Windows.Forms.TableLayoutPanel speedTable;
        private System.Windows.Forms.TableLayoutPanel pitchTable;
        private System.Windows.Forms.Label lblButtonStatus;
        private System.Windows.Forms.Button btnSpeak;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.SplitContainer splitContainer5;
    }
}