using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    public class SecurityImpl : Security
    {
        public SecurityImpl(string sym, string exchange, SecurityType type)
        {
            _sym = sym;
            _destex = exchange;
            _type = type;
        }
        public SecurityImpl() : this("", "", SecurityType.STK) { }
        public SecurityImpl(string sym) : this(sym, "", SecurityType.STK) { }
        public SecurityImpl(string sym, SecurityType type) : this(sym, "", type) { }
        protected string _sym = "";
        protected SecurityType _type = SecurityType.STK;
        protected string _destex = "";
        public virtual string Symbol { get { return _sym; } set { _sym = value; } }
        public virtual string Name { get { return _sym; } set { } }
        public string DestEx { get { return _destex; } set { _destex = value; } }
        public SecurityType Type { get { return _type; } set { _type = value; } }
        public virtual bool isValid { get { return _sym != ""; } }
        public bool hasDest { get { return _destex != ""; } }
        public string FullName { get { return ToString(); } }
        public override string ToString()
        {
            string[] r = { _sym, _type.ToString(), _destex };
            if (_type!= SecurityType.STK) return string.Join(" ", r);
            return _sym;
        }
        public string Serialize()
        {
            int t = (int)_type;
            string[] r = { _sym, t.ToString(), _destex };
            return string.Join(" ", r);
        }
        public static SecurityImpl Parse(string msg) { return Parse(msg, 0); }
        public static SecurityImpl Parse(string msg, int date)
        {
            string[] r = msg.Split(' ');
            SecurityImpl sec = new SecurityImpl();
            sec.Symbol = r[0];
            if (r.Length > 2)
            {
                int f2id = SecurityID(r[2]);
                int f1id = SecurityID(r[1]);
                if (f2id != -1)
                {
                    sec.Type = (SecurityType)f2id;
                    sec.DestEx = r[1];
                }
                else if (f1id != -1)
                {
                    sec.Type = (SecurityType)f1id;
                    sec.DestEx = r[2];
                }
            }
            else if (r.Length > 1)
            {
                int f1id = SecurityID(r[1]);
                if (f1id != -1) sec.Type = (SecurityType)f1id;
            }
            sec.Date = date;
            return sec;
        }

        static int SecurityID(string type)
        {
            int id = -1;
            try
            {
                id = (int)(SecurityType)Enum.Parse(typeof(SecurityType), type);
            }
            catch (Exception) { }
            return id;
        }
        int _date = 0;
        public int Date { get { return _date; } set { _date = value; } }
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode() + _date;
        }
        /// <summary>
        /// Says whether stock contains historical data.
        /// </summary>
        public bool hasHistorical { get { return (_histfile != null) && !_histfile.EndOfStream; } }
        private System.IO.StreamReader _histfile = null;
        /// <summary>
        /// Fetches next historical tick for stock, or invalid tick if no historical data is available.
        /// </summary>
        public TickImpl NextTick
        {
            get
            {
                if (!hasHistorical) return new TickImpl();
                TickImpl t = new TickImpl();
                do
                {
                    t = (TickImpl)eSigTick.FromStream(Symbol, _histfile);
                } while ((!t.isValid) && hasHistorical);
                if (!t.isValid) { _histfile.Close(); _histfile = null; }
                return t;
            }
        }
        /// <summary>
        /// Initializes a security with historical data from tick archive file
        /// </summary>
        public static SecurityImpl FromFile(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            SecurityImpl s = eSigTick.InitEpf(sr);
            s._histfile = sr;
            return s;
        }
    }


}
