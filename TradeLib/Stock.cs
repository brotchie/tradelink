using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TradeLib
{
    [Serializable]
    public class Stock
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
        public virtual bool isValid { get { return (symbol != null) && isStock(symbol); } }
        public virtual bool Load(string sym)
        {
            sym = sym.ToUpper();
            if (!isStock(sym)) return false;
            symbol = sym;
            return true;
        }
        public static bool isStock(string sym)
        {
            Regex sfilt = new Regex("[$/]{0,1}[a-z]+", RegexOptions.IgnoreCase);
            if (!sfilt.IsMatch(sym)) return false; // invalid symbol
            return true;
        }
        public override string ToString()
        {
            return Symbol;
        }

    }
}
