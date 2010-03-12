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
            this.components = new System.ComponentModel.Container();
            this._send = new System.Windows.Forms.Button();
            this._user = new System.Windows.Forms.TextBox();
            this._pass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._desc = new System.Windows.Forms.TextBox();
            this._body = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // _send
            // 
            this._send.Location = new System.Drawing.Point(272, 42);
            this._send.Name = "_send";
            this._send.Size = new System.Drawing.Size(63, 27);
            this._send.TabIndex = 0;
            this._send.Text = "send";
            this._send.UseVisualStyleBackColor = true;
            this._send.Click += new System.EventHandler(this._email_Click);
            // 
            // _user
            // 
            this._user.Location = new System.Drawing.Point(66, 12);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(100, 26);
            this._user.TabIndex = 2;
            this.toolTip1.SetToolTip(this._user, "Valid google account (double-click to create one)");
            this._user.DoubleClick += new System.EventHandler(this._user_DoubleClick);
            // 
            // _pass
            // 
            this._pass.Location = new System.Drawing.Point(235, 10);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(100, 26);
            this._pass.TabIndex = 3;
            this._pass.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(181, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Pass:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Desc:";
            // 
            // _desc
            // 
            this._desc.Location = new System.Drawing.Point(66, 44);
            this._desc.Name = "_desc";
            this._desc.Size = new System.Drawing.Size(191, 26);
            this._desc.TabIndex = 7;
            // 
            // _body
            // 
            this._body.Location = new System.Drawing.Point(21, 83);
            this._body.Multiline = true;
            this._body.Name = "_body";
            this._body.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._body.Size = new System.Drawing.Size(314, 194);
            this._body.TabIndex = 8;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(17, 13);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(47, 20);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "User:";
            this.toolTip1.SetToolTip(this.linkLabel1, "Click to create a new google account if you don\'t have one.");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // CrashReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 295);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this._body);
            this.Controls.Add(this._desc);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this._send);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CrashReport";
            this.Text = "Report a bug or issue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _send;
        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _desc;
        private System.Windows.Forms.TextBox _body;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}