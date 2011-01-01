namespace TradeLink.AppKit
{
    partial class TextPrompt
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
            this._msg = new System.Windows.Forms.Label();
            this._txt = new System.Windows.Forms.TextBox();
            this._ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _msg
            // 
            this._msg.AutoSize = true;
            this._msg.Location = new System.Drawing.Point(13, 13);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(0, 20);
            this._msg.TabIndex = 0;
            // 
            // _txt
            // 
            this._txt.Location = new System.Drawing.Point(13, 74);
            this._txt.Name = "_txt";
            this._txt.Size = new System.Drawing.Size(362, 26);
            this._txt.TabIndex = 1;
            // 
            // _ok
            // 
            this._ok.Location = new System.Drawing.Point(124, 111);
            this._ok.Name = "_ok";
            this._ok.Size = new System.Drawing.Size(102, 31);
            this._ok.TabIndex = 2;
            this._ok.Text = "ok";
            this._ok.UseVisualStyleBackColor = true;
            this._ok.Click += new System.EventHandler(this._ok_Click);
            // 
            // TextPrompt
            // 
            this.AcceptButton = this._ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 153);
            this.Controls.Add(this._ok);
            this.Controls.Add(this._txt);
            this.Controls.Add(this._msg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TextPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _msg;
        private System.Windows.Forms.TextBox _txt;
        private System.Windows.Forms.Button _ok;
    }
}