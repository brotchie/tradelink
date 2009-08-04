namespace ServerDBFX
{
    partial class ServerDBFXMain
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
            this._un = new System.Windows.Forms.TextBox();
            this._pw = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._login = new System.Windows.Forms.Button();
            this._togmsg = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _un
            // 
            this._un.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerDBFX.Properties.Settings.Default, "un", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._un.Location = new System.Drawing.Point(79, 31);
            this._un.Name = "_un";
            this._un.Size = new System.Drawing.Size(100, 26);
            this._un.TabIndex = 0;
            this._un.Text = global::ServerDBFX.Properties.Settings.Default.un;
            // 
            // _pw
            // 
            this._pw.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerDBFX.Properties.Settings.Default, "pw", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pw.Location = new System.Drawing.Point(79, 63);
            this._pw.Name = "_pw";
            this._pw.PasswordChar = '*';
            this._pw.Size = new System.Drawing.Size(100, 26);
            this._pw.TabIndex = 1;
            this._pw.Text = global::ServerDBFX.Properties.Settings.Default.pw;
            this._pw.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Pass:";
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(79, 106);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 27);
            this._login.TabIndex = 4;
            this._login.Text = "login";
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // _togmsg
            // 
            this._togmsg.Location = new System.Drawing.Point(190, 106);
            this._togmsg.Name = "_togmsg";
            this._togmsg.Size = new System.Drawing.Size(35, 27);
            this._togmsg.TabIndex = 5;
            this._togmsg.Text = "!";
            this._togmsg.UseVisualStyleBackColor = true;
            this._togmsg.Click += new System.EventHandler(this._togmsg_Click);
            // 
            // ServerDBFXMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 145);
            this.Controls.Add(this._togmsg);
            this.Controls.Add(this._login);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._pw);
            this.Controls.Add(this._un);
            this.Name = "ServerDBFXMain";
            this.Text = "dbfx Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _un;
        private System.Windows.Forms.TextBox _pw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.Button _togmsg;
    }
}

