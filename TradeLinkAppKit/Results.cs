using System;
using System.Collections.Generic;
using TradeLink.Common;
using System.Reflection;
using TradeLink.API;

namespace TradeLink.AppKit
{
    /// <summary>
    /// track results
    /// </summary>
    public class Results
    {
        /// <summary>
        /// create default results instance
        /// </summary>
        public Results() : this(.01m, .01m, 0) { }
        /// <summary>
        /// create results instance with risk free return, comission and report time
        /// </summary>
        /// <param name="rfr"></param>
        /// <param name="com"></param>
        /// <param name="reporttime">0 to disable reports, otherwise 16:46:00 = 164600</param>
        public Results(decimal rfr, decimal com, int reporttime)
        {
            RiskFreeRate = rfr;
            Comission = com;
            ReportTime = reporttime;
        }
        int _livecheckafterXticks = 1;
        /// <summary>
        /// wait to do live test after X ticks have arrived
        /// </summary>
        public int CheckLiveAfterTickCount { get { return _livecheckafterXticks; } set { _livecheckafterXticks = value; } }
        int _livetickdelaymax = 60;
        /// <summary>
        /// if a tick is within this many seconds of current system time on same day, tick stream is considered live and reports can be sent
        /// </summary>
        public int CheckLiveMaxDelaySec { get { return _livetickdelaymax; } set { _livetickdelaymax = value; } }
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
        bool sendreport = false;
        bool SendReport { get { return sendreport; } set { sendreport = value; } }
        int ReportTime { get { return _rt; } set { _rt = value; sendreport = (_rt != 0); } }
        bool _livecheck = true;
        bool _islive = false;
        public bool isLive { get { return _islive; } }

        System.Text.StringBuilder _msg = new System.Text.StringBuilder(bufsize);

        /// <summary>
        /// pass debugs to results for report generation
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="appendtime"></param>
        public void GotDebug(string msg, bool appendtime)
        {
            if (appendtime)
                _msg.AppendLine(_time + ": " + msg);
            else
                _msg.AppendLine(msg);
        }
        /// <summary>
        /// pass debug messages to results for report generation
        /// </summary>
        /// <param name="msg"></param>
        public void GotDebug(string msg)
        {
            _msg.AppendLine(_time + ": " + msg);
        }
        int _time = 0;
        int _ticks = 0;
        /// <summary>
        /// pass ticks as they arrive (only necessary if using report time)
        /// </summary>
        /// <param name="k"></param>
        public void newTick(Tick k)
        {
            _time = k.time;
            if (_livecheck && (_ticks++>CheckLiveAfterTickCount))
            {
                bool dmatch = k.date == Util.ToTLDate();
                bool tmatch = Util.FTDIFF(k.time, Util.ToTLTime()) < CheckLiveMaxDelaySec;
                _islive = dmatch && tmatch;
                _livecheck = false;

            }
            ScheduledReportHit(k.time);
        }

