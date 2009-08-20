using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using Blackwood.Framework;
using BWCMessageLib;

namespace ServerBlackwood
{

    public class ServerBlackwood : TLServer_WM
    {

        // broker members
        private BWStock m_Stock = null;
        private BWSymbolData m_SymbolData = null;
        private BWSession m_Session;
        		private delegate void DisplaySymbolDataHandler(object sender, BWSymbolData symbolData);
		private DisplaySymbolDataHandler SymbolDataHandler;
		private delegate void DisplayLevel1Handler(object sender, BWLevel1Quote level1Quote);
		private DisplayLevel1Handler Level1Handler;
		private delegate void DisplayLevel2Handler(object sender, BWLevel2Quote level2Quote);
		private DisplayLevel2Handler Level2Handler;
		private delegate void DisplayTradeHandler(object sender, BWTrade trade);
		private DisplayTradeHandler TradeHandler;
	
		private delegate void DisplayInfoHandler(object sender, BWStockInfo info);
		private DisplayInfoHandler InfoHandler;

        // tradelink members
        public event DebugFullDelegate SendDebug;
        private bool _valid = false;
        public bool isValid { get { return _valid; } }
        PositionTracker pt = new PositionTracker();

        public ServerBlackwood()
        {
            // broker stuff
            m_Session = new BWSession();
            m_Session.OnAccountMessage += new BWSession.AccountMessageHandler(m_Session_OnAccountMessage);
            m_Session.OnNYSEImbalanceMessage += new BWSession.NYSEImbalanceMessageHandler(m_Session_OnNYSEImbalanceMessage);
            m_Session.OnExecutionMessage += new BWSession.ExecutionMessageHandler(m_Session_OnExecutionMessage);
            m_Session.OnOrderMessage += new BWSession.OrderMessageHandler(m_Session_OnOrderMessage);
            foreach (BWStock s in m_Session.GetOpenPositions())
            {
                Position p = new PositionImpl(s.Symbol, (decimal)s.Price, s.Size, (decimal)s.ClosedPNL);
                pt.Adjust(p);
            }
                

            // tradelink stuff
            newProviderName = Providers.Blackwood;
            newUnknownRequest += new UnknownMessageDelegate(ServerBlackwood_newUnknownRequest);
            newFeatureRequest += new MessageArrayDelegate(ServerBlackwood_newFeatureRequest);
            newOrderCancelRequest += new UIntDelegate(ServerBlackwood_newOrderCancelRequest);
            newSendOrderRequest += new OrderDelegate(ServerBlackwood_newSendOrderRequest);
            newImbalanceRequest += new VoidDelegate(ServerBlackwood_newImbalanceRequest);
            newRegisterStocks += new DebugDelegate(ServerBlackwood_newRegisterStocks);
            newPosList += new PositionArrayDelegate(ServerBlackwood_newPosList);
        }

        Position[] ServerBlackwood_newPosList(string account)
        {
            return pt.ToArray();
        }

        void ServerBlackwood_newRegisterStocks(string msg)
        {
            Basket b = BasketImpl.FromString(msg);
            foreach (Security s in b)
            {
                // make sure we don't already have a subscribption for this
                if (_symstk.Contains(s.Symbol)) continue;
                BWStock stk = m_Session.GetStock(s.Symbol);
                stk.Subscribe();
                stk.OnTrade += new BWStock.TradeHandler(stk_OnTrade);
                stk.OnLevel2Update += new BWStock.Level2UpdateHandler(stk_OnLevel2Update);
                stk.OnLevel1Update += new BWStock.Level1UpdateHandler(stk_OnLevel1Update);
                _stocks.Add(stk);
                _symstk.Add(s.Symbol);
            }
            

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
            newTick(k);
        }

        void stk_OnLevel1Update(object sender, BWLevel1Quote quote)
        {
            if (_depth > 0) return;

        }

        void stk_OnTrade(object sender, BWTrade print)
        {
            Tick k = new TickImpl(print.Symbol);
            k.trade = (decimal)print.Price;
            k.size = print.Size;
            k.ex = print.MarketMaker;
            newTick(k);
        }

