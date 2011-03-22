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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DASServerMain));
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(66, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "User_ID";
            // 
            // textBoxID
            // 
            this.textBoxID.Location = new System.Drawing.Point(159, 88);
            this.textBoxID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.Size = new System.Drawing.Size(141, 26);
            this.textBoxID.TabIndex = 1;
            this.textBoxID.Text = "donz2";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(159, 154);
            this.textBoxPwd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.PasswordChar = '*';
            this.textBoxPwd.Size = new System.Drawing.Size(141, 26);
            this.textBoxPwd.TabIndex = 3;
            this.textBoxPwd.Text = "das123";
            this.textBoxPwd.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 154);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password:";
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(199, 210);
            this.buttonLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(101, 35);
            this.buttonLogin.TabIndex = 4;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.AutoSize = true;
            this.labelMessage.ForeColor = System.Drawing.Color.Tomato;
            this.labelMessage.Location = new System.Drawing.Point(66, 34);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(0, 20);
            this.labelMessage.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(199, 252);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(53, 29);
            this.button1.TabIndex = 6;
            this.button1.Text = "!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Image = global::ServerDAS.Properties.Resources.bug;
            this.button2.Location = new System.Drawing.Point(258, 252);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(42, 29);
            this.button2.TabIndex = 7;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DASServerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 335);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelMessage);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.textBoxPwd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

