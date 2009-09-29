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
            this.label4 = new System.Windows.Forms.Label();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this._account = new System.Windows.Forms.TextBox();
            this._saveskins = new System.Windows.Forms.Button();
            this._remskin = new System.Windows.Forms.Button();
            this._skins = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.archivetickbox);
            this.groupBox1.Controls.Add(this._account);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 76);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
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
            // _saveskins
            // 
            this._saveskins.Location = new System.Drawing.Point(236, 23);
            this._saveskins.Name = "_saveskins";
            this._saveskins.Size = new System.Drawing.Size(31, 29);
            this._saveskins.TabIndex = 22;
            this._saveskins.Text = "S";
            this.toolTip1.SetToolTip(this._saveskins, "save current settings from running skins");
            this._saveskins.UseVisualStyleBackColor = true;
            // 
            // _remskin
            // 
            this._remskin.Location = new System.Drawing.Point(201, 24);
            this._remskin.Name = "_remskin";
            this._remskin.Size = new System.Drawing.Size(29, 28);
            this._remskin.TabIndex = 21;
            this._remskin.Text = "-";
            this.toolTip1.SetToolTip(this._remskin, "delete current skin");
            this._remskin.UseVisualStyleBackColor = true;
            // 
            // _skins
            // 
            this._skins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._skins.FormattingEnabled = true;
            this._skins.Location = new System.Drawing.Point(15, 25);
            this._skins.Name = "_skins";
            this._skins.Size = new System.Drawing.Size(180, 28);
            this._skins.TabIndex = 20;
            this.toolTip1.SetToolTip(this._skins, "Skins you have saved");
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this._saveskins);
            this.groupBox2.Controls.Add(this._skins);
            this.groupBox2.Controls.Add(this._remskin);
            this.groupBox2.Location = new System.Drawing.Point(12, 94);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 72);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Preconfiged Response Skins";
            // 
            // ASPOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 192);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ASPOptions";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.Button _saveskins;
        public System.Windows.Forms.Button _remskin;
        public System.Windows.Forms.ComboBox _skins;
        public System.Windows.Forms.CheckBox archivetickbox;
        public System.Windows.Forms.TextBox _account;
    }
}