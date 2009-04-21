using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Common
{

    /// <summary>
    /// A simulated broker class for TradeLink.
    /// Processes orders and fills them against external tick feed. (live or historical)
    /// </summary>
    public class Broker
    {
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

        public Broker() 
        {
            Reset();

        }
        public const string DEFAULTBOOK = "DEFAULT";
        protected Account DEFAULT = new Account(DEFAULTBOOK, "Defacto account when account not provided");
        protected Dictionary<Account, List<Order>> MasterOrders = new Dictionary<Account, List<Order>>();
        protected Dictionary<string, List<Trade>> MasterTrades = new Dictionary<string, List<Trade>>();
        protected List<Order> Orders { get { return MasterOrders[DEFAULT]; } set { MasterOrders[DEFAULT] = value; } }
        protected List<Trade> FillList { get { return MasterTrades[DEFAULT.ID]; } set { MasterTrades[DEFAULT.ID] = value; } }
        public string[] Accounts { get { List<string> alist = new List<string>(); foreach (Account a in MasterOrders.Keys) alist.Add(a.ID); return alist.ToArray(); } }
        uint _nextorderid = OrderImpl.Unique;

        public Order BestBid(string symbol,Account account) { return BestBidOrOffer(symbol,true,account); }
        public Order BestBid(string symbol) { return BestBidOrOffer(symbol,true); }
        public Order BestOffer(string symbol, Account  account) { return BestBidOrOffer(symbol,false,account); }
        public Order BestOffer(string symbol) { return BestBidOrOffer(symbol,false); }

        public Order BestBidOrOffer(string symbol,bool side)
        {
            Order best = new OrderImpl();
            Order next = new OrderImpl();
            foreach (Account a in MasterOrders.Keys)
            {
                // get our first order
                if (!best.isValid)
                {
                    // if we don't have a valid one yet, check this account
                    best = new OrderImpl(BestBidOrOffer(symbol,side,a));
                    continue;  // keep checking the accounts till we find a valid one
                }
                // now we have our first order, which will be best if we can't find a second one
                next = new OrderImpl(BestBidOrOffer(symbol,side,a));
                if (!next.isValid) continue; // keep going till we have a second order
                best = BestBidOrOffer(best, next); // when we have two, compare and get best
                // then keep fetching next valid order to see if it's better
            }
            return best; // if there's no more orders left, this is best
        }

        public Order BestBidOrOffer(string sym, bool side,Account Account)
        {
            Order best = new OrderImpl();
            if (!MasterOrders.ContainsKey(Account)) return best;
            foreach (OrderImpl o in MasterOrders[Account])
            {
                if (o.symbol != sym) continue;
                if (o.Side != side) continue;
                if (!best.isValid)
                {
                    best = new OrderImpl(o);
                    continue;
                }
                Order test = BestBidOrOffer(best, o);
                if (test.isValid) best = new OrderImpl(test);
            }
            return best;
        }

        // takes two orders and returns the better one
        // if orders aren't for same side or symbol or not limit, returns invalid order
        // if orders are equally good, adds them together
        public Order BestBidOrOffer(Order first,Order second)
        {
            if ((first.symbol!= second.symbol) || (first.side!=second.side) || !first.isLimit || !second.isLimit)
                return new OrderImpl(); // if not comparable return an invalid order
            if ((first.side && (first.price > second.price)) || // if first is better, use it
                (!first.side && (first.price < second.price)))
                return new OrderImpl(first);
            else if ((first.side && (first.price < second.price)) || // if second is better, use it
                (!first.side && (first.price > second.price)))
                return new OrderImpl(second);

            // if order is matching then add the sizes
            OrderImpl add = new OrderImpl(first);
            add.size = add.UnSignedSize + second.UnsignedSize * (add.Side ? 1 : -1);
            return add;
        }

        protected void AddOrder(Order o,Account a) 
        {
            if (!a.isValid) throw new Exception("Invalid account provided"); // account must be good
            if ((FillMode== FillMode.OwnBook) && a.Execute)
            {
                // get best bid or offer from opposite side,
                // see if we can match against this BBO and cross locally
                Order match = BestBidOrOffer(o.symbol, !o.side);

                // first we need to make sure the book we're matching to allows executions
                Account ma = new Account();
                if (acctlist.TryGetValue(match.Account,out ma) && ma.Execute) 
                {
                    // if it's allowed, try to match it
                    bool filled = o.Fill(match);
                    int avail = o.UnsignedSize;
                    // if it matched 
                    if (filled)
                    {
                        // record trade
                        Trade t = (Trade)o;
                        MasterTrades[a.ID].Add(t); 
                        // notify the trade occured
                        if (GotFill != null) 
                            GotFill(t);

                        // update the order's size (in case it was a partial fill)
                        o.size = (avail - Math.Abs(t.xsize)) * (o.side ? 1 : -1);

                        // if it was a full fill, no need to add order to the book
                        if (Math.Abs(t.xsize) == avail) return;
                    }
                }
            }
            // add any remaining order to book as new liquidity route
            if (!MasterOrders.ContainsKey(a))  // see if we have a book for this account
                MasterOrders.Add(a,new List<Order>()); // if not, create one
            o.Account = a.ID; // make sure order knows his account
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
            List<Order> orderlist = new List<Order>();
            if (!MasterOrders.TryGetValue(a, out orderlist)) return false;
            int cancelidx = -1;
            for (int i = orderlist.Count-1; i>=0; i--) // and every order
                if (orderlist[i].id == orderid) // if we have order with requested id
                {
                    if ((GotOrderCancel != null) && a.Notify)
                        GotOrderCancel(orderlist[i].symbol, orderlist[i].side,orderid); //send cancel notifcation to any subscribers
                    cancelidx = i;
                }
            if (cancelidx == -1) return false;
            MasterOrders[a].RemoveAt(cancelidx);
            return true;
        }
        /// <summary>
        /// Sends the order to the broker. (uses the default account)
        /// </summary>
        /// <param name="o">The order to be send.</param>
        /// <returns>status code</returns>
        public int sendOrder(Order o) 
        {
            if (!o.isValid) return (int)MessageTypes.INVALID_ORDERSIZE;
            if (o.Account == "") // make sure book is clearly stamped
            {
                o.Account = DEFAULT.ID;
                return sendOrder(o, DEFAULT);
            }
            Account a = new Account();
            if (!acctlist.TryGetValue(o.Account,out a))
                AddAccount(new Account(o.Account));
            return sendOrder(o, acctlist[o.Account]);

        }
        /// <summary>
        /// Sends the order to the broker for a specific account.
        /// </summary>
        /// <param name="o">The order to be sent.</param>
        /// <param name="a">the account to send with the order.</param>
        /// <returns>status code</returns>
        public int sendOrder(Order o,Account a)
        {
            if (o.id == 0) // if order id isn't set, set it
                o.id = _nextorderid++;
            if ((GotOrder != null) && a.Notify)
                GotOrder(o);
            AddOrder(o, a);

            return (int)MessageTypes.OK;
        }

        List<string> hasopened = new List<string>();

        /// <summary>
        /// Executes any open orders allowed by the specified tick.
        /// </summary>
        /// <param name="tick">The tick.</param>
        /// <returns>the number of orders executed using the tick.</returns>
        public int Execute(Tick tick)
        {
            if (GotTick != null) GotTick(tick);
            if (!tick.isTrade) return 0;
            int filledorders = 0;
            foreach (Account a in MasterOrders.Keys)
            { // go through each account
                // if account has requested no executions, skip it
                if (!a.Execute) continue;
                // track orders being removed and trades that need notification
                List<int> notifytrade = new List<int>();
                List<int> remove = new List<int>();
                // go through each order in the account
                for (int i = 0; i < MasterOrders[a].Count; i++)
                { 
                    Order o = MasterOrders[a][i];
                    if (tick.symbol != o.symbol) continue; //make sure tick is for the right stock
                    bool filled = false;
                    if (o.TIF == "OPG")
                    {
                        // if it's already opened, we missed our shot
                        if (hasopened.Contains(o.symbol)) continue;
                        // otherwise make sure it's really the opening
                        if (((o.symbol.Length < 4) && (tick.ex.Contains("NYS"))) ||
                            (o.symbol.Length > 3))
                        {
                            // it's the opening tick, so fill it as an opg
                            filled = o.Fill(tick, true);
                            // mark this symbol as already being open
                            hasopened.Add(tick.symbol);
                        }

                    } 
                    else // otherwise fill order normally
                        filled = o.Fill(tick); // fill our trade
                    if (filled)
                    {
                        remove.Add(i);
                        tick.size -= o.UnsignedSize;
                        if (!MasterTrades.ContainsKey(a.ID)) MasterTrades.Add(a.ID, new List<Trade>());
                        MasterTrades[a.ID].Add((Trade)o); // record trade
                        // market it for notification
                        notifytrade.Add(MasterTrades[a.ID].Count-1);
                        filledorders++; // count the trade
                    }
                }
                // remove the filled orders
                for (int i = remove.Count - 1; i >= 0; i--)
                    MasterOrders[a].RemoveAt(remove[i]);
                // notify subscribers of trade
                if ((GotFill != null) && a.Notify)
                    foreach (int tradeidx in notifytrade)
                        GotFill(MasterTrades[a.ID][tradeidx]); 

            }
            return filledorders;
        }

        /// <summary>
        /// Resets this instance, clears all orders/trades/accounts held by the broker.
        /// </summary>
        public void Reset()
        {
            CancelOrders();
            acctlist.Clear();
            MasterOrders.Clear();
            MasterTrades.Clear();
            AddAccount(DEFAULT);
        }
        protected Dictionary<string, Account> acctlist = new Dictionary<string, Account>();
        protected void AddAccount(Account a)
        {
            Account t = null;
            if (acctlist.TryGetValue(a.ID, out t)) return; // already had it
            MasterOrders.Add(a, new List<Order>());
            MasterTrades.Add(a.ID, new List<Trade>());
            acctlist.Add(a.ID, a);
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
                    GotOrderCancel(o.symbol,o.side,o.id); //send cancel notifcation to any subscribers
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
            Position pos = new PositionImpl(symbol);
            if (!MasterTrades.ContainsKey(a.ID)) return pos;
            foreach (TradeImpl trade in MasterTrades[a.ID]) 
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
            Position pos = new PositionImpl(symbol);
            decimal pl = 0;
            if (!MasterTrades.ContainsKey(a.ID)) return pl;
            foreach (Trade trade in MasterTrades[a.ID])
            {
                if (trade.symbol == pos.Symbol)
                    pl += pos.Adjust(trade);
            }
            return pl;
        }

        FillMode _fm = FillMode.OwnBook;
        /// <summary>
        /// Gets or sets the fill mode this broker uses when executing orders
        /// </summary>
        public FillMode FillMode { get { return _fm; } set { _fm = value; } }

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
            foreach (TradeImpl trade in MasterTrades[a.ID])
            {
                if (!poslist.ContainsKey(trade.symbol))
                {
                    poslist.Add(trade.symbol, new PositionImpl(trade.symbol));
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
            PositionImpl pos = new PositionImpl(symbol);
            decimal points = 0;
            if (!MasterTrades.ContainsKey(account.ID)) return points;
            foreach (TradeImpl t in MasterTrades[account.ID])
            {
                points += Calc.ClosePT(pos, t);
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
            Dictionary<string, PositionImpl> poslist = new Dictionary<string, PositionImpl>();
            Dictionary<string, decimal> ptlist = new Dictionary<string, decimal>();
            if (!MasterTrades.ContainsKey(account.ID)) return 0;
            foreach (TradeImpl trade in MasterTrades[account.ID])
            {
                if (!poslist.ContainsKey(trade.symbol))
                {
                    poslist.Add(trade.symbol, new PositionImpl(trade.symbol));
                    ptlist.Add(trade.symbol, 0);
                }
                ptlist[trade.symbol] += Calc.ClosePT(poslist[trade.symbol], trade);
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


    public enum FillMode
    {
        OwnBook = 0,
        HistBookOnly = 1,
    }
}
