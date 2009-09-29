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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
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
            // ASPOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 107);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ASPOptions";
            this.Text = "Options";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.CheckBox archivetickbox;
        public System.Windows.Forms.TextBox _account;
    }
}