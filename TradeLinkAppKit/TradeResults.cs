using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using TradeLink.Common;
using System.Reflection;
using TradeLink.API;
using System.IO;
using System.Windows.Forms;

namespace TradeLink.AppKit
{
    public partial class TradeResults : UserControl
    {
        DataTable dt = new DataTable("results");
        DataGridView dg = new DataGridView();
        PositionTracker pt = new PositionTracker(100);
        FileSystemWatcher fw;
        public const string RESULTS_POSTFIX = "Trades.csv";

        string _watchpath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        /// <summary>
        /// change path where new files are watched
        /// </summary>
        public string Path { get { return _watchpath; } set { _watchpath = value; } }


        public TradeResults()
        {
            InitializeComponent();
            dt.Columns.Add("Stat");
            dt.Columns.Add("Result");
            dg.RowHeadersVisible = false;
            dg.DataSource = dt;
            dg.Parent = splitContainer1.Panel2;
            dg.Dock = DockStyle.Fill;
            dg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("RiskFreeRate", new EventHandler(changerfr));
            ContextMenu.MenuItems.Add("Comission", new EventHandler(changecomm));
            dg.ReadOnly = true;
            dg.BackColor = Color.White;
            dg.AutoGenerateColumns = true;
            dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dg.ColumnHeadersHeight = (int)(dg.ColumnHeadersDefaultCellStyle.Font.Height*1.5);
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

        decimal _rfr = .001m;
        decimal _comm = .01m;
        void changerfr(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Risk Free Rate: ", "Update RfR", _rfr.ToString("P2"), 0, 0);
            decimal rfr = 0;
            input.Replace("%", "");
            if (decimal.TryParse(input, out rfr))
            {
                _rfr = (rfr / 100);
                tradefiles_SelectedIndexChanged(null, null);
            }
        }

        void changecomm(object s, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Per Contract Comission: ", "Update Comission", _comm.ToString("F3"), 0, 0);
            decimal c = 0;
            if (decimal.TryParse(input, out c))
            {
                _comm = c;
                tradefiles_SelectedIndexChanged(null, null);
            }
        }
        void TattleMain_MouseEnter(object sender, EventArgs e)
        {
            dg.AutoResizeColumnHeadersHeight();
        }

        void WatchPath() { WatchPath(Path); }
        void WatchPath(string path)
        {
            if (!_autowatch) return;
            fw = new FileSystemWatcher(path, "*" + RESULTS_POSTFIX);
            fw.IncludeSubdirectories = false;
            fw.EnableRaisingEvents = true;
            fw.Created += new FileSystemEventHandler(fw_Created);
            fw.Renamed += new RenamedEventHandler(fw_Renamed);
            fw.Deleted += new FileSystemEventHandler(fw_Deleted);
            fw.Changed += new FileSystemEventHandler(fw_Changed);
            ResetFiles(path);
        }

        /// <summary>
        /// clears results
        /// </summary>
        public void Clear()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(Clear));
            else
            {
                dt.Clear();
                tradefiles.Items.Clear();
                _resultlists.Clear();
                refreshgrid();
            }
        }


        /// <summary>
        /// rediscover files in a path
        /// </summary>
        public void ResetFiles() { ResetFiles(Path); }
        public void ResetFiles(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fis = di.GetFiles("*" + RESULTS_POSTFIX);
            foreach (FileInfo fi in fis)
            {
                NewResultFile(fi.Name);
            }
            // display last one
            DisplayResults(tradefiles.SelectedIndex);
        }

        void fw_Changed(object sender, FileSystemEventArgs e)
        {
            if (!_autowatch) return;
            DisplayResults(Results.FetchResults(LoadResults(e.Name),_rfr,_comm,debug));
            tradefiles.SelectedItem = e.Name;
        }

