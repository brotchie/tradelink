namespace TradeLink.AppKit
{
    partial class TradeResultsWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TradeResultsWindow));
            this.tradeResults1 = new TradeLink.AppKit.TradeResults();
            this.SuspendLayout();
            // 
            // tradeResults1
            // 
            this.tradeResults1.AutoWatch = false;
            this.tradeResults1.BackColor = System.Drawing.Color.White;
            this.tradeResults1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tradeResults1.Location = new System.Drawing.Point(0, 0);
            this.tradeResults1.Name = "tradeResults1";
            this.tradeResults1.Path = "C:\\Users\\jfranta\\Documents";
            this.tradeResults1.Size = new System.Drawing.Size(536, 364);
            this.tradeResults1.TabIndex = 0;
            // 
            // TradeResultsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 364);
            this.Controls.Add(this.tradeResults1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TradeResultsWindow";
            this.Text = "Results";
            this.ResumeLayout(false);

        }

        #endregion

        private TradeResults tradeResults1;
    }
}