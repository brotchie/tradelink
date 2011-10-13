using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// Holds a succession of bars.  Will acceptt ticks and automatically create new bars as needed.
    /// </summary>
    public class BarListImpl : TradeLink.API.BarList
    {
        /// <summary>
        /// converts integer array of intervals to BarIntervals... supplying custom interval for any unrecognized interval types.
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static BarInterval[] Int2BarInterval(int[] intervals) { List<BarInterval> o = new List<BarInterval>(); foreach (int i in intervals) { try { BarInterval bi = (BarInterval)i; o.Add(bi); } catch (Exception) { o.Add(BarInterval.CustomTime); } } return o.ToArray(); }
        /// <summary>
        /// converts array of BarIntervals to integer intervals.
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static int[] BarInterval2Int(BarInterval[] ints) { List<int> o = new List<int>(); foreach (BarInterval bi in ints) o.Add((int)bi); return o.ToArray(); }
        /// <summary>
        /// gets array of all possible non custom bar intevals
        /// </summary>
        public static BarInterval[] ALLINTERVALS { get { return new BarInterval[] { BarInterval.FiveMin, BarInterval.Minute, BarInterval.Hour, BarInterval.ThirtyMin, BarInterval.FifteenMin, BarInterval.Day }; } }
        // holds available intervals
        int[] _availint = new int[0];
        // holds all raw data
        IntervalData[] _intdata = new IntervalData[0];
        // holds index into raw data using interval type
        Dictionary<int, int> _intdataidx = new Dictionary<int, int>();
        /// <summary>
        /// creates barlist with defined symbol and requests all intervals
        /// </summary>
        /// <param name="symbol"></param>
        public BarListImpl(string symbol) : this(symbol, ALLINTERVALS) { }
        /// <summary>
        /// creates a barlist with requested interval and defined symbol
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="symbol"></param>
        public BarListImpl(BarInterval interval, string symbol) : this(symbol, new BarInterval[] { interval }) { }
        /// <summary>
        /// creates a barlist with requested custom interval and defined symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        public BarListImpl(string symbol, int interval) : this(symbol, interval, BarInterval.CustomTime) { }
        /// <summary>
        /// creates a barlist with custom interval and a custom type (tick/vol)
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="type"></param>
        public BarListImpl(string symbol, int interval, BarInterval type) : this(symbol, new int[] { interval }, new BarInterval[] { type }) { }
        /// <summary>
        /// creates a barlist with requested interval.  symbol will be defined by first tick received
        /// </summary>
        /// <param name="interval"></param>
        public BarListImpl(BarInterval interval) : this(string.Empty, new BarInterval[] { interval }) { }
        /// <summary>
        /// creates barlist with no symbol defined and requests 5min bars
        /// </summary>
        public BarListImpl() : this(string.Empty,new BarInterval[] { BarInterval.FiveMin,BarInterval.Minute,BarInterval.Hour,BarInterval.ThirtyMin,BarInterval.FifteenMin, BarInterval.Day }) { }
        /// <summary>
        /// creates barlist with specified symbol and requested intervals
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="intervals"></param>
        public BarListImpl(string symbol, BarInterval[] intervals) : this(symbol,BarInterval2Int(intervals), intervals) {}
        /// <summary>
        /// make copy of a barlist.  remember you must re-setup GotNewBar events after using this.
        /// </summary>
        /// <param name="original"></param>
        public BarListImpl(BarList original) : this(original.Symbol,original.CustomIntervals,original.Intervals) 
        {
            for (int j = 0; j<original.Intervals.Length; j++)
            {
                original.DefaultInterval = original.Intervals[j];
                for (int i = 0; i < original.Count; i++)
                {
                    addbar(this, original[i, original.Intervals[j]], j);
                }
            }
        }
        /// <summary>
        /// fill bars with arbitrary price data for a symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="prices"></param>
        /// <param name="startdate"></param>
        /// <param name="blt"></param>
        /// <param name="interval"></param>
        /// <param name="debugs"></param>
        /// <returns></returns>
        public static bool backfillbars(string sym, decimal[] prices, int startdate, ref BarListTracker blt, int interval, DebugDelegate debugs)
        {
            // ensure we have closing data
            if (prices.Length == 0)
            {
                if (debugs != null)
                    debugs(sym + " no price data provided/available, will have to wait until bars are created from market.");
                return false;
            }
            // get start day
            int date = startdate;
            // make desired numbers of ticks
            DateTime n = DateTime.Now;
            bool ok = true;
            for (int i = prices.Length - 1; i >= 0; i--)
            {
                // get time now - exitlen*60
                int nt = Util.ToTLTime(n.Subtract(new TimeSpan(0, i * interval, 0)));
                Tick k = TickImpl.NewTrade(sym, date, nt, prices[i], 100, string.Empty);
                ok &= k.isValid && k.isTrade;
                blt.newTick(k);
            }
            if (ok && (debugs != null))
                debugs(sym + " bars backfilled using: " + Calc.parray(prices));
            return ok;
        }
        /// <summary>
        /// insert a bar at particular place in the list.
        /// REMEMBER YOU MUST REHANDLE GOTNEWBAR EVENT AFTER CALLING THIS.
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="b"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static BarListImpl InsertBar(BarList bl, Bar b, int position)
        {
            
            BarListImpl copy = new BarListImpl(bl);
            for (int j = 0; j < bl.CustomIntervals.Length; j++)
            {
                if (bl.CustomIntervals[j] != b.Interval)
                    continue;
                int count = bl.IntervalCount(b.Interval);
                if (count != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (i == position)
                        {
                            addbar(copy, b, j);
                        }
                        addbar(copy, bl[i, (BarInterval)b.Interval], j);
                    }
                }
                else
                    addbar(copy, b, 0);
            }
            return copy;
        }
        /// <summary>
        /// insert one barlist into another barlist
        /// REMEMBER: You must re-handle the GotNewBar event after calling this method.
        /// You should also ensure that inserted barlist has same intervals/types as original barlist.
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        public static BarListImpl InsertBarList(BarList bl, BarList insert)
        {
            BarListImpl copy = new BarListImpl(bl);
            for (int j = 0; j < bl.CustomIntervals.Length; j++)
            {
                for (int k = 0; k < insert.CustomIntervals.Length; k++)
                {
                    if (bl.CustomIntervals[j] != insert.CustomIntervals[k])
                        continue;
                    for (int l = 0; l < insert.Count; l++)
                    {
                        for (int m = 0; m < bl.Count; m++)
                        {
                            if (l == m)
                            {
                                addbar(copy, insert[l, (BarInterval)insert.CustomIntervals[k]], j);
                            }
                            addbar(copy, bl[m], j);
                        }
                    }
                }
            }
            return copy;
        }

        /// <summary>
        /// creates a barlist with array of custom intervals
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="intervals"></param>
        public BarListImpl(string symbol, int[] intervals, BarInterval[] types)
        {            // set symbol
            _sym = symbol;
            // set intervals requested
            _availint = intervals;
            // size length of interval data to # of requested intervals
            _intdata = new IntervalData[_availint.Length];
            // create interval data object for each interval
            for (int i = 0; i < _availint.Length; i++)
            {
                try
                {
                    // save index to this data for the interval
                    _intdataidx.Add(_availint[i], i);
                }
                // if key was already present, already had this interval
                catch (Exception) { continue; }
                // set default interval to first one
                if (i == 0)
                    _defaultint = _availint[0];
                // create data object appropriate for type of interval
                switch (types[i])
                {
                    case BarInterval.CustomTicks:
                        _intdata[i] = new TickIntervalData(_availint[i]);
                        break;
                    case BarInterval.CustomVol:
                        _intdata[i] = new VolIntervalData(_availint[i]);
                        break;
                    default:
                        _intdata[i] = new TimeIntervalData(_availint[i]);
                        break;
                }
            
                // subscribe to bar events
                _intdata[i].NewBar += new SymBarIntervalDelegate(BarListImpl_NewBar);
            }
        }

        int _defaultint = (int)BarInterval.FiveMin;
        // array functions
        public decimal[] Open() { return _intdata[_intdataidx[_defaultint]].open().ToArray(); }
        public decimal[] High() { return _intdata[_intdataidx[_defaultint]].high().ToArray(); }
        public decimal[] Low() { return _intdata[_intdataidx[_defaultint]].low().ToArray(); }
        public decimal[] Close() { return _intdata[_intdataidx[_defaultint]].close().ToArray(); }
        public long[] Vol() { return _intdata[_intdataidx[_defaultint]].vol().ToArray(); }
        public int[] Date() { return _intdata[_intdataidx[_defaultint]].date().ToArray(); }
        public int[] Time() { return _intdata[_intdataidx[_defaultint]].time().ToArray(); }
        public decimal[] Open(int interval) { return _intdata[_intdataidx[interval]].open().ToArray(); }
        public decimal[] High(int interval) { return _intdata[_intdataidx[interval]].high().ToArray(); }
        public decimal[] Low(int interval) { return _intdata[_intdataidx[interval]].low().ToArray(); }
        public decimal[] Close(int interval) { return _intdata[_intdataidx[interval]].close().ToArray(); }
        public int[] Date(int interval) { return _intdata[_intdataidx[interval]].date().ToArray(); }
        public int[] Time(int interval) { return _intdata[_intdataidx[interval]].time().ToArray(); }

        public decimal[] Open(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].open().ToArray(); }
        public decimal[] High(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].high().ToArray(); }
        public decimal[] Low(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].low().ToArray(); }
        public decimal[] Close(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].close().ToArray(); }
        public int[] Date(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].date().ToArray(); }
        public int[] Time(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].time().ToArray(); }

        /// <summary>
        /// gets intervals available/requested by this barlist when it was created
        /// </summary>
        public BarInterval[] Intervals { get { return  Int2BarInterval(_availint); } }
        /// <summary>
        /// gets all available/requested intervals as a custom array of integers
        /// </summary>
        public int[] CustomIntervals { get { return _availint; } }

        /// <summary>
        /// set true for new bar.  don't use this, use GotNewBar event as it's faster.
        /// </summary>
        [Obsolete("this is deprecated method.  use GotNewBar event")]
        public bool NewBar { get { return _intdata[_intdataidx[_defaultint]].isRecentNew(); } }

        // standard accessors
        /// <summary>
        /// symbol for bar
        /// </summary>
        public string Symbol { get { return _sym; } set { _sym = value.Replace(AMEX,""); } }
        /// <summary>
        /// returns true if bar has symbol and has requested intervals
        /// </summary>
        public bool isValid { get { return (_sym != string.Empty) && (_intdata.Length>0); } }
        public IEnumerator GetEnumerator() 
        { 
            int idx = _intdataidx[_defaultint]; 
            int max = _intdata[idx].Count(); 
            for (int i = 0; i < max; i++) 
                yield return _intdata[idx].GetBar(i,Symbol); 
        }
        /// <summary>
        /// gets first bar in any interval
        /// </summary>
        public int First { get { return 0; } }
        /// <summary>
        /// gets or sets the default interval in seconds
        /// </summary>
        public int DefaultCustomInterval { get { return _defaultint; } set { _defaultint = value; } }
        /// <summary>
        /// gets or sets the default interval in bar intervals
        /// </summary>
        public BarInterval DefaultInterval { get { return Int2BarInterval(new int[] { _defaultint })[0]; } set { _defaultint = (int)value; } }
        /// <summary>
        /// gets specific bar in specified interval
        /// </summary>
        /// <param name="barnumber"></param>
        /// <returns></returns>
        public Bar this[int barnumber] 
        { 
            get 
            { 
                return _intdata[_intdataidx[_defaultint]].GetBar(barnumber,Symbol); 
            } 
        }
        /// <summary>
        /// gets a specific bar in specified interval
        /// </summary>
        /// <param name="barnumber"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Bar this[int barnumber,BarInterval interval] { get { return _intdata[_intdataidx[(int)interval]].GetBar(barnumber,Symbol); } }
        /// <summary>
        /// gets count for given bar interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int IntervalCount(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].Count(); }
        /// <summary>
        /// gets count for given bar interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int IntervalCount(int interval) { return _intdata[_intdataidx[interval]].Count(); }
        /// <summary>
        /// gets a specific bar in specified seconds interval
        /// </summary>
        /// <param name="barnumber"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Bar this[int barnumber, int interval] { get { return _intdata[_intdataidx[interval]].GetBar(barnumber, Symbol); } }
        /// <summary>
        /// gets the last bar in default interval
        /// </summary>
        public int Last { get { return _intdata[_intdataidx[_defaultint]].Last(); } }
        /// <summary>
        /// gets the # of bars in default interval
        /// </summary>
        public int Count { get { return _intdata[_intdataidx[_defaultint]].Count(); } }
        /// <summary>
        /// gets the last bar in specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int LastInterval(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].Last(); }
        /// <summary>
        /// gets the last bar for a specified seconds interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int LastInterval(int interval) { return _intdata[_intdataidx[interval]].Last(); }
        /// <summary>
        /// gets count of bars in specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int CountInterval(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].Count(); }
        /// <summary>
        /// gets the count of bars in a specified seconds interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int CountInterval(int interval) { return _intdata[_intdataidx[interval]].Count(); }
        /// <summary>
        /// gets most recent bar from default interval
        /// </summary>
        public Bar RecentBar { get { return this[Last]; } }
        /// <summary>
        /// gets most recent bar from specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Bar RecentBarInterval(BarInterval interval) { return this[LastInterval(interval), interval]; }
        /// <summary>
        /// returns true if barslist has at least minimum # of bars for specified interval
        /// </summary>
        /// <param name="minBars"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool Has(int minBars, BarInterval interval) { return minBars<=CountInterval(interval); }
        /// <summary>
        /// returns true if barlist has minimum number of bars for specified seconds interval
        /// </summary>
        /// <param name="minBars"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public bool Has(int minBars, int interval) { return minBars <= CountInterval(interval); }
        /// <summary>
        /// returns true if barlist has at least minimum # of bars for default interval
        /// </summary>
        /// <param name="minBars"></param>
        /// <returns></returns>
        public bool Has(int minBars) { return Has(minBars, _defaultint); }
        
        /// <summary>
        /// this event is thrown when a new bar arrives
        /// </summary>
        public event SymBarIntervalDelegate GotNewBar;
        void BarListImpl_NewBar(string symbol, int interval)
        {
            // if event is handled by user, pass the event
            if (GotNewBar != null)
                GotNewBar(symbol, interval);
        }
        /// <summary>
        /// erases all bar data
        /// </summary>
        public void Reset()
        {
            foreach (IntervalData id in _intdata)
            {
                id.Reset();
            }
        }

        string _sym = string.Empty;
        int _symh = 0;
        bool _valid = false;
        public void newTick(Tick k)
        {
            // only pay attention to trades and indicies
            if (k.trade == 0) return;
            // make sure we have a symbol defined 
            if (!_valid)
            {
                _symh = k.symbol.GetHashCode();
                _sym = k.symbol;
                _valid = true;
            }
            // make sure tick is from our symbol
            if (_symh != k.symbol.GetHashCode()) return;
            // add tick to every requested bar interval
            for (int i = 0; i < _intdata.Length; i++)
                _intdata[i].newTick(k);
        }

        public void newPoint(string symbol, decimal p, int time, int date, int size)
        {
            // add tick to every requested bar interval
            for (int i = 0; i < _intdata.Length; i++)
                _intdata[i].newPoint(symbol,p,time,date,size);
        }

        /// <summary>
        /// Create a barlist from a succession of bar records provided as comma-delimited OHLC+volume data.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="file">The file containing the CSV records.</param>
        /// <returns></returns>
        public static BarListImpl FromCSV(string symbol, string file)
        {
            BarListImpl b = new BarListImpl(BarInterval.Day, symbol);
            string[] line = file.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] != string.Empty)
                    addbar(b,BarImpl.FromCSV(line[i]),0);
            }
            return b;
        }

        /// <summary>
        /// find the bar # that matches a given time
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="time"></param>
        /// <param name="bint"></param>
        /// <returns></returns>
        public static int GetNearestIntraBar(BarList bl, int time, BarInterval bint) { return GetNearestIntraBar(bl, time, bint, null); }
        public static int GetNearestIntraBar(BarList bl, int time, BarInterval bint,DebugDelegate debug)
        {
            try
            {
                long barid = TimeIntervalData.getbarid(time, bl.RecentBar.Bardate, (int)bint);
                BarListImpl bli = (BarListImpl)bl;
                TimeIntervalData tid = (TimeIntervalData)bli._intdata[bli._intdataidx[(int)bint]];
                for (int i = 0; i < tid.Count(); i++)
                    if (tid.ids[i] == barid)
                        return i;
            }
            catch (Exception ex)
            {
                if (debug != null)
                    debug("error getting nearest bar from: " + bl.Symbol + " at: " + time + " for: " + bint + " error: " + ex.Message + ex.StackTrace);
            }
            return -1;
        }

        internal static void addbar(BarListImpl b, Bar mybar, int instdataidx)
        {
            b._intdata[instdataidx].addbar(mybar);
        }
        private const string AMEX = ":AMEX";
        /// <summary>
        /// Populate the day-interval barlist of this instance from a URL, where the results are returned as a CSV file.  URL should accept requests in the form of http://url/get.py?sym=IBM
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private static BarList DayFromURL(string url, string Symbol) { return DayFromURL(url, Symbol, true); }
        private static BarList DayFromURL(string url,string Symbol,bool appendAMEXonfail)
        {
            BarListImpl bl = new BarListImpl(BarInterval.Day,Symbol);
            if (Symbol == "") return bl;
            System.Net.WebClient wc = new System.Net.WebClient();
            string res = "";
            try
            {
                res = wc.DownloadString(url + Symbol);
            }
            catch (Exception) 
            {
                if (appendAMEXonfail)
                    return DayFromURL(url, Symbol + AMEX, false);
                return bl;
                
            }
            string[] line = res.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] != "") 
                    addbar(bl,BarImpl.FromCSV(line[i]),0);
            }
            return bl;
        }

        public static bool DayFromGoogleAsync(string Symbol, BarListDelegate resultHandler) { return DayFromGoogleAsync(Symbol, resultHandler, true); }
        public static bool DayFromGoogleAsync(string Symbol, BarListDelegate resultHandler, bool appendAmexOnFail)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted +=new System.Net.DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            try
            {
                wc.DownloadStringAsync(new Uri(GOOGURL + Symbol), new BarListDownload(Symbol, resultHandler,appendAmexOnFail));
            }
            catch (System.Net.WebException) { return false; }
            catch (Exception) { return false; }
            return true;
        }

        /// <summary>
        /// attempts to get year worth of daily data from google, if fails tries yahoo.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static BarList DayFromAny(string symbol)
        {
            BarList bl = BarListImpl.DayFromGoogle(symbol);
            if (bl.Count == 0)
                bl = BarListImpl.DayFromYahoo(symbol);
            return bl;
        }

        public static BarList DayFromGoogle(string symbol, int startdate)
        {
            return
                DayFromGoogle(symbol, startdate, Util.ToTLDate());
        }
        /// <summary>
        /// gets specific date range of bars from google
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        public static BarList DayFromGoogle(string symbol, int startdate, int enddate)
        {
            const string AMEX = ":AMEX";
            if ((symbol == null) || (symbol == string.Empty)) return new
BarListImpl();
            string url = @"http://finance.google.com/finance/historical?
histperiod=daily&startdate=" + startdate + "&enddate=" + enddate + "&output=csv&q=" + symbol;
            BarListImpl bl = new BarListImpl(BarInterval.Day, symbol);
            System.Net.WebClient wc = new System.Net.WebClient();
            string res = "";
            try
            {
                res = wc.DownloadString(url);
            }
            catch (Exception)
            {
                if (!symbol.Contains(AMEX))
                    DayFromGoogle(symbol + AMEX, startdate, enddate);
                return bl;

            }
            string[] line = res.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] == "") continue;
                Bar b = BarImpl.FromCSV(line[i]);
                foreach (Tick k in BarImpl.ToTick(b))
                    bl.newTick(k);
            }
            return bl;
        }



        static void wc_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            string res = string.Empty;
            BarListDownload bld = (BarListDownload)e.UserState;
            if (!bld.isValid)
                return;
            if (e.Cancelled || (e.Error != null))
            {
                if (bld.AppendAMEXonFail)
                {
                    DayFromGoogleAsync(bld.Symbol + AMEX, bld.DoResults,false);
                    return;
                }
                bld.DoResults(new BarListImpl(BarInterval.Day, bld.Symbol));
                return;
            }
            res = e.Result;
            BarListImpl bl = new BarListImpl(BarInterval.Day, bld.Symbol);
            string[] line = res.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] != "")
                    addbar(bl, BarImpl.FromCSV(line[i]), 0);
            }
            bld.DoResults(bl);
        }

        private class BarListDownload
        {
            public BarListDownload(string Symbol, BarListDelegate bld,bool appendAMEX)
            {
                AppendAMEXonFail = appendAMEX;
                this.Symbol = Symbol;
                DoResults = bld;
            }
            public bool AppendAMEXonFail = true;
            public string Symbol = string.Empty;
            public BarListDelegate DoResults;
            public bool isValid { get { return (DoResults != null) && (Symbol != string.Empty); } }
        }

        /// <summary>
        /// Populate the day-interval barlist using google finance as the source.
        /// </summary>
        /// <returns></returns>
        public static BarList DayFromGoogle(string symbol)
        {
            return DayFromURL(GOOGURL,symbol);
        }
        const string GOOGURL = @"http://finance.google.com/finance/historical?histperiod=daily&start=250&num=25&output=csv&q=";

        /// <summary>
        /// Build a barlist using an EPF file as the source
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>barlist</returns>
        public static BarList FromEPF(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            SecurityImpl s = eSigTick.InitEpf(sr);
            BarList b = new BarListImpl(s.Symbol);
            while (!sr.EndOfStream)
                b.newTick(eSigTick.FromStream(s.Symbol, sr));
            return b;
        }

        private static BarListImpl _fromepf;
        private static bool _uselast = true;
        private static bool _usebid = true;
        /// <summary>
        /// get a barlist from tick data
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static BarList FromTIK(string filename) { return FromTIK(filename, true, true); }
        /// <summary>
        /// get a barlist from tick data and optionally use bid/ask data to construct bars
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="uselast"></param>
        /// <param name="usebid"></param>
        /// <returns></returns>
        public static BarList FromTIK(string filename, bool uselast, bool usebid)
        {
            _uselast = uselast;
            _usebid = usebid;
            SecurityImpl s = SecurityImpl.FromTIK(filename);
            s.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
            _fromepf = new BarListImpl(s.Symbol);
            while (s.HistSource.NextTick()) ;
            return _fromepf;
        }
        /// <summary>
        /// create barlist from a tik file using given intervals/types
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="uselast"></param>
        /// <param name="usebid"></param>
        /// <param name="intervals"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static BarList FromTIK(string filename, bool uselast, bool usebid, int[] intervals, BarInterval[] types)
        {
            _uselast = uselast;
            _usebid = usebid;
            SecurityImpl s = SecurityImpl.FromTIK(filename);
            s.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
            _fromepf = new BarListImpl(s.Symbol,intervals,types);
            while (s.HistSource.NextTick()) ;
            return _fromepf;
        }

        static void HistSource_gotTick(Tick t)
        {
            if (_uselast)
                _fromepf.newTick(t);
            else
            {
                if (t.hasAsk && !_usebid)
                    _fromepf.newPoint(t.symbol,t.ask, t.time, t.date, t.AskSize);
                else if (t.hasBid && _usebid)
                    _fromepf.newPoint(t.symbol, t.bid, t.time, t.date, t.BidSize);
            }
        }
        /// <summary>
        /// gets index of bar that preceeds given date
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetBarIndexPreceeding(BarList chart, int date)
        {
            // look for previous day's close
            for (int j = chart.Last; (j >= chart.First); j--)
            {
                if (chart.Date()[j] < date)
                {
                    return j;
                }
            }
            // first bar
            return -1;
        }
        /// <summary>
        /// gets preceeding bar by time (assumes same day)
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int GetBarIndexPreceeding(BarList chart, int date, int time)
        {
            // look for previous day's close
            for (int j = chart.Last; (j >= chart.First); j--)
            {
                if (chart.Date()[j] > date) continue;
                if (chart.Time()[j] < time || date > chart.Date()[j])
                {
                    return j;
                }
            }
            // first bar
            return -1;
        }
        /// <summary>
        /// gets bar that preceeds a given date (invalid if no preceeding bar)
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Bar GetBarPreceeding(BarList chart, int date)
        {
            // look for previous day's close
            for (int j = chart.Last; (j >= chart.First); j--)
            {
                if (chart.Date()[j] < date)
                {
                    return chart[j];
                }
            }
            return new BarImpl();
        }

        public static Bar GetBar(BarList chart, int date)
        {
            // look for previous day's close
            for (int j = chart.Last; (j >= chart.First); j--)
            {
                if (chart.Date()[j] == date)
                {
                    return chart[j];
                }
            }
            return new BarImpl();
        }

        /// <summary>
        /// Populate the day-interval barlist using google finance as the source.
        /// </summary>
        /// <returns></returns>
        ///
        public static BarList DayFromYahoo(string symbol) { return DayFromYahoo(symbol, null, null); }
        public static BarList DayFromYahoo(string symbol, DateTime? startDate, DateTime? endDate)
        {
            string urlTemplate =
           @"http://ichart.finance.yahoo.com/table.csv?s=[symbol]&a=" +
             "[startMonth]&b=[startDay]&c=[startYear]&d=[endMonth]&e=" +
                "[endDay]&f=[endYear]&g=d&ignore=.csv";

            if (!endDate.HasValue) endDate = DateTime.Now;
            if (!startDate.HasValue) startDate = DateTime.Now.AddYears(-5);
            if (symbol == null || symbol.Length < 1)
                throw new ArgumentException("Symbol invalid: " + symbol);
            // NOTE: Yahoo's scheme uses a month number 1 less than actual e.g. Jan. ="0"
            int strtMo = startDate.Value.Month - 1;
            string startMonth = strtMo.ToString();
            string startDay = startDate.Value.Day.ToString();
            string startYear = startDate.Value.Year.ToString();

            int endMo = endDate.Value.Month - 1;
            string endMonth = endMo.ToString();
            string endDay = endDate.Value.Day.ToString();
            string endYear = endDate.Value.Year.ToString();

            urlTemplate = urlTemplate.Replace("[symbol]", symbol);

            urlTemplate = urlTemplate.Replace("[startMonth]", startMonth);
            urlTemplate = urlTemplate.Replace("[startDay]", startDay);
            urlTemplate = urlTemplate.Replace("[startYear]", startYear);

            urlTemplate = urlTemplate.Replace("[endMonth]", endMonth);
            urlTemplate = urlTemplate.Replace("[endDay]", endDay);
            urlTemplate = urlTemplate.Replace("[endYear]", endYear);
            return DayFromURL(urlTemplate, symbol);
        }


        /// <summary>
        /// Populate the day-interval barlist using Euronext.com as the source.
        /// </summary>
        /// <param name="isin">The ISIN (mnemonics not accepted)</param>
        /// <returns></returns>
        ///
        public static BarList DayFromEuronext(string isin) { return DayFromEuronext(isin, null, null); }
        public static BarList DayFromEuronext(string isin, DateTime? startDate, DateTime? endDate)
        {
            string market;
            string urlTemplate =
                @"http://www.euronext.com/tools/datacentre/dataCentreDownloadExcell.jcsv?cha=2593&lan=EN&fileFormat=txt&separator=.&dateFormat=dd/MM/yy" + 
                "&isinCode=[symbol]&selectedMep=[market]&indexCompo=&opening=on&high=on&low=on&closing=on&volume=on&dateFrom=[startDay]/[startMonth]/[startYear]&" +
                "dateTo=[endDay]/[endMonth]/[endYear]&typeDownload=2";

            if (!endDate.HasValue) endDate = DateTime.Now;
            if (!startDate.HasValue) startDate = DateTime.Now.AddYears(-5);
            if (isin == null || !Regex.IsMatch(isin, "[A-Za-z0-9]{12}"))
                throw new ArgumentException("Invalid ISIN: " + isin);

            /* ugly hack to get the market number from the isin (not always valid..) */
            CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;
            if (myComp.IsPrefix(isin, "BE")) market = "3";
            else if (myComp.IsPrefix(isin, "FR")) market = "1";
            else if (myComp.IsPrefix(isin, "NL")) market = "2";
            else if (myComp.IsPrefix(isin, "PT")) market = "5";
            else market = "1";
            
            string startMonth = startDate.Value.Month.ToString();
            string startDay = startDate.Value.Day.ToString();
            string startYear = startDate.Value.Year.ToString();

            string endMonth = endDate.Value.Month.ToString();
            string endDay = endDate.Value.Day.ToString();
            string endYear = endDate.Value.Year.ToString();

            urlTemplate = urlTemplate.Replace("[symbol]", isin);
            urlTemplate = urlTemplate.Replace("[market]", market);
            urlTemplate = urlTemplate.Replace("[startMonth]", startMonth);
            urlTemplate = urlTemplate.Replace("[startDay]", startDay);
            urlTemplate = urlTemplate.Replace("[startYear]", startYear);

            urlTemplate = urlTemplate.Replace("[endMonth]", endMonth);
            urlTemplate = urlTemplate.Replace("[endDay]", endDay);
            urlTemplate = urlTemplate.Replace("[endYear]", endYear);

            BarListImpl bl = new BarListImpl(BarInterval.Day, isin);
            System.Net.WebClient wc = new System.Net.WebClient();
            StreamReader res;
            try
            {
                res = new StreamReader(wc.OpenRead(urlTemplate));
                int skipCount = 0;
                string tmp = null;
                do
                {
                    tmp = res.ReadLine();
                    if (skipCount++ < 7) 
                        continue;
                    tmp = tmp.Replace(";", ",");
                    Bar b = BarImpl.FromCSV(tmp, isin, (int)BarInterval.Day);
                    foreach (Tick k in BarImpl.ToTick(b))
                        bl.newTick(k);
                } while (tmp != null);
            }
            catch (Exception)
            {
                return bl;
            }
            
            return bl;
        }


        /// <summary>
        /// load previous days bar data from tick files located in tradelink tick folder
        /// </summary>
        /// <param name="PreviousDay"></param>
        /// <param name="syms"></param>
        /// <param name="AttemptToLoadPreviousDayBars"></param>
        /// <param name="_blt"></param>
        /// <param name="NewBarEvents"></param>
        /// <param name="deb"></param>
        /// <returns></returns>
        public static bool LoadPreviousBars(int PreviousDay, string[] syms, bool AttemptToLoadPreviousDayBars, ref BarListTracker _blt, SymBarIntervalDelegate NewBarEvents, DebugDelegate deb)
        {
            if (AttemptToLoadPreviousDayBars)
            {
                bool errors = false;
                foreach (string sym in syms)
                {
                    string fn = Util.TLTickDir + "\\" + sym + PreviousDay + TikConst.DOT_EXT;
                    if (System.IO.File.Exists(fn))
                    {
                        try
                        {
                            BarList test = BarListImpl.FromTIK(fn);
                            _blt[sym] = BarListImpl.FromTIK(fn, true, false, _blt[sym].CustomIntervals, _blt[sym].Intervals);
                            _blt[sym].GotNewBar += NewBarEvents;
                            if (deb != null)
                                deb(sym + " loaded historical bars from: " + fn);
                        }
                        catch (Exception ex)
                        {
                            errors = true;
                            if (deb != null)
                            {
                                deb(sym + " error loading historical bars from: " + fn);
                                deb(ex.Message + ex.StackTrace);
                            }
                        }
                    }
                    else
                    {
                        errors = true;
                        if (deb != null)
                        {
                            deb(sym + " starting from zero, no historical bar data at: " + fn);
                        }
                    }
                }
                return !errors;
            }
            return true;
        }

        /// <summary>
        /// given some number of intervals, return a list of same intervals with duplicates removed
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static BarInterval[] GetUniqueIntervals(params BarInterval[] ints)
        {
            List<BarInterval> final = new List<BarInterval>(ints.Length);
            foreach (BarInterval bi in ints)
                if (!final.Contains(bi))
                    final.Add(bi);
            return final.ToArray();
        }

        /// <summary>
        /// given some number of intervals, return a list of same intervals with duplicates removed
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public static int[] GetUniqueIntervals(params int[] ints)
        {
            List<int> final = new List<int>(ints.Length);
            foreach (int bi in ints)
                if (!final.Contains(bi))
                    final.Add(bi);
            return final.ToArray();
        }


    }








}