using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TradeLink.Common;
using System.Net;
using TradeLink.API;

namespace Chartographer
{
    public partial class ChartMain : Form
    {
        public event BarListUpdated newChartData;
        WebClient client = new WebClient();
        Dictionary<string, BarListImpl> blbox = new Dictionary<string, BarListImpl>();
        public ChartMain()
        {
            InitializeComponent();
            stickychartsbox.Checked = Chartographer.Properties.Settings.Default.stickychartson;
            maxchartbox.Checked = Chartographer.Properties.Settings.Default.maxcharts;
            try
            {
                StartPosition = FormStartPosition.Manual;
                Location = Chartographer.Properties.Settings.Default.startwindow;
                
            }
            catch (NullReferenceException) { }
            this.Move += new EventHandler(Form1_Move);
            Chartographer.Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            Text = "Chart " + Util.TLVersion();
        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Chartographer.Properties.Settings.Default.Save();
        }

        void Form1_Move(object sender, EventArgs e)
        {
            Chartographer.Properties.Settings.Default.startwindow = Location;
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null) return;
            fetchstate f = (fetchstate)e.UserState;
            Security s = f.ChartStock;
            if (!blbox.ContainsKey(s.Symbol)) blbox.Add(s.Symbol, BarListImpl.FromCSV(s.Symbol,e.Result));
            else blbox[s.Symbol] = BarListImpl.FromCSV(s.Symbol,e.Result);
            if (f.NewChart)
            {
                ChartImpl c = new ChartImpl(blbox[s.Symbol], true);
                c.Symbol = s.Symbol;
                try
                {
                    c.StartPosition = FormStartPosition.Manual;
                    c.Location = Chartographer.Properties.Settings.Default.chartstart;
                }
                catch (NullReferenceException) { }
                newChartData += new BarListUpdated(c.NewBarList);
                c.FetchStock += new SecurityDelegate(c_FetchStock);
                c.Move += new EventHandler(c_Move);
                c.Icon = Chartographer.Properties.Resources.chart;
                if (maxchartbox.Checked) c.WindowState = FormWindowState.Maximized;
                if (blackbackground.Checked) c.BackColor = Color.Black;
                c.Show();
            }
            else if (newChartData != null) newChartData(blbox[s.Symbol]);
            
        }

        void c_FetchStock(Security stock)
        {
            downloaddata(stock,false);
        }

        void c_Move(object sender, EventArgs e)
        {
            if (stickychartsbox.Checked) Chartographer.Properties.Settings.Default.chartstart = ((Form)sender).Location;
        }
        const string GOOGURL = "http://finance.google.com/finance/historical?histperiod=daily&start=250&num=25&output=csv&q=";


        private void chartsymbolbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chartsymbolbox.Text = chartsymbolbox.Text.ToUpper();
            downloaddata(new SecurityImpl(chartsymbolbox.Text));
        }

        private void downloaddata(Security s)
        {
            downloaddata(s, true);
        }

        private void downloaddata(Security s, bool newChart)
        {
            if (!s.isValid) return;
            Uri goog = new Uri(GOOGURL + s.Symbol);
            fetchstate f = new fetchstate(s, newChart);
            try
            {
                client.DownloadStringAsync(goog, f);
            }
            catch (WebException) { return; }
        }

        private void stickychartsbox_CheckedChanged(object sender, EventArgs e)
        {
            Chartographer.Properties.Settings.Default.stickychartson = stickychartsbox.Checked;
        }

        private void maxchartbox_CheckedChanged(object sender, EventArgs e)
        {
            Chartographer.Properties.Settings.Default.maxcharts = maxchartbox.Checked;

        }

        private void blackbackground_CheckedChanged(object sender, EventArgs e)
        {
            Chartographer.Properties.Settings.Default.blackchartbg = blackbackground.Checked;
        }

        public class fetchstate
        {
            Security s;
            bool newwind = true;
            public Security ChartStock { get { return s; } }
            public bool NewChart { get { return newwind; } }
            public fetchstate(Security stock, bool NewChart) { s = stock; newwind = NewChart; }
            public fetchstate(Security stock) : this(stock, true) { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            od.DefaultExt = "*.EPF";
            od.Filter = "TickFiles|*.EPF";
            od.InitialDirectory = "c:\\program files\\tradelink\\tickdata\\";
            od.Multiselect = false;
            od.ShowDialog();
            BarList bl = BarListImpl.FromEPF(od.FileName);
            ChartImpl c = new ChartImpl(bl, false);
            c.Symbol = bl.Symbol;
            try
            {
                c.StartPosition = FormStartPosition.Manual;
                c.Location = Chartographer.Properties.Settings.Default.chartstart;
            }
            catch (NullReferenceException) { }
            newChartData += new BarListUpdated(c.NewBarList);
            c.Move += new EventHandler(c_Move);
            c.Icon = Chartographer.Properties.Resources.chart;
            if (maxchartbox.Checked) c.WindowState = FormWindowState.Maximized;
            if (blackbackground.Checked) c.BackColor = Color.Black;
            c.Show();
        }

        private void chartsymbolbox_Click(object sender, EventArgs e)
        {
            chartsymbolbox.Clear();
        }
    }
}