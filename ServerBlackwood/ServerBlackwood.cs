using System;
using System.Collections.Generic;
using Blackwood.Framework;
using BWCMessageLib;
using TradeLink.API;
using TradeLink.Common;
namespace ServerBlackwood
{
    public delegate void BWConnectedEventHandler(object sender, bool BWConnected);
    public delegate void TLSendDelegate(string message, MessageTypes type, string client);
   
    public class ServerBlackwood 
    {
        // broker members
        private BWSession m_Session;
        private System.ComponentModel.IContainer components;
        private uint bwHistReqID = 5000;
        public event BWConnectedEventHandler BWConnectedEvent;
        protected virtual void OnBWConnectedEvent(bool BWConnected) { BWConnectedEvent(this, BWConnected); }
        
       
        // tradelink members
        public event DebugFullDelegate SendDebug;
        private bool _valid = false;
        public bool isValid { get { return _valid; } }
        PositionTracker pt = new PositionTracker();
        
        public ServerBlackwood(TLServer tls)
        {
            tl = tls;
            // broker stuff
            m_Session = new BWSession();
            m_Session.OnAccountMessage += new BWSession.AccountMessageHandler(m_Session_OnAccountMessage);
            //m_Session.OnNYSEImbalanceMessage += new BWSession.NYSEImbalanceMessageHandler(m_Session_OnNYSEImbalanceMessage);
            m_Session.OnExecutionMessage += new BWSession.ExecutionMessageHandler(m_Session_OnExecutionMessage);
            //m_Session.OnOrderMessage += new BWSession.OrderMessageHandler(m_Session_OnOrderMessage);
            m_Session.OnPositionMessage += new BWSession.PositionMessageHandler(m_Session_OnPositionMessage);
            m_Session.OnHistMessage += new BWSession.HistoricMessageHandler(m_Session_OnHistMessage);
            m_Session.OnTimeMessage += new BWSession.TimeMessageHandler(m_Session_OnTimeMessage);
            // tradelink stuff
            tl.newProviderName = Providers.Blackwood;
            tl.newAcctRequest += new StringDelegate(ServerBlackwood_newAccountRequest);
            tl.newUnknownRequest += new UnknownMessageDelegate(ServerBlackwood_newUnknownRequest);
            tl.newFeatureRequest += new MessageArrayDelegate(ServerBlackwood_newFeatureRequest);
            tl.newOrderCancelRequest += new LongDelegate(ServerBlackwood_newOrderCancelRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(ServerBlackwood_newSendOrderRequest);
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newPosList += new PositionArrayDelegate(ServerBlackwood_newPosList);
            //tl.newImbalanceRequest += new VoidDelegate(ServerBlackwood_tl.newImbalanceRequest);
            //DOMRequest += new IntDelegate(ServerBlackwood_DOMRequest);
            
            
        }

        bool _noverb = true;
        public bool VerbuseDebugging { get { return !_noverb; } set { _noverb = !value; tl.VerboseDebugging = VerbuseDebugging; } }

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

        void tl_newRegisterSymbols(string client, string symbols)
        {
            Basket b = BasketImpl.FromString(symbols);
            foreach (Security s in b)
            {
                // make sure we don't already have a subscription for this
                if (_symstk.Contains(s.Symbol)) continue;
                BWStock stk = m_Session.GetStock(s.Symbol);
                stk.Subscribe();
                stk.OnTrade += new BWStock.TradeHandler(stk_OnTrade);
                //stk.OnLevel2Update += new BWStock.Level2UpdateHandler(stk_OnLevel2Update);
                stk.OnLevel1Update += new BWStock.Level1UpdateHandler(stk_OnLevel1Update);
                _stocks.Add(stk);
                _symstk.Add(s.Symbol);
                v("added level1 subscription for: " + s.Symbol);
            }
            // get existing list
            Basket list = tl.AllClientBasket;
            // remove old subscriptions
            for (int i = 0; i < _symstk.Count; i++)
            {

                if (!list.ToString().Contains(_symstk[i]) && (_stocks[i]!=null))
                {
                    debug(_symstk[i] + " not needed, removing...");
                    try
                    {
                        _stocks[i].Unsubscribe();
                        _stocks[i] = null;
                        _symstk[i] = string.Empty;
                    }
                    catch { }
                }
            }
        }
        Position[] ServerBlackwood_newPosList(string account)
        {
            foreach (BWStock s in m_Session.GetOpenPositions())
            {
                Position p = new PositionImpl(s.Symbol, (decimal)s.Price, s.Size, (decimal)s.ClosedPNL);
                pt.Adjust(p);
                v(p.Symbol + " found position: " + p.ToString());
            }
            return pt.ToArray();
        }
        List<BWStock> _stocks = new List<BWStock>();
        List<string> _symstk = new List<string>();
        

        void ServerBlackwood_newImbalanceRequest()
        {
            v("received imbalance request.");
            m_Session.RequestNYSEImbalances();
        }
        bool isunique(Order o)
        {
            bool ret = !_bwOrdIds.ContainsKey(o.id);
            return ret;
        }
        
        IdTracker _id = new IdTracker();
        long ServerBlackwood_newSendOrderRequest(Order o)
        {
            v(o.symbol + " received sendorder request for: " + o.ToString());
            if ((o.id != 0) && !isunique(o))
            {
                v(o.symbol + " dropping duplicate order: " + o.ToString());
                return (long)MessageTypes.DUPLICATE_ORDERID;
            }
            if (o.id == 0)
                o.id = _id.AssignId;
            int orderCID = (int)o.id;
            string sSymbol = o.symbol;
            ORDER_SIDE orderSide = (o.side ? ORDER_SIDE.SIDE_BUY : ORDER_SIDE.SIDE_SELL);
            BWVenue orderVenue = getVenueFromBW(o);
            BWOrderType orderType = (o.isStop ? (o.isLimit ? BWOrderType.STOP_LIMIT : BWOrderType.STOP_MARKET) : (o.isLimit ? BWOrderType.LIMIT : BWOrderType.MARKET));
            int orderTIF = (int)getDurationFromBW(o);
            uint  orderSize = (uint)o.UnsignedSize;
            int orderReserve = o.UnsignedSize;
            float orderPrice = (float)o.price;
            float orderStopPrice = (float)o.stopp;
            // create a new BWOrder with these parameters
            BWOrder bwOrder = new BWOrder(m_Session, sSymbol, orderSide, orderSize, orderPrice, orderType, orderTIF, orderVenue, false, orderSize);
            bwOrder.CustomID = orderCID;
            bwOrder.SmartID = orderCID;
            // subscribe to this order's events
            bwOrder.BWOrderUpdateEvent += new BWOrder.BWOrderUpdateHandler(bwOrder_BWOrderUpdateEvent);
            // add a BWStock object for this symbol to the list of stocks that have had orders placed
            // so that it can be referred to for position management
            try
            {
                // GetStock throws an exception if not connected to Market Data
                BWStock stock = m_Session.GetStock(bwOrder.Symbol);
            }
            catch (ClientPortalConnectionException e)
            {
                debug(e.Message);
            }
            // send the order
            bwOrder.Send();
            _bwOrdIds.Add(o.id, 0);
            v(o.symbol + " sent order: " + o.ToString());
            return (long)MessageTypes.OK;
        }
        void ServerBlackwood_newOrderCancelRequest(long tlID)
        {
            v("cancel request received for: " + tlID);
            int bwID = 0;
            bool match = false;
            if (_bwOrdIds.TryGetValue(tlID, out bwID))
            { 
                foreach (BWOrder o in m_Session.GetOpenOrders())
                {
                    if (o.OrderID == (int)bwID)
                    {
                        match = true;
                        o.Cancel();
                        v("found blackwood order: " + o.OrderID + " for tlid: " + tlID + " and requested cancel.");
                    }
                }
            }
            if (!match)
                v("could not cancel order: " + tlID + " as no matching blackwood order was found.");
        }
        MessageTypes[] ServerBlackwood_newFeatureRequest()
        {
            v("received feature request.");
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.OK);
            f.Add(MessageTypes.BROKERNAME);
            f.Add(MessageTypes.CLEARCLIENT);
            f.Add(MessageTypes.CLEARSTOCKS);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.VERSION);
            f.Add(MessageTypes.IMBALANCEREQUEST);
            //f.Add(MessageTypes.IMBALANCERESPONSE);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.BARREQUEST);
            f.Add(MessageTypes.BARRESPONSE);
            return f.ToArray();
        }
        long ServerBlackwood_newUnknownRequest(MessageTypes t, string msg)
        {
            int _depth = 0;
            MessageTypes ret = MessageTypes.UNKNOWN_MESSAGE;
            switch (t)
            {
                case MessageTypes.DOMREQUEST:
                    
                    _depth = Convert.ToInt32(msg);
                    v("received DOM request for depth: " + _depth);
                    ret = MessageTypes.OK;
                    break;
                case MessageTypes.ISSHORTABLE:
                    return (long)(m_Session.GetStock(msg).IsHardToBorrow() ? 0 : 1);
                case MessageTypes.BARREQUEST:
                    v("received bar request: " + msg);
                    string[] r = msg.Split(',');
                    DBARTYPE barType = getBarTypeFromBW(r[(int)BarRequestField.BarInt]);
                    int tlDateS = int.Parse(r[(int)BarRequestField.StartDate]);
                    int tlTimeS = int.Parse(r[(int)BarRequestField.StartTime]);
                    DateTime dtStart = TradeLink.Common.Util.ToDateTime(tlDateS,tlTimeS);
                    int tlDateE = int.Parse(r[(int)BarRequestField.StartDate]);
                    int tlTimeE = int.Parse(r[(int)BarRequestField.StartTime]);
                    DateTime dtEnd = TradeLink.Common.Util.ToDateTime(tlDateE, tlTimeE);
                    uint custInt = 1;
                    if (!uint.TryParse(r[(int)BarRequestField.CustomInterval], out custInt))
                    {
                       custInt = 1;
                    }
                 
                    m_Session.RequestHistoricData(r[(int)BarRequestField.Symbol], barType, dtStart, dtEnd, custInt, ++bwHistReqID);
                    ret = MessageTypes.OK;
                    break;
            }
            return (long)ret;
        }
        string ServerBlackwood_newAccountRequest()
        {
            return _acct;
        }
        void stk_OnLevel1Update(object sender, BWLevel1Quote quote)
        {
            Tick k = new TickImpl(quote.Symbol);
            k.depth = 0;
            k.bid = (decimal)quote.Bid;
            k.BidSize = quote.BidSize;
            k.ask = (decimal)quote.Ask;
            k.os = quote.AskSize;
            k.date = date;
            k.time = TradeLink.Common.Util.ToTLTime();
            tl.newTick(k);
        }

