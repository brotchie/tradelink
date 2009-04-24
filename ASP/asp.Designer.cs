namespace ASP
{
    partial class ASP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ASP));
            this.LoadDLL = new System.Windows.Forms.Button();
            this._availresponses = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Trade = new System.Windows.Forms.Button();
            this._resnames = new System.Windows.Forms.ListBox();
            this._symstraded = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this._msg = new System.Windows.Forms.ListBox();
            this.debugon = new System.Windows.Forms.CheckBox();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this._newrespbox = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._togglemsgs = new System.Windows.Forms.Button();
            this._account = new System.Windows.Forms.TextBox();
            this._twithelp = new System.Windows.Forms.Button();
            this._skins = new System.Windows.Forms.ComboBox();
            this._remskin = new System.Windows.Forms.Button();
            this._saveskins = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this._newrespbox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LoadDLL
            // 
            this.LoadDLL.Location = new System.Drawing.Point(19, 31);
            this.LoadDLL.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LoadDLL.Name = "LoadDLL";
            this.LoadDLL.Size = new System.Drawing.Size(148, 29);
            this.LoadDLL.TabIndex = 0;
            this.LoadDLL.Text = "Response Library";
            this.toolTip1.SetToolTip(this.LoadDLL, "Change current response library");
            this.LoadDLL.UseVisualStyleBackColor = true;
            this.LoadDLL.Click += new System.EventHandler(this.LoadDLL_Click);
            // 
            // _availresponses
            // 
            this._availresponses.FormattingEnabled = true;
            this._availresponses.Location = new System.Drawing.Point(136, 31);
            this._availresponses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._availresponses.Name = "_availresponses";
            this._availresponses.Size = new System.Drawing.Size(287, 28);
            this._availresponses.TabIndex = 1;
            this._availresponses.Text = "Select a response";
            this.toolTip1.SetToolTip(this._availresponses, "List of responses available in current response library");
            this._availresponses.SelectedIndexChanged += new System.EventHandler(this.Boxes_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 34);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "New Response:";
            // 
            // Trade
            // 
            this.Trade.Location = new System.Drawing.Point(331, 64);
            this.Trade.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Trade.Name = "Trade";
            this.Trade.Size = new System.Drawing.Size(95, 26);
            this.Trade.TabIndex = 3;
            this.Trade.Text = "Trade";
            this.Trade.UseVisualStyleBackColor = true;
            this.Trade.Click += new System.EventHandler(this.Trade_Click);
            // 
            // _resnames
            // 
            this._resnames.FormattingEnabled = true;
            this._resnames.HorizontalScrollbar = true;
            this._resnames.ItemHeight = 20;
            this._resnames.Location = new System.Drawing.Point(14, 134);
            this._resnames.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._resnames.Name = "_resnames";
            this._resnames.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._resnames.Size = new System.Drawing.Size(434, 124);
            this._resnames.TabIndex = 4;
            this._resnames.TabStop = false;
            this.toolTip1.SetToolTip(this._resnames, "Active Responses");
            // 
            // _symstraded
            // 
            this._symstraded.Location = new System.Drawing.Point(136, 64);
            this._symstraded.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._symstraded.Name = "_symstraded";
            this._symstraded.Size = new System.Drawing.Size(187, 26);
            this._symstraded.TabIndex = 5;
            this._symstraded.KeyUp += new System.Windows.Forms.KeyEventHandler(this.stock_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 70);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Trade on Symbol";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 393);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(468, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(0, 17);
            // 
            // _msg
            // 
            this._msg.Dock = System.Windows.Forms.DockStyle.Right;
            this._msg.FormattingEnabled = true;
            this._msg.HorizontalScrollbar = true;
            this._msg.ItemHeight = 20;
            this._msg.Location = new System.Drawing.Point(112, 0);
            this._msg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._msg.Name = "_msg";
            this._msg.Size = new System.Drawing.Size(356, 384);
            this._msg.TabIndex = 12;
            this._msg.Visible = false;
            // 
            // debugon
            // 
            this.debugon.Appearance = System.Windows.Forms.Appearance.Button;
            this.debugon.AutoSize = true;
            this.debugon.Checked = true;
            this.debugon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.debugon.Location = new System.Drawing.Point(199, 68);
            this.debugon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugon.Name = "debugon";
            this.debugon.Size = new System.Drawing.Size(67, 30);
            this.debugon.TabIndex = 13;
            this.debugon.Text = "Debug";
            this.toolTip1.SetToolTip(this.debugon, "display debug messages from responses");
            this.debugon.UseVisualStyleBackColor = true;
            // 
            // archivetickbox
            // 
            this.archivetickbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.archivetickbox.AutoSize = true;
            this.archivetickbox.Checked = true;
            this.archivetickbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.archivetickbox.Location = new System.Drawing.Point(97, 68);
            this.archivetickbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.archivetickbox.Name = "archivetickbox";
            this.archivetickbox.Size = new System.Drawing.Size(95, 30);
            this.archivetickbox.TabIndex = 14;
            this.archivetickbox.Text = "Save Ticks";
            this.toolTip1.SetToolTip(this.archivetickbox, "Save ticks for later analysis/replay in TradeLink");
            this.archivetickbox.UseVisualStyleBackColor = true;
            // 
            // _newrespbox
            // 
            this._newrespbox.Controls.Add(this.label1);
            this._newrespbox.Controls.Add(this._availresponses);
            this._newrespbox.Controls.Add(this.label2);
            this._newrespbox.Controls.Add(this._symstraded);
            this._newrespbox.Controls.Add(this.Trade);
            this._newrespbox.Location = new System.Drawing.Point(14, 12);
            this._newrespbox.Name = "_newrespbox";
            this._newrespbox.Size = new System.Drawing.Size(435, 114);
            this._newrespbox.TabIndex = 16;
            this._newrespbox.TabStop = false;
            this._newrespbox.Text = "Trade a Respose";
            // 
            // _togglemsgs
            // 
            this._togglemsgs.Location = new System.Drawing.Point(19, 69);
            this._togglemsgs.Name = "_togglemsgs";
            this._togglemsgs.Size = new System.Drawing.Size(33, 29);
            this._togglemsgs.TabIndex = 18;
            this._togglemsgs.Text = "~";
            this.toolTip1.SetToolTip(this._togglemsgs, "toggle display of debug window");
            this._togglemsgs.UseVisualStyleBackColor = true;
            this._togglemsgs.Click += new System.EventHandler(this._togglemsgs_Click);
            // 
            // _account
            // 
            this._account.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASP.Properties.Settings.Default, "accountname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._account.Location = new System.Drawing.Point(344, 70);
            this._account.Name = "_account";
            this._account.Size = new System.Drawing.Size(82, 26);
            this._account.TabIndex = 15;
            this._account.Text = global::ASP.Properties.Settings.Default.accountname;
            this.toolTip1.SetToolTip(this._account, "specify trading account name/id where orders are sent");
            // 
            // _twithelp
            // 
            this._twithelp.Location = new System.Drawing.Point(58, 68);
            this._twithelp.Name = "_twithelp";
            this._twithelp.Size = new System.Drawing.Size(33, 30);
            this._twithelp.TabIndex = 19;
            this._twithelp.Text = "?";
            this.toolTip1.SetToolTip(this._twithelp, "twit for help");
            this._twithelp.UseVisualStyleBackColor = true;
            this._twithelp.Click += new System.EventHandler(this._twithelp_Click);
            // 
            // _skins
            // 
            this._skins.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._skins.FormattingEnabled = true;
            this._skins.Location = new System.Drawing.Point(174, 32);
            this._skins.Name = "_skins";
            this._skins.Size = new System.Drawing.Size(180, 28);
            this._skins.TabIndex = 20;
            this.toolTip1.SetToolTip(this._skins, "skins found");
            this._skins.SelectedIndexChanged += new System.EventHandler(this._skins_SelectedIndexChanged);
            // 
            // _remskin
            // 
            this._remskin.Location = new System.Drawing.Point(360, 33);
            this._remskin.Name = "_remskin";
            this._remskin.Size = new System.Drawing.Size(29, 28);
            this._remskin.TabIndex = 21;
            this._remskin.Text = "-";
            this.toolTip1.SetToolTip(this._remskin, "remove selected skin");
            this._remskin.UseVisualStyleBackColor = true;
            this._remskin.Click += new System.EventHandler(this._remskin_Click);
            // 
            // _saveskins
            // 
            this._saveskins.Location = new System.Drawing.Point(395, 32);
            this._saveskins.Name = "_saveskins";
            this._saveskins.Size = new System.Drawing.Size(31, 29);
            this._saveskins.TabIndex = 22;
            this._saveskins.Text = "S";
            this.toolTip1.SetToolTip(this._saveskins, "save all loaded skins");
            this._saveskins.UseVisualStyleBackColor = true;
            this._saveskins.Click += new System.EventHandler(this._saveskins_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._saveskins);
            this.groupBox1.Controls.Add(this._remskin);
            this.groupBox1.Controls.Add(this._skins);
            this.groupBox1.Controls.Add(this._twithelp);
            this.groupBox1.Controls.Add(this.LoadDLL);
            this.groupBox1.Controls.Add(this._togglemsgs);
            this.groupBox1.Controls.Add(this.debugon);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.archivetickbox);
            this.groupBox1.Controls.Add(this._account);
            this.groupBox1.Location = new System.Drawing.Point(14, 274);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(435, 109);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ASP Options";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(273, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Account:";
            // 
            // ASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(468, 415);
            this.Controls.Add(this._msg);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._newrespbox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this._resnames);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ASP";
            this.Text = "ASP";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this._newrespbox.ResumeLayout(false);
            this._newrespbox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoadDLL;
        private System.Windows.Forms.ComboBox _availresponses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Trade;
        private System.Windows.Forms.ListBox _resnames;
        private System.Windows.Forms.TextBox _symstraded;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListBox _msg;
        private System.Windows.Forms.CheckBox debugon;
        private System.Windows.Forms.CheckBox archivetickbox;
        private System.Windows.Forms.TextBox _account;
        private System.Windows.Forms.GroupBox _newrespbox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _togglemsgs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _twithelp;
        private System.Windows.Forms.ComboBox _skins;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button _remskin;
        private System.Windows.Forms.Button _saveskins;
    }
}

