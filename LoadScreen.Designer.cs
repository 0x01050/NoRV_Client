namespace NoRV
{
    partial class LoadScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoadScreen));
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.lstAppointments = new System.Windows.Forms.ListBox();
            this.splitter = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).BeginInit();
            this.splitter.Panel1.SuspendLayout();
            this.splitter.Panel2.SuspendLayout();
            this.splitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // picLoading
            // 
            this.picLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picLoading.ErrorImage = null;
            this.picLoading.Image = global::NoRV.Properties.Resources.loading;
            this.picLoading.InitialImage = null;
            this.picLoading.Location = new System.Drawing.Point(0, 0);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(150, 32);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picLoading.TabIndex = 0;
            this.picLoading.TabStop = false;
            // 
            // lstAppointments
            // 
            this.lstAppointments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAppointments.FormattingEnabled = true;
            this.lstAppointments.Location = new System.Drawing.Point(0, 0);
            this.lstAppointments.Name = "lstAppointments";
            this.lstAppointments.Size = new System.Drawing.Size(384, 211);
            this.lstAppointments.TabIndex = 1;
            this.lstAppointments.DoubleClick += new System.EventHandler(this.lstAppointments_DoubleClick);
            // 
            // splitter
            // 
            this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitter.IsSplitterFixed = true;
            this.splitter.Location = new System.Drawing.Point(0, 0);
            this.splitter.Name = "splitter";
            this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitter.Panel1
            // 
            this.splitter.Panel1.Controls.Add(this.picLoading);
            this.splitter.Panel1Collapsed = true;
            this.splitter.Panel1MinSize = 32;
            // 
            // splitter.Panel2
            // 
            this.splitter.Panel2.Controls.Add(this.lstAppointments);
            this.splitter.Panel2MinSize = 0;
            this.splitter.Size = new System.Drawing.Size(384, 211);
            this.splitter.SplitterDistance = 32;
            this.splitter.TabIndex = 2;
            // 
            // LoadScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 211);
            this.Controls.Add(this.splitter);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NoRV Client - Load Appointments";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoadScreen_FormClosing);
            this.Load += new System.EventHandler(this.LoadScreen_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.splitter.Panel1.ResumeLayout(false);
            this.splitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitter)).EndInit();
            this.splitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.ListBox lstAppointments;
        private System.Windows.Forms.SplitContainer splitter;
    }
}