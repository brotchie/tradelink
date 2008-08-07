using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TradeLib;

namespace TimeSales
{
    public partial class TnS : Form
    {
        public TnS()
        {
            
            InitializeComponent();
            tsgrid.RowHeadersVisible = false;
            tsgrid.ShowEditingIcon = false;
            tsgrid.ColumnHeadersVisible = true;
            tsgrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tsgrid.Columns.Add("Time", "Time");
            tsgrid.Columns.Add("Trade", "Trade");
            tsgrid.Columns.Add("TSize", "TSize");
            tsgrid.Columns.Add("Bid", "Bid");
            tsgrid.Columns.Add("Ask", "Ask");
            tsgrid.Columns.Add("BSize", "BSize");
            tsgrid.Columns.Add("ASize", "ASize");
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.WorkerReportsProgress = true;
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            this.Shown +=new EventHandler(toolStripButton1_Click);
            status("Click 'Open' to load time and sales.    " + Util.TLSIdentity());

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            if (autoresizebut.Checked)
            {
                status("Resizing columns, please wait...");
                tsgrid.AutoResizeColumns();
            }
            if (e.Error != null)
                status(headline + e.Error.ToString());
            else if (e.Cancelled)
                status(headline + " (load canceled)");
            else
                status(headline);
        }
        string symbol = "";
        int date = 0;
        BackgroundWorker bw = new BackgroundWorker();
        int total = 0;
        string headline { get { return symbol + " on " + date + " "; } }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Title = "Select the tick file you wish to view";
            if (Directory.Exists(Util.TLTickDir))
                od.InitialDirectory = Util.TLTickDir;
            od.Multiselect = false;
            od.Filter = "Time & Sales |*.EPF";
            od.DefaultExt = "*.epf";
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            if (od.ShowDialog() == DialogResult.OK)
            {
                this.Refresh();
                LoadEPF(od.FileName);
                od.Dispose();
            }

        }

        void LoadEPF(string file)
        {
            StreamReader sr = new StreamReader(file);
            Stock s = eSigTick.InitEpf(sr);
            total = 0;
            symbol = s.Symbol;
            date = s.Date;
            FileInfo fi = new FileInfo(file);
            total = (int)Math.Ceiling((decimal)fi.Length / 39);
            bw.RunWorkerAsync(sr);


        }

        void status(string message)
        {
            if (toolStrip1.InvokeRequired)
                toolStrip1.Invoke(new DebugDelegate(status), new object[] { message });
            else
                selected.Text = message;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            status(headline+"(loading " + e.ProgressPercentage + "%)");
            toolStripProgressBar1.Value = e.ProgressPercentage;
            
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            StreamReader sr = (StreamReader)e.Argument;
            tsgrid.Rows.Clear();
            int line = 0;
            while (!sr.EndOfStream)
            {
                if (bw.CancellationPending)
                    break;
                line++;
                NewTick(eSigTick.FromStream(symbol,sr));
                bw.ReportProgress((int)(100*line / (decimal)total));
            }
            status(headline + " (cleaning up)");
            sr.Close();
           
            
        }

        delegate void TickCallback(Tick t);
        void NewTick(Tick t)
        {
            string time = string.Format("{0}:{1:00}",t.time,t.sec);
            string trade = "";
            string bid = "";
            string ask = "";
            string ts = "";
            string bs = "";
            string os = "";
            if (t.trade!=0) trade = t.trade.ToString("N2");
            if (t.size!=0) ts = t.size.ToString();
            if (t.bid != 0) bid = t.bid.ToString("N2");
            if (t.ask != 0) ask = t.ask.ToString("N2");
            if (t.bs != 0) bs = t.bs.ToString();
            if (t.os != 0) os = t.os.ToString();
            if (tsgrid.InvokeRequired)
            {
                try
                {
                    tsgrid.Invoke(new TickDelegate(NewTick), new object[] { t });
                }
                catch (Exception) { }
            }
            else tsgrid.Rows.Add(time,trade, ts,bid,ask,bs,os); 
        }

        private void autoresizebut_CheckedChanged(object sender, EventArgs e)
        {
            status("Resizing columns, please wait...");
            tsgrid.AutoResizeColumns();
            status(headline);
        }


        

        
    }
}