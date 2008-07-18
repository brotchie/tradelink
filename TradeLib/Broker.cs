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
        public delegate void OrderCancelDelegate(string sym, bool side, uint id);
        /// <summary>
        /// Occurs when [got order cancel].
        /// </summary>
        public event OrderCancelDelegate GotOrderCancel;
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
        public const string DEFAULTBOOK = "DEFAULT";
        protected Account DEFAULT = new Account(DEFAULTBOOK,"Defacto account when account not provided");
        protected Dictionary<Account, List<Order>> MasterOrders = new Dictionary<Account, List<Order>>();
        protected Dictionary<string, List<Trade>> MasterTrades = new Dictionary<string, List<Trade>>();
        protected List<Order> Orders { get { return MasterOrders[DEFAULT]; } set { MasterOrders[DEFAULT] = value; } }
        protected List<Trade> FillList { get { return MasterTrades[DEFAULT.ID]; } set { MasterTrades[DEFAULT.ID] = value; } }
        public string[] Accounts { get { List<string> alist = new List<string>(); foreach (Account a in MasterOrders.Keys) alist.Add(a.ID); return alist.ToArray(); } }
        uint _nextorderid = 1;

        public Order BestBid(string symbol,Account account) { return BestBidOrOffer(symbol,true,account); }
        public Order BestBid(string symbol) { return BestBidOrOffer(symbol,true); }
        public Order BestOffer(string symbol, Account  account) { return BestBidOrOffer(symbol,false,account); }
        public Order BestOffer(string symbol) { return BestBidOrOffer(symbol,false); }

        public Order BestBidOrOffer(string symbol,bool side)
        {
            Order best = new Order();
            Order next = new Order();
            foreach (Account a in MasterOrders.Keys)
            {
                // get our first order
                if (!best.isValid)
                {
                    // if we don't have a valid one yet, check this account
                    best = new Order(BestBidOrOffer(symbol,side,a));
                    continue;  // keep checking the accounts till we find a valid one
                }
                // now we have our first order, which will be best if we can't find a second one
                next = new Order(BestBidOrOffer(symbol,side,a));
                if (!next.isValid) continue; // keep going till we have a second order
                best = BestBidOrOffer(best, next); // when we have two, compare and get best
                // then keep fetching next valid order to see if it's better
            }
            return best; // if there's no more orders left, this is best
        }

        public Order BestBidOrOffer(string sym, bool side,Account Account)
        {
            Order best = new Order();
            if (!MasterOrders.ContainsKey(Account)) return best;
            foreach (Order o in MasterOrders[Account])
            {
                if (o.symbol != sym) continue;
                if (o.Side != side) continue;
                if (!best.isValid)
                {
                    best = new Order(o);
                    continue;
                }
                Order test = BestBidOrOffer(best, o);
                if (test.isValid) best = new Order(test);
            }
            return best;
        }

        // takes two orders and returns the better one
        // if orders aren't for same side or symbol or not limit, returns invalid order
        // if orders are equally good, adds them together
        public Order BestBidOrOffer(Order first,Order second)
        {
            if ((first.symbol!= second.symbol) || (first.side!=second.side) || !first.isLimit || !second.isLimit)
                return new Order(); // if not comparable return an invalid order
            if ((first.side && (first.price > second.price)) || // if first is better, use it
                (!first.side && (first.price < second.price)))
                return new Order(first);
            else if ((first.side && (first.price < second.price)) || // if second is better, use it
                (!first.side && (first.price > second.price)))
                return new Order(second);

            // if order is matching then add the sizes
            Order add = new Order(first);
            add.size = add.UnSignedSize + second.UnSignedSize * (add.Side ? 1 : -1);
            return add;
        }


        protected void AddOrder(Order o,Account a) 
        {
            if (!a.isValid) throw new Exception("Invalid account provided"); // account must be good
            if (!MasterOrders.ContainsKey(a))  // see if we have a book for this account
                MasterOrders.Add(a,new List<Order>()); // if not, create one
            o.Account = a.ID; // make sure order knows his account
            if (o.id == 0) // if order id isn't set, set it
                o.id = _nextorderid++;
            MasterOrders[a].Add(o); // record the order
        }
        public bool CancelOrder(uint orderid)
        {
            bool worked = false;
            foreach (Account a in MasterOrders.Keys)
                worked |= CancelOrder(a, orderid);
            return worked;
        }
        public bool CancelOrder(Account a, uint orderid)
        {
            if (!MasterOrders.ContainsKey(a)) return false;
            for (int i = 0; i < MasterOrders[a].Count; i++) // and every order
                if (MasterOrders[a][i].id == orderid) // if we have order with requested id
                {
                    if ((GotOrderCancel != null) && a.Notify)
                        GotOrderCancel(MasterOrders[a][i].symbol, MasterOrders[a][i].Side,orderid); //send cancel notifcation to any subscribers
                    MasterOrders[a].RemoveAt(i); // remove/cancel order
                    return true;
                }
            return false;
        }
        /// <summary>
        /// Sends the order to the broker. (uses the default account)
        /// </summary>
        /// <param name="o">The order to be send.</param>
        /// <returns>true if the order was accepted.</returns>
        public uint sendOrder(Order o) 
        { 
            if (o.Account=="")
                return sendOrder(o, DEFAULT);
            return sendOrder(o, new Account(o.Account));

        }
        /// <summary>
        /// Sends the order to the broker for a specific account.
        /// </summary>
        /// <param name="o">The order to be sent.</param>
        /// <param name="a">the account to send with the order.</param>
        /// <returns>order id if order was accepted, zero otherwise</returns>
        public uint sendOrder(Order o,Account a)
        {
            if ((!o.isValid) || (!a.isValid))
            {
                if (GotWarning != null)
                    GotWarning(!o.isValid ? "Invalid order: " + o.ToString() : "Invalid Account" + a.ToString());
                return 0;
            }
            AddOrder(o, a);
            if ((GotOrder != null) && a.Notify) 
                GotOrder(o);
            return o.id;
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
                // if account has requested no executions, skip it
                if (!a.Execute) continue;
                // go through each order in the account
                for (int i = 0; i < MasterOrders[a].Count; i++)
                { 
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
                        if (!MasterTrades.ContainsKey(a.ID)) MasterTrades.Add(a.ID, new List<Trade>());
                        o.Fill(tick); // fill our trade
                        availablesize -= mysize; // don't let other trades fill on same tick
                        MasterTrades[a.ID].Add((Trade)o); // record trade
                        if ((GotFill != null) && a.Notify) 
                            GotFill((Trade)o); // notify subscribers after recording trade
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
            CancelOrders();
            MasterOrders.Clear();
            MasterTrades.Clear();
            MasterOrders.Add(DEFAULT, new List<Order>());
            MasterTrades.Add(DEFAULT.ID, new List<Trade>());
        }
        public void CancelOrders() 
        {
            foreach (Account a in MasterOrders.Keys)
                CancelOrders(a);
        }
        public void CancelOrders(Account a) 
        {
            if (!MasterOrders.ContainsKey(a)) return;
            foreach (Order o in MasterOrders[a])
                if ((GotOrderCancel != null) && a.Notify)
                    GotOrderCancel(o.symbol,o.Side,o.id); //send cancel notifcation to any subscribers
            MasterOrders[a].Clear();  // clear the account
        }
        /// <summary>
        /// Gets the complete execution list for this account
        /// </summary>
        /// <param name="a">account to request blotter from.</param>
        /// <returns></returns>
        public List<Trade> GetTradeList(Account a) { List<Trade> res; bool worked = MasterTrades.TryGetValue(a.ID, out res); return worked ? res : new List<Trade>(); }
        /// <summary>
        /// Gets the list of open orders for this account.
        /// </summary>
        /// <param name="a">Account.</param>
        /// <returns></returns>
        public List<Order> GetOrderList(Account a) { List<Order> res; bool worked = MasterOrders.TryGetValue(a, out res); return worked ? res : new List<Order>(); }
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
            if (!MasterTrades.ContainsKey(a.ID)) return pos;
            foreach (Trade trade in MasterTrades[a.ID]) 
                if (trade.symbol==symbol) 
                    pos.Adjust(trade);
            return pos;
        }

        /// <summary>
        /// Gets the closed PL for a particular symbol and brokerage account.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="a">The Account.</param>
        /// <returns>Closed PL</returns>
        public decimal GetClosedPL(string symbol, Account a)
        {
            Position pos = new Position(symbol);
            decimal pl = 0;
            if (!MasterTrades.ContainsKey(a.ID)) return pl;
            foreach (Trade trade in MasterTrades[a.ID])
            {
                if (trade.symbol == pos.Symbol)
                    pl += pos.Adjust(trade);
            }
            return pl;
        }

        /// <summary>
        /// Gets the closed PL for a particular symbol on the default account.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public decimal GetClosedPL(string symbol) { return GetClosedPL(symbol, DEFAULT); }
        /// <summary>
        /// Gets the closed PL for an entire account. (all symbols)
        /// </summary>
        /// <param name="a">The account.</param>
        /// <returns>Closed PL</returns>
        public decimal GetClosedPL(Account a)
        {
            Dictionary<string, Position> poslist = new Dictionary<string, Position>();
            Dictionary<string,decimal> pllist = new Dictionary<string,decimal>();
            if (!MasterTrades.ContainsKey(a.ID)) return 0;
            foreach (Trade trade in MasterTrades[a.ID])
            {
                if (!poslist.ContainsKey(trade.symbol))
                {
                    poslist.Add(trade.symbol, new Position(trade.symbol));
                    pllist.Add(trade.symbol, 0);
                }
                pllist[trade.symbol] += poslist[trade.symbol].Adjust(trade);
            }
            decimal pl = 0;
            foreach (string sym in pllist.Keys)
                pl += pllist[sym];
            return pl;
        }
        /// <summary>
        /// Gets the closed PL for all symbols on the default account.
        /// </summary>
        /// <returns>Closed PL</returns>
        public decimal GetClosedPL() { return GetClosedPL(DEFAULT); }

        /// <summary>
        /// Gets the closed points (points = PL on per-share basis) for given symbol/account.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="account">The account.</param>
        /// <returns>points</returns>
        public decimal GetClosedPT(string symbol, Account account)
        {
            Position pos = new Position(symbol);
            decimal points = 0;
            if (!MasterTrades.ContainsKey(account.ID)) return points;
            foreach (Trade t in MasterTrades[account.ID])
            {
                points += BoxMath.ClosePT(pos, t);
                pos.Adjust(t);
            }
            return points;
        }
        /// <summary>
        /// Gets the closed PT/Points for given symbol on default account.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public decimal GetClosedPT(string symbol) { return GetClosedPT(symbol, DEFAULT); }
        /// <summary>
        /// Gets the closed Points on a specific account, all symbols.
        /// </summary>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public decimal GetClosedPT(Account account)
        {
            Dictionary<string, Position> poslist = new Dictionary<string, Position>();
            Dictionary<string, decimal> ptlist = new Dictionary<string, decimal>();
            if (!MasterTrades.ContainsKey(account.ID)) return 0;
            foreach (Trade trade in MasterTrades[account.ID])
            {
                if (!poslist.ContainsKey(trade.symbol))
                {
                    poslist.Add(trade.symbol, new Position(trade.symbol));
                    ptlist.Add(trade.symbol, 0);
                }
                ptlist[trade.symbol] += BoxMath.ClosePT(poslist[trade.symbol], trade);
                poslist[trade.symbol].Adjust(trade);
            }
            decimal points = 0;
            foreach (string sym in ptlist.Keys)
                points += ptlist[sym];
            return points;

        }
        /// <summary>
        /// Gets the closed Points on the default account.
        /// </summary>
        /// <returns></returns>
        public decimal GetClosedPT() { return GetClosedPT(DEFAULT); }



    }

    public enum Brokers
    {
        Unknown = -1,
        TradeLinkSimulation = 0,
        Assent,
        InteractiveBrokers,
        Genesis,
        Bright,
        Echo,
    }

}
