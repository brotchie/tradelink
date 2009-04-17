namespace TradeLink.Common
{
    partial class TwitPopup
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
            this.twitControl1 = new TradeLink.Common.TwitControl();
            this.SuspendLayout();
            // 
            // twitControl1
            // 
            this.twitControl1.Location = new System.Drawing.Point(4, -1);
            this.twitControl1.Name = "twitControl1";
            this.twitControl1.Size = new System.Drawing.Size(355, 62);
            this.twitControl1.TabIndex = 0;
            // 
            // TwitPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 73);
            this.Controls.Add(this.twitControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "TwitPopup";
            this.Text = "TradeLink Twit";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private TwitControl twitControl1;

    }
}