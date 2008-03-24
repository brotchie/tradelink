using System;

namespace TradeLib
{

    public class Index : Instrument
    {
        public static bool isIdx(string sym)
        {
            return (sym.StartsWith("/") || sym.StartsWith("$"));
        }
        public override bool isValid { get { return Index.isIdx(this.name); } }
        public Index(Index copythisidx)
        {
            Name = copythisidx.Name;
            last = copythisidx.Value;
            open = copythisidx.open;
            high = copythisidx.high;
            low = copythisidx.low;
            close = copythisidx.close;
            date = copythisidx.date;
            time = copythisidx.time;
        }
        public Index(string symbol) : this(symbol, 0, 0, 0, 1000000, 0,0,0) { }
        public Index(string symbol, decimal tick, decimal o, decimal h, decimal l, decimal c) : this(symbol, tick, o, h, l, c, 0, 0) { }
        public Index(string symbol, decimal tick, decimal o, decimal h, decimal l, decimal c, int date, int time)
        {
            Name = symbol;
            last = tick;
            open = o;
            high = h;
            low = l;
            close = c;
            Date = date;
            Time = time;
        }
        string name = "";
        int time = 0;
        int date = 0;
        decimal open = 0;
        decimal high = 0;
        decimal low = 10000000;
        decimal close = 0;
        decimal last = 0;
        public int Date { get { return date; } set { date = value; } }
        public int Time { get { return time; } set { time = value; } }
        public override string Name { get { return name; } set { if (isIdx(value)) name = value; } }
        public decimal Value { get { return last; } }
        public decimal Open { get { return open; } }
        public decimal High { get { return high; } }
        public decimal Low { get { return low; } }
        public decimal Close { get { return close; } }
        enum iorder
        {
            sym = 0,
            date,
            time,
            value,
            open,
            high,
            low,
            close,
        }

        public static string Serialize(Index i)
        {
            string s = "";
            s = i.Name+","+i.Date + "," + i.Time + "," + i.Value + "," + i.Open + "," + i.High + "," + i.Low + "," + i.Close + ",";
            return s;
        }
        public static Index Deserialize(string val)
        {
            string[] r = val.Split(',');
            Index i = null;
            try
            {
                i = new Index(r[(int)iorder.sym], Convert.ToDecimal(r[(int)iorder.value]), Convert.ToDecimal(r[(int)iorder.open]), Convert.ToDecimal(r[(int)iorder.high]), Convert.ToDecimal(r[(int)iorder.low]), Convert.ToDecimal(r[(int)iorder.close]), Convert.ToInt32(r[(int)iorder.date]), Convert.ToInt32(r[(int)iorder.time]));
            }
            catch (InvalidCastException) { }
            return i;
        }

        public static string ToTLmsg(Index i)
        {
            string s = "";
            s = i.Name + ","+i.Value + "," + i.Open + "," + i.High + "," + i.Low + "," + i.Close + ",";
            return s;
        }
        
        public Tick ToTick()
        {
        	Tick t = new Tick(Name);
        	t.time = this.Time;
        	t.date = this.Date;
        	t.trade = this.Value;
            t.size = -1;
        	return t;
        }

        public static Index FromTLmsg(string index, string val)
        {
            string[] r = val.Split(',');
            Index i = new Index(index);
            try
            {
                i = new Index(index, Convert.ToDecimal(r[1]), Convert.ToDecimal(r[2]), Convert.ToDecimal(r[3]), Convert.ToDecimal(r[4]), Convert.ToDecimal(r[5]));
            }
            catch (Exception) { return null; }
            return i;
        }
    }
}
