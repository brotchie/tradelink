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
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._execsel = new System.Windows.Forms.ComboBox();
            this._datasel = new System.Windows.Forms.ComboBox();
            this._providerfallback = new System.Windows.Forms.CheckBox();
            this._brokertimeout = new System.Windows.Forms.NumericUpDown();
            this._virtids = new System.Windows.Forms.CheckBox();
            this._account = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._brokertimeout)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._brokertimeout);
            this.groupBox1.Controls.Add(this._virtids);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.archivetickbox);
            this.groupBox1.Controls.Add(this._account);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 118);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 20);
            this.label1.TabIndex = 20;
            this.label1.Text = "Broker Reconnect Timeout (sec):";
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
            this.archivetickbox.Location = new System.Drawing.Point(162, 26);
            this.archivetickbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.archivetickbox.Name = "archivetickbox";
            this.archivetickbox.Size = new System.Drawing.Size(95, 30);
            this.archivetickbox.TabIndex = 14;
            this.archivetickbox.Text = "Save Ticks";
            this.toolTip1.SetToolTip(this.archivetickbox, "archive ticks automatically for playback/study");
            this.archivetickbox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._providerfallback);
            this.groupBox2.Controls.Add(this._datasel);
            this.groupBox2.Controls.Add(this._execsel);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(13, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(380, 153);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Providers";
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 1;
            this.label3.Text = "Data Feed:";
            // 
            // _execsel
            // 
            this._execsel.FormattingEnabled = true;
            this._execsel.Location = new System.Drawing.Point(145, 23);
            this._execsel.Name = "_execsel";
            this._execsel.Size = new System.Drawing.Size(208, 28);
            this._execsel.TabIndex = 2;
            // 
            // _datasel
            // 
            this._datasel.FormattingEnabled = true;
            this._datasel.Location = new System.Drawing.Point(145, 63);
            this._datasel.Name = "_datasel";
            this._datasel.Size = new System.Drawing.Size(208, 28);
            this._datasel.TabIndex = 3;
            // 
            // _providerfallback
            // 
            this._providerfallback.AutoSize = true;
            this._providerfallback.Checked = global::ASP.Properties.Settings.Default.feedfallback;
            this._providerfallback.CheckState = System.Windows.Forms.CheckState.Checked;
            this._providerfallback.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "feedfallback", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._providerfallback.Location = new System.Drawing.Point(18, 111);
            this._providerfallback.Name = "_providerfallback";
            this._providerfallback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._providerfallback.Size = new System.Drawing.Size(137, 24);
            this._providerfallback.TabIndex = 4;
            this._providerfallback.Text = "Fallback to any";
            this.toolTip1.SetToolTip(this._providerfallback, "Fallback to any provider if preferred not found.");
            this._providerfallback.UseVisualStyleBackColor = true;
            // 
            // _brokertimeout
            // 
            this._brokertimeout.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::ASP.Properties.Settings.Default, "brokertimeoutsec", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._brokertimeout.Location = new System.Drawing.Point(264, 73);
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
            // _virtids
            // 
            this._virtids.Appearance = System.Windows.Forms.Appearance.Button;
            this._virtids.AutoSize = true;
            this._virtids.Checked = global::ASP.Properties.Settings.Default.usevirtualids;
            this._virtids.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::ASP.Properties.Settings.Default, "usevirtualids", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._virtids.Location = new System.Drawing.Point(264, 26);
            this._virtids.Name = "_virtids";
            this._virtids.Size = new System.Drawing.Size(90, 30);
            this._virtids.TabIndex = 18;
            this._virtids.Text = "Virtual Ids";
            this._virtids.UseVisualStyleBackColor = true;
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
            this.ClientSize = new System.Drawing.Size(415, 305);
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
        public System.Windows.Forms.CheckBox _virtids;
        private System.Windows.Forms.Label label1;
        internal System.Windows.Forms.NumericUpDown _brokertimeout;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox _providerfallback;
        public System.Windows.Forms.ComboBox _datasel;
        public System.Windows.Forms.ComboBox _execsel;
    }
}