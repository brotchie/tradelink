using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;
namespace TradeLink.Common
{
    internal class TimeIntervalData : IntervalData
    {
        public event SymBarIntervalDelegate NewBar;
        public List<decimal> open() { return opens; }
        public List<decimal> close() { return closes; }
        public List<decimal> high() { return highs; }
        public List<decimal> low() { return lows; }
        public List<long> vol() { return vols; }
        public List<int> date() { return dates; }
        public List<int> time() { return times; }
        public List<int> tick() { return ticks; }
        public bool isRecentNew() { return _isRecentNew; }
        public int Count() { return _Count; }
        public int Last() { return _Count - 1; }
        public TimeIntervalData(int unitsPerInterval)
        {
            intervallength = unitsPerInterval;
        }
        public void Reset()
        {
            opens.Clear();
            closes.Clear();
            highs.Clear();
            lows.Clear();
            dates.Clear();
            times.Clear();
            vols.Clear();
            _Count = 0;
        }
        void newbar()
        {
            _Count++;
            opens.Add(0);
            closes.Add(0);
            highs.Add(0);
            lows.Add(decimal.MaxValue);
            vols.Add(0);
            times.Add(0);
            dates.Add(0);
        }
        public void addbar(Bar mybar)
        {
            _Count++;
            closes.Add(mybar.Close);
            opens.Add(mybar.Open);
            dates.Add(mybar.Bardate);
            highs.Add(mybar.High);
            lows.Add(mybar.Low);
            vols.Add(mybar.Volume);
            times.Add(mybar.Bartime);
        }
        long curr_barid = -1;
        int intervallength = 60;
        internal List<decimal> opens = new List<decimal>();
        internal List<decimal> closes = new List<decimal>();
        internal List<decimal> highs = new List<decimal>();
        internal List<decimal> lows = new List<decimal>();
        internal List<long> vols = new List<long>();
        internal List<int> dates = new List<int>();
        internal List<int> times = new List<int>();
        internal List<int> ticks = new List<int>();
        internal int _Count = 0;
        internal bool _isRecentNew = false;
        public Bar GetBar(int index, string symbol)
        {
            Bar b = new BarImpl();
            if ((index < 0) || (index >= _Count)) return b;
            b = new BarImpl(opens[index], highs[index], lows[index], closes[index], vols[index], dates[index], times[index], symbol);
            if (index == Last()) b.isNew = _isRecentNew;
            return b;
        }
        public Bar GetBar(string symbol) { return GetBar(Last(), symbol); }
        public void newTick(Tick k)
        {
            // ignore quotes
            if (k.trade == 0) return;
            // get the barcount
            long barid = getbarid(k.time, k.date);
            // if not current bar
            if (barid != curr_barid)
            {
                // create a new one
                newbar();
                // mark it
                _isRecentNew = true;
                // make it current
                curr_barid = barid;
                // set time
                times[times.Count - 1] = k.time;
                // set date
                dates[dates.Count - 1] = k.date;
            }
            else _isRecentNew = false;
            // blend tick into bar
            // open
            if (opens[Last()] == 0) opens[Last()] = k.trade;
            // high
            if (k.trade > highs[Last()]) highs[Last()] = k.trade;
            // low
            if (k.trade < lows[Last()]) lows[Last()] = k.trade;
            // close
            closes[Last()] = k.trade;
            // volume
            if (k.size >= 0)
                vols[Last()] += k.size;
            // notify barlist
            if (_isRecentNew)
                NewBar(k.symbol, intervallength);
        }
        public void newPoint(decimal p, int time, int date, int size)
        {

            // get the barcount
            long barid = getbarid(time,date);
            // if not current bar
            if (barid != curr_barid)
            {
                // create a new one
                newbar();
                // mark it
                _isRecentNew = true;
                // make it current
                curr_barid = barid;
                // set time
                times[times.Count - 1] = time;
                // set date
                dates[dates.Count - 1] = date;
            }
            else _isRecentNew = false;
            // blend tick into bar
            // open
            if (opens[Last()] == 0) opens[Last()] = p;
            // high
            if (p > highs[Last()]) highs[Last()] = p;
            // low
            if (p < lows[Last()]) lows[Last()] = p;
            // close
            closes[Last()] = p;
            // volume
            if (size>=0)
                vols[Last()] += size;


        }

        private long getbarid(int time, int date)
        {
            // get time elapsed to this point
            int elap = Util.FT2FTS(time);
            // get number of this bar in the day for this interval
            long bcount = (int)((double)elap / intervallength);
            // add the date to the front of number to make it unique
            bcount += (long)date * 10000;
            return bcount;
        }

    }
}
