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
            tsgrid.Columns.Add("Sec", "Sec");
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

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsgrid.AutoResizeColumns();
        }
        BackgroundWorker bw = new BackgroundWorker();
        int total = 0;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.InitialDirectory = "c:\\program files\\tradelink\\tickdata\\";
            od.Multiselect = false;
            od.Filter = "Time & Sales |*.EPF";
            od.DefaultExt = "*.epf";
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            od.ShowDialog();
            this.Refresh();
            LoadEPF(od.FileName);
            od.Dispose();

        }

        void LoadEPF(string file)
        {
            StreamReader sr = new StreamReader(file);
            Stock s = eSigTick.InitEpf(sr);
            total = 0;
            selected.Text = s.Symbol + " " + s.Date;
            FileInfo fi = new FileInfo(file);
            total = (int)Math.Ceiling((decimal)fi.Length / 40);
            bw.RunWorkerAsync(sr);


        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            StreamReader sr = (StreamReader)e.Argument;
            tsgrid.Rows.Clear();
            int line = 0;
            while (!sr.EndOfStream)
            {
                line++;
                eSigTick t = new eSigTick();
                t.Load(sr.ReadLine());
                NewTick(t);
                bw.ReportProgress((int)(100*line / total));
            }
            sr.Close();
            e.Result = true;
            
            
        }

        delegate void TickCallback(Tick t);
        void NewTick(Tick t)
        {
            if (tsgrid.InvokeRequired)
            {
                TickCallback d = new TickCallback(NewTick);
                this.Invoke(d, new object[] { t });
            }
            else tsgrid.Rows.Add(t.time,t.sec, t.trade, t.size, t.bid, t.ask, t.bs, t.os); 
        }


        

        
    }
}