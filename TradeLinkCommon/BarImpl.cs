using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{


    /// <summary>
    /// A single bar of price data, which represents OHLC and volume for an interval of time.
    /// </summary>
    public class BarImpl : TickIndicator, TradeLink.API.Bar
    {
        string _sym = "";
        public string Symbol { get { return _sym; } }
        private ulong h = ulong.MinValue;
        private ulong l = ulong.MaxValue;
        private ulong o = 0;
        private ulong c = 0;
        private long v = 0;
        private int tradesinbar = 0;
        private bool _new = false;
        private BarInterval tunits = BarInterval.FiveMin; //5min bar default
        private int bartime = 0;
        private int bardate = 0;
        private bool DAYEND = false;
        public bool DayEnd { get { return DAYEND; } }
        public ulong lHigh { get { return h; } }
        public ulong lLow { get { return l; } }
        public ulong lOpen { get { return o; } }
        public ulong lClose { get { return c; } }
        public decimal High { get { return h*Const.IPRECV; } }
        public decimal Low { get { return l * Const.IPRECV; } }
        public decimal Open { get { return o * Const.IPRECV; } }
        public decimal Close { get { return c * Const.IPRECV; } }
        public long Volume { get { return v; } }
        public bool isNew { get { return _new; } set { _new = value; } }
        public bool isValid { get { return (h >= l) && (o != 0) && (c != 0); } }
        public int TradeCount { get { return tradesinbar; } }

        public BarImpl() : this(BarInterval.FiveMin) { }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, int vol, int date, int time)
        {
            h = (ulong)(high*Const.IPREC);
            o = (ulong)(open * Const.IPREC);
            l = (ulong)(low * Const.IPREC);
            c = (ulong)(close * Const.IPREC);
            v = vol;
            bardate = date;
            bartime = time;
        }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, int date, int time, string symbol)
        {
            h = (ulong)(high * Const.IPREC);
            o = (ulong)(open * Const.IPREC);
            l = (ulong)(low * Const.IPREC);
            c = (ulong)(close * Const.IPREC);
            v = vol;
            bardate = date;
            bartime = time;
            _sym = symbol;
        }
        public BarImpl(BarImpl b)
        {
            v = b.Volume;
            h = b.lHigh;
            l = b.lLow;
            o = b.lOpen;
            c = b.lClose;
            DAYEND = b.DAYEND;
            bartime = b.bartime;
            bardate = b.bardate;
        }
        
        
        public BarImpl(BarInterval tu) 
        {
            tunits = tu;
        }
        public int Bartime { get { return bartime; } }
        public int Bardate { get { return bardate; } }
        private int BarTime(int time) 
        {
            // get time elapsed to this point
            int elap = Util.FT2FTS(time);
            // get seconds per bar
            int secperbar = (int)tunits;
            // get number of this bar in the day for this interval
            int bcount = (int)((double)elap / secperbar);
            return bcount;
        }

        /// <summary>
        /// Accepts the specified tick.
        /// </summary>
        /// <param name="t">The tick you want to add to the bar.</param>
        /// <returns>true if the tick is accepted, false if it belongs to another bar.</returns>
        public bool newTick(Tick t)
        {
            if (_sym == "") _sym = t.symbol;
            if (_sym != t.symbol) throw new InvalidTick();
            if (bartime == 0) { bartime = BarTime(t.time); bardate = t.date;}
            if (bardate != t.date) DAYEND = true;
            else DAYEND = false;
            // check if this bar's tick
            if ((BarTime(t.time) != bartime) || (bardate!=t.date)) return false; 
            // if tick doesn't have trade or index, ignore
            if (!t.isTrade && !t.isIndex) return true; 
            tradesinbar++; // count it
            _new = tradesinbar == 1;
            // only count volume on trades, not indicies
            if (!t.isIndex) v += t.size; // add trade size to bar volume
            if (o == 0) o = t.ltrade;
            if (t.ltrade > h) h = t.ltrade;
            if (t.ltrade < l) l = t.ltrade;
            c = t.ltrade;
            return true;
        }
        public override string ToString() { return "OHLC ("+bartime+") " + o + "," + h + "," + l + "," + c; }
        /// <summary>
        /// Create bar object from a CSV file providing OHLC+Volume data.
        /// </summary>
        /// <param name="record">The record in comma-delimited format.</param>
        /// <returns>The equivalent Bar</returns>
        public static Bar FromCSV(string record) { return FromCSV(record, string.Empty); }
        public static Bar FromCSV(string record,string symbol)
        {
            // google used as example
            string[] r = record.Split(',');
            if (r.Length < 6) return null;
            DateTime d = new DateTime();
            try
            {
                d = DateTime.Parse(r[0]);
            }
            catch (System.FormatException) { return null; }
            int date = (d.Year*10000)+(d.Month*100)+d.Day;
            decimal open = Convert.ToDecimal(r[1],System.Globalization.CultureInfo.InvariantCulture);
            decimal high = Convert.ToDecimal(r[2], System.Globalization.CultureInfo.InvariantCulture);
            decimal low = Convert.ToDecimal(r[3], System.Globalization.CultureInfo.InvariantCulture);
            decimal close = Convert.ToDecimal(r[4], System.Globalization.CultureInfo.InvariantCulture);
            long vol = Convert.ToInt64(r[5]);
            return new BarImpl(open,high,low,close,vol,date,0,symbol);
        }

    }


}
