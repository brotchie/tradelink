using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using TradeLink.API;

namespace TimeSales
{
    public partial class TnS : Form
    {
        public const string PROGRAM = "Time&Sales";
        DataTable _dt = new DataTable();
        DataGridView _dg = new DataGridView();
        SafeBindingSource _bs = new SafeBindingSource();

        public TnS()
        {
            
            InitializeComponent();
            initgrid();
            

            SetColumnContext();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            this.Shown +=new EventHandler(toolStripButton1_Click);
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
            od.Title = "TickFiles in: "+Util.TLTickDir;
            od.Multiselect = false;
            od.Filter = "Time & Sales |"+TikConst.WILDCARD_EXT;
            od.DefaultExt = TikConst.WILDCARD_EXT;
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            if (od.ShowDialog() == DialogResult.OK)
            {
                _dt.Clear();
                SafeBindingSource.refreshgrid(_dg, _bs, false);
                LoadEPF(od.FileName);
                
                od.Dispose();
            }

        }

        void LoadEPF(string file)
        {
            SecurityImpl s = SecurityImpl.FromTIK(file);
            total = s.ApproxTicks;
            symbol = s.Symbol;
            date = s.Date;
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
            string time = t.time.ToString();
            string trade = "";
            string bid = "";
            string ask = "";
            string ts = "";
            string bs = "";
            string os = "";
            string be = "";
            string oe = "";
            string ex = "";
            if (t.isIndex)
            {
                trade = t.trade.ToString("N2");
            }
            else if (t.isTrade)
            {
                trade = t.trade.ToString("N2");
                ts = t.size.ToString();
                ex = t.ex;
            }
            if (t.hasBid)
            {
                bs = t.bs.ToString();
                be = t.be;
                bid = t.bid.ToString("N2");
            }
            if (t.hasAsk)
            {
                ask = t.ask.ToString("N2");
                oe = t.oe;
                os = t.os.ToString();
            }

            _dt.Rows.Add(time, trade, ts, ex, bid, ask, bs, os, be, oe);
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