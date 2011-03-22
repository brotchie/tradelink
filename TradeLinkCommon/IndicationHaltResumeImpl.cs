using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// receive indication information via tradelink
    /// </summary>
    public struct IndicationImpl : Indication
    {
        public IndicationImpl(string sym, string ex, int time, int validity, decimal high, decimal low)
        {
            _sym = sym;
            _ex = ex;
            _high = high;
            _low = low;
            _time = time;
            _validity = (validity == 1);
        }
        string _sym;
        string _ex;
        int _time;
        bool _validity;
        decimal _high;
        decimal _low;

        public string Symbol { get { return _sym; } }
        public string Exchange { get { return _ex; } }
        public int Time { get { return _time; } }
        public bool isValid { get { return _validity; } }
        public decimal High { get { return _high; } }
        public decimal Low { get { return _low; } }

        public static string Serialize(Indication i)
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(i.Symbol);
            sb.Append(d);
            sb.Append(i.Exchange);
            sb.Append(d);
            sb.Append(i.Time);
            sb.Append(d);
            sb.Append(i.isValid);
            sb.Append(d);
            sb.Append(i.High);
            sb.Append(d);
            sb.Append(i.Low);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static Indication Deserialize(string msg)
        {
            IndicationImpl i = new IndicationImpl();
            string[] r = msg.Split(',');

            i._sym = r[0];
            i._ex = r[1];

            int t = 0;
            if (int.TryParse(r[2], out t))
                i._time = t;
            
            bool v = false;
            if (bool.TryParse(r[3], out v))
                i._validity = v;

            decimal d = 0;
            if (decimal.TryParse(r[4], out d))
                i._high = d;
            if (decimal.TryParse(r[5], out d))
                i._low = d;
            
            return i;
        }
    }

    /// <summary>
    /// receive indication information via tradelink
    /// </summary>
    public struct HaltResumeImpl : HaltResume
    {
        public HaltResumeImpl(string sym, string ex, int time, string status, string reason)
        {
            _sym = sym;
            _ex = ex;
            _time = time;
            _status = status;
            _reason = reason;
        }
        string _sym;
        string _ex;
        int _time;
        string _status;
        string _reason;

        public string Symbol { get { return _sym; } }
        public string Exchange { get { return _ex; } }
        public int Time { get { return _time; } }
        public string Status { get { return _status; } }
        public string Reason { get { return _reason; } }

        public static string Serialize(HaltResume i)
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(i.Symbol);
            sb.Append(d);
            sb.Append(i.Exchange);
            sb.Append(d);
            sb.Append(i.Time);
            sb.Append(d);
            sb.Append(i.Status);
            sb.Append(d);
            sb.Append(i.Reason);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Serialize(this);
        }

        public static HaltResume Deserialize(string msg)
        {
            HaltResumeImpl i = new HaltResumeImpl();
            string[] r = msg.Split(',');

            i._sym = r[0];
            i._ex = r[1];

            int t = 0;
            if (int.TryParse(r[2], out t))
                i._time = t;

            i._status = r[3];
            i._reason = r[4];

            return i;
        }
    }
}
