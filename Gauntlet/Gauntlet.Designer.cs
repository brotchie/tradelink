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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.studypage = new System.Windows.Forms.TabPage();
            this._stopbut = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.usedates = new System.Windows.Forms.CheckBox();
            this.usestocks = new System.Windows.Forms.CheckBox();
            this.daylist = new System.Windows.Forms.ListBox();
            this.monthlist = new System.Windows.Forms.ListBox();
            this.yearlist = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.lastmessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.queuebut = new System.Windows.Forms.Button();
            this.reslist = new System.Windows.Forms.ListBox();
            this.stocklist = new System.Windows.Forms.ListBox();
            this.optionpage = new System.Windows.Forms.TabPage();
            this.csvnamesunique = new System.Windows.Forms.CheckBox();
            this.indicatorscsv = new System.Windows.Forms.CheckBox();
            this.tradesincsv = new System.Windows.Forms.CheckBox();
            this.ordersincsv = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.messagewrite = new System.Windows.Forms.CheckBox();
            this.clearmessages = new System.Windows.Forms.CheckBox();
            this.saveonexit = new System.Windows.Forms.CheckBox();
            this.savesettings = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.showdebug = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.messagepage = new System.Windows.Forms.TabPage();
            this.messages = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1.SuspendLayout();
            this.studypage.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.optionpage.SuspendLayout();
            this.messagepage.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.studypage);
            this.tabControl1.Controls.Add(this.optionpage);
            this.tabControl1.Controls.Add(this.messagepage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(629, 414);
            this.tabControl1.TabIndex = 1;
            // 
            // studypage
            // 
            this.studypage.Controls.Add(this._stopbut);
            this.studypage.Controls.Add(this.label6);
            this.studypage.Controls.Add(this.label5);
            this.studypage.Controls.Add(this.label3);
            this.studypage.Controls.Add(this.usedates);
            this.studypage.Controls.Add(this.usestocks);
            this.studypage.Controls.Add(this.daylist);
            this.studypage.Controls.Add(this.monthlist);
            this.studypage.Controls.Add(this.yearlist);
            this.studypage.Controls.Add(this.statusStrip1);
            this.studypage.Controls.Add(this.label7);
            this.studypage.Controls.Add(this.queuebut);
            this.studypage.Controls.Add(this.reslist);
            this.studypage.Controls.Add(this.stocklist);
            this.studypage.Location = new System.Drawing.Point(4, 29);
            this.studypage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Name = "studypage";
            this.studypage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Size = new System.Drawing.Size(621, 381);
            this.studypage.TabIndex = 0;
            this.studypage.Text = "Studies";
            this.studypage.UseVisualStyleBackColor = true;
            // 
            // _stopbut
            // 
            this._stopbut.Enabled = false;
            this._stopbut.Location = new System.Drawing.Point(534, 300);
            this._stopbut.Name = "_stopbut";
            this._stopbut.Size = new System.Drawing.Size(78, 31);
            this._stopbut.TabIndex = 17;
            this._stopbut.Text = "Stop";
            this._stopbut.UseVisualStyleBackColor = true;
            this._stopbut.Click += new System.EventHandler(this._stopbut_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(236, 138);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Day";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(174, 138);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Month";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 138);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Year";
            // 
            // usedates
            // 
            this.usedates.Appearance = System.Windows.Forms.Appearance.Button;
            this.usedates.AutoSize = true;
            this.usedates.Location = new System.Drawing.Point(14, 165);
            this.usedates.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.usedates.Name = "usedates";
            this.usedates.Size = new System.Drawing.Size(62, 30);
            this.usedates.TabIndex = 13;
            this.usedates.Text = "Dates";
            this.toolTip1.SetToolTip(this.usedates, "toggle whether per-date filters are used");
            this.usedates.UseVisualStyleBackColor = true;
            this.usedates.CheckedChanged += new System.EventHandler(this.usedates_CheckedChanged);
            // 
            // usestocks
            // 
            this.usestocks.Appearance = System.Windows.Forms.Appearance.Button;
            this.usestocks.AutoSize = true;
            this.usestocks.Location = new System.Drawing.Point(14, 10);
            this.usestocks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.usestocks.Name = "usestocks";
            this.usestocks.Size = new System.Drawing.Size(79, 30);
            this.usestocks.TabIndex = 12;
            this.usestocks.Text = "Symbols";
            this.toolTip1.SetToolTip(this.usestocks, "toggle whether per-symbol filters are used");
            this.usestocks.UseVisualStyleBackColor = true;
            this.usestocks.CheckedChanged += new System.EventHandler(this.usestocks_CheckedChanged);
            // 
            // daylist
            // 
            this.daylist.ColumnWidth = 30;
            this.daylist.Enabled = false;
            this.daylist.HorizontalScrollbar = true;
            this.daylist.ItemHeight = 20;
            this.daylist.Location = new System.Drawing.Point(240, 165);
            this.daylist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.daylist.MultiColumn = true;
            this.daylist.Name = "daylist";
            this.daylist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.daylist.Size = new System.Drawing.Size(99, 124);
            this.daylist.TabIndex = 11;
            this.toolTip1.SetToolTip(this.daylist, "days to trade on response");
            // 
            // monthlist
            // 
            this.monthlist.Enabled = false;
            this.monthlist.FormattingEnabled = true;
            this.monthlist.ItemHeight = 20;
            this.monthlist.Location = new System.Drawing.Point(178, 165);
            this.monthlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.monthlist.Name = "monthlist";
            this.monthlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.monthlist.Size = new System.Drawing.Size(54, 124);
            this.monthlist.TabIndex = 10;
            this.toolTip1.SetToolTip(this.monthlist, "months to trade on response");
            // 
            // yearlist
            // 
            this.yearlist.Enabled = false;
            this.yearlist.FormattingEnabled = true;
            this.yearlist.ItemHeight = 20;
            this.yearlist.Location = new System.Drawing.Point(106, 166);
            this.yearlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.yearlist.Name = "yearlist";
            this.yearlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.yearlist.Size = new System.Drawing.Size(62, 124);
            this.yearlist.Sorted = true;
            this.yearlist.TabIndex = 9;
            this.toolTip1.SetToolTip(this.yearlist, "years to trade on this response");
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar1,
            this.lastmessage});
            this.statusStrip1.Location = new System.Drawing.Point(4, 340);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(613, 36);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ProgressBar1
            // 
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
            this.label7.Location = new System.Drawing.Point(343, 138);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "Responses";
            // 
            // queuebut
            // 
            this.queuebut.Location = new System.Drawing.Point(106, 300);
            this.queuebut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.queuebut.Name = "queuebut";
            this.queuebut.Size = new System.Drawing.Size(421, 31);
            this.queuebut.TabIndex = 4;
            this.queuebut.Text = "Run the Gauntlet";
            this.toolTip1.SetToolTip(this.queuebut, "start the backtesting run");
            this.queuebut.UseVisualStyleBackColor = true;
            this.queuebut.Click += new System.EventHandler(this.queuebut_Click);
            // 
            // reslist
            // 
            this.reslist.FormattingEnabled = true;
            this.reslist.ItemHeight = 20;
            this.reslist.Location = new System.Drawing.Point(347, 166);
            this.reslist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.reslist.Name = "reslist";
            this.reslist.Size = new System.Drawing.Size(265, 124);
            this.reslist.TabIndex = 2;
            this.toolTip1.SetToolTip(this.reslist, "select response to trade");
            this.reslist.SelectedIndexChanged += new System.EventHandler(this.boxlist_SelectedIndexChanged);
            // 
            // stocklist
            // 
            this.stocklist.Enabled = false;
            this.stocklist.FormattingEnabled = true;
            this.stocklist.ItemHeight = 20;
            this.stocklist.Location = new System.Drawing.Point(106, 9);
            this.stocklist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stocklist.MultiColumn = true;
            this.stocklist.Name = "stocklist";
            this.stocklist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.stocklist.Size = new System.Drawing.Size(506, 124);
            this.stocklist.TabIndex = 0;
            this.toolTip1.SetToolTip(this.stocklist, "list of symbols found in your tick folder");
            // 
            // optionpage
            // 
            this.optionpage.Controls.Add(this.csvnamesunique);
            this.optionpage.Controls.Add(this.indicatorscsv);
            this.optionpage.Controls.Add(this.tradesincsv);
            this.optionpage.Controls.Add(this.ordersincsv);
            this.optionpage.Controls.Add(this.button4);
            this.optionpage.Controls.Add(this.button3);
            this.optionpage.Controls.Add(this.messagewrite);
            this.optionpage.Controls.Add(this.clearmessages);
            this.optionpage.Controls.Add(this.saveonexit);
            this.optionpage.Controls.Add(this.savesettings);
            this.optionpage.Controls.Add(this.label2);
            this.optionpage.Controls.Add(this.button2);
            this.optionpage.Controls.Add(this.showdebug);
            this.optionpage.Controls.Add(this.label1);
            this.optionpage.Controls.Add(this.button1);
            this.optionpage.Location = new System.Drawing.Point(4, 29);
            this.optionpage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Name = "optionpage";
            this.optionpage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Size = new System.Drawing.Size(621, 381);
            this.optionpage.TabIndex = 1;
            this.optionpage.Text = "Options";
            this.optionpage.UseVisualStyleBackColor = true;
            // 
            // csvnamesunique
            // 
            this.csvnamesunique.AutoSize = true;
            this.csvnamesunique.Checked = global::WinGauntlet.Properties.Settings.Default.csvnamesunique;
            this.csvnamesunique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.csvnamesunique.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "csvnamesunique", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.csvnamesunique.Location = new System.Drawing.Point(306, 154);
            this.csvnamesunique.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.csvnamesunique.Name = "csvnamesunique";
            this.csvnamesunique.Size = new System.Drawing.Size(191, 24);
            this.csvnamesunique.TabIndex = 26;
            this.csvnamesunique.Text = "Unique CSV filenames";
            this.toolTip1.SetToolTip(this.csvnamesunique, "ensure CSV filenames never duplicate");
            this.csvnamesunique.UseVisualStyleBackColor = true;
            // 
            // indicatorscsv
            // 
            this.indicatorscsv.AutoSize = true;
            this.indicatorscsv.Checked = global::WinGauntlet.Properties.Settings.Default.indicatorsincsv;
            this.indicatorscsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "indicatorsincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.indicatorscsv.Location = new System.Drawing.Point(306, 223);
            this.indicatorscsv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.indicatorscsv.Name = "indicatorscsv";
            this.indicatorscsv.Size = new System.Drawing.Size(154, 24);
            this.indicatorscsv.TabIndex = 25;
            this.indicatorscsv.Text = "Indicators in CSV";
            this.toolTip1.SetToolTip(this.indicatorscsv, "save indicators to excel or R-compatible file");
            this.indicatorscsv.UseVisualStyleBackColor = true;
            // 
            // tradesincsv
            // 
            this.tradesincsv.AutoSize = true;
            this.tradesincsv.Checked = global::WinGauntlet.Properties.Settings.Default.tradesincsv;
            this.tradesincsv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tradesincsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "tradesincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tradesincsv.Location = new System.Drawing.Point(306, 189);
            this.tradesincsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tradesincsv.Name = "tradesincsv";
            this.tradesincsv.Size = new System.Drawing.Size(133, 24);
            this.tradesincsv.TabIndex = 24;
            this.tradesincsv.Text = "Trades in CSV";
            this.toolTip1.SetToolTip(this.tradesincsv, "save trades to excel or R-compatible file");
            this.tradesincsv.UseVisualStyleBackColor = true;
            // 
            // ordersincsv
            // 
            this.ordersincsv.AutoSize = true;
            this.ordersincsv.Checked = global::WinGauntlet.Properties.Settings.Default.ordersincsv;
            this.ordersincsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "ordersincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ordersincsv.Location = new System.Drawing.Point(306, 257);
            this.ordersincsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ordersincsv.Name = "ordersincsv";
            this.ordersincsv.Size = new System.Drawing.Size(132, 24);
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
            this.messagewrite.Size = new System.Drawing.Size(206, 24);
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
            this.clearmessages.Size = new System.Drawing.Size(201, 24);
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
            this.saveonexit.Size = new System.Drawing.Size(119, 24);
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
            // showdebug
            // 
            this.showdebug.AutoSize = true;
            this.showdebug.Location = new System.Drawing.Point(42, 189);
            this.showdebug.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showdebug.Name = "showdebug";
            this.showdebug.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.showdebug.Size = new System.Drawing.Size(186, 24);
            this.showdebug.TabIndex = 2;
            this.showdebug.Text = "Response Debugging";
            this.toolTip1.SetToolTip(this.showdebug, "show your responses SendDebug messages in messages window");
            this.showdebug.UseVisualStyleBackColor = true;
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
            this.messagepage.Size = new System.Drawing.Size(621, 381);
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
            this.messages.Size = new System.Drawing.Size(621, 381);
            this.messages.TabIndex = 0;
            this.messages.Text = "";
            // 
            // Gauntlet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 414);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Gauntlet";
            this.Text = "Gauntlet";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Gauntlet_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.studypage.ResumeLayout(false);
            this.studypage.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.optionpage.ResumeLayout(false);
            this.optionpage.PerformLayout();
            this.messagepage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage studypage;
        private System.Windows.Forms.TabPage optionpage;
        private System.Windows.Forms.CheckBox showdebug;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage messagepage;
        private System.Windows.Forms.ListBox stocklist;
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
        private System.Windows.Forms.CheckBox usestocks;
        private System.Windows.Forms.ListBox daylist;
        private System.Windows.Forms.ListBox monthlist;
        private System.Windows.Forms.ListBox yearlist;
        private System.Windows.Forms.CheckBox usedates;
        private System.Windows.Forms.CheckBox clearmessages;
        private System.Windows.Forms.CheckBox messagewrite;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox ordersincsv;
        private System.Windows.Forms.CheckBox tradesincsv;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox indicatorscsv;
        private System.Windows.Forms.CheckBox csvnamesunique;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button _stopbut;
    }
}

