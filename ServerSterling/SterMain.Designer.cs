namespace SterServer
{
    partial class SterMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SterMain));
            this.msgbox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // msgbox
            // 
            this.msgbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msgbox.FormattingEnabled = true;
            this.msgbox.ItemHeight = 20;
            this.msgbox.Location = new System.Drawing.Point(0, 0);
            this.msgbox.Name = "msgbox";
            this.msgbox.Size = new System.Drawing.Size(278, 244);
            this.msgbox.TabIndex = 0;
            // 
            // SterMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 248);
            this.Controls.Add(this.msgbox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SterMain";
            this.Text = "SterServer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox msgbox;
    }
}

