namespace TradeLink.Common
{
    partial class NewVersion
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
            this.urlloclab = new System.Windows.Forms.LinkLabel();
            this.statuslab = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // urlloclab
            // 
            this.urlloclab.AutoSize = true;
            this.urlloclab.Location = new System.Drawing.Point(12, 51);
            this.urlloclab.Name = "urlloclab";
            this.urlloclab.Size = new System.Drawing.Size(191, 20);
            this.urlloclab.TabIndex = 0;
            this.urlloclab.TabStop = true;
            this.urlloclab.Text = "A new version is available.";
            // 
            // statuslab
            // 
            this.statuslab.AutoSize = true;
            this.statuslab.Location = new System.Drawing.Point(12, 9);
            this.statuslab.Name = "statuslab";
            this.statuslab.Size = new System.Drawing.Size(227, 20);
            this.statuslab.TabIndex = 1;
            this.statuslab.Text = "You are running an old version.";
            // 
            // NewVersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 80);
            this.Controls.Add(this.statuslab);
            this.Controls.Add(this.urlloclab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewVersion";
            this.Text = "Update Available";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel urlloclab;
        private System.Windows.Forms.Label statuslab;
    }
}