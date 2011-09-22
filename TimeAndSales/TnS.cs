using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace TimeSales
{
    public partial class TnS : AppTracker
    {
        public const string PROGRAM = "Time&Sales";
        DataTable _dt = new DataTable();
        DataGridView _dg = new DataGridView();
        SafeBindingSource _bs = new SafeBindingSource();
        int _dp = Properties.Settings.Default.DecimalPlaces;
        string _dpf = "N2";

        public TnS()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            initgrid();

            _dpf = "N" + _dp;
            SetColumnContext();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            if (Environment.GetCommandLineArgs().Length == 1)
                this.Shown += new EventHandler(toolStripButton1_Click);
            else if (System.IO.File.Exists(Environment.GetCommandLineArgs()[1]))
                LoadEPF(Environment.GetCommandLineArgs()[1]);
            status("Click 'Open' to load time and sales.    " + Util.TLSIdentity());

        }
        const string TIME = "Time";
        const string TRADE = "Trade";
        const string TSIZE = "TSize";
        const string TX = "TExch";
        const string BID = "Bid";
        const string BSIZE = "BSize";
        const string BX = "BExch";
        const string ASK = "Ask";
        const string ASIZE = "ASize";
        const string AX = "AExch";
        const string DP = "Depth";

        void initgrid()
        {
            _dt.Columns.Add(TIME);
            _dt.Columns.Add(TRADE);
            _dt.Columns.Add(TSIZE);
            _dt.Columns.Add(TX);
            _dt.Columns.Add(BID);
            _dt.Columns.Add(ASK);
            _dt.Columns.Add(BSIZE);
            _dt.Columns.Add(ASIZE);
            _dt.Columns.Add(BX);
            _dt.Columns.Add(AX);
            _dt.Columns.Add(DP);
            _bs.DataSource = _dt;
            _dg.DataSource = _bs;
            _dg.AllowUserToAddRows = false;
            _dg.AllowUserToDeleteRows = false;
            _dg.AllowUserToOrderColumns = true;
            _dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _dg.Location = new System.Drawing.Point(0, 0);
            _dg.Margin = new System.Windows.Forms.Padding(4);
            _dg.Name = "tsgrid";
            _dg.ReadOnly = true;
            _dg.RowTemplate.Height = 24;
            _dg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _dg.Size = new System.Drawing.Size(827, 322);
            _dg.TabIndex = 0;

            _dg.RowHeadersVisible = false;
            _dg.ShowEditingIcon = false;
            _dg.ColumnHeadersVisible = true;
            _dg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            _dg.Parent = this;
            _dg.Dock = DockStyle.Fill;
        }



        void SetColumnContext()
        {
            _dg.ContextMenuStrip = new ContextMenuStrip();
            for (int i = 0; i<_dg.Columns.Count; i++)
            {
                bool grey = !_dg.Columns[i].Visible;
                string col = _dg.Columns[i].HeaderText;
                _dg.ContextMenuStrip.Items.Add(col,null,ToggleCol);
            }
            
        }

        void ToggleCol(object sender, EventArgs e)
        {
            string col = ((ToolStripItem)sender).Text;
            if (!_dg.Columns.Contains(col)) return;
            _dg.Columns[col].Visible = !_dg.Columns[col].Visible;
            _dg.Refresh();
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (autoresizebut.Checked)
            {
                status("Resizing columns, please wait...");
                _dg.AutoResizeColumns();
            }
            if (e.Error != null) ;
            else if (!e.Cancelled)
                status(headline);
            SafeBindingSource.refreshgrid(_dg, _bs, true);
        }
        string symbol = "";
        int date = 0;
        BackgroundWorker bw = new BackgroundWorker();
        int total = 0;
        string headline { get { return symbol + " on " + date + " "; } }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
            {
                status("close or wait till load complete.");
                return;
            }
            OpenFileDialog od = new OpenFileDialog();
            od.Title = "Browse to TickData";
            od.Multiselect = false;
            od.Filter = "Time & Sales |"+TikConst.WILDCARD_EXT;
            od.DefaultExt = TikConst.WILDCARD_EXT;
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            if (od.ShowDialog() == DialogResult.OK)
            {
                string file = od.FileName;
                LoadEPF(file);
            }

        }

        void LoadEPF(string file)
        {
            SecurityImpl s;
            try
            {
                _dt.Clear();
                s = SecurityImpl.FromTIK(file);
                if (s.Type == SecurityType.CASH)
                    _dp = 5;
                total = s.ApproxTicks;
                symbol = s.Symbol;
                date = s.Date;
            }
            catch (Exception ex) { status("Error.  Is file closed?"); return; }
            if (!bw.IsBusy)
                bw.RunWorkerAsync(s);
            else 
                status("try again.");


        }

        void status(string message)
        {
            if (toolStrip1.InvokeRequired)
                toolStrip1.Invoke(new DebugDelegate(status), new object[] { message });
            else
            {
                selected.Text = message;
                selected.Invalidate();
            }
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            status(headline+"(loading " + e.ProgressPercentage + "%)");
            toolStripProgressBar1.Value = e.ProgressPercentage > 100 ? 100 : e.ProgressPercentage;
            
        }

        int line;

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            SafeBindingSource.refreshgrid(_dg, _bs, false);
            SecurityImpl s = (SecurityImpl)e.Argument;
            s.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
            line = 0;
            while (s.HistSource.NextTick() && !bw.CancellationPending)
                ;
            
            status(headline + " (cleaning up)");
            s.HistSource.Close();
            
        }

        void HistSource_gotTick(Tick t)
        {
            
            line++;
            DateTime d = Util.FT2DT(t.time);
            string time = d.ToString("HH:mm:ss");
            string trade = "";
            string bid = "";
            string ask = "";
            string ts = "";
            string bs = "";
            string os = "";
            string be = "";
            string oe = "";
            string ex = "";
            string depth = "";
            if (t.isIndex)
            {
                trade = t.trade.ToString(_dpf);
            }
            else if (t.isTrade)
            {
                trade = t.trade.ToString(_dpf);
                ts = t.size.ToString();
                ex = t.ex;
            }
            if (t.hasBid)
            {
                bs = t.bs.ToString();
                be = t.be;
                bid = t.bid.ToString(_dpf);
                depth = t.depth.ToString();
            }
            if (t.hasAsk)
            {
                ask = t.ask.ToString(_dpf);
                oe = t.oe;
                os = t.os.ToString();
                depth = t.depth.ToString();
            }

            _dt.Rows.Add(time, trade, ts, ex, bid, ask, bs, os, be, oe,depth);
            if (Math.Abs(_dg.FirstDisplayedScrollingRowIndex-_dt.Rows.Count)<100)
                SafeBindingSource.refreshgrid(_dg, _bs,false);
            int per = (int)(100 * line / (double)total);
            if (per % 5 == 0)
                bw.ReportProgress(per);
           
        }


        private void autoresizebut_CheckedChanged(object sender, EventArgs e)
        {
            status("Resizing columns, please wait...");
            _dg.AutoResizeColumns();
            status(headline);
        }


        

        
    }
}