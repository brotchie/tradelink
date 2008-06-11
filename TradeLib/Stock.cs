using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TradeLib
{
    /// <summary>
    /// Used to hold information specific to a particular equity instrument, eg name date-traded,etc.
    /// </summary>
    [Serializable]
    public class Stock : Instrument
    {
        public override Security SecurityType
        {
            get { return Security.STK;  }
        }
        public Stock(string s) 
        {
            if (!Stock.isStock(s)) return;
            symbol = s;
        }
        public Stock(string symbol, int Date) : this(symbol) { date = Date; }
        private string symbol = null;
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
        public string Symbol { get { return symbol; }  }
        public override string Name { get { return Symbol; } set { } }
        public override bool isValid { get { return (symbol != null) && isStock(symbol); } }
        /// <summary>
        /// Test if symbol is valid stock name
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="val">whether symbol passes the stock name test.</param>
        /// <returns></returns>
        public static bool isStock(string sym)
        {
            Regex sfilt = new Regex("^[A-Z]{1,4}$");
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
        public bool hasHistorical { get { return (_histfile != null); } }
        private System.IO.StreamReader _histfile = null;
        /// <summary>
        /// Fetches next historical tick for stock, or invalid tick if no historical data is available.
        /// </summary>
        public Tick NextTick { get { if (!hasHistorical) return new Tick(); return (Tick)eSigTick.FromStream(_histfile); } }
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