        int date = TradeLink.Common.Util.ToTLDate();
        int time = 0;

        void m_Session_OnTimeMessage(object sender, BWTime timeMsg)
        {
            date = TradeLink.Common.Util.ToTLDate(timeMsg.ServerTime);
            time = TradeLink.Common.Util.ToTLTime(timeMsg.ServerTime);
        }
        void stk_OnLevel2Update(object sender, BWLevel2Quote quote)
        {
            Tick k = new TickImpl(quote.Symbol);
            k.depth = quote.EcnOrder;
            k.bid = (decimal)quote.Bid;
            k.BidSize = quote.BidSize;
            k.be = quote.MarketMaker;
            k.ask = (decimal)quote.Ask;
            k.os = quote.AskSize;
            k.oe = quote.MarketMaker;
            k.date = date;
            k.time = TradeLink.Common.Util.ToTLTime();
            tl.newTick(k);
        }
        void stk_OnTrade(object sender, BWTrade print)
        {
            Tick k = new TickImpl(print.Symbol);
            k.trade = (decimal)print.Price;
            k.size = print.Size;
            k.ex = print.MarketMaker;
            k.date = date;
            k.time = TradeLink.Common.Util.ToTLTime();
            tl.newTick(k);
        }
        TLServer tl;
        public void Start()
        {
            if (tl != null)
                tl.Start();
        }
        //Redundant, already subscribing to order update event.
        //void m_Session_OnOrderMessage(object sender, BWOrder orderMsg)
        //{
        //    Order o = new OrderImpl(orderMsg.Symbol, (int)orderMsg.Size);
        //    o.side = (orderMsg.OrderSide == ORDER_SIDE.SIDE_BUY) || (orderMsg.OrderSide == ORDER_SIDE.SIDE_COVER);
        //    o.stopp = (decimal)orderMsg.StopPrice;
        //    o.price = (decimal)orderMsg.LimitPrice;
        //    o.time = TradeLink.Common.Util.DT2FT(orderMsg.OrderTime);
        //    o.date = TradeLink.Common.Util.ToTLDate(orderMsg.OrderTime);
        //    o.ex = orderMsg.Venue.ToString();
        //    o.id = (long)orderMsg.CustomID;
        //    o.Account = _acct;
        //    tl.newOrder(o);
        //}
        double _cpl = 0;
        string _acct = string.Empty;
        void m_Session_OnAccountMessage(object sender, BWAccount accountMsg)
        {
            
            string str = m_Session.Account;
            string[] strArr = str.Split('~');
            _acct = strArr[0];
            _cpl = accountMsg.ClosedProfit;
            
        }
        void m_Session_OnExecutionMessage(object sender, BWExecution executionMsg)
        {
            foreach (KeyValuePair<long,int> ordID in _bwOrdIds)
                if ( ordID.Value == executionMsg.OrderID)
                {
                    Trade t = new TradeImpl(executionMsg.Symbol, (decimal)executionMsg.Price, executionMsg.Size);
                    t.side = (executionMsg.Side == ORDER_SIDE.SIDE_COVER) || (executionMsg.Side == ORDER_SIDE.SIDE_BUY);
                    t.xtime = TradeLink.Common.Util.DT2FT(executionMsg.ExecutionTime);
                    t.xdate = TradeLink.Common.Util.ToTLDate(executionMsg.ExecutionTime);
                    t.Account = executionMsg.UserID.ToString();
                    t.id = ordID.Key;
                    t.ex = executionMsg.MarketMaker; 
                    tl.newFill(t);
                    v(t.symbol + " sent fill notification for: " + t.ToString());
                }
        }
        void m_Session_OnNYSEImbalanceMessage(object sender, BWNYSEImbalance imbalanceMsg)
        {
            string s = imbalanceMsg.Symbol;
            int i = imbalanceMsg.ImbalanceVolume;
            int it = TradeLink.Common.Util.DT2FT(imbalanceMsg.Time);
            int pi = imbalanceMsg.InitImbalanceVolume;
            int pt = TradeLink.Common.Util.DT2FT(imbalanceMsg.InitTime);
            string ex = imbalanceMsg.FeedID.ToString();
            Imbalance imb = new ImbalanceImpl(s, ex, i, it, pi, pt, i);
            tl.newImbalance(imb);
        }
        void m_Session_OnPositionMessage(object sender, BWPosition positionMsg)
        {
            string sym = positionMsg.Symbol;
            int size = positionMsg.Size;
            decimal price = (decimal)positionMsg.Price;
            decimal cpl = (decimal)positionMsg.CloseProfit;
            //string ac = positionMsg.UserID.ToString();
            Position p = new PositionImpl(sym, price, size, cpl, _acct);
            pt.NewPosition(p);
            v(p.Symbol + " new position information: " + p.ToString());
        }
        public bool Start(string user, string pw, string ipaddress, int data2)
        {
            v("got start request on blackwood connector.");
            System.Net.IPAddress bwIP = System.Net.IPAddress.Parse(ipaddress);
            
            // register for notification of a disconnection from the client portal
            m_Session.OnMarketDataClientPortalConnectionChange += new BWSession.ClientPortalConnectionChangeHandler(OnMarketConnectionChange);
            
            try
            {
                m_Session.ConnectToOrderRouting(user, pw, bwIP, Properties.Settings.Default.orderport, true, true, true);
                m_Session.ConnectToHistoricData(user, pw, bwIP, Properties.Settings.Default.historicalport);
                m_Session.ConnectToMarketData(user, pw, bwIP, Properties.Settings.Default.dataport, true);
                //if (chkUseMulticast.Checked)
                //	m_Session.ConnectToMulticast(System.Net.IPAddress.Parse(txtBoxMultiServerIP.Text), Convert.ToInt32(txtMultiDataPort.Text), true);	
            }
            catch (Blackwood.Framework.ClientPortalConnectionException)
            {
                debug("error: Unable to connect to market data client portal.");
                _valid = false;
                return _valid;
            }
            _valid = true;
            return _valid;
        }
        public new void Stop()
        {
            try
            {
                v("got stop request on blackwood connector.");
                m_Session.DisconnectFromOrders();
                m_Session.DisconnectFromHistoricData();
                m_Session.DisconnectFromMarketData();
                m_Session.CloseSession();
                m_Session.OnMarketDataClientPortalConnectionChange -= new BWSession.ClientPortalConnectionChangeHandler(OnMarketConnectionChange);
            }
            catch { }
        }
        private void OnMarketConnectionChange(object sender, bool Connected)
        {
            
            //Make sure both market data and order routing is alive before connected is true.
            if (m_Session.IsConnectedToMarketData & m_Session.IsConnectedToOrderRouting)
            {
                OnBWConnectedEvent(Connected);
            }
            else
            {
                OnBWConnectedEvent(false);
            }
            //Send to debug window detailed connection info.
            debug("connected market data: " + m_Session.IsConnectedToMarketData.ToString());
            debug("connected order port: " + m_Session.IsConnectedToOrderRouting.ToString());
            debug("connected history port: " + m_Session.IsConnectedToHistoricData.ToString());
        }
        private void m_Session_OnHistMessage(object sender, BWHistResponse histMsg)
        {
            if (histMsg.Error.Length > 0)
            {
                debug("ERROR: " + histMsg.Error);
            }
            else
            {
                v(histMsg.Symbol + " received bar history data containing " + histMsg.bars.Length + " bars.");
                if (histMsg.bars != null && histMsg.bars.Length > 0)
                {
                    string sym = histMsg.Symbol;
                                        
                    foreach (BWBar bar in histMsg.bars)
                    {
                        int tlDate = TradeLink.Common.Util.ToTLDate(bar.time);
                        int tlTime = TradeLink.Common.Util.ToTLTime(bar.time);
                        Bar tlBar = new BarImpl((decimal)bar.open, (decimal)bar.high, (decimal)bar.low, (decimal)bar.close, (int)bar.volume, tlDate, tlTime,sym,(int)histMsg.Interval);
                        for (int i = 0; i < tl.NumClients; i++)
                            tl.TLSend(BarImpl.Serialize(tlBar), MessageTypes.BARRESPONSE, i.ToString());
                        
                       
                    }
                }
                //else if (histMsg.ticks != null && histMsg.ticks.Length > 0)
                //{
                //    foreach (BWTickData tick in histMsg.ticks)
                //    {
                //        Tick tlTick = new TickImpl(tick.symbol);
                //        tlTick.ask = (decimal)tick.askprice;
                //        tlTick.AskSize = (int)tick.asksize;
                //        tlTick.bid = (decimal)tick.bidprice;
                //        tlTick.BidSize = (int)tick.bidsize;
                //        tlTick.trade = (decimal)tick.price;
                //        tlTick.size = (int)tick.size;
                //    }
                //}
            }
        }
        private BWTIF getDurationFromBW(Order o)
        {
            BWTIF bwTIF;
            string strTIF = o.TIF;
            switch (strTIF)
            {
                case "DAY":
                    bwTIF = BWTIF.DAY;
                    break;
                case "IOC":
                    bwTIF = BWTIF.IOC;
                    break;
                case "FOK":
                    bwTIF = BWTIF.FOK;
                    break;
                case "CLO":
                    bwTIF = BWTIF.CLO;
                    break;
                case "OPG":
                    bwTIF = BWTIF.OPG;
                    break;
                default:
                    bwTIF = BWTIF.DAY;
                    break;
            }
            return bwTIF;
        }
        private BWVenue getVenueFromBW(Order o)
        {
            BWVenue bwVenue;
            string strFeed = o.ex;
            switch (strFeed)
            {
                case "ARCA":
                    bwVenue = BWVenue.ARCA;
                    break;
                case "BATS":
                    bwVenue = BWVenue.BATS;
                    break;
                case "INET":
                    bwVenue = BWVenue.INET;
                    break;
                case "NASDAQ":
                    bwVenue = BWVenue.NASDAQ;
                    break;
                case "SDOT":
                    bwVenue = BWVenue.SDOT;
                    break;
                case "NITE":
                    bwVenue = BWVenue.NITE;
                    break;
                case "EDGA":
                    bwVenue = BWVenue.EDGA;
                    break;
                case "EDGX":
                    bwVenue = BWVenue.EDGX;
                    break;
                case "CSFB":
                    bwVenue = BWVenue.CSFB;
                    break;
                default:
                    bwVenue = BWVenue.NONE;
                    break;
            }
            return bwVenue;
        }
        private DBARTYPE getBarTypeFromBW(string str)
        {
            DBARTYPE bwType;
            switch (str)
            {
                case "DAILY":
                    bwType = DBARTYPE.DAILY;
                    break;
                case "WEEKLY":
                    bwType = DBARTYPE.WEEKLY;
                    break;
                case "MONTHLY":
                    bwType = DBARTYPE.MONTHLY;
                    break;
                case "TICK":
                    bwType = DBARTYPE.TICK;
                    break;
                case "INTRADAY":
                default:
                    bwType = DBARTYPE.INTRADAY;
                    break;
            }
            return bwType;
        }
        //Keep cross reference list between TL order ID and BW order ID.
        Dictionary<long, int> _bwOrdIds = new Dictionary<long, int>();
        void bwOrder_BWOrderUpdateEvent(object sender, BWOrderStatus BWOrderStatus)
        {
            BWOrder bwo = (BWOrder)sender;
            long id = (long)bwo.CustomID;
            Order o = new OrderImpl(bwo.Symbol, (int)bwo.Size);
            o.id = (long)bwo.CustomID;
            o.side = (bwo.OrderSide == ORDER_SIDE.SIDE_BUY) || (bwo.OrderSide == ORDER_SIDE.SIDE_COVER);
            o.price = (decimal)bwo.LimitPrice;
            o.stopp = (decimal)bwo.StopPrice;
            o.Account = bwo.UserID.ToString();
            o.ex = bwo.Venue.ToString();
        
            switch (BWOrderStatus)
            {
                case BWOrderStatus.ACCEPTED:
                    {
 
                        tl.newOrder(o);
                        v(o.symbol + " sent order acknowledgement for: " + o.ToString());
                        if (_bwOrdIds.ContainsKey(o.id))
                            {
                                _bwOrdIds[o.id] = bwo.OrderID;
                            } 
                    }
                    break;
                case BWOrderStatus.CANCELED:
                    {
                        
                        tl.newCancel(id);
                        v("sent cancel notification for order: " + id);
                        
                    }
                    break;
                case BWOrderStatus.REJECTED:
                    {
                            tl.newCancel(id);
                        debug("Rejected: " + bwo.CustomID.ToString() + bwo.RejectReason);
                    }
                    break;
            }
        }
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(DebugImpl.Create(msg));
        }
     }
}
