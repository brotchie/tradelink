using System;
using System.Collections.Generic;
using TradeLib;

namespace TradeLib
{
    public delegate void OrderDelegate(Order o);    
    public class Broker
    {
        public event OrderDelegate GotOrder;
        public event FillDelegate GotFill;
        public event DebugDelegate GotWarning;
        public Broker() 
        {
            Reset();

        }
        protected Account DEFAULT = new Account("DEFAULT","Defacto account when account not provided");
        protected Dictionary<Account, List<Order>> MasterOrders = new Dictionary<Account, List<Order>>();
        protected Dictionary<Account, List<Trade>> MasterTrades = new Dictionary<Account, List<Trade>>();
        protected List<Order> Orders { get { return MasterOrders[DEFAULT]; } set { MasterOrders[DEFAULT] = value; } }
        protected List<Trade> FillList { get { return MasterTrades[DEFAULT]; } set { MasterTrades[DEFAULT] = value; } } 
        protected void AddOrder(Order o,Account a) 
        { 
            if (!MasterOrders.ContainsKey(a)) 
                MasterOrders.Add(a,new List<Order>());
            o.accountid = a.ID;
            MasterOrders[a].Add(o);
        }
        public bool sendOrder(Order o) { return sendOrder(o, DEFAULT); }
        public bool sendOrder(Order o,Account a)
        {
            if ((!o.isValid) || (!a.isValid))
            {
                if (GotWarning != null)
                    GotWarning(!o.isValid ? "Invalid order: " + o.ToString() : "Invalid Account" + a.ToString());
                return false;
            }
            AddOrder(o, a);
            if (GotOrder != null) GotOrder(o);
            return true;
        }
        public int Execute(Tick tick)
        {
            if (!tick.isTrade) return 0;
            int availablesize = (int)Math.Abs(tick.size);
            int max = this.Orders.Count;
            int filledorders = 0;
            foreach (Account a in MasterOrders.Keys)
            { // go through each account
                for (int i = 0; i < MasterOrders[a].Count; i++)
                { // go through each order
                    Order o = MasterOrders[a][i];
                    if (tick.sym != o.symbol) continue; //make sure tick is for the right stock
                    int mysize = (int)Math.Abs(o.size);
                    if (((mysize <= availablesize) && (o.price == 0) && (o.stopp == 0)) || //market order
                        (o.side && (mysize <= availablesize) && (tick.trade <= o.price) && (o.stopp == 0)) || // buy limit
                        (!o.side && (mysize <= availablesize) && (tick.trade >= o.price) && (o.stopp == 0)) || //sell limit
                        (o.side && (mysize <= availablesize) && (tick.trade >= o.stopp) && (o.price == 0)) || // buy stop
                        (!o.side && (mysize <= availablesize) && (tick.trade <= o.stopp) && (o.price == 0))) // sell stop
                    { // sort filled trades by symbol
                        MasterOrders[a].RemoveAt(i);
                        if (!MasterTrades.ContainsKey(a)) MasterTrades.Add(a, new List<Trade>());
                        o.Fill(tick); // fill our trade
                        availablesize -= mysize; // don't let other trades fill on same tick
                        if (GotFill != null) GotFill((Trade)o); // notify subscribers
                        MasterTrades[a].Add((Trade)o);
                        filledorders++; // count the trade
                    }
                }
            }
            return filledorders;
        }

        public void Reset()
        {
            MasterOrders.Clear();
            MasterTrades.Clear();
            MasterOrders.Add(DEFAULT, new List<Order>());
            MasterTrades.Add(DEFAULT, new List<Trade>());
        }
        public void CancelOrders() { CancelOrders(DEFAULT); }
        public void CancelOrders(Account a) { MasterOrders[a].Clear(); }
        public List<Trade> GetTradeList(Account a) { return MasterTrades[a]; }
        public List<Order> GetOrderList(Account a) { return MasterOrders[a]; }
        public List<Trade> GetTradeList() { return GetTradeList(DEFAULT); }
        public List<Order> GetOrderList() { return GetOrderList(DEFAULT); }

        public Position GetOpenPosition(string symbol) { return GetOpenPosition(symbol, DEFAULT); }
        public Position GetOpenPosition(string symbol,Account a)
        {
            Position pos = new Position(symbol);
            if (!MasterTrades.ContainsKey(a)) return pos;
            List<Trade> OT = MasterTrades[a];
            foreach (Trade trade in OT) 
                if (trade.symbol==symbol) 
                    pos.Adjust(trade);
            return pos;
        }


    }

}
