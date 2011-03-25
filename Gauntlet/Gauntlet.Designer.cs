namespace WinGauntlet
{
    partial class Gauntlet
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gauntlet));
            this.button1 = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.studypage = new System.Windows.Forms.TabPage();
            this._viewresults = new System.Windows.Forms.Button();
            this._twithelp = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lastmessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.queuebut = new System.Windows.Forms.Button();
            this.reslist = new System.Windows.Forms.ListBox();
            this.tickFileFilterControl1 = new TradeLink.AppKit.TickFileFilterControl();
            this.optionpage = new System.Windows.Forms.TabPage();
            this._docapcon = new System.Windows.Forms.Button();
            this._capitalprompt = new System.Windows.Forms.CheckBox();
            this._usebidask = new System.Windows.Forms.CheckBox();
            this._indicatcsv = new System.Windows.Forms.CheckBox();
            this._debugfile = new System.Windows.Forms.CheckBox();
            this._unique = new System.Windows.Forms.CheckBox();
            this.ordersincsv = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.messagewrite = new System.Windows.Forms.CheckBox();
            this.clearmessages = new System.Windows.Forms.CheckBox();
            this.saveonexit = new System.Windows.Forms.CheckBox();
            this.savesettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this._debugs = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.messagepage = new System.Windows.Forms.TabPage();
            this.messages = new System.Windows.Forms.RichTextBox();
            this._resulttab = new System.Windows.Forms.TabPage();
            this.tradeResults1 = new TradeLink.AppKit.TradeResults();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._portfoliosim = new System.Windows.Forms.CheckBox();
            this.tabs.SuspendLayout();
            this.studypage.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.optionpage.SuspendLayout();
            this.messagepage.SuspendLayout();
            this._resulttab.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(306, 101);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(32, 28);
            this.button1.TabIndex = 0;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.studypage);
            this.tabs.Controls.Add(this.optionpage);
            this.tabs.Controls.Add(this.messagepage);
            this.tabs.Controls.Add(this._resulttab);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(629, 542);
            this.tabs.TabIndex = 1;
            // 
            // studypage
            // 
            this.studypage.Controls.Add(this._viewresults);
            this.studypage.Controls.Add(this._twithelp);
            this.studypage.Controls.Add(this.statusStrip1);
            this.studypage.Controls.Add(this.label7);
            this.studypage.Controls.Add(this.queuebut);
            this.studypage.Controls.Add(this.reslist);
            this.studypage.Controls.Add(this.tickFileFilterControl1);
            this.studypage.Location = new System.Drawing.Point(4, 29);
            this.studypage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Name = "studypage";
            this.studypage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Size = new System.Drawing.Size(621, 509);
            this.studypage.TabIndex = 0;
            this.studypage.Text = "Studies";
            this.studypage.UseVisualStyleBackColor = true;
            // 
            // _viewresults
            // 
            this._viewresults.Location = new System.Drawing.Point(421, 421);
            this._viewresults.Name = "_viewresults";
            this._viewresults.Size = new System.Drawing.Size(191, 31);
            this._viewresults.TabIndex = 20;
            this._viewresults.Text = "Raw Results Folder";
            this.toolTip1.SetToolTip(this._viewresults, "View Results Folder");
            this._viewresults.UseVisualStyleBackColor = true;
            this._viewresults.Click += new System.EventHandler(this._viewresults_Click);
            // 
            // _twithelp
            // 
            this._twithelp.Image = ((System.Drawing.Image)(resources.GetObject("_twithelp.Image")));
            this._twithelp.Location = new System.Drawing.Point(9, 421);
            this._twithelp.Name = "_twithelp";
            this._twithelp.Size = new System.Drawing.Size(31, 31);
            this._twithelp.TabIndex = 19;
            this.toolTip1.SetToolTip(this._twithelp, "open bug report");
            this._twithelp.UseVisualStyleBackColor = true;
            this._twithelp.Click += new System.EventHandler(this._twithelp_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar1,
            this.lastmessage});
            this.statusStrip1.Location = new System.Drawing.Point(4, 468);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(613, 36);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Enabled = false;
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(150, 30);
            // 
            // lastmessage
            // 
            this.lastmessage.Name = "lastmessage";
            this.lastmessage.Size = new System.Drawing.Size(129, 31);
            this.lastmessage.Text = "No active runs.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(102, 302);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "Responses";
            // 
            // queuebut
            // 
            this.queuebut.Location = new System.Drawing.Point(106, 421);
            this.queuebut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.queuebut.Name = "queuebut";
            this.queuebut.Size = new System.Drawing.Size(308, 31);
            this.queuebut.TabIndex = 4;
            this.queuebut.Text = "Run the Gauntlet";
            this.toolTip1.SetToolTip(this.queuebut, "start the backtesting run");
            this.queuebut.UseVisualStyleBackColor = true;
            this.queuebut.Click += new System.EventHandler(this.queuebut_Click);
            // 
            // reslist
            // 
            this.reslist.ColumnWidth = 300;
            this.reslist.FormattingEnabled = true;
            this.reslist.ItemHeight = 20;
            this.reslist.Location = new System.Drawing.Point(106, 327);
            this.reslist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reslist.MultiColumn = true;
            this.reslist.Name = "reslist";
            this.reslist.Size = new System.Drawing.Size(506, 84);
            this.reslist.TabIndex = 2;
            this.toolTip1.SetToolTip(this.reslist, "select response to trade");
            this.reslist.SelectedIndexChanged += new System.EventHandler(this.boxlist_SelectedIndexChanged);
            // 
            // tickFileFilterControl1
            // 
            this.tickFileFilterControl1.Location = new System.Drawing.Point(9, 9);
            this.tickFileFilterControl1.Name = "tickFileFilterControl1";
            this.tickFileFilterControl1.Size = new System.Drawing.Size(607, 298);
            this.tickFileFilterControl1.TabIndex = 18;
            // 
            // optionpage
            // 
            this.optionpage.Controls.Add(this._portfoliosim);
            this.optionpage.Controls.Add(this._docapcon);
            this.optionpage.Controls.Add(this._capitalprompt);
            this.optionpage.Controls.Add(this._usebidask);
            this.optionpage.Controls.Add(this._indicatcsv);
            this.optionpage.Controls.Add(this._debugfile);
            this.optionpage.Controls.Add(this._unique);
            this.optionpage.Controls.Add(this.ordersincsv);
            this.optionpage.Controls.Add(this.button4);
            this.optionpage.Controls.Add(this.button3);
            this.optionpage.Controls.Add(this.messagewrite);
            this.optionpage.Controls.Add(this.clearmessages);
            this.optionpage.Controls.Add(this.saveonexit);
            this.optionpage.Controls.Add(this.savesettings);
            this.optionpage.Controls.Add(this.label2);
            this.optionpage.Controls.Add(this.button2);
            this.optionpage.Controls.Add(this._debugs);
            this.optionpage.Controls.Add(this.label1);
            this.optionpage.Controls.Add(this.button1);
            this.optionpage.Location = new System.Drawing.Point(4, 29);
            this.optionpage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Name = "optionpage";
            this.optionpage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Size = new System.Drawing.Size(621, 509);
            this.optionpage.TabIndex = 1;
            this.optionpage.Text = "Options";
            this.optionpage.UseVisualStyleBackColor = true;
            // 
            // _docapcon
            // 
            this._docapcon.Location = new System.Drawing.Point(264, 9);
            this._docapcon.Name = "_docapcon";
            this._docapcon.Size = new System.Drawing.Size(234, 35);
            this._docapcon.TabIndex = 31;
            this._docapcon.Text = "make capital connection";
            this._docapcon.UseVisualStyleBackColor = true;
            this._docapcon.Click += new System.EventHandler(this._docapcon_Click);
            // 
            // _capitalprompt
            // 
            this._capitalprompt.AutoSize = true;
            this._capitalprompt.Checked = global::WinGauntlet.Properties.Settings.Default.capitalprompt;
            this._capitalprompt.CheckState = System.Windows.Forms.CheckState.Checked;
            this._capitalprompt.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "capitalprompt", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._capitalprompt.Location = new System.Drawing.Point(42, 290);
            this._capitalprompt.Name = "_capitalprompt";
            this._capitalprompt.Size = new System.Drawing.Size(205, 24);
            this._capitalprompt.TabIndex = 30;
            this._capitalprompt.Text = "Allow capital connections";
            this._capitalprompt.UseVisualStyleBackColor = true;
            // 
            // _usebidask
            // 
            this._usebidask.AutoSize = true;
            this._usebidask.Checked = global::WinGauntlet.Properties.Settings.Default.UseBidAskFills;
            this._usebidask.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "UseBidAskFills", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._usebidask.Location = new System.Drawing.Point(306, 290);
            this._usebidask.Name = "_usebidask";
            this._usebidask.Size = new System.Drawing.Size(146, 24);
            this._usebidask.TabIndex = 29;
            this._usebidask.Text = "Use Bid/Ask Fills";
            this.toolTip1.SetToolTip(this._usebidask, "Use Bid/Ask to fill orders, otherwise last trade is used.  This should generally " +
                    "be enabled for for-ex");
            this._usebidask.UseVisualStyleBackColor = true;
            // 
            // _indicatcsv
            // 
            this._indicatcsv.AutoSize = true;
            this._indicatcsv.Location = new System.Drawing.Point(306, 223);
            this._indicatcsv.Name = "_indicatcsv";
            this._indicatcsv.Size = new System.Drawing.Size(135, 24);
            this._indicatcsv.TabIndex = 28;
            this._indicatcsv.Text = "Indicators CSV";
            this.toolTip1.SetToolTip(this._indicatcsv, "save indicators to log for analysis");
            this._indicatcsv.UseVisualStyleBackColor = true;
            // 
            // _debugfile
            // 
            this._debugfile.AutoSize = true;
            this._debugfile.Location = new System.Drawing.Point(306, 189);
            this._debugfile.Name = "_debugfile";
            this._debugfile.Size = new System.Drawing.Size(166, 24);
            this._debugfile.TabIndex = 27;
            this._debugfile.Text = "Messages in log file";
            this.toolTip1.SetToolTip(this._debugfile, "Saves messages to a text file for review");
            this._debugfile.UseVisualStyleBackColor = true;
            // 
            // _unique
            // 
            this._unique.AutoSize = true;
            this._unique.Checked = global::WinGauntlet.Properties.Settings.Default.csvnamesunique;
            this._unique.CheckState = System.Windows.Forms.CheckState.Checked;
            this._unique.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "csvnamesunique", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._unique.Location = new System.Drawing.Point(306, 154);
            this._unique.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this._unique.Name = "_unique";
            this._unique.Size = new System.Drawing.Size(151, 24);
            this._unique.TabIndex = 26;
            this._unique.Text = "Unique filenames";
            this.toolTip1.SetToolTip(this._unique, "ensure filenames never duplicate");
            this._unique.UseVisualStyleBackColor = true;
            // 
            // ordersincsv
            // 
            this.ordersincsv.AutoSize = true;
            this.ordersincsv.Checked = global::WinGauntlet.Properties.Settings.Default.ordersincsv;
            this.ordersincsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "ordersincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ordersincsv.Location = new System.Drawing.Point(306, 257);
            this.ordersincsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ordersincsv.Name = "ordersincsv";
            this.ordersincsv.Size = new System.Drawing.Size(129, 24);
            this.ordersincsv.TabIndex = 22;
            this.ordersincsv.Text = "Orders in CSV";
            this.toolTip1.SetToolTip(this.ordersincsv, "save orders to excel or R-compatible file");
            this.ordersincsv.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(81, 9);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 35);
            this.button4.TabIndex = 20;
            this.button4.Text = "Discard";
            this.toolTip1.SetToolTip(this.button4, "discard changes made since last save");
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(166, 9);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 35);
            this.button3.TabIndex = 19;
            this.button3.Text = "Defaults";
            this.toolTip1.SetToolTip(this.button3, "return to gauntlet default values");
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // messagewrite
            // 
            this.messagewrite.AutoSize = true;
            this.messagewrite.Checked = global::WinGauntlet.Properties.Settings.Default.writeonmessages;
            this.messagewrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.messagewrite.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "writeonmessages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.messagewrite.Location = new System.Drawing.Point(42, 257);
            this.messagewrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messagewrite.Name = "messagewrite";
            this.messagewrite.Size = new System.Drawing.Size(203, 24);
            this.messagewrite.TabIndex = 18;
            this.messagewrite.Text = "Disable Message Editing";
            this.toolTip1.SetToolTip(this.messagewrite, "disable modifying or making notes in messages window");
            this.messagewrite.UseVisualStyleBackColor = true;
            // 
            // clearmessages
            // 
            this.clearmessages.AutoSize = true;
            this.clearmessages.Checked = global::WinGauntlet.Properties.Settings.Default.clearmessages;
            this.clearmessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clearmessages.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "clearmessages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clearmessages.Location = new System.Drawing.Point(42, 223);
            this.clearmessages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clearmessages.Name = "clearmessages";
            this.clearmessages.Size = new System.Drawing.Size(198, 24);
            this.clearmessages.TabIndex = 17;
            this.clearmessages.Text = "Clear Messages on Run";
            this.toolTip1.SetToolTip(this.clearmessages, "clear messages window for each run");
            this.clearmessages.UseVisualStyleBackColor = true;
            // 
            // saveonexit
            // 
            this.saveonexit.AutoSize = true;
            this.saveonexit.Checked = global::WinGauntlet.Properties.Settings.Default.saveonexit;
            this.saveonexit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveonexit.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "saveonexit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.saveonexit.Location = new System.Drawing.Point(42, 154);
            this.saveonexit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveonexit.Name = "saveonexit";
            this.saveonexit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.saveonexit.Size = new System.Drawing.Size(116, 24);
            this.saveonexit.TabIndex = 14;
            this.saveonexit.Text = "Save on Exit";
            this.toolTip1.SetToolTip(this.saveonexit, "save gauntlet options on exit");
            this.saveonexit.UseVisualStyleBackColor = true;
            // 
            // savesettings
            // 
            this.savesettings.Location = new System.Drawing.Point(9, 9);
            this.savesettings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.savesettings.Name = "savesettings";
            this.savesettings.Size = new System.Drawing.Size(63, 35);
            this.savesettings.TabIndex = 13;
            this.savesettings.Text = "Save";
            this.toolTip1.SetToolTip(this.savesettings, "save these options");
            this.savesettings.UseVisualStyleBackColor = true;
            this.savesettings.Click += new System.EventHandler(this.savesettings_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 106);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(165, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Response DLL";
            this.toolTip1.SetToolTip(this.label2, "select response library used to populate response list on Studies tab");
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(42, 103);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 26);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // _debugs
            // 
            this._debugs.AutoSize = true;
            this._debugs.Checked = true;
            this._debugs.CheckState = System.Windows.Forms.CheckState.Checked;
            this._debugs.Location = new System.Drawing.Point(42, 189);
            this._debugs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._debugs.Name = "_debugs";
            this._debugs.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._debugs.Size = new System.Drawing.Size(183, 24);
            this._debugs.TabIndex = 2;
            this._debugs.Text = "Response Debugging";
            this.toolTip1.SetToolTip(this._debugs, "show your responses SendDebug messages in messages window");
            this._debugs.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(346, 106);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Tick Folder";
            this.toolTip1.SetToolTip(this.label1, "select folder that is scanned for historical data");
            // 
            // messagepage
            // 
            this.messagepage.Controls.Add(this.messages);
            this.messagepage.Location = new System.Drawing.Point(4, 29);
            this.messagepage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messagepage.Name = "messagepage";
            this.messagepage.Size = new System.Drawing.Size(621, 509);
            this.messagepage.TabIndex = 4;
            this.messagepage.Text = "Messages";
            this.messagepage.UseVisualStyleBackColor = true;
            // 
            // messages
            // 
            this.messages.BackColor = System.Drawing.SystemColors.Window;
            this.messages.DataBindings.Add(new System.Windows.Forms.Binding("ReadOnly", global::WinGauntlet.Properties.Settings.Default, "writeonmessages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.messages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messages.Location = new System.Drawing.Point(0, 0);
            this.messages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messages.Name = "messages";
            this.messages.ReadOnly = global::WinGauntlet.Properties.Settings.Default.writeonmessages;
            this.messages.Size = new System.Drawing.Size(621, 509);
            this.messages.TabIndex = 0;
            this.messages.Text = "";
            // 
            // _resulttab
            // 
            this._resulttab.Controls.Add(this.tradeResults1);
            this._resulttab.Location = new System.Drawing.Point(4, 29);
            this._resulttab.Name = "_resulttab";
            this._resulttab.Padding = new System.Windows.Forms.Padding(3);
            this._resulttab.Size = new System.Drawing.Size(621, 509);
            this._resulttab.TabIndex = 5;
            this._resulttab.Text = "Results";
            this._resulttab.UseVisualStyleBackColor = true;
            // 
            // tradeResults1
            // 
            this.tradeResults1.AutoWatch = false;
            this.tradeResults1.BackColor = System.Drawing.Color.White;
            this.tradeResults1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tradeResults1.Location = new System.Drawing.Point(3, 3);
            this.tradeResults1.Name = "tradeResults1";
            this.tradeResults1.Path = "C:\\Users\\jfranta\\Documents";
            this.tradeResults1.Size = new System.Drawing.Size(615, 503);
            this.tradeResults1.TabIndex = 0;
            // 
            // _portfoliosim
            // 
            this._portfoliosim.AutoSize = true;
            this._portfoliosim.Checked = global::WinGauntlet.Properties.Settings.Default.PortfolioRealisticSim;
            this._portfoliosim.CheckState = System.Windows.Forms.CheckState.Checked;
            this._portfoliosim.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "PortfolioRealisticSim", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._portfoliosim.Location = new System.Drawing.Point(42, 320);
            this._portfoliosim.Name = "_portfoliosim";
            this._portfoliosim.Size = new System.Drawing.Size(234, 24);
            this._portfoliosim.TabIndex = 32;
            this._portfoliosim.Text = "Portfolio-realistic Sim (slower)";
            this._portfoliosim.UseVisualStyleBackColor = true;
            // 
            // Gauntlet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 542);
            this.Controls.Add(this.tabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Gauntlet";
            this.Text = "Gauntlet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Gauntlet_FormClosing);
            this.tabs.ResumeLayout(false);
            this.studypage.ResumeLayout(false);
            this.studypage.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.optionpage.ResumeLayout(false);
            this.optionpage.PerformLayout();
            this.messagepage.ResumeLayout(false);
            this._resulttab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage studypage;
        private System.Windows.Forms.TabPage optionpage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage messagepage;
        private System.Windows.Forms.Button queuebut;
        private System.Windows.Forms.ListBox reslist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox messages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox saveonexit;
        private System.Windows.Forms.Button savesettings;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel lastmessage;
        private System.Windows.Forms.CheckBox clearmessages;
        private System.Windows.Forms.CheckBox messagewrite;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox ordersincsv;
        private System.Windows.Forms.CheckBox _unique;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox _debugfile;
        private System.Windows.Forms.CheckBox _indicatcsv;
        private TradeLink.AppKit.TickFileFilterControl tickFileFilterControl1;
        private System.Windows.Forms.Button _twithelp;
        private System.Windows.Forms.Button _viewresults;
        private System.Windows.Forms.CheckBox _debugs;
        private System.Windows.Forms.TabPage _resulttab;
        private TradeLink.AppKit.TradeResults tradeResults1;
        private System.Windows.Forms.CheckBox _usebidask;
        private System.Windows.Forms.CheckBox _capitalprompt;
        private System.Windows.Forms.Button _docapcon;
        private System.Windows.Forms.CheckBox _portfoliosim;
    }
}

