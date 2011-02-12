namespace ASP
{
    partial class ASPOptions
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._datasel = new System.Windows.Forms.ComboBox();
            this._execsel = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._brokertimeout = new System.Windows.Forms.NumericUpDown();
            this._providerfallback = new System.Windows.Forms.CheckBox();
            this._usemkttime = new System.Windows.Forms.CheckBox();
            this._saveinds = new System.Windows.Forms.CheckBox();
            this._capconprompt = new System.Windows.Forms.CheckBox();
            this._portal = new System.Windows.Forms.TextBox();
            this._dest = new System.Windows.Forms.TextBox();
            this._account = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._brokertimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._usemkttime);
            this.groupBox1.Controls.Add(this._saveinds);
            this.groupBox1.Controls.Add(this._capconprompt);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this._portal);
            this.groupBox1.Controls.Add(this._dest);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.archivetickbox);
            this.groupBox1.Controls.Add(this._account);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(442, 154);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(162, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 20);
            this.label6.TabIndex = 22;
            this.label6.Text = "Portal Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 20);
            this.label5.TabIndex = 19;
            this.label5.Text = "Dest:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Account:";
            // 
            // archivetickbox
            // 
            this.archivetickbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.archivetickbox.AutoSize = true;
            this.archivetickbox.Checked = true;
            this.archivetickbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.archivetickbox.Location = new System.Drawing.Point(19, 105);
            this.archivetickbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.archivetickbox.Name = "archivetickbox";
            this.archivetickbox.Size = new System.Drawing.Size(95, 30);
            this.archivetickbox.TabIndex = 14;
            this.archivetickbox.Text = "Save Ticks";
            this.toolTip1.SetToolTip(this.archivetickbox, "archive ticks automatically for playback/study");
            this.archivetickbox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "FeedTimeout (sec):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._datasel);
            this.groupBox2.Controls.Add(this._brokertimeout);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this._providerfallback);
            this.groupBox2.Controls.Add(this._execsel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(13, 172);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(441, 180);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Providers";
            // 
            // _datasel
            // 
            this._datasel.FormattingEnabled = true;
            this._datasel.Location = new System.Drawing.Point(161, 63);
            this._datasel.Name = "_datasel";
            this._datasel.Size = new System.Drawing.Size(192, 28);
            this._datasel.TabIndex = 3;
            // 
            // _execsel
            // 
            this._execsel.FormattingEnabled = true;
            this._execsel.Location = new System.Drawing.Point(161, 23);
            this._execsel.Name = "_execsel";
            this._execsel.Size = new System.Drawing.Size(192, 28);
            this._execsel.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Feed:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Execution:";
            // 
            // _brokertimeout
            // 
            this._brokertimeout.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::ASP.Properties.Settings.Default, "brokertimeoutsec", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._brokertimeout.Location = new System.Drawing.Point(263, 133);
            this._brokertimeout.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this._brokertimeout.Name = "_brokertimeout";
            this._brokertimeout.Size = new System.Drawing.Size(90, 26);
            this._brokertimeout.TabIndex = 19;
            this._brokertimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this._brokertimeout, "0 disables timeout");
            this._brokertimeout.Value = global::ASP.Properties.Settings.Default.brokertimeoutsec;
            this._brokertimeout.ValueChanged += new System.EventHandler(this._brokertimeout_ValueChanged);
            // 
            // _providerfallback
            // 
            this._providerfallback.AutoSize = true;
            this._providerfallback.Checked = global::ASP.Properties.Settings.Default.feedfallback;
            this._providerfallback.CheckState = System.Windows.Forms.CheckState.Checked;
            this._providerfallback.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "feedfallback", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._providerfallback.Location = new System.Drawing.Point(14, 103);
            this._providerfallback.Name = "_providerfallback";
            this._providerfallback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._providerfallback.Size = new System.Drawing.Size(134, 24);
            this._providerfallback.TabIndex = 4;
            this._providerfallback.Text = "Fallback to any";
            this.toolTip1.SetToolTip(this._providerfallback, "Fallback to any provider if preferred not found.");
            this._providerfallback.UseVisualStyleBackColor = true;
            // 
            // _usemkttime
            // 
            this._usemkttime.Appearance = System.Windows.Forms.Appearance.Button;
            this._usemkttime.AutoSize = true;
            this._usemkttime.Checked = global::ASP.Properties.Settings.Default.UseMarketTimeStamps;
            this._usemkttime.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "UseMarketTimeStamps", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._usemkttime.Location = new System.Drawing.Point(255, 105);
            this._usemkttime.Name = "_usemkttime";
            this._usemkttime.Size = new System.Drawing.Size(157, 30);
            this._usemkttime.TabIndex = 25;
            this._usemkttime.Text = "MarketTimeStamps";
            this._usemkttime.UseVisualStyleBackColor = true;
            this._usemkttime.CheckedChanged += new System.EventHandler(this._usemkttime_CheckedChanged);
            // 
            // _saveinds
            // 
            this._saveinds.Appearance = System.Windows.Forms.Appearance.Button;
            this._saveinds.AutoSize = true;
            this._saveinds.Checked = global::ASP.Properties.Settings.Default.saveindicators;
            this._saveinds.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "saveindicators", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._saveinds.Location = new System.Drawing.Point(120, 105);
            this._saveinds.Name = "_saveinds";
            this._saveinds.Size = new System.Drawing.Size(129, 30);
            this._saveinds.TabIndex = 24;
            this._saveinds.Text = "Save Indicators";
            this._saveinds.UseVisualStyleBackColor = true;
            // 
            // _capconprompt
            // 
            this._capconprompt.AutoSize = true;
            this._capconprompt.Checked = global::ASP.Properties.Settings.Default.capitalconnections;
            this._capconprompt.CheckState = System.Windows.Forms.CheckState.Checked;
            this._capconprompt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "capitalconnections", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._capconprompt.Location = new System.Drawing.Point(166, 68);
            this._capconprompt.Name = "_capconprompt";
            this._capconprompt.Size = new System.Drawing.Size(205, 24);
            this._capconprompt.TabIndex = 23;
            this._capconprompt.Text = "Allow capital connections";
            this._capconprompt.UseVisualStyleBackColor = true;
            // 
            // _portal
            // 
            this._portal.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASP.Properties.Settings.Default, "portal", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._portal.Location = new System.Drawing.Point(264, 31);
            this._portal.Name = "_portal";
            this._portal.Size = new System.Drawing.Size(90, 26);
            this._portal.TabIndex = 21;
            this._portal.Text = global::ASP.Properties.Settings.Default.portal;
            this._portal.TextChanged += new System.EventHandler(this._portal_TextChanged);
            // 
            // _dest
            // 
            this._dest.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASP.Properties.Settings.Default, "defaultdest", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._dest.Location = new System.Drawing.Point(80, 65);
            this._dest.Name = "_dest";
            this._dest.Size = new System.Drawing.Size(76, 26);
            this._dest.TabIndex = 20;
            this._dest.Text = global::ASP.Properties.Settings.Default.defaultdest;
            this.toolTip1.SetToolTip(this._dest, "Default exchange/destination");
            // 
            // _account
            // 
            this._account.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASP.Properties.Settings.Default, "accountname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._account.Location = new System.Drawing.Point(80, 28);
            this._account.Name = "_account";
            this._account.Size = new System.Drawing.Size(76, 26);
            this._account.TabIndex = 15;
            this._account.Text = global::ASP.Properties.Settings.Default.accountname;
            this.toolTip1.SetToolTip(this._account, "destination account orders and executions are sent for");
            // 
            // ASPOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 374);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ASPOptions";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._brokertimeout)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.CheckBox archivetickbox;
        public System.Windows.Forms.TextBox _account;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.NumericUpDown _brokertimeout;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox _providerfallback;
        public System.Windows.Forms.ComboBox _datasel;
        public System.Windows.Forms.ComboBox _execsel;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox _dest;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox _portal;
        public System.Windows.Forms.CheckBox _capconprompt;
        public System.Windows.Forms.CheckBox _saveinds;
        public System.Windows.Forms.CheckBox _usemkttime;
    }
}