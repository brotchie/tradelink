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
        public Position() : this("") { }
        public Position(Position p) : this(p.Symbol, p.AvgPrice, p.Size, p.ClosedPL) { }
        public Position(string symbol) : this(symbol, 0m, 0,0) { }
        public Position(string symbol, decimal price, int size) : this(symbol, price, size, 0) { }
        public Position(string symbol, decimal price, int size, decimal closedpl) { _sym = symbol; if (size == 0) price = 0; _price = price; _size = size; _closedpl = closedpl; }
        public Position(Trade t) 
        {
            if (!t.isValid) throw new Exception("Can't construct a position object from invalid trade.");
            _sym = t.symbol; _price = t.xprice; _size = t.xsize; _date = t.xdate; _time = t.time; _sec = t.xsec;
            if (_size>0) _size *= t.side ? 1 : -1;
        }
        protected string _sym = "";
        protected int _size = 0;
        protected decimal _price = 0;
        protected int _date = 0;
        protected int _time = 0;
        protected int _sec = 0;
        protected decimal _closedpl = 0;
        public bool isValid
        {
            get { return hasSymbol && (((AvgPrice == 0) && (Size == 0)) || ((AvgPrice != 0) && (Size != 0))); }
        }
        public decimal ClosedPL { get { return _closedpl; } }
        public bool hasSymbol { get { return _sym != ""; } }
        public string Symbol { get { return _sym; } }
        public decimal Price { get { return _price; } }
        public decimal AvgPrice { get { return _price; } }
        public int Size { get { return _size; } }
        public int UnsignedSize { get { return Math.Abs(_size); } }
        public bool isLong { get { return _size > 0; } }
        public bool isFlat { get { return _size==0; } }
        public bool isShort { get { return _size < 0; } }
        public int FlatSize { get { return _size * -1; } }
        // returns any closed PL calculated on position basis (not per share)
        /// <summary>
        /// Adjusts the position by applying a new position.
        /// </summary>
        /// <param name="pos">The position adjustment to apply.</param>
        /// <returns></returns>
        public decimal Adjust(Position pos)
        {
            if (this.hasSymbol && (this.Symbol != pos.Symbol)) throw new Exception("Invalid Position: Position MUST have a symbol.");
            if (!hasSymbol && pos.hasSymbol) _sym = pos.Symbol;
            if (!pos.isValid) throw new Exception("Invalid position adjustment, existing:" + this.ToString() + " adjustment:" + pos.ToString());
            if (pos.isFlat) return 0; // nothing to do
            bool oldside = isLong;
            decimal pl = Calc.ClosePL(this,pos.ToTrade());
            if (this.isFlat) this._price = pos._price; // if we're leaving flat just copy price
            else if ((pos.isLong && this.isLong) || (!pos.isLong && !this.isLong)) // sides match, adding so adjust price
                this._price = ((this._price * this._size) + (pos._price * pos._size)) / (pos._size + this._size);
            this._size += pos._size; // adjust the size
            if (oldside != isLong) _price = pos.AvgPrice; // this is for when broker allows flipping sides in one trade
            if (this.isFlat) _price = 0; // if we're flat after adjusting, size price back to zero
            _closedpl += pl; // update running closed pl
            return pl;
        }
        /// <summary>
        /// Adjusts the position by applying a new trade or fill.
        /// </summary>
        /// <param name="t">The new fill you want this position to reflect.</param>
        /// <returns></returns>
        public decimal Adjust(Trade t) { return Adjust(new Position(t)); }

        public override string ToString()
        {
            return Symbol+" "+Size+"@"+AvgPrice.ToString("N2");
        }
        public Trade ToTrade()
        {
            DateTime dt = (_date*_time!=0) ? Util.ToDateTime(_date, _time, _sec) : DateTime.Now;
            return new Trade(Symbol, AvgPrice, Size,dt );
        }

        public static Position Deserialize(string msg)
        {
            
            string[] r = msg.Split(',');
            string sym = r[(int)pf.symbol];
            decimal price = Convert.ToDecimal(r[(int)pf.price]);
            decimal cpl = Convert.ToDecimal(r[(int)pf.closedpl]);
            int size = Convert.ToInt32(r[(int)pf.size]);
            Position p = new Position(sym,price,size,cpl);
            return p;
        }

        public string Serialize()
        {
            string[] r = new string[] { Symbol, AvgPrice.ToString("N2"), Size.ToString(), ClosedPL.ToString("N2") };
            return string.Join(",", r);
        }

        enum pf
        {
            symbol,
            price,
            size,
            closedpl,
        }
    }
}
