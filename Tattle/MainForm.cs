using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using System.Reflection;
using TradeLink.API;

namespace Tattle
{
    public partial class MainForm : Form
    {
        DataTable dt = new DataTable("results");
        DataGrid dg = new DataGrid();
        const string fid = "Gauntlet.Trades";
        FileSystemWatcher fw;
        public MainForm()
        {
            InitializeComponent();

            dt.Columns.Add("Stat");
            dt.Columns.Add("Result");
            dg.RowHeadersVisible = false;
            dg.DataSource = dt;
            dg.Parent = splitContainer1.Panel2;
            dg.Dock = DockStyle.Fill;
            dg.ReadOnly = true;
            dg.BackColor = Color.White;
            dg.HeaderBackColor = dg.BackColor;
            dg.HeaderForeColor = dg.ForeColor;
            dg.BackgroundColor = Color.White;
            dg.CaptionVisible = false;
            dg.Font = new Font(FontFamily.GenericSansSerif, 10);
            WatchPath();
            BackColor = Color.White;
            splitContainer1.Panel2.BackColor = Color.White;
            tradefiles.SelectedIndexChanged += new EventHandler(tradefiles_SelectedIndexChanged);
            Text = "Tattle " + Util.TLVersion();
        }

        void WatchPath() { WatchPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal)); }
        void WatchPath(string path)
        {
            fw = new FileSystemWatcher(path, fid+"*.csv");
            fw.IncludeSubdirectories = false;
            fw.EnableRaisingEvents = true;
            fw.Created += new FileSystemEventHandler(fw_Created);
            fw.Renamed += new RenamedEventHandler(fw_Renamed);
            fw.Deleted += new FileSystemEventHandler(fw_Deleted);
            fw.Changed += new FileSystemEventHandler(fw_Changed);
            ResetFiles(path);
        }


        void ResetFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fis = di.GetFiles("*.csv");
            tradefiles.Items.Clear();
            int newest = 0;
            foreach (FileInfo fi in fis)
            {
                if (fi.Name.Contains(fid))
                {
                    tradefiles.Items.Add(fi.Name);
                    System.Text.RegularExpressions.Match datepart =
                        System.Text.RegularExpressions.Regex.Match(fi.Name, "[0-9]{8}", System.Text.RegularExpressions.RegexOptions.None);
                    if (datepart.Success)
                    {
                        int thisdate = Convert.ToInt32(datepart.ToString());
                        if (thisdate > newest)
                        {
                            newest = thisdate;
                            tradefiles.SelectedIndex = tradefiles.Items.Count - 1;
                        }
                    }
                    else tradefiles.SelectedIndex = tradefiles.Items.Count - 1;
                }
            }
            tradefiles_SelectedIndexChanged(null, null);
        }

        void fw_Changed(object sender, FileSystemEventArgs e)
        {
            DisplayResults(FetchResults(e.Name));
            tradefiles.SelectedItem = e.Name;
        }

        void fw_Deleted(object sender, FileSystemEventArgs e)
        {
            remresult(e.Name);
        }

        void fw_Renamed(object sender, RenamedEventArgs e)
        {
            remresult(e.Name);
        }

        void fw_Created(object sender, FileSystemEventArgs e)
        {
            newresult(e.Name);
        }

        void remresult(string name)
        {
            if (tradefiles.InvokeRequired)
                Invoke(new DebugDelegate(remresult), new object[] { name });
            else if (name.Contains(fid))
            {
                tradefiles.Items.Remove(name);
            }
        }

        void newresult(string name)
        {
            if (tradefiles.InvokeRequired)
                Invoke(new DebugDelegate(newresult), new object[] { name });
            else
            {
                if (!name.Contains(fid)) return;
                tradefiles.Items.Add(name);
                tradefiles.SelectedIndex = tradefiles.Items.Count - 1;
                DisplayResults(FetchResults((string)tradefiles.SelectedItem));
            }
        }

        void tradefiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayResults(FetchResults((string)tradefiles.SelectedItem));
        }


        Results FetchResults(string name)
        {
            if (name == null) return new Results();
            StreamReader sr = new StreamReader(fw.Path +@"\"+ name);
            sr.ReadLine();
            Results r = new Results();
            while (!sr.EndOfStream)
            {
                TradeResult tr = TradeResult.Init(sr.ReadLine());
                if (!r.SymbolsTraded.Contains(tr.Source.symbol))
                    r.SymbolsTraded += tr.Source.symbol + ",";
                r.Trades++;
                r.HundredLots += (int)(tr.Source.xsize / 100);
                r.GrossPL += tr.ClosedPL;
                if (tr.ClosedPL>0) r.Winners++;
                if (tr.ClosedPL < 0) r.Losers++;
                if ((tr.OpenSize == 0) && (tr.ClosedPL == 0)) r.Flats++;
                if (tr.ClosedPL > r.MaxWin) r.MaxWin = tr.ClosedPL;
                if (tr.ClosedPL < r.MaxLoss) r.MaxLoss = tr.ClosedPL;
                if (tr.OpenPL > r.MaxOpenWin) r.MaxOpenWin = tr.OpenPL;
                if (tr.OpenPL < r.MaxOpenLoss) r.MaxOpenLoss = tr.OpenPL;
            }
            sr.Close();
            return r;

        }



        void DisplayResults(Results r)
        {
            dt.Clear();
            Type t = r.GetType();
            FieldInfo[] fis = t.GetFields();
            foreach (FieldInfo fi in fis)
            {
                string format = null;
                if (fi.GetType() == typeof(Decimal)) format = "N2";
                dt.Rows.Add(fi.Name, (format!=null) ? string.Format(format,fi.GetValue(r)) : fi.GetValue(r).ToString());
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                string format = null;
                if (pi.GetType() == typeof(Decimal)) format = "N2";
                dt.Rows.Add(pi.Name, (format!=null) ? string.Format(format, pi.GetValue(r,null)) : pi.GetValue(r,null).ToString());
            }
        }
   }

    public class Results
    {
        public string SymbolsTraded = "";
        public decimal GrossPL = 0;
        public decimal NetPL { get { return GrossPL - Commissions; } }
        public int Winners = 0;
        public int Losers = 0;
        public int Flats = 0;
        public decimal MaxWin =0 ;
        public decimal MaxLoss = 0;
        public decimal MaxOpenWin=0;
        public decimal MaxOpenLoss=0;
        public int HundredLots=0;
        public int Trades=0;
        public decimal CommissionPerShare = 0.01m;
        public decimal Commissions { get { return HundredLots * 100 * CommissionPerShare; } }
        public decimal WLRatio { get { return (Losers==0) ? 0 : Winners/Losers; }}
        public decimal GrossMargin { get { return (GrossPL==0) ? 0 : NetPL / GrossPL; } }
    }

    public class TradeResult : TradeImpl
    {
        public Trade Source;
        public decimal OpenPL;
        public decimal ClosedPL;
        public int OpenSize;
        public int ClosedSize;
        public decimal AvgPrice;
        const int s = 7;
        // we're reading these values from file, 
        // bc it's faster than recalculating each time
        public static TradeResult Init(string resultline)
        {
            string[] res = resultline.Split(',');
            TradeResult r = new TradeResult();
            r.Source = TradeImpl.FromString(resultline);
            r.OpenPL = Convert.ToDecimal(res[s]);
            r.ClosedPL = Convert.ToDecimal(res[s + 1]);
            r.OpenSize = Convert.ToInt32(res[s + 2]);
            r.ClosedSize = Convert.ToInt32(res[s + 3]);
            r.AvgPrice = Convert.ToDecimal(res[s + 4]);
            return r;
        }

    }
}