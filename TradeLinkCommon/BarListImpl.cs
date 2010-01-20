using System;
using System.Collections.Generic;
using System.Collections;
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
            _intdata = new IntervalData[intervals.Length];
            // create interval data object for each interval
            for (int i = 0; i < intervals.Length; i++)
            {
                try
                {
                    // save index to this data for the interval
                    _intdataidx.Add(intervals[i], i);
                }
                // if key was already present, already had this interval
                catch (Exception) { continue; }
                // set default interval to first one
                if (i == 0)
                    _defaultint = intervals[0];
                // create data object appropriate for type of interval
                switch (types[i])
                {
                    case BarInterval.CustomTicks:
                        _intdata[i] = new TickIntervalData(intervals[i]);
                        break;
                    case BarInterval.CustomVol:
                        _intdata[i] = new VolIntervalData(intervals[i]);
                        break;
                    default:
                        _intdata[i] = new TimeIntervalData(intervals[i]);
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
        public IEnumerator GetEnumerator() { int idx = _intdataidx[_defaultint]; int max = _intdata[idx].Count(); for (int i = 0; i < max; i++) yield return _intdata[idx].GetBar(Symbol); }
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

        public void newPoint(decimal p, int time, int date, int size)
        {
            // add tick to every requested bar interval
            for (int i = 0; i < _intdata.Length; i++)
                _intdata[i].newPoint(p,time,date,size);
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
        public static int GetNearestIntraBar(BarList bl, int time, BarInterval bint)
        {
            long barid = TimeIntervalData.getbarid(time, bl.RecentBar.Bardate, (int)bint);
            BarListImpl bli = (BarListImpl)bl;
            TimeIntervalData tid = (TimeIntervalData)bli._intdata[bli._intdataidx[(int)bint]];
            for (int i = 0; i < tid.Count(); i++)
                if (tid.ids[i] == barid)
                    return i;
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
            catch (Exception ex) { return false; }
            return true;
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
        public static BarList FromTIK(string filename)
        {
            SecurityImpl s = SecurityImpl.FromTIK(filename);
            s.HistSource.gotTick += new TickDelegate(HistSource_gotTick);
            _fromepf = new BarListImpl(s.Symbol);
            while (s.HistSource.NextTick()) ;
            return _fromepf;
        }

        static void HistSource_gotTick(Tick t)
        {
            _fromepf.newTick(t);
        }

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


    }








}