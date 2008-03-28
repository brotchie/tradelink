using System;
using System.Collections.Generic;

namespace TradeLib
{
    /// <summary>
    /// Holds a succession of bars.  Will acceptt ticks and automatically create new bars as needed.
    /// </summary>
    public class BarList
    {
        public Bar this[int index]
        {
            get { return Get(index); }
        }
        public Bar t { get { return Get(BarZero); } }
        public Bar p { get { return Get(BarZero - 1); } }
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
        public bool HasBar() { return NumBars() > 0; }
        public bool Has(int MinimumBars) { return NumBars() >= MinimumBars; }
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

        public int NumBars() { return DefaultBar.Count; }
        /// <summary>
        /// Gets the bar zero.  This is the last or most recent bar in the list.
        /// </summary>
        public int BarZero { get { return NumBars() - 1; } }
        public int Last { get { return BarZero; } }
        protected bool NEWBAR = false;
        /// <summary>
        /// Gets a value indicating whether most recently added bar is a [new bar].
        /// </summary>
        /// <value><c>true</c> if [new bar]; otherwise, <c>false</c>.</value>
        public bool NewBar { get { return NEWBAR; } }
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
        /// <summary>
        /// Adds the tick to the barlist, creates other bars if needed.
        /// </summary>
        /// <param name="t">The tick to add.</param>
        public void AddTick(Tick t)
        {
            NEWBAR = false;
            if (t.sym != Symbol) return; //don't process ticks for other stocks
            if (!t.isTrade) return; // don't process quotes
            // if we have no bars, add bar with a tick
            if (NumBars() == 0)
            {
                minlist.Add(new Bar(t, BarInterval.Minute));
                fivelist.Add(new Bar(t, BarInterval.FiveMin));
                fifteenlist.Add(new Bar(t, BarInterval.FifteenMin));
                hourlist.Add(new Bar(t, BarInterval.Hour));
                daylist.Add(new Bar(t, BarInterval.Day));
                NEWBAR = true;
            }
            else
            {
                // if we have at least a bar, get most current bar
                BarInterval saveint = Int;
                foreach (BarInterval inv in Enum.GetValues(typeof(BarInterval)))
                {
                    Int = inv;
                    Bar cbar = (Bar)DefaultBar[NumBars() - 1];
                    // if tick fits in current bar, then we're done for this interval
                    if (cbar.Accept(t)) continue;
                    else
                    {
                        DefaultBar.Add(new Bar(t, Int)); // otherwise we need another bar in this interval
                        NEWBAR = true;
                    }
                }
                Int = saveint;
            }
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