        public bool ScheduledReportHit(int time)
        {
            if (_islive && sendreport && (time>=_rt))
            {
                sendreport = false;
                debug("hit report time: " + ReportTime + " at: " + time);
                Report();
                return true;
            }
            return false;
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
        public Results FetchResults(decimal rfr, decimal commiss) { return FetchResults(TradeResult.ResultsFromTradeList(fills), rfr, commiss, debug); }
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
                r.ResultsDateTime = Util.ToTLDateTime();
                r.ComPerShare = CommissionPerContractShare;
                r.RiskFreeRet = string.Format("{0:P2}", RiskFreeRate);
                int consecWinners = 0;
                int consecLosers = 0;
                List<long> exitscounted = new List<long>();
                decimal winpl = 0;
                decimal losepl = 0;
                Dictionary<string, int> tradecount = new Dictionary<string, int>();
                List<decimal> negret = new List<decimal>(results.Count);

                foreach (TradeResult tr in results)
                {
                    if (tradecount.ContainsKey(tr.Source.symbol))
                        tradecount[tr.Source.symbol]++;
                    else
                        tradecount.Add(tr.Source.symbol, 1);
                    if (!days.Contains(tr.Source.xdate))
                        days.Add(tr.Source.xdate);
                    int usizebefore = pt[tr.Source.symbol].UnsignedSize;
                    pt.Adjust(tr.Source);
                    bool isclosing = pt[tr.Source.symbol].UnsignedSize<usizebefore;
                    // calculate MIU and store on array
                    decimal miu = Calc.Sum(Calc.MoneyInUse(pt));
                    if (miu!=0)
                        _MIU.Add(miu);
                    // if we closed something, update return
                    if (isclosing)
                    {
                        // get p&l for portfolio
                        decimal pl = Calc.Sum(Calc.AbsoluteReturn(pt));
                        // count return
                        _return.Add(pl);
                        // get pct return for portfolio
                        decimal pctret = _MIU[_MIU.Count - 1] == 0 ? 0 : pl / _MIU[_MIU.Count - 1];
                        // if it is below our zero, count it as negative return
                        if (pctret < 0)
                            negret.Add(pl);
                    }

                    if (!r.Symbols.Contains(tr.Source.symbol))
                        r.Symbols += tr.Source.symbol + ",";
                    r.Trades++;
                    r.SharesTraded += Math.Abs(tr.Source.xsize);
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

                if (r.Trades != 0)
                {
                    r.AvgPerTrade = Math.Round((losepl + winpl) / r.Trades, 2);
                    r.AvgLoser = r.Losers == 0 ? 0 : Math.Round(losepl / r.Losers, 2);
                    r.AvgWin = r.Winners == 0 ? 0 : Math.Round(winpl / r.Winners, 2);
                    r.MoneyInUse = Math.Round(Calc.Max(_MIU.ToArray()), 2);
                    r.MaxPL = Math.Round(Calc.Max(_return.ToArray()), 2);
                    r.MinPL = Math.Round(Calc.Min(_return.ToArray()), 2);
                    r.MaxDD = string.Format("{0:P2}", Calc.MaxDDPct(fills));
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


                try
                {
                    r.SharpeRatio = _return.Count < 2 ? 0 : Math.Round(Calc.SharpeRatio(_return[_return.Count - 1], Calc.StdDev(_return.ToArray()), (RiskFreeRate*r.MoneyInUse)), 3);
                }
                catch (Exception ex)
                {
                    if (d != null)
                        d("sharpe error: " + ex.Message);
                }

                try
                {
                    if (_return.Count == 0)
                        r.SortinoRatio = 0;
                    else if (negret.Count == 1)
                        r.SortinoRatio = 0;
                    else if ((negret.Count == 0) && (_return[_return.Count - 1] == 0))
                        r.SortinoRatio = 0;
                    else if ((negret.Count == 0) && (_return[_return.Count - 1] > 0))
                        r.SortinoRatio = decimal.MaxValue;
                    else if ((negret.Count == 0) && (_return[_return.Count - 1] < 0))
                        r.SortinoRatio = decimal.MinValue;
                    else
                        r.SortinoRatio = Math.Round(Calc.SortinoRatio(_return[_return.Count - 1], Calc.StdDev(negret.ToArray()), (RiskFreeRate * r.MoneyInUse)), 3);
                }
                catch (Exception ex)
                {
                    if (d != null)
                        d("sortino error: " + ex.Message);
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

        public string ResultsId = string.Empty;

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

        public long ResultsDateTime = 0;
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
        public int SharesTraded = 0;
        public int HundredLots { get { return (int)Math.Round((double)SharesTraded / 100, 0); } }
        public int Trades = 0;
        public int SymbolCount = 0;
        public int DaysTraded = 0;
        public decimal GrossPerDay = 0;
        public decimal GrossPerSymbol = 0;
        public decimal SharpeRatio = 0;
        public decimal SortinoRatio = 0;
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
            return ToString(":\t");
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
                if (fi.FieldType == typeof(Decimal)) format = "{0:N2}";
                sb.AppendLine(fi.Name + delim + (format != null ? string.Format(format, fi.GetValue(this)) : fi.GetValue(this).ToString()));
            }
            PropertyInfo[] pis = t.GetProperties();
            foreach (PropertyInfo pi in pis)
            {
                string format = null;
                if (pi.PropertyType == typeof(Decimal)) format = "{0:N2}";
                sb.AppendLine(pi.Name + delim + (format != null ? string.Format(format, pi.GetValue(this, null)) : pi.GetValue(this, null).ToString()));
            }
            foreach (string ps in PerSymbolStats)
            {
                if ((ps == null) || (ps == string.Empty))
                    continue;

                string pst = ps.Replace(":", delim);
                sb.AppendLine(pst);
            }
            return sb.ToString();

        }
    }

}
