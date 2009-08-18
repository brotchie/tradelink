namespace ServerBlackwood
{
    partial class ServerBlackwoodMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerBlackwoodMain));
            this._report = new System.Windows.Forms.Button();
            this._togmsg = new System.Windows.Forms.Button();
            this._login = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._pw = new System.Windows.Forms.TextBox();
            this._un = new System.Windows.Forms.TextBox();
            this._ipaddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(217, 101);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(32, 27);
            this._report.TabIndex = 15;
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // _togmsg
            // 
            this._togmsg.Location = new System.Drawing.Point(175, 101);
            this._togmsg.Name = "_togmsg";
            this._togmsg.Size = new System.Drawing.Size(35, 27);
            this._togmsg.TabIndex = 14;
            this._togmsg.Text = "!";
            this._togmsg.UseVisualStyleBackColor = true;
            this._togmsg.Click += new System.EventHandler(this._togmsg_Click);
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(94, 101);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 27);
            this._login.TabIndex = 13;
            this._login.Text = "login";
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Pass:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "User:";
            // 
            // _pw
            // 
            this._pw.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerBlackwood.Properties.Settings.Default, "pw", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pw.Location = new System.Drawing.Point(94, 69);
            this._pw.Name = "_pw";
            this._pw.PasswordChar = '*';
            this._pw.Size = new System.Drawing.Size(100, 26);
            this._pw.TabIndex = 10;
            this._pw.Text = global::ServerBlackwood.Properties.Settings.Default.pw;
            this._pw.UseSystemPasswordChar = true;
            // 
            // _un
            // 
            this._un.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerBlackwood.Properties.Settings.Default, "un", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._un.Location = new System.Drawing.Point(94, 37);
            this._un.Name = "_un";
            this._un.Size = new System.Drawing.Size(100, 26);
            this._un.TabIndex = 9;
            this._un.Text = global::ServerBlackwood.Properties.Settings.Default.un;
            // 
            // _ipaddress
            // 
            this._ipaddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerBlackwood.Properties.Settings.Default, "ipaddress", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._ipaddress.Location = new System.Drawing.Point(94, 5);
            this._ipaddress.Name = "_ipaddress";
            this._ipaddress.Size = new System.Drawing.Size(100, 26);
            this._ipaddress.TabIndex = 16;
            this._ipaddress.Text = global::ServerBlackwood.Properties.Settings.Default.ipaddress;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = "Address:";
            // 
            // ServerBlackwoodMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(278, 138);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._ipaddress);
            this.Controls.Add(this._report);
            this.Controls.Add(this._togmsg);
            this.Controls.Add(this._login);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._pw);
            this.Controls.Add(this._un);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerBlackwoodMain";
            this.Text = "Blackwood";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _report;
        private System.Windows.Forms.Button _togmsg;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _pw;
        private System.Windows.Forms.TextBox _un;
        private System.Windows.Forms.TextBox _ipaddress;
        private System.Windows.Forms.Label label3;
    }
}