        void ServerBlackwood_newImbalanceRequest()
        {
            m_Session.RequestImbalances();
        }
        int _depth = 0;
        long ServerBlackwood_newUnknownRequest(MessageTypes t, string msg)
        {
            MessageTypes ret = MessageTypes.UNKNOWN_MESSAGE;
            switch (t)
            {
                case MessageTypes.DOMREQUEST:
                    _depth = Convert.ToInt32(msg);
                    ret = MessageTypes.OK;
                    break;
                case MessageTypes.ISSHORTABLE:
                    return (long)(m_Session.GetStock(msg).IsHardToBorrow() ? 0 : 1);
                    break;
            }
            return (long)ret;
        }

        void m_Session_OnOrderMessage(object sender, BWOrder orderMsg)
        {
            Order o = new OrderImpl(orderMsg.Symbol,(int)orderMsg.Size);
            o.side = (orderMsg.OrderSide == ORDER_SIDE.SIDE_BUY) || (orderMsg.OrderSide == ORDER_SIDE.SIDE_COVER);
            o.stopp = (decimal)orderMsg.StopPrice;
            o.price = (decimal)orderMsg.LimitPrice;
            o.time = TradeLink.Common.Util.DT2FT(orderMsg.OrderTime);
            o.date = TradeLink.Common.Util.ToTLDate(orderMsg.OrderTime);
            o.ex = orderMsg.Venue.ToString();
            o.id = (uint)orderMsg.CustomID;
            newOrder(o);
        }

        string _acct = string.Empty;
        double _cpl = 0;

        void m_Session_OnAccountMessage(object sender, BWAccount accountMsg)
        {
            _cpl = accountMsg.ClosedProfit;
        }

        void m_Session_OnExecutionMessage(object sender, BWExecution executionMsg)
        {
            Trade t = new TradeImpl(executionMsg.Symbol, (decimal)executionMsg.Price, executionMsg.Size);
            t.side = (executionMsg.Side == ORDER_SIDE.SIDE_COVER) || (executionMsg.Side == ORDER_SIDE.SIDE_BUY);
            t.xtime = TradeLink.Common.Util.DT2FT(executionMsg.ExecutionTime);
            t.xdate = TradeLink.Common.Util.ToTLDate(executionMsg.ExecutionTime);
            t.Account = executionMsg.Account;
            // might need to keep a table if this is not custom id,
            // which looks like it's not
            t.id = (uint)executionMsg.OrderID;
            newFill(t);
        }

        void m_Session_OnNYSEImbalanceMessage(object sender, BWNYSEImbalance imbalanceMsg)
        {
            string s = imbalanceMsg.Symbol;
            int i = imbalanceMsg.ImbalanceVolume;
            int it = TradeLink.Common.Util.DT2FT(imbalanceMsg.Time);
            int pi = imbalanceMsg.InitImbalanceVolume;
            int pt = TradeLink.Common.Util.DT2FT(imbalanceMsg.InitTime);
            string ex = imbalanceMsg.FeedID.ToString();
            Imbalance imb = new ImbalanceImpl(s,ex,i,it,pi,pt,i);
            newImbalance(imb);
            
        }


		private void stock_OnSymbolData(object sender, BWSymbolData symbolData)
		{
			object[] args = {sender, symbolData};
			this.BeginInvoke(SymbolDataHandler, args);
		}



		private void DisplayLevel2(object sender, BWFeed Feed)
		{		

		}



        public bool Start(string user, string pw, string ipaddress, int data2 )
        {
            				// grab IP from textBox
				System.Net.IPAddress bwIP = System.Net.IPAddress.Parse(ipaddress);
			
				// register for notification of a disconnection from the client portal
                m_Session.OnMarketDataClientPortalConnectionChange += new Blackwood.Framework.BWSession.ClientPortalConnectionChangeHandler(OnMarketDataClientPortalConnectionChange);
               	// register for notification of a disconnection from the client portal
			   //m_Session.OnOrdersClientPortalConnectionChange += new Blackwood.Framework.BWSession.ClientPortalConnectionChangeHandler(OnOrderClientPortalConnectionChange);

				// equivalent to m_session.ConnectToMarketData
				// calls the overload of ConnectionToMarketData that takes an IP but uses the default port
				try 
				{
					m_Session.ConnectToMarketData(user, pw, bwIP, Properties.Settings.Default.dataport, true);
					m_Session.ConnectToOrderRouting(user, pw, bwIP, Properties.Settings.Default.orderport,true, true, true);
					m_Session.ConnectToHistoricData(user, pw, bwIP, Properties.Settings.Default.historicalport);
					
					//if (chkUseMulticast.Checked)
					//	m_Session.ConnectToMulticast(System.Net.IPAddress.Parse(txtBoxMultiServerIP.Text), Convert.ToInt32(txtMultiDataPort.Text), true);	
				}
				catch (Blackwood.Framework.ClientPortalConnectionException) 
				{
					debug("error: Unable to connect to market data client portal.");
				}
				finally 
				{
					if (m_Session.ConnectedToMarketData)
					{
                        
						debug("connected");
                        _valid = true;
					}
				}

				m_Session.DisconnectFromMarketData();
				if (!m_Session.ConnectedToMarketData) 
				{
                    debug("disconnected");
                    _valid = false;
				}


                return _valid;
        }

