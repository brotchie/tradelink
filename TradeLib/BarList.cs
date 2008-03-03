using System;
using System.Collections.Generic;

namespace TradeLib
{
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
        public BarList(BarInterval PreferredInterval, string stock)
        {
            Int = PreferredInterval;
            sym = stock;
        }
        public bool HasBar() { return NumBars() > 0; }
        public bool Has(int MinimumBars) { return NumBars() >= MinimumBars; }
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
        public int BarZero { get { return NumBars() - 1; } }
        public int Last { get { return BarZero; } }
        protected bool NEWBAR = false;
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
        public BarInterval Int { get { return interval; } set { interval = value; } }
        public Bar Get(int BarNum) { return Get(BarNum, Int); }
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

        public bool DayFromGoogle()
        {
            const string GOOGURL = @"http://finance.google.com/finance/historical?histperiod=daily&start=250&num=25&output=csv&q=";
            return DayFromURL(GOOGURL);
        }

        public static BarList FromEPF(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            Stock s = eSigTick.InitEpf(sr);
            BarList b = new BarList(BarInterval.FiveMin, s.Symbol);
            while (!sr.EndOfStream)
                b.AddTick(eSigTick.FromStream(s.Symbol,sr));
            return b;
        }

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
