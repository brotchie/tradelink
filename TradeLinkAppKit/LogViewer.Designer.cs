namespace TradeLink.AppKit
{
    partial class LogViewer
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
            this._logs = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _logs
            // 
            this._logs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logs.FormattingEnabled = true;
            this._logs.ItemHeight = 20;
            this._logs.Location = new System.Drawing.Point(0, 0);
            this._logs.Name = "_logs";
            this._logs.Size = new System.Drawing.Size(278, 244);
            this._logs.TabIndex = 0;
            // 
            // LogViewerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 248);
            this.Controls.Add(this._logs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LogViewerMain";
            this.Text = "Log Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox _logs;
    }
}

