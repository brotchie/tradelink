namespace ServerDAS
{
    partial class DASServerMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.toginfo = new System.Windows.Forms.Button();
            this._verbose = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User_ID";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(91, 22);
            this.textBoxID.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(96, 20);
            this.textBoxID.TabIndex = 1;
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(91, 65);
            this.textBoxPwd.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.PasswordChar = '*';
            this.textBoxPwd.Size = new System.Drawing.Size(96, 20);
            this.textBoxPwd.TabIndex = 3;
            this.textBoxPwd.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 65);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(119, 89);
            this.buttonLogin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(68, 23);
            this.buttonLogin.TabIndex = 4;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.ForeColor = System.Drawing.Color.Tomato;
            this.labelMessage.Location = new System.Drawing.Point(44, 22);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 13);
            this.labelMessage.TabIndex = 5;
            // 
            // toginfo
            // 
            this.toginfo.Location = new System.Drawing.Point(166, 118);
            this.toginfo.Name = "toginfo";
            this.toginfo.Size = new System.Drawing.Size(20, 23);
            this.toginfo.TabIndex = 6;
            this.toginfo.Text = "!";
            this.toolTip1.SetToolTip(this.toginfo, "Show/hide connector message window");
            this.toginfo.UseVisualStyleBackColor = true;
            this.toginfo.Click += new System.EventHandler(this.toginfo_Click);
            // 
            // _verbose
            // 
            this._verbose.Appearance = System.Windows.Forms.Appearance.Button;
            this._verbose.AutoSize = true;
            this._verbose.Checked = global::ServerDAS.Properties.Settings.Default.VerboseDebugging;
            this._verbose.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ServerDAS.Properties.Settings.Default, "VerboseDebugging", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._verbose.Location = new System.Drawing.Point(32, 118);
            this._verbose.Name = "_verbose";
            this._verbose.Size = new System.Drawing.Size(74, 23);
            this._verbose.TabIndex = 7;
            this._verbose.Text = "Verbose: off";
            this.toolTip1.SetToolTip(this._verbose, "Toggle level of debugging in message window");
            this._verbose.UseVisualStyleBackColor = true;
            this._verbose.CheckedChanged += new System.EventHandler(this._verbose_CheckedChanged);
            // 
            // DASServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 165);
            this.Controls.Add(this._verbose);
            this.Controls.Add(this.toginfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPwd);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DASServerMain";
            this.Text = "ServerDAS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button toginfo;
        private System.Windows.Forms.CheckBox _verbose;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

