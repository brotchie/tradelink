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
    public partial class TattleMain : Form
    {
        DataTable dt = new DataTable("results");
        DataGridView dg = new DataGridView();

        FileSystemWatcher fw;
        const string FID = "Trades.csv";
        public const string PROGRAM = "Tattle";
        public TattleMain()
        {
            InitializeComponent();

            dt.Columns.Add("Stat");
            dt.Columns.Add("Result");
            dg.RowHeadersVisible = false;
            dg.DataSource = dt;
            dg.Parent = splitContainer1.Panel2;
            dg.Dock = DockStyle.Fill;
            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            dg.ReadOnly = true;
            dg.BackColor = Color.White;
            dg.AutoGenerateColumns = true;
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dg.ColumnHeadersDefaultCellStyle.BackColor = dg.BackColor;
            dg.ColumnHeadersDefaultCellStyle.ForeColor = dg.ForeColor;
            dg.BackgroundColor = Color.White;
            dg.Font = new Font(FontFamily.GenericSansSerif, 10);
            WatchPath();
            BackColor = Color.White;
            splitContainer1.Panel2.BackColor = Color.White;
            tradefiles.SelectedIndexChanged += new EventHandler(tradefiles_SelectedIndexChanged);
            Text = "Tattle " + Util.TLVersion();
            refreshgrid();
            MouseEnter += new EventHandler(TattleMain_MouseEnter);
        }

        void TattleMain_MouseEnter(object sender, EventArgs e)
        {
            dg.AutoResizeColumnHeadersHeight();
        }

        void WatchPath() { WatchPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal)); }
        void WatchPath(string path)
        {
            fw = new FileSystemWatcher(path, "*"+FID);
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
            FileInfo[] fis = di.GetFiles("*" + FID);
            tradefiles.Items.Clear();
            int newest = 0;
            foreach (FileInfo fi in fis)
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
            else if (name.Contains(FID))
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
                if (!name.Contains(FID)) return;
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
            StreamReader sr ;








            Results r = new Results();
            try
            {
                sr = new StreamReader(fw.Path + @"\" + name);
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    TradeResult tr = TradeResult.Init(sr.ReadLine());
                    if (!r.Symbols.Contains(tr.Source.symbol))
                        r.Symbols += tr.Source.symbol + ",";
                    r.Trades++;
                    r.HundredLots += (int)(tr.Source.xsize / 100);
                    r.GrossPL += tr.ClosedPL;
                    if (tr.ClosedPL > 0) r.Winners++;
                    if (tr.ClosedPL < 0) r.Losers++;
                    if ((tr.OpenSize == 0) && (tr.ClosedPL == 0)) r.Flats++;
                    if (tr.ClosedPL > r.MaxWin) r.MaxWin = tr.ClosedPL;
                    if (tr.ClosedPL < r.MaxLoss) r.MaxLoss = tr.ClosedPL;
                    if (tr.OpenPL > r.MaxOpenWin) r.MaxOpenWin = tr.OpenPL;
                    if (tr.OpenPL < r.MaxOpenLoss) r.MaxOpenLoss = tr.OpenPL;
                }
                sr.Close();
            }
            catch (Exception) {  }
            return r;

        }



        void DisplayResults(Results r)
        {
            dt.BeginLoadData();
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
            dt.EndLoadData();
            refreshgrid();
        }

        void refreshgrid()
        {
            dg.Invalidate();
        }
   }



    public class Results
    {
        public string Symbols = "";
        public decimal GrossPL = 0;
        public string NetPL { get { return v2s(GrossPL - (HundredLots * 100 * ComPerShare)); } }
        public int Winners = 0;
        public int Losers = 0;
        public int Flats = 0;
        public decimal MaxWin =0 ;
        public decimal MaxLoss = 0;
        public decimal MaxOpenWin=0;
        public decimal MaxOpenLoss=0;
        public int HundredLots=0;
        public int Trades=0;
        public decimal ComPerShare = 0.01m;
        public string Commissions { get { return v2s(HundredLots * 100 * ComPerShare); } }
        string v2s(decimal v) { return v.ToString("N2"); }
        public string WLRatio { get { return v2s((Losers==0) ? 0 : (Winners/Losers)); }}
        public string GrossMargin { get { return v2s((GrossPL == 0) ? 0 : ((GrossPL - (HundredLots * 100 * ComPerShare)) / GrossPL)); } }
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
