namespace TradeLink.AppKit
{
    partial class CrashReport
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
            this._email = new System.Windows.Forms.Button();
            this._ignore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _email
            // 
            this._email.Location = new System.Drawing.Point(12, 12);
            this._email.Name = "_email";
            this._email.Size = new System.Drawing.Size(245, 40);
            this._email.TabIndex = 0;
            this._email.Text = "Send crash info to TradeLink";
            this._email.UseVisualStyleBackColor = true;
            this._email.Click += new System.EventHandler(this._email_Click);
            // 
            // _ignore
            // 
            this._ignore.Location = new System.Drawing.Point(13, 59);
            this._ignore.Name = "_ignore";
            this._ignore.Size = new System.Drawing.Size(244, 34);
            this._ignore.TabIndex = 1;
            this._ignore.Text = "Ignore and quit";
            this._ignore.UseVisualStyleBackColor = true;
            this._ignore.Click += new System.EventHandler(this._ignore_Click);
            // 
            // CrashReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(269, 105);
            this.Controls.Add(this._ignore);
            this.Controls.Add(this._email);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CrashReport";
            this.Text = "An application has crashed.";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _email;
        private System.Windows.Forms.Button _ignore;
    }
}