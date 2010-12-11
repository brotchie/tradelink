namespace TradeLinkTest
{
    partial class GBTradeLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GBTradeLink));
            this.LoginDetails = new System.Windows.Forms.GroupBox();
            this.cboBookServer = new System.Windows.Forms.ComboBox();
            this.cboQuoteServer = new System.Windows.Forms.ComboBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.BookServer = new System.Windows.Forms.Label();
            this.lstStatusList = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.QuoteServer = new System.Windows.Forms.Label();
            this.bt_Connect = new System.Windows.Forms.Button();
            this.txtPasword = new System.Windows.Forms.TextBox();
            this.GBLoginName = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.Label();
            this.txt_GBLoginName = new System.Windows.Forms.TextBox();
            this.LoginDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoginDetails
            // 
            this.LoginDetails.Controls.Add(this.cboBookServer);
            this.LoginDetails.Controls.Add(this.cboQuoteServer);
            this.LoginDetails.Controls.Add(this.btnHelp);
            this.LoginDetails.Controls.Add(this.BookServer);
            this.LoginDetails.Controls.Add(this.lstStatusList);
            this.LoginDetails.Controls.Add(this.btnClose);
            this.LoginDetails.Controls.Add(this.QuoteServer);
            this.LoginDetails.Controls.Add(this.bt_Connect);
            this.LoginDetails.Controls.Add(this.txtPasword);
            this.LoginDetails.Controls.Add(this.GBLoginName);
            this.LoginDetails.Controls.Add(this.Password);
            this.LoginDetails.Controls.Add(this.txt_GBLoginName);
            this.LoginDetails.Location = new System.Drawing.Point(4, 6);
            this.LoginDetails.Name = "LoginDetails";
            this.LoginDetails.Size = new System.Drawing.Size(510, 391);
            this.LoginDetails.TabIndex = 1;
            this.LoginDetails.TabStop = false;
            // 
            // cboBookServer
            // 
            this.cboBookServer.FormattingEnabled = true;
            this.cboBookServer.Location = new System.Drawing.Point(207, 107);
            this.cboBookServer.Name = "cboBookServer";
            this.cboBookServer.Size = new System.Drawing.Size(222, 21);
            this.cboBookServer.TabIndex = 16;
            // 
            // cboQuoteServer
            // 
            this.cboQuoteServer.FormattingEnabled = true;
            this.cboQuoteServer.Location = new System.Drawing.Point(207, 75);
            this.cboQuoteServer.Name = "cboQuoteServer";
            this.cboQuoteServer.Size = new System.Drawing.Size(222, 21);
            this.cboQuoteServer.TabIndex = 15;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(354, 154);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(75, 23);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "Help";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Visible = false;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // BookServer
            // 
            this.BookServer.AutoSize = true;
            this.BookServer.Location = new System.Drawing.Point(76, 111);
            this.BookServer.Name = "BookServer";
            this.BookServer.Size = new System.Drawing.Size(113, 13);
            this.BookServer.TabIndex = 14;
            this.BookServer.Text = "Book Server IP && Port:";
            // 
            // lstStatusList
            // 
            this.lstStatusList.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.lstStatusList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstStatusList.ForeColor = System.Drawing.Color.Green;
            this.lstStatusList.FormattingEnabled = true;
            this.lstStatusList.HorizontalScrollbar = true;
            this.lstStatusList.ItemHeight = 15;
            this.lstStatusList.Location = new System.Drawing.Point(7, 186);
            this.lstStatusList.Name = "lstStatusList";
            this.lstStatusList.ScrollAlwaysVisible = true;
            this.lstStatusList.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstStatusList.Size = new System.Drawing.Size(497, 199);
            this.lstStatusList.TabIndex = 11;
            // 
            // btnClose
            // 
            this.btnClose.ForeColor = System.Drawing.Color.Red;
            this.btnClose.Location = new System.Drawing.Point(160, 154);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // QuoteServer
            // 
            this.QuoteServer.AutoSize = true;
            this.QuoteServer.Location = new System.Drawing.Point(74, 79);
            this.QuoteServer.Name = "QuoteServer";
            this.QuoteServer.Size = new System.Drawing.Size(117, 13);
            this.QuoteServer.TabIndex = 12;
            this.QuoteServer.Text = "Quote Server IP && Port:";
            // 
            // bt_Connect
            // 
            this.bt_Connect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.bt_Connect.Location = new System.Drawing.Point(79, 154);
            this.bt_Connect.Name = "bt_Connect";
            this.bt_Connect.Size = new System.Drawing.Size(75, 23);
            this.bt_Connect.TabIndex = 9;
            this.bt_Connect.Text = "&Connect";
            this.bt_Connect.UseVisualStyleBackColor = true;
            this.bt_Connect.Click += new System.EventHandler(this.bt_Connect_Click);
            // 
            // txtPasword
            // 
            this.txtPasword.Location = new System.Drawing.Point(207, 43);
            this.txtPasword.Name = "txtPasword";
            this.txtPasword.PasswordChar = '*';
            this.txtPasword.Size = new System.Drawing.Size(222, 20);
            this.txtPasword.TabIndex = 11;
            this.txtPasword.UseSystemPasswordChar = true;
            this.txtPasword.WordWrap = false;
            // 
            // GBLoginName
            // 
            this.GBLoginName.AutoSize = true;
            this.GBLoginName.Location = new System.Drawing.Point(73, 15);
            this.GBLoginName.Name = "GBLoginName";
            this.GBLoginName.Size = new System.Drawing.Size(85, 13);
            this.GBLoginName.TabIndex = 8;
            this.GBLoginName.Text = "GB Login Name:";
            // 
            // Password
            // 
            this.Password.AutoSize = true;
            this.Password.Location = new System.Drawing.Point(74, 47);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(56, 13);
            this.Password.TabIndex = 10;
            this.Password.Text = "Password:";
            // 
            // txt_GBLoginName
            // 
            this.txt_GBLoginName.Location = new System.Drawing.Point(207, 11);
            this.txt_GBLoginName.Name = "txt_GBLoginName";
            this.txt_GBLoginName.Size = new System.Drawing.Size(222, 20);
            this.txt_GBLoginName.TabIndex = 9;
            // 
            // GBTradeLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(526, 409);
            this.Controls.Add(this.LoginDetails);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GBTradeLink";
            this.Text = "Graybox TL Server";
            this.LoginDetails.ResumeLayout(false);
            this.LoginDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox LoginDetails;
        private System.Windows.Forms.Button bt_Connect;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListBox lstStatusList;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Label BookServer;
        private System.Windows.Forms.Label QuoteServer;
        private System.Windows.Forms.TextBox txtPasword;
        private System.Windows.Forms.Label GBLoginName;
        private System.Windows.Forms.Label Password;
        private System.Windows.Forms.TextBox txt_GBLoginName;
        private System.Windows.Forms.ComboBox cboBookServer;
        private System.Windows.Forms.ComboBox cboQuoteServer;
    }
}

