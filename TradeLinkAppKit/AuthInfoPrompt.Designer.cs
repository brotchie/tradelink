namespace TradeLink.AppKit
{
    partial class AuthInfoPrompt
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._un = new System.Windows.Forms.TextBox();
            this._pw = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this._stat = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // _un
            // 
            this._un.Location = new System.Drawing.Point(111, 55);
            this._un.Name = "_un";
            this._un.Size = new System.Drawing.Size(100, 26);
            this._un.TabIndex = 2;
            // 
            // _pw
            // 
            this._pw.Location = new System.Drawing.Point(111, 100);
            this._pw.Name = "_pw";
            this._pw.PasswordChar = '*';
            this._pw.Size = new System.Drawing.Size(100, 26);
            this._pw.TabIndex = 3;
            this._pw.UseSystemPasswordChar = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 150);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 27);
            this.button1.TabIndex = 4;
            this.button1.Text = "Accept";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // _stat
            // 
            this._stat.AutoSize = true;
            this._stat.Location = new System.Drawing.Point(13, 13);
            this._stat.Name = "_stat";
            this._stat.Size = new System.Drawing.Size(242, 20);
            this._stat.TabIndex = 5;
            this._stat.Text = "Enter your portal login info below.";
            // 
            // AuthInfoPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(291, 187);
            this.Controls.Add(this._stat);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._pw);
            this.Controls.Add(this._un);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "AuthInfoPrompt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _un;
        private System.Windows.Forms.TextBox _pw;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label _stat;
    }
}