        void fw_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!_autowatch) return;
            remresult(e.Name);
        }

        void fw_Renamed(object sender, RenamedEventArgs e)
        {
            if (!_autowatch) return;
            remresult(e.Name);
        }

        void fw_Created(object sender, FileSystemEventArgs e)
        {
            NewResultFile(e.Name);
        }

        List<List<TradeResult>> _resultlists = new List<List<TradeResult>>();

        /// <summary>
        /// remove a result from list of results 
        /// (if result was loaded from file, does not delete file).
        /// </summary>
        /// <param name="name"></param>
        public void remresult(string name)
        {
            if (tradefiles.InvokeRequired)
                Invoke(new DebugDelegate(remresult), new object[] { name });
            else if (name.Contains(RESULTS_POSTFIX))
            {
                int idx = NameIndex(name);
                _resultlists.RemoveAt(idx);
                tradefiles.Items.RemoveAt(idx);
                refreshgrid();
            }
        }

        /// <summary>
        /// determine if a result's name is unique (adding a result requires a unique name)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool isUniqueName(string name)
        {
            return NameIndex(name) == -1;
        }

        int NameIndex(string name)
        {
            for (int i = 0; i < tradefiles.Items.Count; i++)
                if (tradefiles.Items[i].ToString() == name) return i;
            return -1;
        }

        bool _autowatch = false;
        /// <summary>
        /// determines whether TradeResults will automatically add/remove result
        /// files that show up in 'Path'
        /// </summary>
        public bool AutoWatch { get { return _autowatch; } set { _autowatch = value; } }

        delegate void newresulttradesdel(string name, List<Trade> trades);

        /// <summary>
        /// add a result from list of trades
        /// </summary>
        /// <param name="name"></param>
        /// <param name="trades"></param>
        public void NewResultTrades(string name, List<Trade> trades)
        {
            if (tradefiles.InvokeRequired)
                Invoke(new newresulttradesdel(NewResultTrades), new object[] { name,trades });
            else
            {
                name = System.IO.Path.GetFileNameWithoutExtension(name);
                if (!isUniqueName(name)) return;
                if (trades.Count == 0)
                {
                    debug("No results found for: " + name);
                    return;
                }
                tradefiles.Items.Add(name);
                int idx = tradefiles.Items.Count - 1;
                _resultlists.Add(TradeResult.ResultsFromTradeList(trades));
                tradefiles.SelectedIndex = idx;
            }
        }

        /// <summary>
        /// add a result from a file
        /// </summary>
        /// <param name="filename"></param>
        public void NewResultFile(string filename)
        {
            if (tradefiles.InvokeRequired)
                Invoke(new DebugDelegate(NewResultFile), new object[] { filename });
            else
            {
                if (!isUniqueName(filename)) return;
                List<TradeResult> results = LoadResults(filename);
                if (results.Count == 0)
                {
                    debug("No results found for "+filename);
                    return;
                }
                tradefiles.Items.Add(filename);
                int idx = tradefiles.Items.Count - 1;
                _resultlists.Add(results);
                tradefiles.SelectedIndex = idx;
            }
        }

        void tradefiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tradefiles.SelectedIndex < 0) return;
            DisplayResults(tradefiles.SelectedIndex);
        }


        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }

        public event DebugDelegate SendDebug;

        

        List<TradeResult> LoadResults(string filename)
        {
            if ((filename == null) || !filename.Contains(RESULTS_POSTFIX))  
                return new List<TradeResult>();
            StreamReader sr;
            List<TradeResult> results = new List<TradeResult>(100);
            try
            {
                sr = new StreamReader(_watchpath + @"\" + filename);
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    results.Add(TradeResult.Init(sr.ReadLine()));
                }
                sr.Close();
            }
            catch (Exception) { }
            return results;

        }


        /// <summary>
        /// change selected index
        /// </summary>
        /// <param name="name"></param>
        public void DisplayResults(string name)
        {
            int idx = NameIndex(name) ;
            if (idx== -1) return;
            tradefiles.SelectedIndex = idx;
        }

        void DisplayResults(int idx)
        {
            if (idx == -1) return;
            DisplayResults(Results.FetchResults(_resultlists[idx],_rfr,_comm,debug));
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
                dt.Rows.Add(fi.Name, (format != null) ? string.Format(format, fi.GetValue(r)) : fi.GetValue(r).ToString());
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                string format = null;
                if (pi.GetType() == typeof(Decimal)) format = "N2";
                dt.Rows.Add(pi.Name, (format != null) ? string.Format(format, pi.GetValue(r, null)) : pi.GetValue(r, null).ToString());
            }
            foreach (string ps in r.PerSymbolStats)
            {
                string[] rs= ps.Split(':');
                if (rs.Length != 2) continue;
                dt.Rows.Add(rs[0], rs[1]);
            }
            dt.EndLoadData();
            refreshgrid();
        }

        void refreshgrid()
        {

            dg.Invalidate();

        }
    }


    /// <summary>
    /// track results
    /// </summary>
    public class Results
    {
        public Results() : this(.01m, .01m, 0) { }
        public Results(decimal rfr, decimal com, int reporttime)
        {
            RiskFreeRate = rfr;
            Comission = com;
            ReportTime = reporttime;
        }
        decimal rfr = .01m;
        decimal RiskFreeRate { get { return rfr; } set { rfr = value; } }
        decimal com = .01m;
        decimal Comission { get { return com; } set { com = value; } }
        List<Trade> fills = new List<Trade>();
        /// <summary>
        /// pass fills as they arrive
        /// </summary>
        /// <param name="fill"></param>
        public void GotFill(Trade fill)
        {
            fills.Add(fill);
        }
        /// <summary>
        /// pass new positions as they arrive
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            fills.Add(p.ToTrade());
        }

        int _rt = 0;
        bool sendreport= false;
        bool SendReport { get { return sendreport; } set { sendreport = value; } }
        int ReportTime { get { return _rt; } set { _rt = value; sendreport = (_rt != 0); } }
        bool _livecheck = true;
        bool _islive = false;

        System.Text.StringBuilder _msg;

        public void GotDebug(string msg, bool appendtime)
        {
            if (appendtime)
                _msg.AppendLine(_time + ": " + msg);
            else
                _msg.AppendLine(msg);
        }
        public void GotDebug(string msg)
        {
            _msg.AppendLine(_time + ": " + msg);
        }
        int _time = 0;
        /// <summary>
        /// pass ticks as they arrive (only necessary if using report time)
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            _time = k.time;
            if (_livecheck)
            {
                bool dmatch = k.date == Util.ToTLDate();
                bool tmatch = Util.FTDIFF(k.time, Util.ToTLTime()) < 60;
                _islive = dmatch && tmatch;
                _livecheck = false;
                if (_islive)
                {

                    _msg = new System.Text.StringBuilder(bufsize);
                }

            }
            if (_islive && sendreport && (k.time>=_rt))
            {
                sendreport = false;
                debug(k.symbol + " hit report time: " + ReportTime+" at: "+k.time);
                Report();
            }
        }

        const int bufsize = 100000;
        /// <summary>
        /// generate current report as report event
        /// </summary>
        public void Report()
        {
            if (SendReportEvent != null)
            {
                _msg.Insert(0, FetchResults().ToString());
                SendReportEvent(_msg.ToString());
                _msg = new System.Text.StringBuilder(bufsize);
            }
        }
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        public event DebugDelegate SendDebugEvent;
        public event DebugDelegate SendReportEvent;
        public Results FetchResults() { return FetchResults(RiskFreeRate, Comission); }
        public Results FetchResults(decimal rfr, decimal commiss) { return FetchResults(TradeResult.ResultsFromTradeList(fills), rfr, commiss,debug); }
        /// <summary>
        /// get results from list of traderesults
        /// </summary>
        /// <param name="results"></param>
        /// <param name="RiskFreeRate"></param>
        /// <param name="CommissionPerContractShare"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Results FetchResults(List<TradeResult> results, decimal RiskFreeRate, decimal CommissionPerContractShare, DebugDelegate d)
        {
            return FetchResults(results, RiskFreeRate, CommissionPerContractShare, true, d);
        }
        public static Results FetchResults(List<TradeResult> results, decimal RiskFreeRate, decimal CommissionPerContractShare, bool persymbol, DebugDelegate d)
        {
            try
            {
                List<Trade> fills = new List<Trade>();
                foreach (TradeResult tr in results)
                    fills.Add(tr.Source);
                List<decimal> _MIU = new List<decimal>();
                List<decimal> _return = new List<decimal>();
                List<int> days = new List<int>();
                //clear position tracker
                PositionTracker pt = new PositionTracker(results.Count);
                // setup new results
                Results r = new Results();
                r.ComPerShare = CommissionPerContractShare;
                r.RiskFreeRet = string.Format("{0:P2}", RiskFreeRate);
                int consecWinners = 0;
                int consecLosers = 0;
                List<long> exitscounted = new List<long>();
                decimal winpl = 0;
                decimal losepl = 0;
                Dictionary<string, int> tradecount = new Dictionary<string, int>();

                foreach (TradeResult tr in results)
                {
                    if (tradecount.ContainsKey(tr.Source.symbol))
                        tradecount[tr.Source.symbol]++;
                    else
                        tradecount.Add(tr.Source.symbol, 1);
                    if (!days.Contains(tr.Source.xdate))
                        days.Add(tr.Source.xdate);
                    pt.Adjust(tr.Source);
                    // calculate MIU and store on array
                    decimal miu = Calc.Sum(Calc.MoneyInUse(pt));
                    _MIU.Add(miu);
                    // get p&l
                    decimal pl = Calc.Sum(Calc.AbsoluteReturn(pt, new decimal[pt.Count], true, false));
                    _return.Add(pl);

                    if (!r.Symbols.Contains(tr.Source.symbol))
                        r.Symbols += tr.Source.symbol + ",";
                    r.Trades++;
                    r.HundredLots += (int)(tr.Source.xsize / 100);
                    r.GrossPL += tr.ClosedPL;


                    if ((tr.ClosedPL > 0) && !exitscounted.Contains(tr.Source.id))
                    {
                        if (tr.Source.side)
                        {
                            r.SellWins++;
                            r.SellPL += tr.ClosedPL;
                        }
                        else
                        {
                            r.BuyWins++;
                            r.BuyPL += tr.ClosedPL;
                        }
                        if (tr.Source.id != 0)
                            exitscounted.Add(tr.id);
                        r.Winners++;
                        consecWinners++;
                        consecLosers = 0;
                    }
                    else if ((tr.ClosedPL < 0) && !exitscounted.Contains(tr.Source.id))
                    {
                        if (tr.Source.side)
                        {
                            r.SellLosers++;
                            r.SellPL += tr.ClosedPL;
                        }
                        else
                        {
                            r.BuyLosers++;
                            r.BuyPL += tr.ClosedPL;
                        }
                        if (tr.Source.id != 0)
                            exitscounted.Add(tr.id);
                        r.Losers++;
                        consecLosers++;
                        consecWinners = 0;
                    }
                    if (tr.ClosedPL > 0)
                        winpl += tr.ClosedPL;
                    else if (tr.ClosedPL < 0)
                        losepl += tr.ClosedPL;

                    if (consecWinners > r.ConsecWin) r.ConsecWin = consecWinners;
                    if (consecLosers > r.ConsecLose) r.ConsecLose = consecLosers;
                    if ((tr.OpenSize == 0) && (tr.ClosedPL == 0)) r.Flats++;
                    if (tr.ClosedPL > r.MaxWin) r.MaxWin = tr.ClosedPL;
                    if (tr.ClosedPL < r.MaxLoss) r.MaxLoss = tr.ClosedPL;
                    if (tr.OpenPL > r.MaxOpenWin) r.MaxOpenWin = tr.OpenPL;
                    if (tr.OpenPL < r.MaxOpenLoss) r.MaxOpenLoss = tr.OpenPL;

                }


                try
                {
                    r.SharpeRatio = _return.Count<2 ? 0 : Math.Round(Calc.SharpeRatio(_return[_return.Count - 1], Calc.StdDev(_return.ToArray()), RiskFreeRate), 3);
                }
                catch (Exception ex)
                {
                    if (d != null)
                        d("sharp error: " + ex.Message);
                }

                if (r.Trades != 0)
                {
                    r.AvgPerTrade = Math.Round((losepl + winpl) / r.Trades, 2);
                    r.AvgLoser = r.Losers == 0 ? 0 : Math.Round(losepl / r.Losers, 2);
                    r.AvgWin = r.Winners == 0 ? 0 : Math.Round(winpl / r.Winners, 2);
                    r.MoneyInUse = Math.Round(Calc.Max(_MIU.ToArray()), 2);
                    r.MaxPL = Math.Round(Calc.Max(_return.ToArray()), 2);
                    r.MinPL = Math.Round(Calc.Min(_return.ToArray()), 2);
                    r.MaxDD = string.Format("{0:P1}", Calc.MaxDDPct(fills));
                    r.SymbolCount = pt.Count;
                    r.DaysTraded = days.Count;
                    r.GrossPerDay = Math.Round(r.GrossPL / days.Count, 2);
                    r.GrossPerSymbol = Math.Round(r.GrossPL / pt.Count, 2);
                    if (persymbol)
                    {
                        for (int i = 0; i < pt.Count; i++)
                        {
                            r.PerSymbolStats.Add(pt[i].Symbol + ": " + tradecount[pt[i].Symbol] + " for " + pt[i].ClosedPL.ToString("C2"));
                        }
                    }
                }
                else
                {
                    r.MoneyInUse = 0;
                    r.MaxPL = 0;
                    r.MinPL = 0;
                    r.MaxDD = "0";
                    r.GrossPerDay = 0;
                    r.GrossPerSymbol = 0;
                }

                return r;
            }
            catch (Exception ex)
            {
                if (d != null)
                    d("error generting report: " + ex.Message + ex.StackTrace);
                return new Results();
            }

        }

        bool _persymbol = true;
        bool ShowPerSymbolStats { get { return _persymbol; } set { _persymbol = value; } }
        /// <summary>
        /// get results from list of trades
        /// </summary>
        /// <param name="trades"></param>
        /// <param name="riskfreerate"></param>
        /// <param name="commissionpershare"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Results ResultsFromTradeList(List<Trade> trades, decimal riskfreerate, decimal commissionpershare, DebugDelegate d)
        {
            string[] results = Util.TradesToClosedPL(trades);
            List<TradeResult> tresults = new List<TradeResult>(results.Length);
            foreach (string line in results)
                tresults.Add(TradeResult.Init(line));
            return Results.FetchResults(tresults, riskfreerate, commissionpershare, d);
        }

        internal List<string> PerSymbolStats = new List<string>();

        public string Symbols = "";
        public decimal GrossPL = 0;
        public string NetPL { get { return v2s(GrossPL - (HundredLots * 100 * ComPerShare)); } }
        public decimal BuyPL = 0;
        public decimal SellPL = 0;
        public int Winners = 0;
        public int BuyWins = 0;
        public int SellWins = 0;
        public int SellLosers = 0;
        public int BuyLosers = 0;
        public int Losers = 0;
        public int Flats = 0;
        public decimal AvgPerTrade = 0;
        public decimal AvgWin = 0;
        public decimal AvgLoser = 0;
        public decimal MoneyInUse = 0;
        public decimal MaxPL = 0;
        public decimal MinPL = 0;
        public string MaxDD = "0%";
        public decimal MaxWin = 0;
        public decimal MaxLoss = 0;
        public decimal MaxOpenWin = 0;
        public decimal MaxOpenLoss = 0;
        public int HundredLots = 0;
        public int Trades = 0;
        public int SymbolCount = 0;
        public int DaysTraded = 0;
        public decimal GrossPerDay = 0;
        public decimal GrossPerSymbol = 0;
        public decimal SharpeRatio = 0;
        public decimal ComPerShare = 0.01m;
        public int ConsecWin = 0;
        public int ConsecLose = 0;
        public string WinSeqProbEffHyp { get { return v2s(Math.Min(100, ((decimal)Math.Pow(1 / 2.0, ConsecWin) * (Trades - Flats - ConsecWin + 1)) * 100)) + @" %"; } }
        public string LoseSeqProbEffHyp { get { return v2s(Math.Min(100, ((decimal)Math.Pow(1 / 2.0, ConsecLose) * (Trades - Flats - ConsecLose + 1)) * 100)) + @" %"; } }
        public string Commissions { get { return v2s(HundredLots * 100 * ComPerShare); } }
        string v2s(decimal v) { return v.ToString("N2"); }
        public string WLRatio { get { return v2s((Losers == 0) ? 0 : (Winners / Losers)); } }
        public string GrossMargin { get { return v2s((GrossPL == 0) ? 0 : ((GrossPL - (HundredLots * 100 * ComPerShare)) / GrossPL)); } }
        public string RiskFreeRet = "0%";

        /// <summary>
        /// get string version of results table
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(": ");
        }
        /// <summary>
        /// get results like calc => value where '=>' is the delim
        /// </summary>
        /// <param name="delim"></param>
        /// <returns></returns>
        public string ToString(string delim)
        {
            return ToString(delim, true);
        }
        public string ToString(string delim, bool genpersymbol)
        {
            Type t = GetType();
            FieldInfo[] fis = t.GetFields();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (FieldInfo fi in fis)
            {
                string format = null;
                if (fi.GetType() == typeof(Decimal)) format = "N2";
                sb.AppendLine(fi.Name+delim+ (format != null ? string.Format(format, fi.GetValue(this)) : fi.GetValue(this).ToString()));
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                string format = null;
                if (pi.GetType() == typeof(Decimal)) format = "N2";
                sb.AppendLine(pi.Name + delim + (format != null ? string.Format(format, pi.GetValue(this, null)) : pi.GetValue(this, null).ToString()));
            }
            foreach (string ps in PerSymbolStats)
            {
                if ((ps==null) || (ps==string.Empty)) 
                    continue;

                string pst = ps.Replace(":", delim);
                sb.AppendLine(pst);
            }
            return sb.ToString();

        }
    }

    public class TradeResult : TradeLink.Common.TradeImpl
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
            r.Source = TradeLink.Common.TradeImpl.FromString(resultline);
            r.OpenPL = Convert.ToDecimal(res[s]);
            r.ClosedPL = Convert.ToDecimal(res[s + 1]);
            r.OpenSize = Convert.ToInt32(res[s + 2]);
            r.ClosedSize = Convert.ToInt32(res[s + 3]);
            r.AvgPrice = Convert.ToDecimal(res[s + 4]);
            return r;
        }


        public static List<TradeResult> ResultsFromTradeList(List<Trade> trades)
        {
            string[] results = Util.TradesToClosedPL(trades);
            List<TradeResult> tresults = new List<TradeResult>(results.Length);
            foreach (string line in results)
                tresults.Add(TradeResult.Init(line));
            return tresults;
        }



    }

}
