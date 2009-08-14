using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// used to hold and work with securities.
    /// Supported Securities : Stocks, Futures, Forex.
    /// Options, Bonds, Swaps, etc are supported on certain brokers.
    /// </summary>
    public class SecurityImpl : Security
    {
        public SecurityImpl(string sym, string exchange, SecurityType type)
        {
            _sym = sym;
            _destex = exchange;
            _type = type;
        }
        public SecurityImpl() : this("", "", SecurityType.NIL) { }
        public SecurityImpl(string sym) : this(sym, "", SecurityType.STK) { }
        public SecurityImpl(string sym, SecurityType type) : this(sym, "", type) { }
        protected string _sym = "";
        protected SecurityType _type = SecurityType.NIL;
        protected string _destex = "";
        public bool hasType { get { return _type != SecurityType.NIL; } }
        public virtual string Symbol { get { return _sym; } set { _sym = value; } }
        public virtual string Name { get { return _sym; } set { } }
        public string DestEx { get { return _destex; } set { _destex = value; } }
        public SecurityType Type { get { return _type; } set { _type = value; } }
        public virtual bool isValid { get { return _sym != ""; } }
        public bool hasDest { get { return _destex != ""; } }
        public string FullName { get { return ToString(); } }
        string ts { get { return _type == SecurityType.NIL ? "" : _type.ToString(); } }
        public override string ToString()
        {
            return Serialize(this);
        }
        public static string Serialize(Security sec)
        {
            List<string> p = new List<string>();
            p.Add(sec.Symbol);
            if (sec.hasDest) p.Add(sec.DestEx);
            if ((sec.Type!= SecurityType.NIL) && (sec.Type!= SecurityType.STK))
                p.Add(sec.Type.ToString());
            return string.Join(" ", p.ToArray());
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
                if (f1id != -1)
                    sec.Type = (SecurityType)f1id;
                else sec.DestEx = r[1];
            }
            else
                sec.Type = SecurityType.STK;
            sec.Date = date;
            if (sec.hasDest && !sec.hasType)
                sec.Type = TypeFromExchange(sec.DestEx);
            return sec;
        }

        static SecurityType TypeFromExchange(string ex)
        {
            if ((ex == "GLOBEX") || (ex == "NYMEX") || (ex == "CFE"))
                return SecurityType.FUT;
            else if ((ex == "NYSE") || (ex == "NASDAQ") || (ex == "ARCA"))
                return SecurityType.STK;
            // default to STK if not sure
            return 0;

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
        bool _hashist = false;
        /// <summary>
        /// Says whether stock contains historical data.
        /// </summary>
        public bool hasHistorical { get { return _hashist; } }
        public TikReader HistSource;
        /// <summary>
        /// Fetches next historical tick for stock, or invalid tick if no historical data is available.
        /// </summary>
        public bool NextTick()
        {
            if (HistSource == null) return false;
            bool v = true;
            try
            {
                v = HistSource.NextTick();
            }
            catch (System.IO.EndOfStreamException)
            {
                HistSource.Close();
            }
            catch (System.IO.IOException) { }
            return v;
        }
        int _approxticks = 0;
        public int ApproxTicks { get { return _approxticks; } set { _approxticks = value; } }

        public static SecurityImpl FromTIK(string filename)
        {
            TikReader tr = new TikReader(filename);
            SecurityImpl s = (SecurityImpl)tr.ToSecurity();
            if (s.isValid && tr.isValid)
            {
                s._hashist = true;
                s.HistSource = tr;
                s._approxticks = s.HistSource.ApproxTicks;
            }
            return s;
        }

        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }
    }

    public class EndSecurityTicks : Exception {}


}
