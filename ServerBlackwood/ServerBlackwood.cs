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

        public ServerBlackwood()
        {
            // broker stuff
            SymbolDataHandler = new DisplaySymbolDataHandler(DisplaySymbolData);
			Level1Handler = new DisplayLevel1Handler(DisplayLevel1);
			Level2Handler = new DisplayLevel2Handler(DisplayLevel2);
			TradeHandler = new DisplayTradeHandler(DisplayTrade);
			InfoHandler = new DisplayInfoHandler(DisplayInfo);

            m_Session = new BWSession();

            

            // tradelink stuff
            newProviderName = Providers.Blackwood;
            newFeatureRequest += new MessageArrayDelegate(ServerBlackwood_newFeatureRequest);
            newOrderCancelRequest += new UIntDelegate(ServerBlackwood_newOrderCancelRequest);
            newSendOrderRequest += new OrderDelegate(ServerBlackwood_newSendOrderRequest);
        }

        private void DisplayLevel1(object sender, BWLevel1Quote msgLevel1)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(Level1Handler, new object[] { sender, msgLevel1 });
            }
            else
            {
                if (m_Stock != sender)
                    return;
                debug(msgLevel1.Bid.ToString("F"));
                debug(msgLevel1.Ask.ToString("F"));
                debug(msgLevel1.BidSize.ToString());
                debug(msgLevel1.AskSize.ToString());
            }
        }
        List<BWFeed> feeds = new List<BWFeed>();
        private void DisplayLevel2(object sender, BWLevel2Quote level2Quote)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(Level2Handler, new object[] { sender, level2Quote });
            }
            else
            {
                if (m_Stock != sender)
                    return;
                //	feeds.Add(BWFeed.SDOT);
                feeds.Add(BWFeed.SDOT);
                //	feeds.Add(BWFeed.INET);
                //	feeds.Add(BWFeed.ARCA);
                foreach (BWFeed feed in feeds)
                {

                    BWLevel2Quote[] quotesBid = m_Stock.GetBook(BWQuoteSide.BID, feed);
                    BWLevel2Quote[] quotesAsk = m_Stock.GetBook(BWQuoteSide.ASK, feed);

                    string msg;
                    for (int i = 0; (i < quotesBid.GetLength(0) || i < quotesAsk.GetLength(0)) && i < 8; i++)
                    {
                        msg = "";
                        BWLevel2Quote quote;
                        if (i < quotesBid.GetLength(0))
                        {
                            quote = quotesBid[i];
                            msg = string.Format("{0,6} {1,6} {2,8:F3} {3,8} ", quote.MarketMaker, quote.BWFeed, quote.Bid, quote.BidSize);
                        }
                        else
                        {
                            msg += string.Format("                                ");
                        }

                        if (i < quotesAsk.GetLength(0))
                        {
                            quote = quotesAsk[i];
                            msg += string.Format("{0,6} {1,6} {2,8:F3} {3,8} ", quote.MarketMaker, quote.BWFeed, quote.Ask, quote.AskSize);
                        }
                        else
                        {
                            msg += string.Format("                                ");
                        }
                        //Debug.WriteLine(msg);
                    }
                    //Debug.WriteLine("");
                }

                if ((m_Stock.IsNYSE() && level2Quote.BWFeed != BWFeed.SDOT) ||
                    (!m_Stock.IsNYSE() && level2Quote.BWFeed != BWFeed.NASDAQ))
                    return;

                if (m_Stock.IsNYSE())
                    DisplayLevel2(sender, BWFeed.SDOT);
                else
                    DisplayLevel2(sender, BWFeed.NASDAQ);
            }
        }

        		private void DisplayTrade(object sender, BWTrade msgTrade)
		{
            if (this.InvokeRequired)
            {
                this.BeginInvoke(TradeHandler, new object[] { sender, msgTrade });
            }
            else
            {
                if (m_Stock != sender)
                    return;

                debug(msgTrade.Price.ToString("F"));
                debug((m_Stock.Close() - msgTrade.Price).ToString("F"));
                debug(msgTrade.Time.ToString("HH:mm:ss"));
                debug(m_Stock.GetVolume().ToString());

                string sTrade = string.Format("{0}  {1}  {2}", msgTrade.MarketMaker, msgTrade.Price, msgTrade.Size);
                debug(sTrade);
            }
		}

		private void stock_OnInfoUpdate(object sender, BWStockInfo msgInfo)
		{
			object[] args = { sender, msgInfo };
			this.BeginInvoke(InfoHandler, args);
		//	ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayInfoThread), args);
		}

		private void DisplayInfoThread(Object param)
		{
			object[] args = (object[])param;
			DisplayInfo(args[0], (BWStockInfo)args[1]);
		}

		private void DisplayInfo(object sender, BWStockInfo msgInfo)
		{
            if (this.InvokeRequired)
            {
                this.BeginInvoke(InfoHandler, new object[] { sender, msgInfo });
            }
            else
            {
                if (m_Stock != sender)
                    return;
                debug(msgInfo.Close.ToString());
                debug(msgInfo.Open.ToString());
                debug(msgInfo.Low.ToString("F"));
                debug(msgInfo.High.ToString("F"));
            }
		}

		private void stock_OnSymbolData(object sender, BWSymbolData symbolData)
		{
			object[] args = {sender, symbolData};
			this.BeginInvoke(SymbolDataHandler, args);
		}

		private void DisplaySymbolData(object sender, BWSymbolData symbolData)
		{
            if (this.InvokeRequired)
            {
                this.BeginInvoke(SymbolDataHandler, new object[] { sender, symbolData });
            }
            else
            {
                if (m_Stock != sender)
                    return;

                BWTrade[] Trades = m_Stock.GetTrades(BWFeed.SDOT);

                //Log.Ref.WriteLine(string.Format("MM: Got symbol data message for symbol: {0}", symbolData.Symbol));

                m_SymbolData = new BWSymbolData(symbolData);
                debug(m_SymbolData.StockInfo.Name);
                debug(m_SymbolData.LastTrade.Price.ToString("F"));
                debug((m_SymbolData.StockInfo.Close - m_SymbolData.LastTrade.Price).ToString("F"));
                debug(m_SymbolData.LastTrade.Time.ToString("HH:mm:ss"));
                debug(m_SymbolData.Volume.ToString());
                debug(m_SymbolData.StockInfo.Close.ToString());
                debug(m_SymbolData.StockInfo.Open.ToString());
                debug(m_SymbolData.StockInfo.Low.ToString("F"));
                debug(m_SymbolData.StockInfo.High.ToString("F"));
                debug(m_SymbolData.Level1.Bid.ToString("F"));
                debug(m_SymbolData.Level1.Ask.ToString("F"));
                debug(m_SymbolData.Level1.BidSize.ToString());
                debug(m_SymbolData.Level1.AskSize.ToString());

                string sTrade;
                foreach (FEED_ID feedID in m_SymbolData.Trades.Keys)
                {
                    TradeList listTrade = (TradeList)m_SymbolData.Trades[feedID];
                    foreach (BWTrade msgTrade in listTrade)
                    {
                        sTrade = string.Format("{0}  {1}  {2}", msgTrade.MarketMaker, msgTrade.Price, msgTrade.Size);
                        debug(sTrade);
                    }
                }

                if (m_Stock.IsNYSE())
                    DisplayLevel2(sender, BWFeed.SDOT);
                else
                    DisplayLevel2(sender, BWFeed.NASDAQ);
            }
		}

		private void DisplayLevel2(object sender, BWFeed Feed)
		{		
			try
			{
				BWStock stock = (BWStock)sender;
			
				BWLevel2Quote[] book = stock.GetBook(BWQuoteSide.BID, Feed);
				int count = 10 < book.Length ? 10 : book.Length;
				for (int i=0; i < count; i++) 
				{				
					BWLevel2Quote quoteFromBook = book[i];

					debug(quoteFromBook.Bid.ToString("F"));
					debug(quoteFromBook.BidSize.ToString());
				}	
				book = stock.GetBook(BWQuoteSide.ASK, Feed);
				count = 10 < book.Length ? 10 : book.Length;
				for (int i=0; i < count; i++) 
				{				
					BWLevel2Quote quoteFromBook = book[i];

					debug(quoteFromBook.MarketMaker);
					debug(quoteFromBook.Ask.ToString("F"));
					debug(quoteFromBook.AskSize.ToString());
				}			
			}
			catch (Exception)
			{

			}
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
            return f.ToArray();
            
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(DebugImpl.Create(msg));
        }
    }
}
