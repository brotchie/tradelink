using System;
using System.Collections.Generic;
using TradeLib;

namespace TradeLib
{

    /// <summary>
    /// A simulated broker class for TradeLink.
    /// </summary>
    public class Broker
    {
        /// <summary>
        /// Occurs when [got tick].
        /// </summary>
        public event TickDelegate GotTick;
        /// <summary>
        /// Occurs when [got order].
        /// </summary>
        public event OrderDelegate GotOrder;
        /// <summary>
        /// Occurs when [got fill].
        /// </summary>
        public event FillDelegate GotFill;
        /// <summary>
        /// Occurs when [got warning].  This will happen if an invalid order is received.
        /// </summary>
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
        /// <summary>
        /// Sends the order to the broker. (uses the default account)
        /// </summary>
        /// <param name="o">The order to be send.</param>
        /// <returns>true if the order was accepted.</returns>
        public bool sendOrder(Order o) { return sendOrder(o, DEFAULT); }
        /// <summary>
        /// Sends the order to the broker for a specific account.
        /// </summary>
        /// <param name="o">The order to be sent.</param>
        /// <param name="a">the account to send with the order.</param>
        /// <returns>true if the order was accepted, false otherwise.</returns>
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
        /// <summary>
        /// Executes any open orders allowed by the specified tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>the number of orders executed using the tick.</returns>
        public int Execute(Tick tick)
        {
            if (GotTick != null) GotTick(tick);
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

        /// <summary>
        /// Resets this instance, clears all orders/trades/accounts held by the broker.
        /// </summary>
        public void Reset()
        {
            MasterOrders.Clear();
            MasterTrades.Clear();
            MasterOrders.Add(DEFAULT, new List<Order>());
            MasterTrades.Add(DEFAULT, new List<Trade>());
        }
        public void CancelOrders() { CancelOrders(DEFAULT); }
        public void CancelOrders(Account a) { MasterOrders[a].Clear(); }
        /// <summary>
        /// Gets the complete execution list for this account
        /// </summary>
        /// <param name="a">account to request blotter from.</param>
        /// <returns></returns>
        public List<Trade> GetTradeList(Account a) { return MasterTrades[a]; }
        /// <summary>
        /// Gets the list of open orders for this account.
        /// </summary>
        /// <param name="a">Account.</param>
        /// <returns></returns>
        public List<Order> GetOrderList(Account a) { return MasterOrders[a]; }
        public List<Trade> GetTradeList() { return GetTradeList(DEFAULT); }
        public List<Order> GetOrderList() { return GetOrderList(DEFAULT); }

        /// <summary>
        /// Gets the open positions for the default account.
        /// </summary>
        /// <param name="symbol">The symbol to get a position for.</param>
        /// <returns>current position</returns>
        public Position GetOpenPosition(string symbol) { return GetOpenPosition(symbol, DEFAULT); }
        /// <summary>
        /// Gets the open position for the specified account.
        /// </summary>
        /// <param name="symbol">The symbol to get a position for.</param>
        /// <param name="a">the account.</param>
        /// <returns>current position</returns>
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
