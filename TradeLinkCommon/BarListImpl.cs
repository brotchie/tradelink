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
        public IEnumerator GetEnumerator() { foreach (BarImpl b in DefaultBar) yield return b; }
        public Bar this[int index, BarInterval bint]
        {
            get { return Get(index, bint); }
        }

    
        public Bar this[int index]
        {
            get { return Get(index); }
        }
        public int First { get { return 0; } }
        public BarListImpl(string symbol) : this(BarInterval.FiveMin, symbol) { }
        public BarListImpl() : this(BarInterval.FiveMin, "") { }
        string sym = "";
        public BarListImpl(BarInterval PreferredInt) : this(PreferredInt, "") { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BarList"/> class.
        /// </summary>
        /// <param name="PreferredInterval">The preferred time-interval on requests if none is specified.</param>
        /// <param name="stock">The stock.</param>
        public BarListImpl(BarInterval PreferredInterval, string stock)
        {
            Int = PreferredInterval;
            sym = stock;
        }
        public bool isValid { get { return Has(1); } }
        public bool HasBar() { return Count > 0; }
        public bool Has(int MinimumBars) { return Count >= MinimumBars; }
        public bool Has(int MinimumBars, BarInterval interval)
        {
            switch (interval)
            {
                case BarInterval.Day: return daylist.Count >= MinimumBars; 
                case BarInterval.FifteenMin: return fifteenlist.Count >= MinimumBars; 
                case BarInterval.ThirtyMin: return thirtylist.Count >= MinimumBars; 
                case BarInterval.FiveMin: return fivelist.Count >= MinimumBars; 
                case BarInterval.Hour: return hourlist.Count >= MinimumBars; 
                case BarInterval.Minute: return minlist.Count >= MinimumBars;
            }
            return false;
        }
        /// <summary>
        /// Resets this instance.  Clears all the bars for all time intervals.
        /// </summary>
        public void Reset()
        {
            minlist.Clear();
            fivelist.Clear();
            thirtylist.Clear();
            fifteenlist.Clear();
            hourlist.Clear();
            daylist.Clear();
        }
        protected List<BarImpl> minlist = new List<BarImpl>();
        protected List<BarImpl> fivelist = new List<BarImpl>();
        protected List<BarImpl> fifteenlist = new List<BarImpl>();
        protected List<BarImpl> thirtylist = new List<BarImpl>();
        protected List<BarImpl> hourlist = new List<BarImpl>();
        protected List<BarImpl> daylist = new List<BarImpl>();
        public string Symbol { get { return sym; } }
        public int Count { get { return DefaultBar.Count; } }
        public int Last { get { return Count - 1; } }
        /// <summary>
        /// Returns most recent bar, or an invalid bar if no bars have been received
        /// </summary>
        public Bar RecentBar
        {
            get
            {
                try
                {
                    return this[Last];
                }
                catch (Exception) { return new BarImpl(); }
            }
        }
        /// <summary>
        /// Gets a value indicating whether most recently added bar is a [new bar].
        /// </summary>
        /// <value><c>true</c> if [new bar]; otherwise, <c>false</c>.</value>
        public bool NewBar { get { return this.HasBar() && RecentBar.isNew; } }
        protected BarInterval interval = BarInterval.FiveMin;
        protected List<BarImpl> DefaultBar
        {
            get
            {
                List<BarImpl> bars = new List<BarImpl>();
                switch (Int)
                {
                    case BarInterval.FiveMin: bars = fivelist; break;
                    case BarInterval.Minute: bars = minlist; break;
                    case BarInterval.FifteenMin: bars = fifteenlist; break;
                    case BarInterval.ThirtyMin: bars = thirtylist; break;
                    case BarInterval.Hour: bars = hourlist; break;
                    case BarInterval.Day: bars = daylist; break;
                }
                return bars;
            }
        }
        /// <summary>
        /// Gets or sets the preferred interval.  This applies to reads of a particular bar when no bar is provided.  Does not affect writing new bars or ticks; ticks are always added to every bar.
        /// </summary>
        /// <value>The int.</value>
        public BarInterval Int { get { return interval; } set { interval = value; } }
        /// <summary>
        /// Gets the specified bar num, with the default preferred interval.
        /// </summary>
        /// <param name="BarNum">The bar num.</param>
        /// <returns></returns>
        public BarImpl Get(int BarNum) { return Get(BarNum, Int); }
        /// <summary>
        /// Gets the specified bar number, with the specified bar interval.
        /// </summary>
        /// <param name="i">The barnumber.</param>
        /// <param name="barinterval">The barinterval.</param>
        /// <returns></returns>
        public BarImpl Get(int i, BarInterval barinterval)
        {
            List<BarImpl> bars = new List<BarImpl>();
            switch (barinterval)
            {
                case BarInterval.FiveMin: bars = fivelist; break;
                case BarInterval.Minute: bars = minlist; break;
                case BarInterval.FifteenMin: bars = fifteenlist; break;
                case BarInterval.ThirtyMin: bars = thirtylist; break;
                case BarInterval.Hour: bars = hourlist; break;
                case BarInterval.Day: bars = daylist; break;
            }
            if ((i < 0) || (i >= bars.Count)) return new BarImpl();
            return bars[i];
        }
        /// <summary>
        /// Adds the tick to the barlist, creates other bars if needed.
        /// </summary>
        /// <param name="t">The tick to add.</param>
        public void newTick(Tick t)
        {
            if ((t.symbol != Symbol) && (Symbol == ""))
                this.sym = t.symbol; // if we have no symbol, take ticks symbol
            else if ((t.symbol != Symbol) && (Symbol != ""))
                return; //don't process ticks for other stocks
            if (!t.isTrade && !t.isIndex) return; // don't process quotes
            // if we have no bars, add bar with a tick
            if (Count == 0)
            {
                minlist.Add(new BarImpl(BarInterval.Minute));
                fivelist.Add(new BarImpl(BarInterval.FiveMin));
                fifteenlist.Add(new BarImpl(BarInterval.FifteenMin));
                thirtylist.Add(new BarImpl(BarInterval.ThirtyMin));
                hourlist.Add(new BarImpl(BarInterval.Hour));
                daylist.Add(new BarImpl(BarInterval.Day));
                minlist[minlist.Count - 1].newTick(t);
                fivelist[fivelist.Count - 1].newTick(t);
                fifteenlist[fifteenlist.Count - 1].newTick(t);
                thirtylist[thirtylist.Count - 1].newTick(t);
                hourlist[hourlist.Count - 1].newTick(t);
                daylist[daylist.Count - 1].newTick(t);
            }
            else
            {
                // if we have at least a bar, get most current bar
                foreach (BarInterval inv in Enum.GetValues(typeof(BarInterval)))
                {
                    BarImpl cbar = (BarImpl)this[intervallast(inv), inv];
                    // if tick fits in current bar, then we're done for this interval
                    if (cbar.newTick(t)) continue;
                    else // otherwise we need another bar in this interval
                        if (AddBar(inv))
                            ((BarImpl)this[this.Last,inv]).newTick(t);
               }
            }
            return;
        }

        int intervallast(BarInterval interval)
        {
            switch (interval)
            {
                case BarInterval.Day: return daylist.Count-1;
                case BarInterval.FifteenMin: return fifteenlist.Count - 1;
                case BarInterval.FiveMin: return fivelist.Count - 1;
                case BarInterval.Hour: return hourlist.Count - 1;
                case BarInterval.Minute: return minlist.Count - 1;
                case BarInterval.ThirtyMin: return thirtylist.Count - 1;
            }
            return 0;
        }

        bool AddBar(BarInterval bint)
        {
            switch (bint)
            {
                case BarInterval.Day: daylist.Add(new BarImpl(BarInterval.Day)); break;
                case BarInterval.FifteenMin: fifteenlist.Add(new BarImpl(BarInterval.FifteenMin)); break;
                case BarInterval.FiveMin: fivelist.Add(new BarImpl(BarInterval.FiveMin)); break;
                case BarInterval.Hour: hourlist.Add(new BarImpl(BarInterval.Hour)); break;
                case BarInterval.Minute: minlist.Add(new BarImpl(BarInterval.Minute)); break;
                case BarInterval.ThirtyMin: thirtylist.Add(new BarImpl(BarInterval.ThirtyMin)); break;
                default: return false;
            }
            return true;
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
                BarImpl mybar = null;
                if (line[i] != "") mybar = BarImpl.FromCSV(line[i]);
                if (mybar != null) b.daylist.Add(mybar);
            }
            return b;

        }
        /// <summary>
        /// Populate the day-interval barlist of this instance from a URL, where the results are returned as a CSV file.  URL should accept requests in the form of http://url/get.py?sym=IBM
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private bool DayFromURL(string url)
        {
            if (Symbol == "") return false;
            System.Net.WebClient wc = new System.Net.WebClient();
            string res = "";
            try
            {
                res = wc.DownloadString(url + Symbol);
            }
            catch (System.Net.WebException) { return false; }
            string[] line = res.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                BarImpl mybar = null;
                if (line[i] != "") mybar = BarImpl.FromCSV(line[i]);
                if (mybar != null) daylist.Add(mybar);
            }
            return true;
        }

        /// <summary>
        /// Populate the day-interval barlist using google finance as the source.
        /// </summary>
        /// <returns></returns>
        public bool DayFromGoogle()
        {
            const string GOOGURL = @"http://finance.google.com/finance/historical?histperiod=daily&start=250&num=25&output=csv&q=";
            return DayFromURL(GOOGURL);
        }

        /// <summary>
        /// Build a barlist using an EPF file as the source
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>barlist</returns>
        public static BarListImpl FromEPF(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            SecurityImpl s = eSigTick.InitEpf(sr);
            BarListImpl b = new BarListImpl(BarInterval.FiveMin, s.Symbol);
            while (!sr.EndOfStream)
                b.newTick(eSigTick.FromStream(s.Symbol, sr));
            return b;
        }

    }

}