        private void OnMarketDataClientPortalConnectionChange(object sender, bool Connected)
        {
            debug("connected: " + Connected);
        }

        private BWTIF getDurationFromComboBox(Order o)
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

        private BWVenue getVenueFromComboBox(Order o)
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


        void ServerBlackwood_newSendOrderRequest(Order o)
        {
            string sSymbol = o.symbol;

            ORDER_SIDE orderSide = (o.side ? ORDER_SIDE.SIDE_BUY : ORDER_SIDE.SIDE_SELL);
            BWVenue orderVenue = getVenueFromComboBox(o);
            BWOrderType orderType = (o.isStop ? (o.isStop ? BWOrderType.STOP_LIMIT : BWOrderType.STOP_MARKET) : (o.isLimit ? BWOrderType.LIMIT : BWOrderType.MARKET));
            int orderTIF = (int)getDurationFromComboBox(o);

            uint orderSize = (uint)o.UnsignedSize;
            int orderReserve = o.UnsignedSize;
            float orderPrice = (float)o.price;
            float orderStopPrice = (float)o.stopp;

            // create a new BWOrder with these parameters
            BWOrder bwOrder = new BWOrder(m_Session, sSymbol, orderSide, orderSize, orderPrice, orderStopPrice, orderType, orderTIF, orderVenue, false, orderSize);

            //if (chkPartDontInit.Checked)
            //    bwOrder.PartDontInit = true;
            // subsribe to this order's events
            bwOrder.BWOrderUpdateEvent += new BWOrder.BWOrderUpdateHandler(bwOrder_BWOrderUpdateEvent);

            // add a BWStock object for this symbol to the list of stocks that have had orders placed
            // so that it can be referred to for position management
            try
            {
                // GetStock throws an exception if not connected to Market Data
                BWStock stock = m_Session.GetStock(bwOrder.Symbol);
            }
            catch (ClientPortalConnectionException)
            {
                debug("warning: Not connected to Market Data.  Unable to create BWStock object.  Position management through BWStock object will be unavailable.");
            }
            // send the order
            bwOrder.Send();




        }

        List<BWStock> _stocks = new List<BWStock>();
        List<string> _symstk = new List<string>();

        void bwOrder_BWOrderUpdateEvent(object sender, BWOrderStatus BWOrderStatus)
        {
            BWOrder bwo = (BWOrder)sender;
            switch (BWOrderStatus)
            {
                case BWOrderStatus.ACCEPTED:
                    {
                        Order o = new OrderImpl(bwo.Symbol, (int)bwo.Size);
                        o.side = (bwo.OrderSide == ORDER_SIDE.SIDE_BUY) || (bwo.OrderSide == ORDER_SIDE.SIDE_COVER);
                        o.price = (decimal)bwo.LimitPrice;
                        o.stopp = (decimal)bwo.StopPrice;
                        o.id = (uint)bwo.CustomID;
                        newOrder(o);
                    }
                    break;
                case BWOrderStatus.CANCELED:
                    {
                        uint id = (uint)bwo.CustomID;
                        newOrderCancel(id);
                    }
                    break;
            }

        }

        void ServerBlackwood_newOrderCancelRequest(uint number)
        {
            foreach (BWOrder o in m_Session.GetOpenOrders())
                if (o.CustomID == (int)number)
                    o.Cancel();
        }

        public void Stop()
        {
            try
            {
                m_Session.CloseSession();
            }
            catch { }
        }

        MessageTypes[] ServerBlackwood_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.IMBALANCEREQUEST);
            f.Add(MessageTypes.IMBALANCERESPONSE);
            f.Add(MessageTypes.DOMREQUEST);
            f.Add(MessageTypes.DOMRESPONSE);
            f.Add(MessageTypes.ISSHORTABLE);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            return f.ToArray();
            
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(DebugImpl.Create(msg));
        }
    }
}
