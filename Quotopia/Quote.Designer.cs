namespace Quotopia
{
    partial class Quote
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Quote));
            this.quoteTab = new System.Windows.Forms.TabControl();
            this.Markets = new System.Windows.Forms.TabPage();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statuslab = new System.Windows.Forms.ToolStripStatusLabel();
            this.ordertab = new System.Windows.Forms.TabPage();
            this.ordergrid = new System.Windows.Forms.DataGridView();
            this.oid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.osymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oside = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.osize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oprice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ostop = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oaccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TradeTab = new System.Windows.Forms.TabPage();
            this.TradesView = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Symbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Side = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Settings = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.exchdest = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.accountname = new System.Windows.Forms.TextBox();
            this.resetsetbut = new System.Windows.Forms.Button();
            this.restoredefaultsbut = new System.Windows.Forms.Button();
            this.saveSettingsbut = new System.Windows.Forms.Button();
            this.LogTab = new System.Windows.Forms.TabPage();
            this.statusWindow = new System.Windows.Forms.RichTextBox();
            this.OrderUserCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderStopCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderPriceCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderSizeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderSideCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderTimeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderDateCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.quoteTab.SuspendLayout();
            this.Markets.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.ordertab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ordergrid)).BeginInit();
            this.TradeTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TradesView)).BeginInit();
            this.Settings.SuspendLayout();
            this.LogTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // quoteTab
            // 
            this.quoteTab.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.quoteTab.Controls.Add(this.Markets);
            this.quoteTab.Controls.Add(this.ordertab);
            this.quoteTab.Controls.Add(this.TradeTab);
            this.quoteTab.Controls.Add(this.Settings);
            this.quoteTab.Controls.Add(this.LogTab);
            this.quoteTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.quoteTab.Location = new System.Drawing.Point(0, 0);
            this.quoteTab.Margin = new System.Windows.Forms.Padding(0);
            this.quoteTab.Multiline = true;
            this.quoteTab.Name = "quoteTab";
            this.quoteTab.SelectedIndex = 0;
            this.quoteTab.ShowToolTips = true;
            this.quoteTab.Size = new System.Drawing.Size(808, 379);
            this.quoteTab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.quoteTab.TabIndex = 0;
            // 
            // Markets
            // 
            this.Markets.BackColor = System.Drawing.Color.White;
            this.Markets.Controls.Add(this.statusStrip1);
            this.Markets.ForeColor = System.Drawing.Color.White;
            this.Markets.Location = new System.Drawing.Point(4, 4);
            this.Markets.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Markets.Name = "Markets";
            this.Markets.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Markets.Size = new System.Drawing.Size(800, 346);
            this.Markets.TabIndex = 0;
            this.Markets.Text = "Markets";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.Transparent;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslab});
            this.statusStrip1.Location = new System.Drawing.Point(4, 311);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 30);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statuslab
            // 
            this.statuslab.BackColor = System.Drawing.Color.Transparent;
            this.statuslab.ForeColor = System.Drawing.Color.Black;
            this.statuslab.Name = "statuslab";
            this.statuslab.Size = new System.Drawing.Size(208, 25);
            this.statuslab.Text = "Enter symbols to begin...";
            // 
            // ordertab
            // 
            this.ordertab.Controls.Add(this.ordergrid);
            this.ordertab.Location = new System.Drawing.Point(4, 4);
            this.ordertab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ordertab.Name = "ordertab";
            this.ordertab.Size = new System.Drawing.Size(932, 676);
            this.ordertab.TabIndex = 4;
            this.ordertab.Text = "Orders";
            this.ordertab.UseVisualStyleBackColor = true;
            // 
            // ordergrid
            // 
            this.ordergrid.AllowUserToAddRows = false;
            this.ordergrid.AllowUserToDeleteRows = false;
            this.ordergrid.BackgroundColor = System.Drawing.SystemColors.Window;
            this.ordergrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ordergrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.ordergrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ordergrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.ordergrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ordergrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.oid,
            this.osymbol,
            this.oside,
            this.osize,
            this.oprice,
            this.ostop,
            this.oaccount});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ordergrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.ordergrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ordergrid.Location = new System.Drawing.Point(0, 0);
            this.ordergrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ordergrid.Name = "ordergrid";
            this.ordergrid.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ordergrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.ordergrid.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ordergrid.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.ordergrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            this.ordergrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ordergrid.RowTemplate.Height = 24;
            this.ordergrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ordergrid.Size = new System.Drawing.Size(932, 676);
            this.ordergrid.TabIndex = 0;
            // 
            // oid
            // 
            this.oid.HeaderText = "ID";
            this.oid.Name = "oid";
            this.oid.ReadOnly = true;
            this.oid.Visible = false;
            // 
            // osymbol
            // 
            this.osymbol.HeaderText = "Symbol";
            this.osymbol.Name = "osymbol";
            this.osymbol.ReadOnly = true;
            // 
            // oside
            // 
            this.oside.HeaderText = "Side";
            this.oside.Name = "oside";
            this.oside.ReadOnly = true;
            // 
            // osize
            // 
            this.osize.HeaderText = "Size";
            this.osize.Name = "osize";
            this.osize.ReadOnly = true;
            // 
            // oprice
            // 
            this.oprice.HeaderText = "Price";
            this.oprice.Name = "oprice";
            this.oprice.ReadOnly = true;
            // 
            // ostop
            // 
            this.ostop.HeaderText = "Stop";
            this.ostop.Name = "ostop";
            this.ostop.ReadOnly = true;
            // 
            // oaccount
            // 
            this.oaccount.HeaderText = "Account";
            this.oaccount.Name = "oaccount";
            this.oaccount.ReadOnly = true;
            // 
            // TradeTab
            // 
            this.TradeTab.BackColor = System.Drawing.Color.White;
            this.TradeTab.Controls.Add(this.TradesView);
            this.TradeTab.ForeColor = System.Drawing.SystemColors.MenuText;
            this.TradeTab.Location = new System.Drawing.Point(4, 4);
            this.TradeTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TradeTab.Name = "TradeTab";
            this.TradeTab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TradeTab.Size = new System.Drawing.Size(932, 676);
            this.TradeTab.TabIndex = 3;
            this.TradeTab.Text = "Trades";
            this.TradeTab.UseVisualStyleBackColor = true;
            // 
            // TradesView
            // 
            this.TradesView.AllowUserToAddRows = false;
            this.TradesView.AllowUserToDeleteRows = false;
            this.TradesView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.TradesView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.TradesView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TradesView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.TradesView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TradesView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.TradesView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TradesView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.Time,
            this.Symbol,
            this.Side,
            this.xSize,
            this.xPrice,
            this.Comment,
            this.ColAccount});
            this.TradesView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TradesView.EnableHeadersVisualStyles = false;
            this.TradesView.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.TradesView.Location = new System.Drawing.Point(4, 5);
            this.TradesView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TradesView.Name = "TradesView";
            this.TradesView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.TradesView.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.TradesView.RowHeadersVisible = false;
            this.TradesView.RowTemplate.Height = 24;
            this.TradesView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TradesView.ShowEditingIcon = false;
            this.TradesView.Size = new System.Drawing.Size(924, 666);
            this.TradesView.TabIndex = 0;
            // 
            // Date
            // 
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Time
            // 
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            // 
            // Symbol
            // 
            this.Symbol.HeaderText = "Symbol";
            this.Symbol.Name = "Symbol";
            this.Symbol.ReadOnly = true;
            // 
            // Side
            // 
            this.Side.HeaderText = "Side";
            this.Side.Name = "Side";
            this.Side.ReadOnly = true;
            // 
            // xSize
            // 
            this.xSize.HeaderText = "Size";
            this.xSize.Name = "xSize";
            this.xSize.ReadOnly = true;
            // 
            // xPrice
            // 
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = null;
            this.xPrice.DefaultCellStyle = dataGridViewCellStyle6;
            this.xPrice.HeaderText = "Price";
            this.xPrice.Name = "xPrice";
            this.xPrice.ReadOnly = true;
            // 
            // Comment
            // 
            this.Comment.HeaderText = "Comment";
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            // 
            // ColAccount
            // 
            this.ColAccount.HeaderText = "Account";
            this.ColAccount.Name = "ColAccount";
            this.ColAccount.ReadOnly = true;
            // 
            // Settings
            // 
            this.Settings.BackColor = System.Drawing.Color.Transparent;
            this.Settings.Controls.Add(this.label2);
            this.Settings.Controls.Add(this.exchdest);
            this.Settings.Controls.Add(this.label1);
            this.Settings.Controls.Add(this.accountname);
            this.Settings.Controls.Add(this.resetsetbut);
            this.Settings.Controls.Add(this.restoredefaultsbut);
            this.Settings.Controls.Add(this.saveSettingsbut);
            this.Settings.ForeColor = System.Drawing.SystemColors.MenuText;
            this.Settings.Location = new System.Drawing.Point(4, 4);
            this.Settings.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Settings.Name = "Settings";
            this.Settings.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Settings.Size = new System.Drawing.Size(932, 676);
            this.Settings.TabIndex = 1;
            this.Settings.Text = "Settings";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 20);
            this.label2.TabIndex = 25;
            this.label2.Text = "Exchange:";
            // 
            // exchdest
            // 
            this.exchdest.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quotopia.Properties.Settings.Default, "exchangedest", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.exchdest.Location = new System.Drawing.Point(99, 150);
            this.exchdest.Name = "exchdest";
            this.exchdest.Size = new System.Drawing.Size(176, 26);
            this.exchdest.TabIndex = 24;
            this.exchdest.Text = global::Quotopia.Properties.Settings.Default.exchangedest;
            this.exchdest.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 23;
            this.label1.Text = "Account:";
            // 
            // accountname
            // 
            this.accountname.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Quotopia.Properties.Settings.Default, "accountname", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.accountname.Location = new System.Drawing.Point(99, 118);
            this.accountname.Name = "accountname";
            this.accountname.Size = new System.Drawing.Size(176, 26);
            this.accountname.TabIndex = 22;
            this.accountname.Text = global::Quotopia.Properties.Settings.Default.accountname;
            this.accountname.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // resetsetbut
            // 
            this.resetsetbut.Location = new System.Drawing.Point(108, 20);
            this.resetsetbut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.resetsetbut.Name = "resetsetbut";
            this.resetsetbut.Size = new System.Drawing.Size(89, 35);
            this.resetsetbut.TabIndex = 6;
            this.resetsetbut.Text = "Discard";
            this.resetsetbut.UseVisualStyleBackColor = true;
            this.resetsetbut.Click += new System.EventHandler(this.resetsetbut_Click);
            // 
            // restoredefaultsbut
            // 
            this.restoredefaultsbut.Location = new System.Drawing.Point(206, 20);
            this.restoredefaultsbut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.restoredefaultsbut.Name = "restoredefaultsbut";
            this.restoredefaultsbut.Size = new System.Drawing.Size(90, 35);
            this.restoredefaultsbut.TabIndex = 5;
            this.restoredefaultsbut.Text = "Defaults";
            this.restoredefaultsbut.UseVisualStyleBackColor = true;
            this.restoredefaultsbut.Click += new System.EventHandler(this.restoredefaultsbut_Click);
            // 
            // saveSettingsbut
            // 
            this.saveSettingsbut.Location = new System.Drawing.Point(9, 20);
            this.saveSettingsbut.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.saveSettingsbut.Name = "saveSettingsbut";
            this.saveSettingsbut.Size = new System.Drawing.Size(91, 35);
            this.saveSettingsbut.TabIndex = 4;
            this.saveSettingsbut.Text = "Save";
            this.saveSettingsbut.UseVisualStyleBackColor = true;
            this.saveSettingsbut.Click += new System.EventHandler(this.saveSettingsbut_Click);
            // 
            // LogTab
            // 
            this.LogTab.BackColor = System.Drawing.Color.Transparent;
            this.LogTab.Controls.Add(this.statusWindow);
            this.LogTab.Location = new System.Drawing.Point(4, 4);
            this.LogTab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LogTab.Name = "LogTab";
            this.LogTab.Size = new System.Drawing.Size(932, 676);
            this.LogTab.TabIndex = 2;
            this.LogTab.Text = "Messages";
            this.LogTab.UseVisualStyleBackColor = true;
            // 
            // statusWindow
            // 
            this.statusWindow.AutoWordSelection = true;
            this.statusWindow.BackColor = System.Drawing.SystemColors.Window;
            this.statusWindow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.statusWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusWindow.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusWindow.Location = new System.Drawing.Point(0, 0);
            this.statusWindow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusWindow.Name = "statusWindow";
            this.statusWindow.ReadOnly = true;
            this.statusWindow.Size = new System.Drawing.Size(932, 676);
            this.statusWindow.TabIndex = 0;
            this.statusWindow.Text = "";
            // 
            // OrderUserCol
            // 
            this.OrderUserCol.HeaderText = "Comment";
            this.OrderUserCol.Name = "OrderUserCol";
            this.OrderUserCol.ReadOnly = true;
            // 
            // OrderStopCol
            // 
            this.OrderStopCol.HeaderText = "Stop";
            this.OrderStopCol.Name = "OrderStopCol";
            this.OrderStopCol.ReadOnly = true;
            // 
            // OrderPriceCol
            // 
            this.OrderPriceCol.HeaderText = "Price";
            this.OrderPriceCol.Name = "OrderPriceCol";
            this.OrderPriceCol.ReadOnly = true;
            // 
            // OrderSizeCol
            // 
            this.OrderSizeCol.HeaderText = "Size";
            this.OrderSizeCol.Name = "OrderSizeCol";
            this.OrderSizeCol.ReadOnly = true;
            // 
            // OrderSideCol
            // 
            this.OrderSideCol.HeaderText = "Symbol";
            this.OrderSideCol.Name = "OrderSideCol";
            this.OrderSideCol.ReadOnly = true;
            // 
            // OrderTimeCol
            // 
            this.OrderTimeCol.HeaderText = "Time";
            this.OrderTimeCol.Name = "OrderTimeCol";
            this.OrderTimeCol.ReadOnly = true;
            // 
            // OrderDateCol
            // 
            this.OrderDateCol.HeaderText = "Date";
            this.OrderDateCol.Name = "OrderDateCol";
            this.OrderDateCol.ReadOnly = true;
            // 
            // Quote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(808, 379);
            this.Controls.Add(this.quoteTab);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Quotopia.Properties.Settings.Default, "location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ForeColor = System.Drawing.SystemColors.Window;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::Quotopia.Properties.Settings.Default.location;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Quote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Quotopia";
            this.quoteTab.ResumeLayout(false);
            this.Markets.ResumeLayout(false);
            this.Markets.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ordertab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ordergrid)).EndInit();
            this.TradeTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TradesView)).EndInit();
            this.Settings.ResumeLayout(false);
            this.Settings.PerformLayout();
            this.LogTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl quoteTab;
        private System.Windows.Forms.TabPage Markets;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.TabPage LogTab;
        private System.Windows.Forms.TabPage TradeTab;
        private System.Windows.Forms.DataGridView TradesView;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderUserCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderStopCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderPriceCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderSizeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderSideCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderTimeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderDateCol;
        private System.Windows.Forms.Button saveSettingsbut;
        private System.Windows.Forms.Button restoredefaultsbut;
        private System.Windows.Forms.Button resetsetbut;
        private System.Windows.Forms.RichTextBox statusWindow;
        private System.Windows.Forms.TabPage ordertab;
        private System.Windows.Forms.DataGridView ordergrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn oid;
        private System.Windows.Forms.DataGridViewTextBoxColumn osymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn oside;
        private System.Windows.Forms.DataGridViewTextBoxColumn osize;
        private System.Windows.Forms.DataGridViewTextBoxColumn oprice;
        private System.Windows.Forms.DataGridViewTextBoxColumn ostop;
        private System.Windows.Forms.DataGridViewTextBoxColumn oaccount;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statuslab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox accountname;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox exchdest;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Symbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn Side;
        private System.Windows.Forms.DataGridViewTextBoxColumn xSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn xPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColAccount;
    }
}
