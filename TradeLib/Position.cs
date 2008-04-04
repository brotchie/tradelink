using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    /// <summary>
    /// A position type used to describe the position in a stock or instrument.
    /// </summary>
    public class Position
    {
        public Position(string stock) { sym = stock; }
        public Position(string stock, decimal p, int s) { sym = stock; price = p; size = s; }
        public Position(Trade t) 
        {
            if (!t.isValid) throw new Exception("Can't construct a position object from invalid trade.");
            sym = t.symbol; price = t.xprice; size = t.xsize; date = t.xdate; time = t.time; sec = t.xsec;
            if (size>0) size *= t.side ? 1 : -1;
        }
        private string sym = "";
        private int size = 0;
        private decimal price = 0;
        private int date = 0;
        private int time = 0;
        private int sec = 0;
        public bool isValid
        {
            get { return hasSymbol && (((AvgPrice == 0) && (Size == 0)) || ((AvgPrice != 0) && (Size != 0))); }
        }
        public bool hasSymbol { get { return sym != ""; } }
        public string Symbol { get { return sym; } }
        public decimal Price { get { return price; } }
        public decimal AvgPrice { get { return price; } }
        public int Size { get { return size; } }
        public bool Side { get { return size > 0; } }
        public bool Flat { get { return size==0; } }
        // returns any closed PL calculated on position basis (not per share)
        /// <summary>
        /// Adjusts the position by applying a new position.
        /// </summary>
        /// <param name="pos">The position adjustment to apply.</param>
        /// <returns></returns>
        public decimal Adjust(Position pos)
        {
            if (this.hasSymbol && (this.Symbol != pos.Symbol)) throw new Exception("Invalid Position: Position MUST have a symbol.");
            if (!pos.isValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + pos.ToString());
            if (pos.Flat) return 0; // nothing to do
            decimal pl = BoxMath.ClosePL(this,pos.ToTrade());
            if (this.Flat) this.price = pos.price; // if we're leaving flat just copy price
            else if ((pos.Side && this.Side) || (!pos.Side && !this.Side)) // sides match, adding so adjust price
                this.price = ((this.price * this.size) + (pos.price * pos.size)) / (pos.size + this.size);
            this.size += pos.size; // adjust the size
            if (this.Flat) price = 0; // if we're flat after adjusting, size price back to zero
            return pl;
        }
        /// <summary>
        /// Adjusts the position by applying a new trade or fill.
        /// </summary>
        /// <param name="t">The fill to apply to this position.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) { return Adjust(new Position(t)); }
        public static int Norm2Min(int size)
        {
            int wmult = (int)Math.Ceiling((decimal)size / 100);
            return wmult * 100;
        }

        public override string ToString()
        {
            return Symbol+" "+Size+"@"+AvgPrice.ToString("N2");
        }
        public Trade ToTrade()
        {
            DateTime dt = (date*time!=0) ? Util.ToDateTime(date, time, sec) : DateTime.Now;
            return new Trade(Symbol, AvgPrice, Size,dt );
        }
    }
}
