namespace ServerMB
{
    partial class ServerMBMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerMBMain));
            this._msg = new System.Windows.Forms.ListBox();
            this._user = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this._pass = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._loginbut = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this._id = new System.Windows.Forms.NumericUpDown();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._id)).BeginInit();
            this.SuspendLayout();
            // 
            // _msg
            // 
            this._msg.Dock = System.Windows.Forms.DockStyle.Fill;
            this._msg.FormattingEnabled = true;
            this._msg.ItemHeight = 20;
            this._msg.Location = new System.Drawing.Point(0, 0);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(346, 184);
            this._msg.TabIndex = 0;
            this._msg.Visible = false;
            // 
            // _user
            // 
            this._user.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerMB.Properties.Settings.Default, "username", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._user.Location = new System.Drawing.Point(131, 66);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(100, 26);
            this._user.TabIndex = 1;
            this._user.Text = global::ServerMB.Properties.Settings.Default.username;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Username:";
            // 
            // _pass
            // 
            this._pass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerMB.Properties.Settings.Default, "password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pass.Location = new System.Drawing.Point(130, 99);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(100, 26);
            this._pass.TabIndex = 3;
            this._pass.Text = global::ServerMB.Properties.Settings.Default.password;
            this._pass.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password:";
            // 
            // _loginbut
            // 
            this._loginbut.Location = new System.Drawing.Point(86, 140);
            this._loginbut.Name = "_loginbut";
            this._loginbut.Size = new System.Drawing.Size(92, 29);
            this._loginbut.TabIndex = 5;
            this._loginbut.Text = "login";
            this.toolTip1.SetToolTip(this._loginbut, "if you experience problems using MBTrading, please email tradelink-users@googlegr" +
                    "oups.com");
            this._loginbut.UseVisualStyleBackColor = true;
            this._loginbut.Click += new System.EventHandler(this._loginbut_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "MemberId:";
            // 
            // _id
            // 
            this._id.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::ServerMB.Properties.Settings.Default, "memberid", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._id.Location = new System.Drawing.Point(131, 34);
            this._id.Maximum = new decimal(new int[] {
            -727379968,
            232,
            0,
            0});
            this._id.Name = "_id";
            this._id.Size = new System.Drawing.Size(99, 26);
            this._id.TabIndex = 8;
            this._id.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._id.Value = global::ServerMB.Properties.Settings.Default.memberid;
            // 
            // ServerMBMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 186);
            this.Controls.Add(this._id);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._loginbut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._pass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._user);
            this.Controls.Add(this._msg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ServerMBMain";
            this.Text = "ServerMB BETA";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            ((System.ComponentModel.ISupportInitialize)(this._id)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox _msg;
        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _loginbut;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown _id;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

