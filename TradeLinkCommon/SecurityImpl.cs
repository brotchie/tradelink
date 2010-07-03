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
        /// <summary>
        /// create new security
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="exchange"></param>
        /// <param name="type"></param>
        public SecurityImpl(string sym, string exchange, SecurityType type)
        {
            _sym = sym;
            _destex = exchange;
            _type = type;
        }
        /// <summary>
        /// clone a security
        /// </summary>
        /// <param name="copy"></param>
        public SecurityImpl(Security copy)
        {
            _sym = copy.Symbol;
            _strike = copy.Strike;
            _type = copy.Type;
            _destex = copy.DestEx;
            _date = copy.Date;
            _details = copy.Details;
        }
        /// <summary>
        /// create new security
        /// </summary>
        public SecurityImpl() : this("", "", SecurityType.NIL) { }
        /// <summary>
        /// create new security
        /// </summary>
        /// <param name="sym"></param>
        public SecurityImpl(string sym) : this(sym, "", SecurityType.STK) { }
        /// <summary>
        /// create new security
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="type"></param>
        public SecurityImpl(string sym, SecurityType type) : this(sym, "", type) { }
        string _sym = "";
        SecurityType _type = SecurityType.NIL;
        string _destex = "";
        /// <summary>
        /// whether security has a defined security type
        /// </summary>
        public bool hasType { get { return _type != SecurityType.NIL; } }
        /// <summary>
        /// symbol associated with security
        /// </summary>
        public virtual string Symbol { get { return _sym; } set { _sym = value; } }
        /// <summary>
        /// name (symbol) of security
        /// </summary>
        public virtual string Name { get { return _sym; } set { } }
        /// <summary>
        /// exchange associated with security
        /// </summary>
        public string DestEx { get { return _destex; } set { _destex = value; } }
        /// <summary>
        /// type of security
        /// </summary>
        public SecurityType Type { get { return _type; } set { _type = value; } }
        /// <summary>
        /// whether security is valid
        /// </summary>
        public virtual bool isValid { get { return _sym != ""; } }
        /// <summary>
        /// whether security has a exchange
        /// </summary>
        public bool hasDest { get { return _destex != ""; } }
        /// <summary>
        /// full name of security including all options
        /// </summary>
        public string FullName { get { return ToString(); } }
        string ts { get { return _type == SecurityType.NIL ? "" : _type.ToString(); } }
        string _details = string.Empty;
        /// <summary>
        /// details of security (eg PUT/CALL for options)
        /// </summary>
        public string Details { get { return _details; } set { _details = value; } }
        double _strike = 0;
        /// <summary>
        /// strike price for options
        /// </summary>
        public double Strike { get { return _strike; } set { _strike = value; } }
        /// <summary>
        /// printable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Serialize(this);
        }
        /// <summary>
        /// serialize security as a string
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static string Serialize(Security sec)
        {
            List<string> p = new List<string>();
            p.Add(sec.Symbol);
            if (sec.Type == SecurityType.OPT)
            {
                p.Add(sec.Date.ToString());
                p.Add(sec.Details);
                p.Add(sec.Strike.ToString("F4"));
            }
            if (sec.hasDest) p.Add(sec.DestEx);
            if ((sec.Type!= SecurityType.NIL) && (sec.Type!= SecurityType.STK))
                p.Add(sec.Type.ToString());
            return string.Join(" ", p.ToArray());
        }
        /// <summary>
        /// get a security from a user-specified string
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static SecurityImpl Parse(string msg) { return Parse(msg, 0); }
        /// <summary>
        /// get a security form a user-specified string
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static SecurityImpl Parse(string msg, int date)
        {
            string[] r = msg.Split(' ');
            SecurityImpl sec = new SecurityImpl();
            sec.Symbol = r[0];
            // look for option first
            if (msg.Contains("OPT") || msg.Contains("PUT") || msg.Contains("CALL"))
            {
                sec.Type = SecurityType.OPT;
                msg = msg.ToUpper();
                sec.Details = msg.Contains("PUT") ? "PUT" : "CALL" ;
                msg = msg.Replace("CALL", "");
                msg = msg.Replace("PUT", "");
                msg = msg.Replace("OPT", "");
                r = msg.Split(' ');
                sec.Symbol = r[0];
                sec.Date = ExpirationDate(ref r);
                sec.Strike = StrikePrice(ref r);
                sec.DestEx = Ex(sec.Symbol,ref r);

            }
            else if (r.Length > 2)
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
            if (date!=0)
                sec.Date = date;
            if (sec.hasDest && !sec.hasType)
                sec.Type = TypeFromExchange(sec.DestEx);
            return sec;
        }

        public static bool SecurityExpiration(string longdate, out int date)
        {
            DateTime dt;
            if (DateTime.TryParse(longdate, out dt))
            {
                string tmpdate = string.Format("{0:yyyyMM}", dt);
                if (int.TryParse(tmpdate, out date))
                    return true;
            }
            date = 0;
            return false;
        }

        /// <summary>
        /// deserialize a security from a string
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Security Deserialize(string msg)
        {
            return Parse(msg);
        }

        static int ExpirationDate(ref string [] tests)
        {
            int date = 0;
            for (int i = 0; i < tests.Length; i++)
            {
                string test = tests[i];
                if (int.TryParse(test, out date))
                {
                    tests[i] = string.Empty;
                    return date;
                }
            }
            return 0;
        }

        static double StrikePrice(ref  string[]  tests)
        {
            double p = 0;
            for (int i = 0; i< tests.Length; i++)
            {
                string test = tests[i];
                if (double.TryParse(test, out p))
                {
                    tests[i] = string.Empty;
                    return p;
                }
            }
            return 0;
        }

        static string Ex(string sym, ref string[] tests)
        {
            for (int i = 0; i < tests.Length; i++)
                if ((tests[i] !=sym) && (tests[i] != string.Empty))
                    return tests[i];
            return string.Empty;
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
        /// <summary>
        /// date associated with security
        /// </summary>
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
        /// <summary>
        /// historical source of tick data for security
        /// </summary>
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
        /// <summary>
        /// approximate # of ticks contained in historical security
        /// </summary>
        public int ApproxTicks { get { return _approxticks; } set { _approxticks = value; } }
        /// <summary>
        /// load a security from a historical tick file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
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
        /// <summary>
        /// test whether two securities are equal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return ToString() == obj.ToString();
        }

        /// <summary>
        /// determine security from the filename, without opening file
        /// (use SecurityImpl.FromFile to actually read it in)
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SecurityImpl SecurityFromFileName(string filename)
        {
            try
            {
                filename = System.IO.Path.GetFileName(filename);
                string ds = System.Text.RegularExpressions.Regex.Match(filename, "([0-9]{8})[.]", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Result("$1");
                string sym = filename.Replace(ds, "").Replace(TikConst.DOT_EXT, "");
                SecurityImpl s = new SecurityImpl(sym);
                s.Date = Convert.ToInt32(ds);
                return s;
            }
            catch (Exception) { }
            return new SecurityImpl();

        }
    }

    public class EndSecurityTicks : Exception {}


}
