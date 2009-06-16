namespace TradeLink.AppKit
{
    partial class PopupWindow
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
            this._msg = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _msg
            // 
            this._msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this._msg.Location = new System.Drawing.Point(0, 0);
            this._msg.Multiline = true;
            this._msg.Name = "_msg";
            this._msg.ReadOnly = true;
            this._msg.Size = new System.Drawing.Size(249, 90);
            this._msg.TabIndex = 0;
            // 
            // PopupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(249, 90);
            this.Controls.Add(this._msg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PopupWindow";
            this.Text = "Notice";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _msg;
    }
}