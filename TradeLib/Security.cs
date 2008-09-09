using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public class Security
    {
        public Security(string sym, string exchange, SecurityType type)
        {
            _sym = sym;
            _destex = exchange;
            _type = type;
        }
        public Security() : this("", "", SecurityType.STK) { }
        public Security(string sym) : this(sym, "", SecurityType.STK) { }
        public Security(string sym, SecurityType type) : this(sym, "", type) { }
        protected string _sym = "";
        protected SecurityType _type = SecurityType.STK;
        protected string _destex = "";
        public virtual string Symbol { get { return _sym; } set { _sym = value; } }
        public virtual string Name { get { return _sym; } set { } }
        public string DestEx { get { return _destex; } set { _destex = value; } }
        public SecurityType Type { get { return _type; } set { _type = value; } }
        public virtual bool isValid { get { return _sym != ""; } }
        public bool hasDest { get { return _destex != ""; } }
        public override string ToString()
        {
            int t = (int)_type;
            string[] r = { _sym, t.ToString(), _destex };
            return string.Join(" ", r);
        }
        public static Security Parse(string msg)
        {
            string[] r = msg.Split(' ');
            Console.WriteLine(msg);
            Security sec = new Security();
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
    }

    public enum SecurityType
    {
        STK,
        OPT,
        FUT,
        CFD,
        FOR,
        FOP,
        WAR,
        FOX,
        IDX,
        BND,
    }
}
