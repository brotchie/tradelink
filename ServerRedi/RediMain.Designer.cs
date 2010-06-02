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
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblAcr = new System.Windows.Forms.Label();
            this.txtAcronym = new System.Windows.Forms.TextBox();
            this.txtAccount = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Pass:";
            // 
            // _user
            // 
            this._user.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "un", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._user.Location = new System.Drawing.Point(64, 8);
            this._user.Margin = new System.Windows.Forms.Padding(2);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(77, 20);
            this._user.TabIndex = 2;
            this._user.Text = global::ServerRedi.Properties.Settings.Default.un;
            // 
            // _pass
            // 
            this._pass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "pw", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pass.Location = new System.Drawing.Point(64, 31);
            this._pass.Margin = new System.Windows.Forms.Padding(2);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(77, 20);
            this._pass.TabIndex = 3;
            this._pass.Text = global::ServerRedi.Properties.Settings.Default.pw;
            this._pass.UseSystemPasswordChar = true;
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(12, 105);
            this._login.Margin = new System.Windows.Forms.Padding(2);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(50, 19);
            this._login.TabIndex = 4;
            this._login.Text = "Login";
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // _msg
            // 
            this._msg.Location = new System.Drawing.Point(67, 105);
            this._msg.Margin = new System.Windows.Forms.Padding(2);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(20, 19);
            this._msg.TabIndex = 5;
            this._msg.Text = "!";
            this.toolTip1.SetToolTip(this._msg, "Messages");
            this._msg.UseVisualStyleBackColor = true;
            this._msg.Click += new System.EventHandler(this._msg_Click);
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(91, 105);
            this._report.Margin = new System.Windows.Forms.Padding(2);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(17, 19);
            this._report.TabIndex = 6;
            this.toolTip1.SetToolTip(this._report, "Submit bug or enhancement report ");
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // lblAccount
            // 
            this.lblAccount.AutoSize = true;
            this.lblAccount.Location = new System.Drawing.Point(9, 78);
            this.lblAccount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(50, 13);
            this.lblAccount.TabIndex = 7;
            this.lblAccount.Text = "Account:";
            // 
            // lblAcr
            // 
            this.lblAcr.AutoSize = true;
            this.lblAcr.Location = new System.Drawing.Point(9, 55);
            this.lblAcr.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAcr.Name = "lblAcr";
            this.lblAcr.Size = new System.Drawing.Size(51, 13);
            this.lblAcr.TabIndex = 9;
            this.lblAcr.Text = "Acronym:";
            // 
            // txtAcronym
            // 
            this.txtAcronym.Location = new System.Drawing.Point(64, 56);
            this.txtAcronym.Name = "txtAcronym";
            this.txtAcronym.Size = new System.Drawing.Size(77, 20);
            this.txtAcronym.TabIndex = 10;
            this.txtAcronym.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "acronym", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtAcronym.Text = global::ServerRedi.Properties.Settings.Default.acronym;

            // 
            // txtAccount
            // 
            this.txtAccount.Location = new System.Drawing.Point(64, 78);
            this.txtAccount.Name = "txtAccount";
            this.txtAccount.Size = new System.Drawing.Size(77, 20);
            this.txtAccount.TabIndex = 11;
            this.txtAccount.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ServerRedi.Properties.Settings.Default, "accnt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtAccount.Text = global::ServerRedi.Properties.Settings.Default.accnt;
            // 
            // RediMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 134);
            this.Controls.Add(this.txtAccount);
            this.Controls.Add(this.txtAcronym);
            this.Controls.Add(this.lblAcr);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this._report);
            this.Controls.Add(this._msg);
            this.Controls.Add(this._login);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
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
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.Label lblAcr;
        private System.Windows.Forms.TextBox txtAcronym;
        private System.Windows.Forms.TextBox txtAccount;
    }
}

