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
        public Stock(string s) { Load(s); }
        public Stock(string symbol, int Date) { Load(symbol); date = Date; }
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
        public string Symbol { get { return symbol; } }
        public override string Name { get { return Symbol; } set { } }
        public override bool isValid { get { return (symbol != null) && isStock(symbol); } }
        public virtual bool Load(string sym)
        {
            sym = sym.ToUpper();
            if (!isStock(sym)) return false;
            symbol = sym;
            return true;
        }
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

    }
}
