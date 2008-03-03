using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using TradeLib;

namespace TradeLib
{
    public delegate void OrderDelegate(Order o);    
    public class Broker
    {
        public event OrderDelegate GotOrder;
        public event FillDelegate GotFill;
        public Broker() { }
        public int slippage = 0; // not used presently
        public List<Order> Orders = new List<Order>();
        public Hashtable Fills = new Hashtable();
        public List<Trade> FillList = new List<Trade>();
        public virtual Boolean sendOrder(Order o)
        {
            if (!o.isValid) return false;
            Orders.Add(o);
            if (GotOrder != null) GotOrder(o);
            return true;
        }
        public int Execute(Tick tick)
        {
            if (!tick.isTrade) return 0;
            int max = this.Orders.Count;
            int filledorders = 0;
            for (int i = 0; i < Orders.Count; i++)
            {
                Order o = Orders[i];
                int size = tick.size;
                if (tick.sym!=o.symbol) continue; //make sure tick is for the right stock
                int mysize = (int)Math.Abs(o.size);
                if (((mysize<= size) && (o.price == 0) && (o.stopp == 0)) || //market order
                    (o.side && (mysize<=size) && (tick.trade <= o.price) && (o.stopp == 0)) || // buy limit
                    (!o.side && (mysize<=size)&&(tick.trade >= o.price) && (o.stopp == 0)) || //sell limit
                    (o.side && (mysize<=size) && (tick.trade >= o.stopp) && (o.price == 0)) || // buy stop
                    (!o.side && (mysize<=size) && (tick.trade <= o.stopp) && (o.price == 0))) // sell stop
                { // sort filled trades by symbol
                    Orders.RemoveAt(i); // remove it from order list
                    if (!this.Fills.Contains(o.symbol)) this.Fills.Add(o.symbol, new Queue());
                    o.Fill(tick); // fill our trade
                    if (GotFill != null) GotFill((Trade)o);
                    Queue q = (Queue)this.Fills[o.symbol]; // pull our queue from hash
                    q.Enqueue((Trade)o); // cast our order to a trade and queue it
                    this.Fills[o.symbol] = q; // put queue back on the hashtable
                    this.FillList.Add((Trade)o); // save another copy on linear table
                    filledorders++; // count the trade
                }
            }
            return filledorders;
        }

        public void Reset()
        {
            this.Orders.Clear();
            this.Fills.Clear();
        }
        public void CancelOrders() { this.Orders.Clear(); }

        public Queue GetRoundTurns(string symbol)
        { //gets all closed trades given a symbol
            Queue RT = new Queue();
            Trade t;
            int size = 0;
            Queue OT = (Queue)this.Fills[symbol];
            foreach (object o in OT)
            {
                t = (Trade)o;
                int oldsize = size;
                if (t.side) size += t.xsize;
                else size -= t.xsize;
                if ((oldsize != 0) && (size == 0)) RT.Enqueue(t); // save roundturns
            }
            return RT;
        }

        public Queue GetOpenTrades(string symbol)
        { // gets all unclosed trades given a symbol
            Queue RT = new Queue();
            int size = 0;
            if (!this.Fills.ContainsKey(symbol)) return null;
            Queue OT = (Queue)this.Fills[symbol];
            int totfills = OT.Count;
            for (int i = 0; i<totfills; i++)            
            {
                Trade t = (Trade)OT.Dequeue();
                int oldsize = size;
                if (t.side) size += t.xsize;
                else size -= t.xsize;
                if ((oldsize != 0) && (size == 0)) RT.Enqueue(t); // save roundturns
                else OT.Enqueue(t);
            } 
            return OT;
        }

        public Position GetOpenPosition(string symbol)
        {
            Position pos = new Position(symbol);
            if (!Fills.ContainsKey(symbol)) return pos;
            Queue OT = (Queue)Fills[symbol];
            if (OT == null) return pos;
            foreach (object o in OT) pos.Adjust((Trade)o);
            return pos;
        }


        public static decimal GetFillAvgPriceSide(bool side, Queue RT)
        { // gets average price for all buys (or sells/shorts) in a queue of fills
            decimal price = 0;
            int i = 0;
            foreach (object o in RT)
            {
                Trade t = (Trade)o;
                if (t.side == side) price += t.xprice * t.xsize;
                if (t.side == side) i++;
            }
            return price / i;
        }

        public static decimal GetSymbolClosedPL(Queue RT)
        { // gets closed PL for list of roundturn-completed fills for a given symbol
            decimal pl = 0;
            const bool SELL = false;
            const bool BUY = true;
            pl = GetFillAvgPriceSide(SELL, RT) - GetFillAvgPriceSide(BUY, RT);
            return pl;
        }

        public static decimal GetSymbolOpenPL(Queue OT, decimal MarketPrice)
        {
            decimal pl = 0;
            const bool SELL = false;
            const bool BUY = true;
            bool otside = true; // default assume long
            int size = 0;
            foreach (object o in OT)
            {
                Trade t = (Trade)o;
                otside = t.side == otside;
                size += t.xsize;
            }
            if (otside) pl = (MarketPrice * size) - GetFillAvgPriceSide(BUY, OT);
            else pl = GetFillAvgPriceSide(SELL, OT) - (MarketPrice * size);
            return pl;
        }

        public Hashtable GetAllClosedPL(Hashtable Fills)
        {
            Hashtable symbolpl = new Hashtable();
            string sym;
            Queue sfill;
            IEnumerator e = Fills.GetEnumerator();
            while (e.MoveNext())
            {
                sym = e.Current.ToString();
                sfill = (Queue)Fills[sym];
                symbolpl.Add(sym, GetSymbolClosedPL(GetRoundTurns(sym)));
            }
            return symbolpl;
        }

        public Hashtable GetAllOpenPL(Hashtable Fills, decimal MarketPrice)
        {
            Hashtable symbolpl = new Hashtable();
            string sym = null;
            Queue sfill;
            IEnumerator e = Fills.GetEnumerator();
            while (e.MoveNext())
            {
                sym = e.Current.ToString();
                sfill = (Queue)Fills[sym];
                symbolpl.Add(sym, GetSymbolOpenPL(GetOpenTrades(sym), MarketPrice));
            }
            return symbolpl;
        }
    }

}
