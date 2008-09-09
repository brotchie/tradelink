using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TradeLib
{
    /// <summary>
    /// Used to hold information specific to a particular equity instrument, eg name date-traded,etc.
    /// </summary>
    [Serializable]
    public class Stock : Security
    {
        public Stock(string s) 
        {
            _type = SecurityType.STK;
            if (!Stock.isStock(s)) return;
            _sym = s;
        }
        public Stock(string symbol, int Date) : this(symbol) { date = Date; }
        private decimal dh = 0;
        private decimal dl = 10000000000;
        private decimal dop = 0;
        private decimal yc = 0;
        private decimal tc = 0;
        private int date = 0;
        public int Date { get { return date; } set { date = value; } }
        public decimal DayHigh { get { return dh; } set { dh = value; } }
        public decimal DayLow { get { return dl; } set { dl = value; } }
        public decimal DayOpen { get { return dop; } set { dop = value; } }
        public decimal DayClose { get { return tc; } set { tc = value; } }
        public decimal YestClose { get { return yc; } set { yc = value; } }
        public override string Name { get { return Symbol; } set { } }
        public override bool isValid { get { return (_sym != null) && isStock(_sym); } }
        public override int GetHashCode()
        {
            return _sym.GetHashCode() + date;
        }
        /// <summary>
        /// Test if symbol is valid stock name
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="val">whether symbol passes the stock name test.</param>
        /// <returns></returns>
        public static bool isStock(string sym)
        {
            Regex sfilt = new Regex("^[A-Z0-9 ]{1,20}$");
            bool match = sfilt.IsMatch(sym.ToUpper()); 
            return match;
        }
        public override string ToString()
        {
            return Symbol;
        }
        /// <summary>
        /// Says whether stock contains historical data.
        /// </summary>
        public bool hasHistorical { get { return (_histfile != null) && !_histfile.EndOfStream; } }
        private System.IO.StreamReader _histfile = null;
        /// <summary>
        /// Fetches next historical tick for stock, or invalid tick if no historical data is available.
        /// </summary>
        public Tick NextTick 
        { 
            get 
            { 
                if (!hasHistorical) return new Tick(); 
                Tick t = (Tick)eSigTick.FromStream(Symbol, _histfile); 
                if (!t.isValid) _histfile.Close(); 
                return t; 
            } 
        }
        /// <summary>
        /// Initializes a stock object with historical data from tick archive file
        /// </summary>
        public static Stock FromFile(string filename)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(filename);
            Stock s = eSigTick.InitEpf(sr);
            s._histfile = sr;
            return s;
        }


    }
}
