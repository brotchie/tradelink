using System;

namespace TradeLib
{
    /// <summary>
    /// Specify an order to buy or sell a quantity of stock.
    /// </summary>
    [Serializable]
    public class Order : Trade
    {
        public Order() { }
        public override bool isValid { get { return (symbol != null) && (size != 0); } }
        public bool isMarket { get { return (price == 0) && (stopp == 0); } }
        public bool isLimit { get { return (price != 0); } }
        public bool isStop { get { return (stopp != 0); } }
        public Order(string sym, bool side, int size, decimal p, decimal s, string c, int time, int date)
        {
            this.symbol = sym.ToUpper();
            this.side = side;
            this.size = System.Math.Abs(size);
            this.price = p;
            this.stopp = s;
            this.comment = c;
            this.time = time;
            this.date = date;
            this.isFilled = false;
        }
        public Order(string sym, bool side, int size)
        {
            this.symbol = sym.ToUpper();
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
            this.isFilled = false;
        }
        public Order(string sym, bool side, int size, string c)
        {
            this.symbol = sym.ToUpper();
            this.side = side;
            this.price = 0;
            this.stopp = 0;
            this.comment = c;
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
            this.isFilled = false;
        }
        public Order(string sym, int size)
        {
            this.symbol = sym.ToUpper();
            this.side = size > 0;
            this.price = 0;
            this.stopp = 0;
            this.comment = "";
            this.time = 0;
            this.date = 0;
            this.size = System.Math.Abs(size);
            this.isFilled = false;
        }
        public override string ToString()
        {
            if (this.isFilled) return base.ToString();
            return this.date + ":" + this.time + (side ? " BUY" : " SELL") + Math.Abs(size) + " " + this.symbol + "@" + ((this.price == 0) ? "Market" : this.price.ToString());
        }
    }

}
