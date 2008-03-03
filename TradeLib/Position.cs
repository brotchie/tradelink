using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    public class Position
    {
        public Position(string stock) { sym = stock; }
        public Position(string stock, decimal p, int s) { sym = stock; price = p; size = s; }
        public Position(decimal p, int s) { size = s; price = p; }
        public Position(int s, decimal p) { price = p; size = s; }
        public Position(Trade t) { sym = t.symbol; price = t.xprice; size = t.xsize; date = t.xdate; time = t.time; sec = t.xsec; }
        private string sym = "";
        private int size = 0;
        private decimal price = 0;
        private int date = 0;
        private int time = 0;
        private int sec = 0;
        public bool hasSymbol { get { return sym != ""; } }
        public string Symbol { get { return sym; } }
        public decimal AvgPrice { get { return price; } }
        public int Size { get { return size; } }
        public bool Side { get { return size > 0; } }
        public bool Flat { get { return size==0; } }
        public bool Adjust(Position pos)
        {
            if (this.hasSymbol && (this.Symbol != pos.Symbol)) return false;
            if ((pos.date<=this.date) || (pos.time<=this.time)) return false; //don't process trades older than last
            if (pos.Flat) return false; // nothing to do
            else if ((pos.Side && this.Side) || (!pos.Side && !this.Side)) // sides match, adding so adjust price
                this.price = ((this.price * this.size) + (pos.price * pos.size)) / (pos.size + this.size);
            this.size += pos.size; // adjust the size
            if (this.Flat) price = 0; // if we're flat after adjusting, size price back to zero
            return true;
        }
        public bool Adjust(Trade t) { return Adjust(new Position(t)); }
        public static int Norm2Min(int size)
        {
            int wmult = (int)Math.Ceiling((decimal)size / 100);
            return wmult * 100;
        }
    }
}
