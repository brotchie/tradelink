namespace IQFeedBroker
{
    partial class IQFeedFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IQFeedFrm));
            this.btnDebug = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._login = new System.Windows.Forms.Button();
            this._report = new System.Windows.Forms.Button();
            this._pass = new System.Windows.Forms.TextBox();
            this._user = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._prod = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnDebug
            // 
            this.btnDebug.Location = new System.Drawing.Point(171, 148);
            this.btnDebug.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(29, 27);
            this.btnDebug.TabIndex = 0;
            this.btnDebug.Text = "!";
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "User:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Password:";
            // 
            // _login
            // 
            this._login.Location = new System.Drawing.Point(47, 148);
            this._login.Name = "_login";
            this._login.Size = new System.Drawing.Size(75, 27);
            this._login.TabIndex = 5;
            this._login.Text = "Login";
            this._login.UseVisualStyleBackColor = true;
            this._login.Click += new System.EventHandler(this._login_Click);
            // 
            // _report
            // 
            this._report.Image = ((System.Drawing.Image)(resources.GetObject("_report.Image")));
            this._report.Location = new System.Drawing.Point(207, 148);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(25, 27);
            this._report.TabIndex = 6;
            this._report.UseVisualStyleBackColor = true;
            this._report.Click += new System.EventHandler(this._report_Click);
            // 
            // _pass
            // 
            this._pass.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::IQFeedBroker.Properties.Settings.Default, "Password", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._pass.Location = new System.Drawing.Point(112, 87);
            this._pass.Name = "_pass";
            this._pass.PasswordChar = '*';
            this._pass.Size = new System.Drawing.Size(120, 26);
            this._pass.TabIndex = 2;
            this._pass.Text = global::IQFeedBroker.Properties.Settings.Default.Password;
            this._pass.UseSystemPasswordChar = true;
            // 
            // _user
            // 
            this._user.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::IQFeedBroker.Properties.Settings.Default, "UserID", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._user.Location = new System.Drawing.Point(112, 54);
            this._user.Name = "_user";
            this._user.Size = new System.Drawing.Size(120, 26);
            this._user.TabIndex = 1;
            this._user.Text = global::IQFeedBroker.Properties.Settings.Default.UserID;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Product:";
            // 
            // _prod
            // 
            this._prod.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::IQFeedBroker.Properties.Settings.Default, "PROGRAM_NAME", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._prod.Location = new System.Drawing.Point(112, 19);
            this._prod.Name = "_prod";
            this._prod.Size = new System.Drawing.Size(120, 26);
            this._prod.TabIndex = 8;
            this._prod.Text = global::IQFeedBroker.Properties.Settings.Default.PROGRAM_NAME;
            this._prod.TextChanged += new System.EventHandler(this._prod_TextChanged);
            // 
            // IQFeedFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 187);
            this.Controls.Add(this._prod);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._report);
            this.Controls.Add(this._login);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._pass);
            this.Controls.Add(this._user);
            this.Controls.Add(this.btnDebug);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "IQFeedFrm";
            this.Text = "IQFeed";
            this.Load += new System.EventHandler(this.IQFeedFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDebug;
        private System.Windows.Forms.TextBox _user;
        private System.Windows.Forms.TextBox _pass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _login;
        private System.Windows.Forms.Button _report;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox _prod;
    }
}