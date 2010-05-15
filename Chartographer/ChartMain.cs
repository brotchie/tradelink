using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using TradeLink.Common;
using System.Net;
using TradeLink.API;
using TradeLink.AppKit;

namespace Chartographer
{
    public partial class ChartMain : AppTracker
    {
        public event BarListDelegate newChartData;
        Dictionary<string, BarList> blbox = new Dictionary<string, BarList>();
        public const string PROGRAM = "Chartographer";

        public ChartMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
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

        private void ChartMain_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string f = s[0];
            if (!f.Contains(TikConst.DOT_EXT)) return;
            BarList bl = BarListImpl.FromTIK(f);
            newChart(bl);
        }

        private void ChartMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        void c_Move(object sender, EventArgs e)
        {
            if (stickychartsbox.Checked) Chartographer.Properties.Settings.Default.chartstart = ((Form)sender).Location;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            chartsymbolbox.Text = chartsymbolbox.Text.ToUpper();
            downloaddata(chartsymbolbox.Text);
        }

        private void downloaddata(string symbol)
        {
            downloaddata(symbol, true);
        }

        private void downloaddata(string sym, bool newChart)
        {
            bool r = BarListImpl.DayFromGoogleAsync(sym, new BarListDelegate(gotchart));
        }

        void gotchart(BarList bl)
        {
            newChart(bl);
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

        bool _uselast = Properties.Settings.Default.ChartLast;
        bool _usebid = Properties.Settings.Default.ChartNoLastUseBid;

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.CheckFileExists = true;
            od.CheckPathExists = true;
            od.DefaultExt = TikConst.WILDCARD_EXT;
            od.Filter = "TickFiles|" + TikConst.WILDCARD_EXT;
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                BarList bl = BarListImpl.FromTIK(od.FileName,_uselast,_usebid);
                newChart(bl);
            }

        }

        void newChart(BarList bl)
        {
            if (!blbox.ContainsKey(bl.Symbol))
                blbox.Add(bl.Symbol, bl);
            else blbox[bl.Symbol] = bl;

            Chart c = new Chart(bl, false);
            c.Symbol = bl.Symbol;
            try
            {
                c.StartPosition = FormStartPosition.Manual;
                c.Location = Chartographer.Properties.Settings.Default.chartstart;
            }
            catch (NullReferenceException) { }
            newChartData += new BarListDelegate(c.NewBarList);
            c.Move += new EventHandler(c_Move);
            c.Icon = Chartographer.Properties.Resources.chart;
            if (maxchartbox.Checked) c.WindowState = FormWindowState.Maximized;
            if (blackbackground.Checked) c.chartControl1.BackColor = Color.Black;
            c.Show();
        }

        private void chartsymbolbox_Click(object sender, EventArgs e)
        {
            chartsymbolbox.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void chartsymbolbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1_Click(null, null);
        }
    }
}