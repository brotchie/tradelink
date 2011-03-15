using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{


    /// <summary>
    /// A single bar of price data, which represents OHLC and volume for an interval of time.
    /// </summary>
    public class BarImpl : GotTickIndicator, TradeLink.API.Bar
    {
        public void GotTick(Tick k) { newTick(k); }
        string _sym = "";
        public string Symbol { get { return _sym; } }
        private ulong h = ulong.MinValue;
        private ulong l = ulong.MaxValue;
        private ulong o = 0;
        private ulong c = 0;
        private long v = 0;
        private int tradesinbar = 0;
        private bool _new = false;
        private int units = 300;
        private int _time = 0;
        private int bardate = 0;
        private bool DAYEND = false;
        public int time { get { return _time; } }
        public bool DayEnd { get { return DAYEND; } }
        ulong lHigh { get { return h; } }
        ulong lLow { get { return l; } }
        ulong lOpen { get { return o; } }
        ulong lClose { get { return c; } }
        public decimal High { get { return h*Const.IPRECV; } }
        public decimal Low { get { return l * Const.IPRECV; } }
        public decimal Open { get { return o * Const.IPRECV; } }
        public decimal Close { get { return c * Const.IPRECV; } }
        public long Volume { get { return v; } }
        public bool isNew { get { return _new; } set { _new = value; } }
        public bool isValid { get { return (h >= l) && (o != 0) && (c != 0); } }
        public int TradeCount { get { return tradesinbar; } }

        public BarImpl() : this(BarInterval.FiveMin) { }

        public int Interval { get { return units; } }
        //public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, int date, int time, string symbol) : this(open, high, low, close, vol, date, time, symbol) { }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, int date, int time, string symbol, int interval)
        {
            if (open < 0 || high < 0 || low < 0 || close < 0)
            {
                return;
            }
            else
            {
                units = interval;
                h = (ulong)(high * Const.IPREC);
                o = (ulong)(open * Const.IPREC);
                l = (ulong)(low * Const.IPREC);
                c = (ulong)(close * Const.IPREC);
                v = vol;
                bardate = date;
                _time = time;
                _sym = symbol;
            }
        }
        public BarImpl(BarImpl b)
        {
            v = b.Volume;
            h = b.lHigh;
            l = b.lLow;
            o = b.lOpen;
            c = b.lClose;
            DAYEND = b.DAYEND;
            _time = b._time;
            bardate = b.bardate;
        }
        
        
        public BarImpl(BarInterval tu) 
        {
            units = (int)tu;
        }
        public int Bartime 
        { 
            get 
            { 
                // get num of seconds elaps
                int elap = Util.FT2FTS(_time); 
                // get remainder of dividing by interval
                int rem = elap % Interval;
                // get datetime
                DateTime dt = Util.TLD2DT(bardate);
                // add rounded down result
                dt = dt.AddSeconds(elap-rem);
                // conver back to normal time
                int bt = Util.ToTLTime(dt);
                return bt;
            } 
        }
        public int Bardate { get { return bardate; } }
        private int bt(int time) 
        {
            // get time elapsed to this point
            int elap = Util.FT2FTS(time);
            // get seconds per bar
            int secperbar = Interval;
            // get number of this bar in the day for this interval
            int bcount = (int)((double)elap / secperbar);
            return bcount;
        }

        /// <summary>
        /// Accepts the specified tick.
        /// </summary>
        /// <param name="t">The tick you want to add to the bar.</param>
        /// <returns>true if the tick is accepted, false if it belongs to another bar.</returns>
        public bool newTick(Tick k)
        {
            TickImpl t = (TickImpl)k;
            if (_sym == "") _sym = t.symbol;
            if (_sym != t.symbol) throw new InvalidTick();
            if (_time == 0) { _time = bt(t.time); bardate = t.date;}
            if (bardate != t.date) DAYEND = true;
            else DAYEND = false;
            // check if this bar's tick
            if ((bt(t.time) != _time) || (bardate!=t.date)) return false; 
            // if tick doesn't have trade or index, ignore
            if (!t.isTrade && !t.isIndex) return true; 
            tradesinbar++; // count it
            _new = tradesinbar == 1;
            // only count volume on trades, not indicies
            if (!t.isIndex) v += t.size; // add trade size to bar volume
            if (o == 0) o = t._trade;
            if (t._trade > h) h = t._trade;
            if (t._trade < l) l = t._trade;
            c = t._trade;
            return true;
        }
        public override string ToString() { return "OHLC (" + _time + ") " + Open.ToString("F2") + "," + High.ToString("F2") + "," + Low.ToString("F2") + "," + Close.ToString("F2"); }
        /// <summary>
        /// Create bar object from a CSV file providing OHLC+Volume data.
        /// </summary>
        /// <param name="record">The record in comma-delimited format.</param>
        /// <returns>The equivalent Bar</returns>
        public static Bar FromCSV(string record) { return FromCSV(record, string.Empty, (int)BarInterval.Day); }
        public static Bar FromCSV(string record,string symbol,int interval)
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
            return new BarImpl(open,high,low,close,vol,date,0,symbol,interval);
        }

        public static string Serialize(Bar b)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(b.Open.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.High.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Low.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Close.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Volume);
            sb.Append(d);
            sb.Append(b.Bardate);
            sb.Append(d);
            sb.Append(b.time);
            sb.Append(d);
            sb.Append(b.Symbol);
            sb.Append(d);
            sb.Append(b.Interval.ToString(System.Globalization.CultureInfo.InvariantCulture));
            
            return sb.ToString();
        }

        public static Bar Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            decimal open = Convert.ToDecimal(r[0], System.Globalization.CultureInfo.InvariantCulture);
            decimal high = Convert.ToDecimal(r[1], System.Globalization.CultureInfo.InvariantCulture);
            decimal low = Convert.ToDecimal(r[2], System.Globalization.CultureInfo.InvariantCulture);
            decimal close = Convert.ToDecimal(r[3], System.Globalization.CultureInfo.InvariantCulture);
            long vol = Convert.ToInt64(r[4]);
            int date = Convert.ToInt32(r[5]);
            int time = Convert.ToInt32(r[6]);
            string symbol = r[7];
            int interval = Convert.ToInt32(r[8]);
            return new BarImpl(open, high, low, close, vol, date, time, symbol,interval);
        }

        /// <summary>
        /// convert a bar into an array of ticks
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        public static Tick[] ToTick(Bar bar)
        {
            if (!bar.isValid) return new Tick[0];
            List<Tick> list = new List<Tick>();
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, bar.Open,
(int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
bar.High, (int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, bar.Low,
(int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
bar.Close, (int)((double)bar.Volume / 4), string.Empty));
            return list.ToArray();
        }

        /// <summary>
        /// parses message into a structured bar request
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BarRequest ParseBarRequest(string msg)
        {
            string[] r = msg.Split(',');
            BarRequest br  = new BarRequest();
            br.Symbol = r[(int)BarRequestField.Symbol];
            br.Interval = Convert.ToInt32(r[(int)BarRequestField.BarInt]);
            br.StartDate = int.Parse(r[(int)BarRequestField.StartDate]);
            br.StartTime = int.Parse(r[(int)BarRequestField.StartTime]);
            br.EndDate= int.Parse(r[(int)BarRequestField.EndDate]);
            br.EndTime = int.Parse(r[(int)BarRequestField.EndTime]);
            br.CustomInterval = int.Parse(r[(int)BarRequestField.CustomInterval]);
            br.ID = long.Parse(r[(int)BarRequestField.ID]);
            br.Client = r[(int)BarRequestField.Client];
            return br;
        }

        /// <summary>
        /// request historical data for today
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string BuildBarRequest(string symbol, BarInterval interval)
        {
            return BuildBarRequest(new BarRequest(symbol, (int)interval, Util.ToTLDate(), 0, Util.ToTLDate(), Util.ToTLTime(),string.Empty));
        }
        /// <summary>
        /// bar request for symbol and interval from previous date through present time
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="startdate"></param>
        /// <returns></returns>
        public static string BuildBarRequest(string symbol, BarInterval interval, int startdate)
        {
            return BuildBarRequest(new BarRequest(symbol, (int)interval, startdate, 0, Util.ToTLDate(), Util.ToTLTime(),string.Empty));
        }
        public static string BuildBarRequest(string symbol, int interval, int startdate)
        {
            return BuildBarRequest(new BarRequest(symbol, interval, startdate, 0, Util.ToTLDate(), Util.ToTLTime(), string.Empty));
        }
        /// <summary>
        /// builds bar request
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static string BuildBarRequest(BarRequest br)
        {
            string[] r = new string[] 
            {
                br.Symbol,
                br.Interval.ToString(),
                br.StartDate.ToString(),
                br.StartTime.ToString(),
                br.EndDate.ToString(),
                br.EndTime.ToString(),
                br.ID.ToString(),
                br.CustomInterval.ToString(),
                br.Client,
            };
            return string.Join(",", r);
            
        }

        public static DateTime DateFromBarsBack(int barsback, BarInterval intv) { return DateFromBarsBack(barsback, intv, DateTime.Now); }
        public static DateTime DateFromBarsBack(int barsback, BarInterval intv, DateTime enddate) { return DateFromBarsBack(barsback, (int)intv, enddate); }
        public static DateTime DateFromBarsBack(int barsback, int interval) { return DateFromBarsBack(barsback, interval, DateTime.Now); }
        public static DateTime DateFromBarsBack(int barsback, int interval, DateTime enddate)
        {
           return enddate.Subtract(new TimeSpan(0,0,interval*barsback));
        }

        public static int BarsBackFromDate(BarInterval interval, int startdate) { return BarsBackFromDate(interval, startdate, Util.ToTLDate()); }
        public static int BarsBackFromDate(BarInterval interval, int startdate, int enddate) { return BarsBackFromDate(interval, Util.ToDateTime(startdate, 0), Util.ToDateTime(enddate,Util.ToTLTime())); }
        public static int BarsBackFromDate(BarInterval interval, DateTime startdate, DateTime enddate)
        {
            double start2endseconds = enddate.Subtract(startdate).TotalSeconds;
            int bars = (int)((double)start2endseconds / (int)interval);
            return bars;
        }
        /// <summary>
        /// build bar request for certain # of bars back from present
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="barsback"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string BuildBarRequestBarsBack(string sym, int barsback, int interval)
        {
            DateTime n = DateTime.Now;
            return BarImpl.BuildBarRequest(new BarRequest(sym, interval, Util.ToTLDate(BarImpl.DateFromBarsBack(barsback, interval, n)), Util.ToTLTime(BarImpl.DateFromBarsBack(barsback, interval, n)), Util.ToTLDate(n), Util.ToTLTime(n), string.Empty));
        }

        
    }

    public struct BarRequest
    {
        /// <summary>
        /// client making request
        /// </summary>
        public string Client;
        public int StartDate;
        public int EndDate;
        public int StartTime;
        public int EndTime;
        public int CustomInterval;
        public string Symbol;
        public int Interval;
        public long ID;
        public DateTime StartDateTime { get { return Util.ToDateTime(StartDate,StartTime); } }
        public DateTime EndDateTime { get { return Util.ToDateTime(EndDate, EndTime); } }
        public BarRequest(string symbol, int interval, int startdate, int starttime, int enddate, int endtime, string client)
        {
            Client = client;
            Symbol = symbol;
            Interval = interval;
            StartDate = startdate;
            StartTime = starttime;
            EndDate = enddate;
            EndTime = endtime;
            ID = 0;
            CustomInterval = 0;

        }

        public override string ToString()
        {
            return Symbol + " " + Interval + " " + StartDateTime + "->" + EndDateTime;
        }
        
    }


}
