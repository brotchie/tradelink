namespace ServerNxCore
{
    partial class ServerNxCoreMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerNxCoreMain));
            this.debugControl1 = new TradeLink.AppKit.DebugControl();
            this.SuspendLayout();
            // 
            // debugControl1
            // 
            this.debugControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugControl1.Location = new System.Drawing.Point(0, 0);
            this.debugControl1.Name = "debugControl1";
            this.debugControl1.Size = new System.Drawing.Size(278, 248);
            this.debugControl1.TabIndex = 0;
            this.debugControl1.TimeStamps = true;
            // 
            // ServerNxCoreMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 248);
            this.Controls.Add(this.debugControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerNxCoreMain";
            this.Text = "NxCore";
            this.ResumeLayout(false);

        }

        #endregion

        private TradeLink.AppKit.DebugControl debugControl1;
    }
}

