using System;
using System.Collections.Generic;
using System.Collections;

namespace TradeLib
{
    /// <summary>
    /// Holds a succession of bars.  Will acceptt ticks and automatically create new bars as needed.
    /// </summary>
    public class BarList : TickIndicator
    {
        private int _enum = -1;
        public IEnumerator GetEnumerator() { foreach (Bar b in DefaultBar) yield return b; }
        public Bar this[int index]
        {
            get { return Get(index); }
        }
        public BarList() : this(BarInterval.FiveMin, "") { }
        string sym = "";
        public BarList(BarInterval PreferredInt) : this(PreferredInt, "") { }
        /// <summary>
        /// Initializes a new instance of the <see cref="BarList"/> class.
        /// </summary>
        /// <param name="PreferredInterval">The preferred time-interval on requests if none is specified.</param>
        /// <param name="stock">The stock.</param>
        public BarList(BarInterval PreferredInterval, string stock)
        {
            Int = PreferredInterval;
            sym = stock;
        }
        public bool isValid { get { return Has(1); } }
        public bool HasBar() { return Count> 0; }
        public bool Has(int MinimumBars) { return Count >= MinimumBars; }
        /// <summary>
        /// Resets this instance.  Clears all the bars for all time intervals.
        /// </summary>
        public void Reset()
        {
            minlist.Clear();
            fivelist.Clear();
            fifteenlist.Clear();
            hourlist.Clear();
            daylist.Clear();
        }
        protected List<Bar> minlist = new List<Bar>();
        protected List<Bar> fivelist = new List<Bar>();
        protected List<Bar> fifteenlist = new List<Bar>();
        protected List<Bar> hourlist = new List<Bar>();
        protected List<Bar> daylist = new List<Bar>();
        public string Symbol { get { return sym; } }
        public int Count { get { return DefaultBar.Count; } }
        public int Last { get { return Count -1; } }
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
                catch (NullReferenceException) { return new Bar(); }
            } 
        }
        /// <summary>
        /// Gets a value indicating whether most recently added bar is a [new bar].
        /// </summary>
        /// <value><c>true</c> if [new bar]; otherwise, <c>false</c>.</value>
        public bool NewBar { get { return this.HasBar() && RecentBar.isNew; } }
        protected BarInterval interval = BarInterval.FiveMin;
        protected List<Bar> DefaultBar
        {
            get
            {
                List<Bar> bars = new List<Bar>();
                switch (Int)
                {
                    case BarInterval.FiveMin: bars = fivelist; break;
                    case BarInterval.Minute: bars = minlist; break;
                    case BarInterval.FifteenMin: bars = fifteenlist; break;
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
        public Bar Get(int BarNum) { return Get(BarNum, Int); }
        /// <summary>
        /// Gets the specified bar number, with the specified bar interval.
        /// </summary>
        /// <param name="i">The barnumber.</param>
        /// <param name="barinterval">The barinterval.</param>
        /// <returns></returns>
        public Bar Get(int i, BarInterval barinterval)
        {
            List<Bar> bars = new List<Bar>();
            switch (barinterval)
            {
                case BarInterval.FiveMin: bars = fivelist; break;
                case BarInterval.Minute: bars = minlist; break;
                case BarInterval.FifteenMin: bars = fifteenlist; break;
                case BarInterval.Hour: bars = hourlist; break;
                case BarInterval.Day: bars = daylist; break;
            }
            return bars[i];
        }
        public void AddTick(Tick t) { newTick(t); }
        /// <summary>
        /// Adds the tick to the barlist, creates other bars if needed.
        /// </summary>
        /// <param name="t">The tick to add.</param>
        public bool newTick(Tick t)
        {
            if ((t.sym != Symbol) && (Symbol==""))
                this.sym = t.sym; // if we have no symbol, take ticks symbol
            else if ((t.sym!=Symbol) && (Symbol!=""))
                return NewBar; //don't process ticks for other stocks
            if (!t.isTrade) return NewBar; // don't process quotes
            // if we have no bars, add bar with a tick
            if (Count == 0)
            {
                minlist.Add(new Bar(BarInterval.Minute));
                fivelist.Add(new Bar(BarInterval.FiveMin));
                fifteenlist.Add(new Bar(BarInterval.FifteenMin));
                hourlist.Add(new Bar(BarInterval.Hour));
                daylist.Add(new Bar(BarInterval.Day));
                minlist[minlist.Count - 1].newTick(t);
                fivelist[fivelist.Count - 1].newTick(t);
                fifteenlist[fifteenlist.Count - 1].newTick(t);
                hourlist[hourlist.Count - 1].newTick(t);
                daylist[daylist.Count - 1].newTick(t);
            }
            else
            {
                // if we have at least a bar, get most current bar
                BarInterval saveint = Int;
                foreach (BarInterval inv in Enum.GetValues(typeof(BarInterval)))
                {
                    Int = inv;
                    Bar cbar = RecentBar;
                    // if tick fits in current bar, then we're done for this interval
                    if (cbar.newTick(t)) continue;
                    else
                    {
                        DefaultBar.Add(new Bar(Int)); // otherwise we need another bar in this interval
                        DefaultBar[DefaultBar.Count - 1].newTick(t);
                    }
                }
                Int = saveint;
            }
            return NewBar;
        }

        public Stock ToStock()
        {
            if (!HasBar()) throw new Exception("Can't generate a stock instance from an empty barlist!");
            Stock s = new Stock(Symbol, this.RecentBar.Bardate);
            s.DayHigh = BarMath.HH(this);
            s.DayLow = BarMath.LL(this);
            s.DayOpen = this[0].Open;
            s.DayClose = this.RecentBar.Close;
            return s;
        }

        /// <summary>
        /// Create a barlist from a succession of bar records provided as comma-delimited OHLC+volume data.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="file">The file containing the CSV records.</param>
        /// <returns></returns>
        public static BarList FromCSV(string symbol, string file)
        {
            BarList b = new BarList(BarInterval.Day, symbol);
            string[] line = file.Split(Environment.NewLine.ToCharArray());
            for (int i = line.Length - 1; i > 0; i--)
            {
                Bar mybar = null;
                if (line[i] != "") mybar = Bar.FromCSV(line[i]);
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
                Bar mybar = null;
                if (line[i] != "") mybar = Bar.FromCSV(line[i]);
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
        /// <returns></returns>
        public static BarList FromEPF(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            Stock s = eSigTick.InitEpf(sr);
            BarList b = new BarList(BarInterval.FiveMin, s.Symbol);
            while (!sr.EndOfStream)
                b.AddTick(eSigTick.FromStream(s.Symbol,sr));
            return b;
        }

        /// <summary>
        /// Build a barlist using an IDX (index) file as a source.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        public static BarList FromIDX(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            Index i = Index.Deserialize(sr.ReadLine());
            BarList b = new BarList(BarInterval.FiveMin, i.Name);
            b.AddTick(i.ToTick());
            while (!sr.EndOfStream)
                b.AddTick(Index.Deserialize(sr.ReadLine()).ToTick());
            return b;
        }
            
    }

}
