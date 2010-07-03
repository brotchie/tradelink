namespace TDServer
{
    partial class TDServerMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TDServerMain));
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._login = new System.Windows.Forms.Button();
            this._pass = new System.Windows.Forms.TextBox();
            this._user = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._report = new System.Windows.Forms.Button();
            this._togmsg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password:";
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(70, 115);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 34);
            this._login.TabIndex = 6;
            this._login.Text = "login";
            this.toolTip1.SetToolTip(this._login, "Email tradelink-users@googlegroups.com with any problems using TD ameritrade with" +
                    " tradelink");
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // _pass
            // 
            this._pass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TDServer.Properties.Settings.Default, "password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pass.Location = new System.Drawing.Point(136, 73);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(100, 26);
            this._pass.TabIndex = 2;
            this._pass.Text = global::TDServer.Properties.Settings.Default.password;
            this._pass.UseSystemPasswordChar = true;
            // 
            // _user
            // 
            this._user.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TDServer.Properties.Settings.Default, "username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._user.Location = new System.Drawing.Point(136, 41);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(100, 26);
            this._user.TabIndex = 1;
            this._user.Text = global::TDServer.Properties.Settings.Default.username;
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(192, 115);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(32, 33);
            this._report.TabIndex = 10;
            this.toolTip1.SetToolTip(this._report, "report a bug");
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // _togmsg
            // 
            this._togmsg.Location = new System.Drawing.Point(150, 115);
            this._togmsg.Name = "_togmsg";
            this._togmsg.Size = new System.Drawing.Size(35, 33);
            this._togmsg.TabIndex = 9;
            this._togmsg.Text = "!";
            this.toolTip1.SetToolTip(this._togmsg, "toggle connector messages");
            this._togmsg.UseVisualStyleBackColor = true;
            this._togmsg.Click += new System.EventHandler(this._togmsg_Click);
            // 
            // TDServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 175);
            this.Controls.Add(this._report);
            this.Controls.Add(this._togmsg);
            this.Controls.Add(this._login);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TDServerMain";
            this.Text = "TDServer BETA";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _report;
        private System.Windows.Forms.Button _togmsg;
    }
}

