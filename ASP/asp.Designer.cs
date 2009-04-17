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
            this.Responses = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Trade = new System.Windows.Forms.Button();
            this.boxcriteria = new System.Windows.Forms.ListBox();
            this.stock = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.debugon = new System.Windows.Forms.CheckBox();
            this.archivetickbox = new System.Windows.Forms.CheckBox();
            this._newrespbox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._togglemsgs = new System.Windows.Forms.Button();
            this._account = new System.Windows.Forms.TextBox();
            this._twithelp = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            // Responses
            // 
            this.Responses.FormattingEnabled = true;
            this.Responses.Location = new System.Drawing.Point(136, 31);
            this.Responses.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Responses.Name = "Responses";
            this.Responses.Size = new System.Drawing.Size(287, 28);
            this.Responses.TabIndex = 1;
            this.Responses.Text = "Select a response";
            this.toolTip1.SetToolTip(this.Responses, "List of responses available in current response library");
            this.Responses.SelectedIndexChanged += new System.EventHandler(this.Boxes_SelectedIndexChanged);
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
            // boxcriteria
            // 
            this.boxcriteria.FormattingEnabled = true;
            this.boxcriteria.HorizontalScrollbar = true;
            this.boxcriteria.ItemHeight = 20;
            this.boxcriteria.Location = new System.Drawing.Point(14, 134);
            this.boxcriteria.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.boxcriteria.Name = "boxcriteria";
            this.boxcriteria.Size = new System.Drawing.Size(434, 124);
            this.boxcriteria.TabIndex = 4;
            this.boxcriteria.TabStop = false;
            this.toolTip1.SetToolTip(this.boxcriteria, "Active Responses");
            this.boxcriteria.SelectedIndexChanged += new System.EventHandler(this.boxcriteria_SelectedIndexChanged);
            // 
            // stock
            // 
            this.stock.Location = new System.Drawing.Point(136, 64);
            this.stock.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stock.Name = "stock";
            this.stock.Size = new System.Drawing.Size(187, 26);
            this.stock.TabIndex = 5;
            this.stock.KeyUp += new System.Windows.Forms.KeyEventHandler(this.stock_KeyUp);
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
            this.statusStrip1.Size = new System.Drawing.Size(480, 22);
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
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(162, 0);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(318, 384);
            this.listBox1.TabIndex = 12;
            this.listBox1.Visible = false;
            // 
            // debugon
            // 
            this.debugon.Appearance = System.Windows.Forms.Appearance.Button;
            this.debugon.AutoSize = true;
            this.debugon.Checked = true;
            this.debugon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.debugon.Location = new System.Drawing.Point(249, 71);
            this.debugon.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugon.Name = "debugon";
            this.debugon.Size = new System.Drawing.Size(97, 30);
            this.debugon.TabIndex = 13;
            this.debugon.Text = "Debugging";
            this.toolTip1.SetToolTip(this.debugon, "display debug messages from responses");
            this.debugon.UseVisualStyleBackColor = true;
            // 
            // archivetickbox
            // 
            this.archivetickbox.Appearance = System.Windows.Forms.Appearance.Button;
            this.archivetickbox.AutoSize = true;
            this.archivetickbox.Checked = true;
            this.archivetickbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.archivetickbox.Location = new System.Drawing.Point(147, 71);
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
            this._newrespbox.Controls.Add(this.Responses);
            this._newrespbox.Controls.Add(this.label2);
            this._newrespbox.Controls.Add(this.stock);
            this._newrespbox.Controls.Add(this.Trade);
            this._newrespbox.Location = new System.Drawing.Point(14, 12);
            this._newrespbox.Name = "_newrespbox";
            this._newrespbox.Size = new System.Drawing.Size(435, 114);
            this._newrespbox.TabIndex = 16;
            this._newrespbox.TabStop = false;
            this._newrespbox.Text = "Trade a Respose";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(266, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Account:";
            // 
            // _togglemsgs
            // 
            this._togglemsgs.Location = new System.Drawing.Point(19, 71);
            this._togglemsgs.Name = "_togglemsgs";
            this._togglemsgs.Size = new System.Drawing.Size(122, 30);
            this._togglemsgs.TabIndex = 18;
            this._togglemsgs.Text = "Show Debugs";
            this.toolTip1.SetToolTip(this._togglemsgs, "toggle display of debug window");
            this._togglemsgs.UseVisualStyleBackColor = true;
            this._togglemsgs.Click += new System.EventHandler(this._togglemsgs_Click);
            // 
            // _account
            // 
            this._account.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::ASP.Properties.Settings.Default, "accountname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._account.Location = new System.Drawing.Point(344, 31);
            this._account.Name = "_account";
            this._account.Size = new System.Drawing.Size(100, 26);
            this._account.TabIndex = 15;
            this._account.Text = global::ASP.Properties.Settings.Default.accountname;
            this.toolTip1.SetToolTip(this._account, "specify trading account name/id where orders are sent");
            // 
            // _twithelp
            // 
            this._twithelp.Location = new System.Drawing.Point(353, 71);
            this._twithelp.Name = "_twithelp";
            this._twithelp.Size = new System.Drawing.Size(33, 30);
            this._twithelp.TabIndex = 19;
            this._twithelp.Text = "?";
            this.toolTip1.SetToolTip(this._twithelp, "twit for help");
            this._twithelp.UseVisualStyleBackColor = true;
            this._twithelp.Click += new System.EventHandler(this._twithelp_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._twithelp);
            this.groupBox1.Controls.Add(this.LoadDLL);
            this.groupBox1.Controls.Add(this._togglemsgs);
            this.groupBox1.Controls.Add(this.debugon);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.archivetickbox);
            this.groupBox1.Controls.Add(this._account);
            this.groupBox1.Location = new System.Drawing.Point(14, 274);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(444, 109);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ASP Options";
            // 
            // ASP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 415);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._newrespbox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.boxcriteria);
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
        private System.Windows.Forms.ComboBox Responses;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button Trade;
        private System.Windows.Forms.ListBox boxcriteria;
        private System.Windows.Forms.TextBox stock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox debugon;
        private System.Windows.Forms.CheckBox archivetickbox;
        private System.Windows.Forms.TextBox _account;
        private System.Windows.Forms.GroupBox _newrespbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _togglemsgs;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button _twithelp;
    }
}

