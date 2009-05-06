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
        public static BarInterval[] Int2BarInterval(int[] intervals) { List<BarInterval> o = new List<BarInterval>(); foreach (int i in intervals) { try { BarInterval bi = (BarInterval)i; o.Add(bi); } catch (Exception) { o.Add(BarInterval.Custom); } } return o.ToArray(); }
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
        public BarListImpl(string symbol, int interval) : this(symbol, new int[] { interval }) { }
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
        public BarListImpl(string symbol, BarInterval[] intervals) : this(symbol,BarInterval2Int(intervals)) {}

        /// <summary>
        /// creates a barlist with array of custom intervals
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="intervals"></param>
        public BarListImpl(string symbol, int[] intervals)
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
                // create data object
                _intdata[i] = new IntervalData(intervals[i]);
                // subscribe to bar events
                _intdata[i].NewBar += new SymBarIntervalDelegate(BarListImpl_NewBar);
            }
        }

        int _defaultint = (int)BarInterval.FiveMin;
        // array functions
        public decimal[] Open() { return _intdata[_intdataidx[_defaultint]].opens.ToArray(); }
        public decimal[] High() { return _intdata[_intdataidx[_defaultint]].highs.ToArray(); }
        public decimal[] Low() { return _intdata[_intdataidx[_defaultint]].lows.ToArray(); }
        public decimal[] Close() { return _intdata[_intdataidx[_defaultint]].closes.ToArray(); }
        public int[] Vol() { return _intdata[_intdataidx[_defaultint]].vols.ToArray(); }
        public int[] Date() { return _intdata[_intdataidx[_defaultint]].dates.ToArray(); }
        public int[] Time() { return _intdata[_intdataidx[_defaultint]].times.ToArray(); }
        public decimal[] Open(int interval) { return _intdata[_intdataidx[interval]].opens.ToArray(); }
        public decimal[] High(int interval) { return _intdata[_intdataidx[interval]].highs.ToArray(); }
        public decimal[] Low(int interval) { return _intdata[_intdataidx[interval]].lows.ToArray(); }
        public decimal[] Close(int interval) { return _intdata[_intdataidx[interval]].closes.ToArray(); }
        public int[] Date(int interval) { return _intdata[_intdataidx[interval]].dates.ToArray(); }
        public int[] Time(int interval) { return _intdata[_intdataidx[interval]].times.ToArray(); }

        public decimal[] Open(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].opens.ToArray(); }
        public decimal[] High(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].highs.ToArray(); }
        public decimal[] Low(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].lows.ToArray(); }
        public decimal[] Close(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].closes.ToArray(); }
        public int[] Date(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].dates.ToArray(); }
        public int[] Time(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].times.ToArray(); }

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
        public bool NewBar { get { return _intdata[_intdataidx[_defaultint]].isRecentNew; } }

        // standard accessors
        /// <summary>
        /// symbol for bar
        /// </summary>
        public string Symbol { get { return _sym; } set { _sym = value; } }
        /// <summary>
        /// returns true if bar has symbol and has requested intervals
        /// </summary>
        public bool isValid { get { return (_sym != string.Empty) && (_intdata.Length>0); } }
        public IEnumerator GetEnumerator() { int idx = _intdataidx[_defaultint]; int max = _intdata[idx].Count; for (int i = 0; i < max; i++) yield return _intdata[idx].GetBar(Symbol); }
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
        public Bar this[int barnumber] { get { return _intdata[_intdataidx[_defaultint]].GetBar(barnumber,Symbol); } }
        /// <summary>
        /// gets a specific bar in specified interval
        /// </summary>
        /// <param name="barnumber"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public Bar this[int barnumber,BarInterval interval] { get { return _intdata[_intdataidx[(int)interval]].GetBar(barnumber,Symbol); } }
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
        public int Last { get { return _intdata[_intdataidx[_defaultint]].Last; } }
        /// <summary>
        /// gets the # of bars in default interval
        /// </summary>
        public int Count { get { return _intdata[_intdataidx[_defaultint]].Count; } }
        /// <summary>
        /// gets the last bar in specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int LastInterval(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].Last; }
        /// <summary>
        /// gets the last bar for a specified seconds interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int LastInterval(int interval) { return _intdata[_intdataidx[interval]].Last; }
        /// <summary>
        /// gets count of bars in specified interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int CountInterval(BarInterval interval) { return _intdata[_intdataidx[(int)interval]].Count; }
        /// <summary>
        /// gets the count of bars in a specified seconds interval
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public int CountInterval(int interval) { return _intdata[_intdataidx[interval]].Count; }
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
                id.opens.Clear();
                id.highs.Clear();
                id.lows.Clear();
                id.closes.Clear();
                id.dates.Clear();
                id.times.Clear();
                id.vols.Clear();
                id.Count = 0;
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
        internal static void addbar(BarListImpl b, Bar mybar, int instdataidx)
        {
            b._intdata[instdataidx].Count++;
            b._intdata[instdataidx].closes.Add(mybar.Close);
            b._intdata[instdataidx].opens.Add(mybar.Open);
            b._intdata[instdataidx].dates.Add(mybar.Bardate);
            b._intdata[instdataidx].highs.Add(mybar.High);
            b._intdata[instdataidx].lows.Add(mybar.Close);
            b._intdata[instdataidx].vols.Add(mybar.Volume);
            b._intdata[instdataidx].times.Add(mybar.Bartime);
        }
        /// <summary>
        /// Populate the day-interval barlist of this instance from a URL, where the results are returned as a CSV file.  URL should accept requests in the form of http://url/get.py?sym=IBM
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private static BarList DayFromURL(string url,string Symbol)
        {
            BarListImpl bl = new BarListImpl(BarInterval.Day,Symbol);
            if (Symbol == "") return bl;
            System.Net.WebClient wc = new System.Net.WebClient();
            string res = "";
            try
            {
                res = wc.DownloadString(url + Symbol);
            }
            catch (System.Net.WebException) { return bl; }
            string[] line = res.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                if (line[i] != "") 
                    addbar(bl,BarImpl.FromCSV(line[i]),0);
            }
            return bl;
        }

        /// <summary>
        /// Populate the day-interval barlist using google finance as the source.
        /// </summary>
        /// <returns></returns>
        public static BarList DayFromGoogle(string symbol)
        {
            const string GOOGURL = @"http://finance.google.com/finance/historical?histperiod=daily&start=250&num=25&output=csv&q=";
            return DayFromURL(GOOGURL,symbol);
        }

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


    }

    internal class IntervalData
    {
        internal event SymBarIntervalDelegate NewBar;

        public IntervalData(int secondsPerInterval)
        {
            intervallength = secondsPerInterval;
        }
        void newbar()
        {
            opens.Add(0);
            closes.Add(0);
            highs.Add(0);
            lows.Add(decimal.MaxValue);
            vols.Add(0);
            times.Add(0);
            dates.Add(0);
        }
        long curr_barid = -1;
        int intervallength = 60;
        internal List<decimal> opens = new List<decimal>();
        internal List<decimal> closes = new List<decimal>();
        internal List<decimal> highs = new List<decimal>();
        internal List<decimal> lows = new List<decimal>();
        internal List<int> vols = new List<int>();
        internal List<int> dates = new List<int>();
        internal List<int> times = new List<int>();
        internal int Last { get { return Count - 1; } }
        internal int Count = 0;
        internal bool isRecentNew = false;
        internal Bar GetBar(int index,string symbol)
        {
            Bar b = new BarImpl();
            if ((index<0) || (index>=Count)) return b;
            b = new BarImpl(opens[index], highs[index], lows[index], closes[index], vols[index], dates[index],times[index],symbol);
            if (index == Last) b.isNew = isRecentNew;
            return b;
        }
        internal Bar GetBar(string symbol) { return GetBar(Last,symbol); }
        internal void newTick(Tick k)
        {
            // only pay attention to trades and indicies
            if (k.trade==0) return;
            // get the barcount
            long barid = getbarid(k);
            // if not current bar
            if (barid != curr_barid)
            {
                // create a new one
                newbar();
                // mark it
                isRecentNew = true;
                // count it
                Count++;
                // make it current
                curr_barid = barid;
                // set time
                times[times.Count - 1] = k.time;
                // set date
                dates[dates.Count - 1] = k.date;
            }
            else isRecentNew = false;
            // blend tick into bar
            // open
            if (opens[Last] == 0) opens[Last] = k.trade;
            // high
            if (k.trade > highs[Last]) highs[Last] = k.trade;
            // low
            if (k.trade < lows[Last]) lows[Last] = k.trade;
            // close
            closes[Last] = k.trade;
            // don't set volume for index
            if (k.isIndex) return;
            // volume
            vols[Last] += k.size;
            // notify barlist
            if (isRecentNew)
                NewBar(k.symbol, intervallength);

        }

        private long getbarid(Tick k)
        {
            // get time elapsed to this point
            int elap = Util.FT2FTS(k.time);
            // get number of this bar in the day for this interval
            long bcount = (int)((double)elap / intervallength);
            // add the date to the front of number to make it unique
            bcount += (long)k.date * 10000;
            return bcount;
        }

    }


}