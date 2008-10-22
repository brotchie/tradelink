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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Gauntlet));
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.studypage = new System.Windows.Forms.TabPage();
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
            this.boxlist = new System.Windows.Forms.ListBox();
            this.stocklist = new System.Windows.Forms.ListBox();
            this.optionpage = new System.Windows.Forms.TabPage();
            this.spreadlist = new System.Windows.Forms.TextBox();
            this.spreadliston = new System.Windows.Forms.CheckBox();
            this.spreadallon = new System.Windows.Forms.CheckBox();
            this.csvnamesunique = new System.Windows.Forms.CheckBox();
            this.indicatorscsv = new System.Windows.Forms.CheckBox();
            this.tradesincsv = new System.Windows.Forms.CheckBox();
            this.tradesinwind = new System.Windows.Forms.CheckBox();
            this.ordersincsv = new System.Windows.Forms.CheckBox();
            this.ordersinwind = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.messagewrite = new System.Windows.Forms.CheckBox();
            this.clearmessages = new System.Windows.Forms.CheckBox();
            this.cleartrades = new System.Windows.Forms.CheckBox();
            this.clearorders = new System.Windows.Forms.CheckBox();
            this.saveonexit = new System.Windows.Forms.CheckBox();
            this.savesettings = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.exchlist = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.showdebug = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.orderpage = new System.Windows.Forms.TabPage();
            this.orders = new System.Windows.Forms.DataGridView();
            this.tradepage = new System.Windows.Forms.TabPage();
            this.trades = new System.Windows.Forms.DataGridView();
            this.messagepage = new System.Windows.Forms.TabPage();
            this.messages = new System.Windows.Forms.RichTextBox();
            this.tabControl1.SuspendLayout();
            this.studypage.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.optionpage.SuspendLayout();
            this.orderpage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.orders)).BeginInit();
            this.tradepage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trades)).BeginInit();
            this.messagepage.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 54);
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
            this.tabControl1.Controls.Add(this.orderpage);
            this.tabControl1.Controls.Add(this.tradepage);
            this.tabControl1.Controls.Add(this.messagepage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(687, 414);
            this.tabControl1.TabIndex = 1;
            // 
            // studypage
            // 
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
            this.studypage.Controls.Add(this.boxlist);
            this.studypage.Controls.Add(this.stocklist);
            this.studypage.Location = new System.Drawing.Point(4, 29);
            this.studypage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Name = "studypage";
            this.studypage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.studypage.Size = new System.Drawing.Size(679, 381);
            this.studypage.TabIndex = 0;
            this.studypage.Text = "Studies";
            this.studypage.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(224, 140);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 20);
            this.label6.TabIndex = 16;
            this.label6.Text = "Day";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(159, 140);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Month";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 140);
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
            this.usedates.UseVisualStyleBackColor = true;
            this.usedates.CheckedChanged += new System.EventHandler(this.usedates_CheckedChanged);
            // 
            // usestocks
            // 
            this.usestocks.Appearance = System.Windows.Forms.Appearance.Button;
            this.usestocks.AutoSize = true;
            this.usestocks.Location = new System.Drawing.Point(6, 9);
            this.usestocks.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.usestocks.Name = "usestocks";
            this.usestocks.Size = new System.Drawing.Size(68, 30);
            this.usestocks.TabIndex = 12;
            this.usestocks.Text = "Stocks";
            this.usestocks.UseVisualStyleBackColor = true;
            this.usestocks.CheckedChanged += new System.EventHandler(this.usestocks_CheckedChanged);
            // 
            // daylist
            // 
            this.daylist.ColumnWidth = 2;
            this.daylist.Enabled = false;
            this.daylist.FormattingEnabled = true;
            this.daylist.ItemHeight = 20;
            this.daylist.Location = new System.Drawing.Point(224, 165);
            this.daylist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.daylist.MultiColumn = true;
            this.daylist.Name = "daylist";
            this.daylist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.daylist.Size = new System.Drawing.Size(142, 124);
            this.daylist.Sorted = true;
            this.daylist.TabIndex = 11;
            // 
            // monthlist
            // 
            this.monthlist.Enabled = false;
            this.monthlist.FormattingEnabled = true;
            this.monthlist.ItemHeight = 20;
            this.monthlist.Location = new System.Drawing.Point(163, 165);
            this.monthlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.monthlist.Name = "monthlist";
            this.monthlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.monthlist.Size = new System.Drawing.Size(49, 124);
            this.monthlist.Sorted = true;
            this.monthlist.TabIndex = 10;
            // 
            // yearlist
            // 
            this.yearlist.Enabled = false;
            this.yearlist.FormattingEnabled = true;
            this.yearlist.ItemHeight = 20;
            this.yearlist.Location = new System.Drawing.Point(90, 165);
            this.yearlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.yearlist.Name = "yearlist";
            this.yearlist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.yearlist.Size = new System.Drawing.Size(62, 124);
            this.yearlist.Sorted = true;
            this.yearlist.TabIndex = 9;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ProgressBar1,
            this.lastmessage});
            this.statusStrip1.Location = new System.Drawing.Point(4, 340);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(671, 36);
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
            this.label7.Location = new System.Drawing.Point(377, 140);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 20);
            this.label7.TabIndex = 7;
            this.label7.Text = "Boxes";
            // 
            // queuebut
            // 
            this.queuebut.Location = new System.Drawing.Point(90, 300);
            this.queuebut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.queuebut.Name = "queuebut";
            this.queuebut.Size = new System.Drawing.Size(573, 31);
            this.queuebut.TabIndex = 4;
            this.queuebut.Text = "Run the Gauntlet";
            this.queuebut.UseVisualStyleBackColor = true;
            this.queuebut.Click += new System.EventHandler(this.queuebut_Click);
            // 
            // boxlist
            // 
            this.boxlist.FormattingEnabled = true;
            this.boxlist.ItemHeight = 20;
            this.boxlist.Location = new System.Drawing.Point(381, 165);
            this.boxlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.boxlist.Name = "boxlist";
            this.boxlist.Size = new System.Drawing.Size(280, 124);
            this.boxlist.TabIndex = 2;
            this.boxlist.SelectedIndexChanged += new System.EventHandler(this.boxlist_SelectedIndexChanged);
            // 
            // stocklist
            // 
            this.stocklist.Enabled = false;
            this.stocklist.FormattingEnabled = true;
            this.stocklist.ItemHeight = 20;
            this.stocklist.Location = new System.Drawing.Point(90, 9);
            this.stocklist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stocklist.MultiColumn = true;
            this.stocklist.Name = "stocklist";
            this.stocklist.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.stocklist.Size = new System.Drawing.Size(571, 124);
            this.stocklist.TabIndex = 0;
            // 
            // optionpage
            // 
            this.optionpage.Controls.Add(this.spreadlist);
            this.optionpage.Controls.Add(this.spreadliston);
            this.optionpage.Controls.Add(this.spreadallon);
            this.optionpage.Controls.Add(this.csvnamesunique);
            this.optionpage.Controls.Add(this.indicatorscsv);
            this.optionpage.Controls.Add(this.tradesincsv);
            this.optionpage.Controls.Add(this.tradesinwind);
            this.optionpage.Controls.Add(this.ordersincsv);
            this.optionpage.Controls.Add(this.ordersinwind);
            this.optionpage.Controls.Add(this.button4);
            this.optionpage.Controls.Add(this.button3);
            this.optionpage.Controls.Add(this.messagewrite);
            this.optionpage.Controls.Add(this.clearmessages);
            this.optionpage.Controls.Add(this.cleartrades);
            this.optionpage.Controls.Add(this.clearorders);
            this.optionpage.Controls.Add(this.saveonexit);
            this.optionpage.Controls.Add(this.savesettings);
            this.optionpage.Controls.Add(this.label4);
            this.optionpage.Controls.Add(this.exchlist);
            this.optionpage.Controls.Add(this.label2);
            this.optionpage.Controls.Add(this.button2);
            this.optionpage.Controls.Add(this.showdebug);
            this.optionpage.Controls.Add(this.label1);
            this.optionpage.Controls.Add(this.button1);
            this.optionpage.Location = new System.Drawing.Point(4, 29);
            this.optionpage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Name = "optionpage";
            this.optionpage.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.optionpage.Size = new System.Drawing.Size(679, 381);
            this.optionpage.TabIndex = 1;
            this.optionpage.Text = "Options";
            this.optionpage.UseVisualStyleBackColor = true;
            // 
            // spreadlist
            // 
            this.spreadlist.Location = new System.Drawing.Point(468, 124);
            this.spreadlist.Name = "spreadlist";
            this.spreadlist.Size = new System.Drawing.Size(100, 26);
            this.spreadlist.TabIndex = 30;
            this.spreadlist.Visible = false;
            // 
            // spreadliston
            // 
            this.spreadliston.AutoSize = true;
            this.spreadliston.Location = new System.Drawing.Point(245, 126);
            this.spreadliston.Name = "spreadliston";
            this.spreadliston.Size = new System.Drawing.Size(228, 24);
            this.spreadliston.TabIndex = 29;
            this.spreadliston.Text = "Spread only these symbols :";
            this.spreadliston.UseVisualStyleBackColor = true;
            this.spreadliston.Visible = false;
            this.spreadliston.CheckedChanged += new System.EventHandler(this.spreadliston_CheckedChanged);
            // 
            // spreadallon
            // 
            this.spreadallon.AutoSize = true;
            this.spreadallon.Location = new System.Drawing.Point(8, 127);
            this.spreadallon.Name = "spreadallon";
            this.spreadallon.Size = new System.Drawing.Size(204, 24);
            this.spreadallon.TabIndex = 28;
            this.spreadallon.Text = "Spread trade all symbols";
            this.spreadallon.UseVisualStyleBackColor = true;
            this.spreadallon.Visible = false;
            this.spreadallon.CheckedChanged += new System.EventHandler(this.spreadallon_CheckedChanged);
            // 
            // csvnamesunique
            // 
            this.csvnamesunique.AutoSize = true;
            this.csvnamesunique.Checked = global::WinGauntlet.Properties.Settings.Default.csvnamesunique;
            this.csvnamesunique.CheckState = System.Windows.Forms.CheckState.Checked;
            this.csvnamesunique.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "csvnamesunique", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.csvnamesunique.Location = new System.Drawing.Point(245, 335);
            this.csvnamesunique.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.csvnamesunique.Name = "csvnamesunique";
            this.csvnamesunique.Size = new System.Drawing.Size(191, 24);
            this.csvnamesunique.TabIndex = 26;
            this.csvnamesunique.Text = "Unique CSV filenames";
            this.csvnamesunique.UseVisualStyleBackColor = true;
            // 
            // indicatorscsv
            // 
            this.indicatorscsv.AutoSize = true;
            this.indicatorscsv.Checked = global::WinGauntlet.Properties.Settings.Default.indicatorsincsv;
            this.indicatorscsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "indicatorsincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.indicatorscsv.Location = new System.Drawing.Point(244, 300);
            this.indicatorscsv.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.indicatorscsv.Name = "indicatorscsv";
            this.indicatorscsv.Size = new System.Drawing.Size(154, 24);
            this.indicatorscsv.TabIndex = 25;
            this.indicatorscsv.Text = "Indicators in CSV";
            this.indicatorscsv.UseVisualStyleBackColor = true;
            // 
            // tradesincsv
            // 
            this.tradesincsv.AutoSize = true;
            this.tradesincsv.Checked = global::WinGauntlet.Properties.Settings.Default.tradesincsv;
            this.tradesincsv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tradesincsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "tradesincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tradesincsv.Location = new System.Drawing.Point(244, 265);
            this.tradesincsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tradesincsv.Name = "tradesincsv";
            this.tradesincsv.Size = new System.Drawing.Size(133, 24);
            this.tradesincsv.TabIndex = 24;
            this.tradesincsv.Text = "Trades in CSV";
            this.tradesincsv.UseVisualStyleBackColor = true;
            // 
            // tradesinwind
            // 
            this.tradesinwind.AutoSize = true;
            this.tradesinwind.Checked = global::WinGauntlet.Properties.Settings.Default.tradesinwind;
            this.tradesinwind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tradesinwind.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "tradesinwind", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tradesinwind.Location = new System.Drawing.Point(244, 229);
            this.tradesinwind.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tradesinwind.Name = "tradesinwind";
            this.tradesinwind.Size = new System.Drawing.Size(162, 24);
            this.tradesinwind.TabIndex = 23;
            this.tradesinwind.Text = "Trades in Gauntlet";
            this.tradesinwind.UseVisualStyleBackColor = true;
            // 
            // ordersincsv
            // 
            this.ordersincsv.AutoSize = true;
            this.ordersincsv.Checked = global::WinGauntlet.Properties.Settings.Default.ordersincsv;
            this.ordersincsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "ordersincsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ordersincsv.Location = new System.Drawing.Point(245, 195);
            this.ordersincsv.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ordersincsv.Name = "ordersincsv";
            this.ordersincsv.Size = new System.Drawing.Size(132, 24);
            this.ordersincsv.TabIndex = 22;
            this.ordersincsv.Text = "Orders in CSV";
            this.ordersincsv.UseVisualStyleBackColor = true;
            // 
            // ordersinwind
            // 
            this.ordersinwind.AutoSize = true;
            this.ordersinwind.Checked = global::WinGauntlet.Properties.Settings.Default.ordersinwind;
            this.ordersinwind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ordersinwind.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "ordersinwind", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ordersinwind.Location = new System.Drawing.Point(245, 161);
            this.ordersinwind.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ordersinwind.Name = "ordersinwind";
            this.ordersinwind.Size = new System.Drawing.Size(161, 24);
            this.ordersinwind.TabIndex = 21;
            this.ordersinwind.Text = "Orders in Gauntlet";
            this.ordersinwind.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(81, 9);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(76, 35);
            this.button4.TabIndex = 20;
            this.button4.Text = "Discard";
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
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // messagewrite
            // 
            this.messagewrite.AutoSize = true;
            this.messagewrite.Checked = global::WinGauntlet.Properties.Settings.Default.writeonmessages;
            this.messagewrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.messagewrite.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "writeonmessages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.messagewrite.Location = new System.Drawing.Point(9, 159);
            this.messagewrite.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messagewrite.Name = "messagewrite";
            this.messagewrite.Size = new System.Drawing.Size(206, 24);
            this.messagewrite.TabIndex = 18;
            this.messagewrite.Text = "Disable Message Editing";
            this.messagewrite.UseVisualStyleBackColor = true;
            // 
            // clearmessages
            // 
            this.clearmessages.AutoSize = true;
            this.clearmessages.Checked = global::WinGauntlet.Properties.Settings.Default.clearmessages;
            this.clearmessages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clearmessages.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "clearmessages", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clearmessages.Location = new System.Drawing.Point(9, 335);
            this.clearmessages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clearmessages.Name = "clearmessages";
            this.clearmessages.Size = new System.Drawing.Size(201, 24);
            this.clearmessages.TabIndex = 17;
            this.clearmessages.Text = "Clear Messages on Run";
            this.clearmessages.UseVisualStyleBackColor = true;
            // 
            // cleartrades
            // 
            this.cleartrades.AutoSize = true;
            this.cleartrades.Checked = global::WinGauntlet.Properties.Settings.Default.cleartrades;
            this.cleartrades.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cleartrades.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "cleartrades", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cleartrades.Location = new System.Drawing.Point(9, 300);
            this.cleartrades.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cleartrades.Name = "cleartrades";
            this.cleartrades.Size = new System.Drawing.Size(177, 24);
            this.cleartrades.TabIndex = 16;
            this.cleartrades.Text = "Clear Trades on Run";
            this.cleartrades.UseVisualStyleBackColor = true;
            // 
            // clearorders
            // 
            this.clearorders.AutoSize = true;
            this.clearorders.Checked = global::WinGauntlet.Properties.Settings.Default.clearorders;
            this.clearorders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clearorders.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "clearorders", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.clearorders.Location = new System.Drawing.Point(9, 265);
            this.clearorders.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clearorders.Name = "clearorders";
            this.clearorders.Size = new System.Drawing.Size(176, 24);
            this.clearorders.TabIndex = 15;
            this.clearorders.Text = "Clear Orders on Run";
            this.clearorders.UseVisualStyleBackColor = true;
            // 
            // saveonexit
            // 
            this.saveonexit.AutoSize = true;
            this.saveonexit.Checked = global::WinGauntlet.Properties.Settings.Default.saveonexit;
            this.saveonexit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saveonexit.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::WinGauntlet.Properties.Settings.Default, "saveonexit", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.saveonexit.Location = new System.Drawing.Point(9, 229);
            this.saveonexit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveonexit.Name = "saveonexit";
            this.saveonexit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.saveonexit.Size = new System.Drawing.Size(119, 24);
            this.saveonexit.TabIndex = 14;
            this.saveonexit.Text = "Save on Exit";
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
            this.savesettings.UseVisualStyleBackColor = true;
            this.savesettings.Click += new System.EventHandler(this.savesettings_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(507, 233);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Exchange Filter";
            // 
            // exchlist
            // 
            this.exchlist.FormattingEnabled = true;
            this.exchlist.ItemHeight = 20;
            this.exchlist.Items.AddRange(new object[] {
            "<NoFiltering>",
            "NYS",
            "NMS",
            "PSE"});
            this.exchlist.Location = new System.Drawing.Point(511, 265);
            this.exchlist.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.exchlist.MultiColumn = true;
            this.exchlist.Name = "exchlist";
            this.exchlist.Size = new System.Drawing.Size(127, 84);
            this.exchlist.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(269, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Select Box DLL";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(231, 56);
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
            this.showdebug.Location = new System.Drawing.Point(9, 194);
            this.showdebug.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showdebug.Name = "showdebug";
            this.showdebug.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.showdebug.Size = new System.Drawing.Size(140, 24);
            this.showdebug.TabIndex = 2;
            this.showdebug.Text = "Box Debugging";
            this.showdebug.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Tick Folder";
            // 
            // orderpage
            // 
            this.orderpage.Controls.Add(this.orders);
            this.orderpage.Location = new System.Drawing.Point(4, 29);
            this.orderpage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.orderpage.Name = "orderpage";
            this.orderpage.Size = new System.Drawing.Size(679, 381);
            this.orderpage.TabIndex = 2;
            this.orderpage.Text = "Orders";
            this.orderpage.UseVisualStyleBackColor = true;
            // 
            // orders
            // 
            this.orders.AllowUserToAddRows = false;
            this.orders.AllowUserToDeleteRows = false;
            this.orders.AllowUserToOrderColumns = true;
            this.orders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.orders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.orders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.orders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.orders.Location = new System.Drawing.Point(0, 0);
            this.orders.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.orders.Name = "orders";
            this.orders.ReadOnly = true;
            this.orders.RowHeadersVisible = false;
            this.orders.RowTemplate.Height = 24;
            this.orders.Size = new System.Drawing.Size(679, 381);
            this.orders.TabIndex = 0;
            // 
            // tradepage
            // 
            this.tradepage.Controls.Add(this.trades);
            this.tradepage.Location = new System.Drawing.Point(4, 29);
            this.tradepage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tradepage.Name = "tradepage";
            this.tradepage.Size = new System.Drawing.Size(679, 381);
            this.tradepage.TabIndex = 3;
            this.tradepage.Text = "Trades";
            this.tradepage.UseVisualStyleBackColor = true;
            // 
            // trades
            // 
            this.trades.AllowUserToAddRows = false;
            this.trades.AllowUserToDeleteRows = false;
            this.trades.AllowUserToOrderColumns = true;
            this.trades.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.trades.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.trades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.trades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trades.Location = new System.Drawing.Point(0, 0);
            this.trades.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.trades.Name = "trades";
            this.trades.ReadOnly = true;
            this.trades.RowHeadersVisible = false;
            this.trades.RowTemplate.Height = 24;
            this.trades.Size = new System.Drawing.Size(679, 381);
            this.trades.TabIndex = 0;
            // 
            // messagepage
            // 
            this.messagepage.Controls.Add(this.messages);
            this.messagepage.Location = new System.Drawing.Point(4, 29);
            this.messagepage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.messagepage.Name = "messagepage";
            this.messagepage.Size = new System.Drawing.Size(679, 381);
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
            this.messages.Size = new System.Drawing.Size(679, 381);
            this.messages.TabIndex = 0;
            this.messages.Text = "";
            // 
            // Gauntlet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 414);
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
            this.orderpage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.orders)).EndInit();
            this.tradepage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trades)).EndInit();
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
        private System.Windows.Forms.TabPage orderpage;
        private System.Windows.Forms.TabPage tradepage;
        private System.Windows.Forms.TabPage messagepage;
        private System.Windows.Forms.ListBox stocklist;
        private System.Windows.Forms.Button queuebut;
        private System.Windows.Forms.ListBox boxlist;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RichTextBox messages;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox exchlist;
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
        private System.Windows.Forms.DataGridView orders;
        private System.Windows.Forms.DataGridView trades;
        private System.Windows.Forms.CheckBox cleartrades;
        private System.Windows.Forms.CheckBox clearorders;
        private System.Windows.Forms.CheckBox clearmessages;
        private System.Windows.Forms.CheckBox messagewrite;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.CheckBox ordersinwind;
        private System.Windows.Forms.CheckBox ordersincsv;
        private System.Windows.Forms.CheckBox tradesincsv;
        private System.Windows.Forms.CheckBox tradesinwind;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox indicatorscsv;
        private System.Windows.Forms.CheckBox csvnamesunique;
        private System.Windows.Forms.CheckBox spreadallon;
        private System.Windows.Forms.CheckBox spreadliston;
        private System.Windows.Forms.TextBox spreadlist;
    }
}

