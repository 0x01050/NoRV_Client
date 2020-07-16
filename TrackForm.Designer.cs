namespace NoRV
{
    partial class TrackForm
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
            this.resultImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).BeginInit();
            this.SuspendLayout();
            // 
            // resultImage
            // 
            this.resultImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultImage.Location = new System.Drawing.Point(0, 0);
            this.resultImage.Name = "resultImage";
            this.resultImage.Size = new System.Drawing.Size(854, 480);
            this.resultImage.TabIndex = 0;
            this.resultImage.TabStop = false;
            this.resultImage.Paint += new System.Windows.Forms.PaintEventHandler(this.resultImage_Paint);
            // 
            // TrackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 480);
            this.Controls.Add(this.resultImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TrackForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TrackForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrackForm_FormClosing);
            this.Load += new System.EventHandler(this.TrackForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.resultImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox resultImage;
    }
}