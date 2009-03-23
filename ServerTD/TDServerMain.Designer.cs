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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._login = new System.Windows.Forms.Button();
            this._pass = new System.Windows.Forms.TextBox();
            this._user = new System.Windows.Forms.TextBox();
            this._sourceid = new System.Windows.Forms.TextBox();
            this._msg = new System.Windows.Forms.ListBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(49, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "SourceID:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(49, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Password:";
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(136, 145);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 28);
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
            this._pass.Location = new System.Drawing.Point(136, 113);
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
            this._user.Location = new System.Drawing.Point(136, 81);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(100, 26);
            this._user.TabIndex = 1;
            this._user.Text = global::TDServer.Properties.Settings.Default.username;
            // 
            // _sourceid
            // 
            this._sourceid.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TDServer.Properties.Settings.Default, "sourceid", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._sourceid.Location = new System.Drawing.Point(136, 49);
            this._sourceid.Name = "_sourceid";
            this._sourceid.Size = new System.Drawing.Size(100, 26);
            this._sourceid.TabIndex = 0;
            this._sourceid.Text = global::TDServer.Properties.Settings.Default.sourceid;
            // 
            // _msg
            // 
            this._msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this._msg.FormattingEnabled = true;
            this._msg.ItemHeight = 20;
            this._msg.Location = new System.Drawing.Point(0, 0);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(353, 184);
            this._msg.TabIndex = 7;
            this._msg.Visible = false;
            // 
            // TDServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 185);
            this.Controls.Add(this._login);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this._sourceid);
            this.Controls.Add(this._msg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TDServerMain";
            this.Text = "TDServer BETA";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _sourceid;
        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ListBox _msg;
    }
}

