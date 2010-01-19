namespace ServerRedi
{
    partial class RediMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RediMain));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._user = new System.Windows.Forms.TextBox();
            this._pass = new System.Windows.Forms.TextBox();
            this._login = new System.Windows.Forms.Button();
            this._msg = new System.Windows.Forms.Button();
            this._report = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pass:";
            // 
            // _user
            // 
            this._user.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "un", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._user.Location = new System.Drawing.Point(66, 12);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(114, 26);
            this._user.TabIndex = 2;
            this._user.Text = global::ServerRedi.Properties.Settings.Default.un;
            // 
            // _pass
            // 
            this._pass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "pw", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pass.Location = new System.Drawing.Point(66, 48);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(114, 26);
            this._pass.TabIndex = 3;
            this._pass.Text = global::ServerRedi.Properties.Settings.Default.pw;
            this._pass.UseSystemPasswordChar = true;
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(45, 89);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 29);
            this._login.TabIndex = 4;
            this._login.Text = "Login";
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // _msg
            // 
            this._msg.Location = new System.Drawing.Point(127, 89);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(30, 29);
            this._msg.TabIndex = 5;
            this._msg.Text = "!";
            this.toolTip1.SetToolTip(this._msg, "Messages");
            this._msg.UseVisualStyleBackColor = true;
            this._msg.Click += new System.EventHandler(this._msg_Click);
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(163, 89);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(25, 29);
            this._report.TabIndex = 6;
            this.toolTip1.SetToolTip(this._report, "Submit bug or enhancement report ");
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // RediMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 139);
            this.Controls.Add(this._report);
            this.Controls.Add(this._msg);
            this.Controls.Add(this._login);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RediMain";
            this.Text = "RediServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.Button _msg;
        private System.Windows.Forms.Button _report;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

