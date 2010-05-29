using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;
using MBTCOMLib;
using MBTORDERSLib;
using MBTQUOTELib;


namespace ServerMB
{
    public partial class ServerMBMain : AppTracker, IMbtQuotesNotify
    {
        TLServer_WM tl = new TLServer_WM();
        public MbtComMgr m_ComMgr;
        public MbtOrderClient m_OrderClient;
        public MbtQuotes m_Quotes;
        public MbtOpenOrders m_Orders;
        PositionTracker pt = new PositionTracker();
        bool showmessage = false;
        DebugWindow _dw = new DebugWindow();
        public const string PROGRAM = "ServerMB BETA";
        Log _log = new Log(PROGRAM);
        
        Dictionary<long, string> tl2broker = new Dictionary<long, string>();
        Dictionary<string, long> broker2tl = new Dictionary<string, long>();
        List<string> sentNewOrders = new List<string>();
        Dictionary<string, MbtOpenOrder> orders = new Dictionary<string, MbtOpenOrder>();
        Dictionary<string, bool> cancelids = new Dictionary<string, bool>();
        Dictionary<long, bool> canceledids = new Dictionary<long, bool>();
        
        public ServerMBMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            m_ComMgr = null;
            m_ComMgr = new MbtComMgrClass();
            m_ComMgr.SilentMode = true;
            m_ComMgr.EnableSplash(false);
            m_OrderClient = m_ComMgr.OrderClient;
            m_OrderClient.SilentMode = true;
            if( !m_ComMgr.IsPreviousInstanceDetected("tradelink") )
            {
            	m_OrderClient.OnDemandMode = false;
            }
            m_Quotes = m_ComMgr.Quotes;
            m_Orders = m_OrderClient.OpenOrders;
            
            // tradelink bindings
            tl.newProviderName = Providers.MBTrading;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_newSendOrderRequest);
            tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);

            // mb bindings
			m_OrderClient.OnAccountLoaded += new _IMbtOrderClientEvents_OnAccountLoadedEventHandler( m_OrderClient_OnAccountLoaded );
			m_OrderClient.OnAccountLoading += new _IMbtOrderClientEvents_OnAccountLoadingEventHandler( m_OrderClient_OnAccountLoading );
			m_OrderClient.OnSubmit += new _IMbtOrderClientEvents_OnSubmitEventHandler( m_OrderClient_OnSubmit );
			m_OrderClient.OnClose += new _IMbtOrderClientEvents_OnCloseEventHandler( m_OrderClient_OnClose );
			m_OrderClient.OnConnect += new _IMbtOrderClientEvents_OnConnectEventHandler( m_OrderClient_OnConnect );
			m_OrderClient.OnLogonSucceed += new _IMbtOrderClientEvents_OnLogonSucceedEventHandler( m_OrderClient_OnLogonSucceed );
			m_ComMgr.OnCriticalShutdown += new IMbtComMgrEvents_OnCriticalShutdownEventHandler( m_ComMgr_OnCriticalShutdown );
			m_ComMgr.OnHealthUpdate += new IMbtComMgrEvents_OnHealthUpdateEventHandler( m_ComMgr_OnHealthUpdate );
			m_ComMgr.OnLogonSucceed += new IMbtComMgrEvents_OnLogonSucceedEventHandler( m_ComMgr_OnLogonSucceed );
			m_ComMgr.OnLogonDeny += new IMbtComMgrEvents_OnLogonDenyEventHandler( m_ComMgr_OnLogonDeny );
			m_Quotes.OnClose += new _IMbtQuotesEvents_OnCloseEventHandler( m_Quotes_OnClose );
			m_Quotes.OnConnect += new _IMbtQuotesEvents_OnConnectEventHandler( m_Quotes_OnConnect );
			m_Quotes.OnLogonSucceed += new _IMbtQuotesEvents_OnLogonSucceedEventHandler( m_Quotes_OnLogonSucceed );
	    	m_OrderClient.OnPositionAdded += new _IMbtOrderClientEvents_OnPositionAddedEventHandler( m_OrderClient_OnPositionAdded );
	    	m_OrderClient.OnPositionUpdated += new _IMbtOrderClientEvents_OnPositionUpdatedEventHandler( m_OrderClient_OnPositionUpdated);
	    	//m_OrderClient.OnExecute += new _IMbtOrderClientEvents_OnExecuteEventHandler( m_OrderClient_OnExecute );
	    	m_OrderClient.OnCancelPlaced += new _IMbtOrderClientEvents_OnCancelPlacedEventHandler( m_OrderClient_OnCancelPlaced );
	    	m_OrderClient.OnCancelRejected += new _IMbtOrderClientEvents_OnCancelRejectedEventHandler( m_OrderClient_OnCancelRejected );
	    	//m_OrderClient.OnRemove += new _IMbtOrderClientEvents_OnRemoveEventHandler( m_OrderClient_OnRemove );
	    	m_OrderClient.OnHistoryAdded += new _IMbtOrderClientEvents_OnHistoryAddedEventHandler( m_OrderClient_OnHistoryAdded );
			
            FormClosing += new FormClosingEventHandler(ServerMBMain_FormClosing);

        }
        
        void m_Quotes_OnClose(int ErrorCode)
        {
        	debug(String.Format("quotes close:{0}", ErrorCode));
        }
        
        void m_Quotes_OnConnect(int ErrorCode)
        {
        	debug( String.Format("quotes connect:{0}",ErrorCode));
        }
        
        void m_Quotes_OnLogonSucceed()
        {
        	debug("quotes connected");
        }

        void m_ComMgr_OnLogonDeny(string bstrReason)
        {
            debug("login denied: " + bstrReason);
            BackColor = Color.Red;
        }

        void m_ComMgr_OnLogonSucceed()
        {
            debug("login successful");
            BackColor = Color.Green;
        }
        
        void m_ComMgr_OnHealthUpdate(enumServerIndex index, enumConnectionState state)
        {
        	debug(String.Format("health update:{0} {1}", index, state));
        }
        
        void m_OrderClient_OnAccountLoading(MbtAccount account)
        {
        	debug(String.Format("account loading:{0}", account.Account));
        }
        
        void m_OrderClient_OnAccountLoaded(MbtAccount account)
        {
        	debug(String.Format("account loaded:{0}", account.Account));
        }

        void m_OrderClient_OnAccountUnavailable(MbtAccount account)
        {
        	debug(String.Format("account unavailable:{0}", account.Account));
        }
        
        void m_OrderClient_OnAcknowledge(MbtOpenOrder pOrd)
        {
        	debug(String.Format("order acknowledge:{0}", pOrd.OrderNumber));
        }
        
        void m_OrderClient_OnBalanceUpdate(MbtAccount account)
        {
        	debug(String.Format("account balance updated:{0}", account.Account));
        }
        
        void m_ComMgr_OnCriticalShutdown()
        {
        	debug("critical shutdown");
        }
        
        void rightmessage(object sender, EventArgs e)
        {
            _dw.Toggle();
        }



        void ServerMBMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            try
            {
                _log.Stop();
                m_Quotes.Disconnect();
                m_OrderClient.Disconnect();
            }
            catch (Exception) { }
        }

        long tl_newUnknownRequest( MessageTypes t, string msg )
        {
        	debug(String.Format("Unknown message {0}: {1}", t, msg));
        	return (long)MessageTypes.UNKNOWN_MESSAGE;
        }
        
        Position[] tl_newPosList(string account)
        {
            test();
            int num = m_OrderClient.Positions.Count;
            //TODO: enable some settings in app.config, i.e. VerboseDebugging as done in ServerSterling
            //debug(String.Format("tl_newPosList called for {0} positions:{1}", account, num));
            Position[] posl = new Position[num];
            for (int i = 0; i < num; i++)
            {
                string acct = m_OrderClient.Positions[i].Account.Account;
                decimal cpl = (decimal)m_OrderClient.Positions[i].RealizedPNL;
                int size= m_OrderClient.Positions[i].IntradayPosition;
                decimal price = (decimal)m_OrderClient.Positions[i].IntradayPrice;
                string sym = m_OrderClient.Positions[i].Symbol;
                Position p = new PositionImpl(sym, price, size, cpl, acct);
                posl[i] = p;
                //debug(String.Format("newPosList {4} i:{0} {1} {2} {3}", i, sym, price, size, acct));
            }
            return posl;
        }
        
        void OnPositionChanged( MbtPosition pPos )
        {
        	debug(String.Format("OnPositionChanged {0} {1}",pPos.Symbol, pPos.AggregatePosition));
        	string sym = pPos.Symbol;
        	int size = pPos.AggregatePosition;
        	//TODO: check accuracy of AveragePrice2. Currently known to not correctly include prices from further than 1 day back
        	//AveragePrice2 only available in release candidates so reverting to the calc below that
        	//decimal price = (decimal) pPos.AveragePrice2;
        	decimal price = (pPos.IntradayPosition + pPos.OvernightPosition != 0 )
        		? (decimal)(((pPos.IntradayPosition * pPos.IntradayPrice) + (pPos.OvernightPosition * pPos.OvernightPrice)) / (pPos.IntradayPrice + pPos.OvernightPosition))
        		: 0;
        	//TODO: make this pPos.RealizedPNL2 when it is available
        	decimal cpl = (decimal) pPos.RealizedPNL;
        	string account = pPos.Account.Account;
        	Position p = new PositionImpl(sym, price, size, cpl, account);
        	pt.NewPosition(p);
        }
        
        void m_OrderClient_OnPositionAdded(MbtPosition pPos)
        {
        	//debug(String.Format("adding pos: {0} {1} {2}", pPos.Account.Account, pPos.Symbol, pPos.AggregatePosition));
        	OnPositionChanged( pPos );
        }

        void m_OrderClient_OnPositionUpdated(MbtPosition pPos)
        {
        	//debug(String.Format("updating pos: {0} {1} {2}", pPos.Account.Account, pPos.Symbol, pPos.AggregatePosition));
        	OnPositionChanged( pPos );
        }

        void m_OrderClient_OnLogonSucceed()
        {
        	debug("order client logged in");
        	//debug(String.Format("default account:{0}",m_OrderClient.Accounts.DefaultAccount.Account));
        	m_OrderClient.Accounts.LoadAll();
        }
        
        /// <summary>
        /// Store submitted orders in a dictionary so we have access to info later on
        /// </summary>
        /// <param name="pOrd"></param>
        void m_OrderClient_OnSubmit(MbtOpenOrder pOrd)
        {
        	if (!orders.ContainsKey(pOrd.OrderNumber)) orders.Add(pOrd.OrderNumber, pOrd);
        }
        
        /// <summary>
        /// Clean up orders and tradelink <--> broker id mappings
        /// </summary>
        /// <param name="pOrd"></param>
        void m_OrderClient_OnRemove(MbtOpenOrder pOrd)
        {
//        	//order has been completely filled so do cleanup
//        	string bid = pOrd.OrderNumber;
//        	debug(String.Format("Cleaning up orders for {0}", bid));
//        	//clean up the orders list
//        	if( orders.ContainsKey(bid) ) orders.Remove(bid);
//        	else debug(String.Format("order {0} did not exist in orders", bid));
//        	//clean up mappings
//        	long tlid = 0;
//        	if( broker2tl.TryGetValue(bid, out tlid ) )
//        	{
//        		//got the tradelink id so clear the broker mapping
//        		debug(String.Format("Cleaning up broker2tl mapping {0} - {1}", bid, tlid));
//        		broker2tl.Remove(bid);
//        		//then clear the tradelink mapping
//        		debug(String.Format("cleaning up tl2broker mapping {0} - {1}", tlid, bid));
//        		if( tl2broker.ContainsKey(tlid ) ) tl2broker.Remove(tlid);
//        		else debug(String.Format("Order {0} did not have a matching tlid {1} to remove.", bid, tlid));
//        	}
        }
        
        /// <summary>
        /// Store executed orders in a dictionary so we have access to info later on
        /// </summary>
        /// <param name="pOrd"></param>
        void m_OrderClient_OnExecute(MbtOpenOrder pOrd)
        {
        	//add successfully executed orders to the orders dictionary
        	//done in the history now
        	//if (!orders.ContainsKey(pOrd.OrderNumber)) orders.Add(pOrd.OrderNumber, pOrd);
        }
        
        /// <summary>
        /// Shortcut to get an order via the order number
        /// </summary>
        /// <param name="orderNumber"></param>
        /// <returns></returns>
        MbtOpenOrder GetOrderByOrderNumber(string orderNumber)
        {
        	MbtOpenOrder pOrd;
        	orders.TryGetValue(orderNumber, out pOrd);
        	//return m_Orders.FindByOrderNumber(orderNumber);
        	return pOrd;
        }
        
        /// <summary>
        /// Get the order for a history object then convert that to a TradeLink order
        /// </summary>
        /// <param name="pHist"></param>
        /// <returns></returns>
        OrderImpl HistToTradeLinkOrder(MbtOrderHistory pHist)
        {
        	MbtOpenOrder pOrd = GetOrderByOrderNumber(pHist.OrderNumber);
        	return ToTradeLinkOrder(pOrd);
        }
        
        /// <summary>
        /// Get the order for a history object then convert that to a TradeLink fill
        /// </summary>
        /// <param name="pHist"></param>
        /// <returns></returns>
        TradeImpl HistToTradeLinkFill(MbtOrderHistory pHist)
        {
        	MbtOpenOrder pOrd = GetOrderByOrderNumber(pHist.OrderNumber);
        	TradeImpl fill = ToTradeLinkFill(pOrd, pHist);
//        	//clean it up if the order is completely filled
//        	if( pHist.Quantity == pHist.SharesFilled )
//        	{
//	        	//order has been completely filled so do cleanup
//	        	string bid = pOrd.OrderNumber;
//	        	debug(String.Format("Cleaning up orders for {0}", bid));
//	        	//clean up the orders list
//	        	if( orders.ContainsKey(bid) ) orders.Remove(bid);
//	        	else debug(String.Format("order {0} did not exist in orders", bid));
//	        	//clean up mappings
//	        	long tlid = 0;
//	        	if( broker2tl.TryGetValue(bid, out tlid ) )
//	        	{
//	        		//got the tradelink id so clear the broker mapping
//	        		debug(String.Format("Cleaning up broker2tl mapping {0} - {1}", bid, tlid));
//	        		broker2tl.Remove(bid);
//	        		//then clear the tradelink mapping
//	        		debug(String.Format("cleaning up tl2broker mapping {0} - {1}", tlid, bid));
//	        		if( tl2broker.ContainsKey(tlid ) ) tl2broker.Remove(tlid);
//	        		else debug(String.Format("Order {0} did not have a matching tlid {1} to remove.", bid, tlid));
//	        	}
//	 		  	//clear the order from the orders list
//	        	orders.Remove(pOrd.OrderNumber);
//        	}
        	return fill;
        }
        
        IdTracker _idt = new IdTracker();
        /// <summary>
        /// Convert an MbtOpenOrder to a TradeLink OrderImpl and check if the order token
        /// is mapped to an existing TradeLink order. If so, replace the token with the OrderNumber and
        /// update the mapping for tl2broker with the order number as well. If not generate a new order number
        /// </summary>
        /// <param name="pOrd"></param>
        /// <returns></returns>
        OrderImpl ToTradeLinkOrder(MbtOpenOrder pOrd)
        {
        	//get the token
        	string token = pOrd.Token;
        	//get the actual brokder ID number now
        	string bid = pOrd.OrderNumber, s = "";
        	//check for a matching tradelink order
        	long tlid = 0, v = 0;
        	//first try to get the tlid by token
        	if( !broker2tl.TryGetValue(token, out tlid) )
        	{
        		//ok now try to get it by broker OrderNumber if it has been processed once before.
        		if( !broker2tl.TryGetValue(bid, out tlid ) )
        		{
	        		//this order must be initiated by MB Trading so assign a new one
	        		tlid = _idt.AssignId;
	        		debug(String.Format("No matching TradeLink ID found for token {0}, generated {1}", token, tlid));
        		}
        		//save the mappings
        		if( !broker2tl.ContainsKey(bid) )
        		{
        			broker2tl.Add(bid, tlid);
        		}
        		else
        		{
        			broker2tl.TryGetValue(bid, out v);
        			debug(String.Format("WARNING! This OrderNumber {0} is already mapped to {1}. Overwriting it with {2}", bid, v, tlid));
        			broker2tl[bid] = tlid;
        		}
        		if( !tl2broker.ContainsKey(tlid) )
        		{
        			tl2broker.Add(tlid, bid);
        		}
        		else
        		{
        			tl2broker.TryGetValue(tlid, out s);
        			debug(String.Format("WARNING! This TradeLinkID {0} is already mapped to {1}. Overwriting it with {2}", tlid, s, bid));
        			tl2broker[tlid] = bid;
        		}        		
        	}
        	else
        	{
        		//we have a match so this order was generated by TradeLink
        		//overwrite the Token with the OrderNumber
        		debug(String.Format("Overwriting token {0} with OrderNumber {1} for TradeLink order ID {2}", token, bid, tlid));
        		tl2broker[tlid] = bid;
        		//remove the Token mapping
        		broker2tl.Remove(token);
        		//and map the OrderNumber to the TradeLink ID
        		if( !broker2tl.ContainsKey( bid ) ) broker2tl.Add(bid, tlid);
        	}
            OrderImpl o = new OrderImpl(pOrd.Symbol,pOrd.Quantity);
            o.id = tlid;
            o.side = ( pOrd.BuySell == MBConst.VALUE_BUY );
            o.price = (decimal)pOrd.Price;
            o.stopp = (decimal)pOrd.StopLimit;
            o.TIF = pOrd.TimeInForce == MBConst.VALUE_DAY ? "DAY" : "GTC";
            o.time = Util.DT2FT(pOrd.UTCDateTime);
            o.date = Util.ToTLDate(pOrd.UTCDateTime);
            o.trail = (decimal)pOrd.TrailingOffset;
        	return o;
        }
        
        TradeImpl ToTradeLinkFill(MbtOpenOrder pOrd, MbtOrderHistory pHist)
        {
        	TradeImpl f = new TradeImpl();
        	f.symbol = pOrd.Symbol;
        	f.Account = pOrd.Account.Account;
        	f.xprice = (pOrd.Price > 0) ? (decimal)pOrd.Price : (decimal)pOrd.StopLimit;
        	//f.xsize = pHist.Event == "Executed" ? pHist.Quantity : pHist.SharesFilled;
        	f.xsize = pHist.Quantity;
        	f.side = ( pOrd.BuySell == MBConst.VALUE_BUY );
        	f.xtime = Util.DT2FT(pOrd.UTCDateTime);
        	f.xdate = Util.ToTLDate(pOrd.UTCDateTime);
        	long tlid = 0;
        	if(broker2tl.TryGetValue(pOrd.OrderNumber, out tlid))
        		f.id = tlid;
        	else
        		debug(String.Format("WARNING: No order matching this fill {0}",pOrd.OrderNumber));
        	//debug(String.Format("New fill {1}\n is valid:{0}\ndump:{2}", f.isValid, f.ToString(), Util.DumpObjectProperties(f)));
        	debug(String.Format("New fill {1} is valid:{0}", f.isValid, f.ToString()));
        	return f;
        }
        
        void m_OrderClient_OnClose(int ErrorCode)
        {
        	debug(String.Format("order client closing:{0}",ErrorCode));
        }
        
        void m_OrderClient_OnConnect(int ErrorCode)
        {
        	debug(String.Format("order client connect:{0}",ErrorCode));
        	debug(String.Format("order client default account:{0}",m_OrderClient.Accounts.DefaultAccount.Account));
        	//m_OrderClient.Accounts.LoadAll();
        }
        
        void m_OrderClient_OnCancelPlaced(MbtOpenOrder pOrd)
        {
        	debug(String.Format("Order cancel placed for {0}", pOrd.OrderNumber));
        }
        
        void m_OrderClient_OnCancelRejected(MbtOpenOrder pOrd)
        {
        	debug(String.Format("Order Cancel REJECTED for {0}", pOrd.OrderNumber));
        }
        
        void m_OrderClient_OnHistoryAdded(MbtOrderHistory pHist)
        {
        	long tlid = 0;
        	double price = (pHist.Price != 0 ) ? pHist.Price : pHist.StopLimit;
        	broker2tl.TryGetValue(pHist.OrderNumber, out tlid);
        	debug(String.Format("OnHistoryAdded {0} {1} {2} {3} {4} {5}", pHist.Symbol, pHist.Event, pHist.OrderNumber, tlid, price, pHist.Quantity));
        	switch( pHist.Event )
        	{
        		case "Accepted":
        		case "Live":
        			if( !sentNewOrders.Contains(pHist.OrderNumber) )
        			{
			        	OrderImpl o = HistToTradeLinkOrder(pHist);
			            tl.newOrder(o);
			            sentNewOrders.Add(pHist.OrderNumber);
        			}
        			break;
        		case "Executed":
		        	//OrderImpl o = HistToTradeLinkOrder(pHist);
		            //tl.newOrder(o);
		            TradeImpl f = HistToTradeLinkFill(pHist);
		            pt.Adjust(f);
		            tl.newFill(f);
		            break;
        		case "Order Cancelled":
		            //TODO: have ot make sure order cancellations are happening correctly, especially on repeat requests from TradeLink
		            string bid = pHist.OrderNumber;
		            //long tlid = 0;
		            if( broker2tl.TryGetValue(bid, out tlid) )
		            {
		            	debug(String.Format("Order {0} cancelled, matched order {1} and sending newOrderCancel to TradeLink",bid,tlid));
		            	//remove the order mappings
		            	if( broker2tl.ContainsKey(bid) )
			            	broker2tl.Remove(bid);
		            	if( tl2broker.ContainsKey(tlid) )
		            		tl2broker.Remove(tlid);
		            	cancelids.Remove(bid);
		            	orders.Remove(bid);
		            	tl.newOrderCancel(tlid);
		            	canceledids.Add(tlid, true);
		            }
		            else
		            	debug(String.Format("Order {0} cancelled but no matching TradeLink order to cancel",bid));
        			break;
        		default:
        			break;
        	}
        }
        
        bool test() { return test(""); }
        bool test( Order o) { return test(o.Account); }
        bool test(string tlAcct)
        {
        	if (!m_ComMgr.IsConnected)
        	{
        		debug("message rejected, must login first.");
        		return false;
        	}
        	if (tlAcct != null && tlAcct != "")
        	{
	        	int num = m_OrderClient.Accounts.Count;
        		for (int i = 0; i < num; i++)
        		{
        			if( m_OrderClient.Accounts[i].Account != null && tlAcct == m_OrderClient.Accounts[i].Account && m_OrderClient.Accounts[i].State == enumAcctState.asLoaded )
        				return true;
        			else
        			{
        				debug(String.Format("message rejected, account {0} must be completely loaded first", tlAcct));
        				return false;
        			}
        		}
        	}
        	else
        	{
        		if (m_OrderClient.Accounts.DefaultAccount != null && m_OrderClient.Accounts.DefaultAccount.State == enumAcctState.asLoaded )
        			return true;
        		else
        		{
        			debug("message rejected, default account must be completely loaded first");
        			return false;
        		}
        	}
        	return true;
        }
        string tl_newAcctRequest()
        {
            test();
            int num = 0;
            if (m_OrderClient.Accounts != null )
            	num = m_OrderClient.Accounts.Count;
            string[] accts = new string[num];
            for (int i = 0; i < num; i++)
                accts[i] = m_OrderClient.Accounts[i].Account;
            //TODO:add this when verbose debugging is enabled
            //debug(String.Format("Accounts requested: {0}", string.Join(",", accts)));
            return string.Join(",", accts);
        }

        /// <summary>
        /// TradeLink sent a cancel so we need to map the cancel to an existing
        /// broker OrderNumber and cancel that
        /// </summary>
        /// <param name="pRec"></param>
        void tl_newOrderCancelRequest(long tlid)
        {
            test();
            string res = null;
            string bid = null;
            if( tl2broker.TryGetValue(tlid, out bid ) )
            {
            	if( !canceledids.ContainsKey(tlid) && !cancelids.ContainsKey(bid) )
            	{
            		//this order has never been canceled or had a cancel request placed on it
	            	m_OrderClient.Cancel(bid, ref res);
	            	debug(String.Format("Cancel request for order {1} received from TradeLink. msg: {0}", res, tlid));
	            	//mark it has having had a cancel request
	            	cancelids.Add(bid, true);
            	}
            	else if( !canceledids.ContainsKey(tlid) && cancelids.ContainsKey(bid) )
            	{
            		//this order has had a cancel request but has not completed
            		debug(String.Format("WARNING: Repeated cancel request for order {0} received from TradeLink. MBT Order is {1}. Checking if complete", tlid, bid));
			   		if( m_Orders.FindByOrderNumber(bid) == null )
            		{
			   			//this order has been cancelled successfully. Clean up.
			   			debug(String.Format("WARNING: Repeated cancel request for order that no longer exists: {0} received from TradeLink. MBT Order was {1}. Checking if complete", tlid, bid));
		            	if( broker2tl.ContainsKey(bid) )
			            	broker2tl.Remove(bid);
		            	if( tl2broker.ContainsKey(tlid) )
		            		tl2broker.Remove(tlid);
		            	if( orders.ContainsKey(bid) )
			            	orders.Remove(bid);
		            	//make sure tradelink got the message!
		            	tl.newOrderCancel(tlid);
		            	canceledids.Add(tlid,true);
            		}            		
            	}
            }
        	else if( canceledids.ContainsKey(tlid) )
        	{
        		//this order or has successfully cancelled
        		//m_OrderClient.Cancel(bid, ref res);
        		debug(String.Format("WARNING: Repeated cancel request for completely canceled order {0} received from TradeLink. No current MBT Order exists for it", tlid));
        		//make sure TradeLink really REALLY gets the message this time!
        		tl.newOrderCancel(tlid);
        	}
            else
            	debug(String.Format("No matching broker order found to cancel for {0}", tlid));
        }

        void tl_newRegisterStocks(string msg)
        {
            test();
            string [] syms = msg.Split(',');
            m_Quotes.UnadviseAll(this);
            for (int i = 0; i < syms.Length; i++)
            {
				if  (syms[i].Contains(".")) {
					//we can reasonably assume this is an options request
					m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfOptions);
				} else if (syms[i].Contains("/"))
				{
					//we know (or can at least reasonably assume) this is forex
					//advise only level1 bid-ask quotes
					m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelOne);
					//m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelTwo);
				} else {
					//probably equity, advise time and sales
					m_Quotes.AdviseSymbol(this, syms[i], ((int)enumQuoteServiceFlags.qsfTimeAndSales));
				}
			}
        }

        void MBTQUOTELib.IMbtQuotesNotify.OnOptionsData(ref OPTIONSRECORD pRec)
        {
            //not yet implemented
        }

        void MBTQUOTELib.IMbtQuotesNotify.OnTSData(ref TSRECORD pRec)
        {
            TickImpl k = new TickImpl();
            k.symbol = pRec.bstrSymbol;
            enumTickType tt = (enumTickType)pRec.lType;
            switch (tt)
            {
                case enumTickType.ttAskTick:
                    k.ask = (decimal)pRec.dPrice;
                    k.oe = pRec.bstrExchange;
                    k.os = pRec.lSize;
                    break;
                case enumTickType.ttBidTick:
                    k.bid = (decimal)pRec.dPrice;
                    k.be = pRec.bstrExchange;
                    k.bs= pRec.lSize;
                    break;
                case enumTickType.ttTradeTick:
                    k.trade = (decimal)pRec.dPrice;
                    k.ex = pRec.bstrExchange;
                    k.size= pRec.lSize;
                    break;
            }

            tl.newTick(k);
        }
        
        /// <summary>
        /// Provides quote data but each quote has both a bid and ask. As forex does not actually have a trade in the quotes,
        /// the Response must manage how it wants to handle the bid/ask, i.e. if you're long send....
        /// pt[symbol].isLong ? k.bid : k.ask.... and opposite if you're entering
        /// </summary>
        /// <param name="pQuote"></param>
        void MBTQUOTELib.IMbtQuotesNotify.OnQuoteData(ref QUOTERECORD pQuote)
        {
        	TickImpl k = new TickImpl(pQuote.bstrSymbol);
            k.time = Util.DT2FT(pQuote.UTCDateTime);
            k.date = Util.ToTLDate(pQuote.UTCDateTime);
            k.ask = (decimal)pQuote.dAsk;
            k.bid = (decimal)pQuote.dBid;
            k.os = k.AskSize = pQuote.lAskSize;
            k.bs = k.BidSize = pQuote.lBidSize;
            k.ex = k.be = k.oe = pQuote.bstrMarket;
            //k.trade = k.bid;
            tl.newTick(k);
        }
        
        /// <summary>
        /// Process Level2 data but this currently causes issues with creating valid orders and needs more investigation.
        /// Currently using Level1 data from OnQuoteData
        /// </summary>
        /// <param name="pRec"></param>
        void MBTQUOTELib.IMbtQuotesNotify.OnLevel2Data(ref LEVEL2RECORD pRec)
        {
            TickImpl k = new TickImpl(pRec.bstrSymbol);
            k.ex = pRec.bstrSource;
            k.time = Util.DT2FT(pRec.UTCTime);
            enumMarketSide ems = (enumMarketSide)pRec.side;
            switch (ems)
            {
                case enumMarketSide.msAsk:
                    k.ask = (decimal)pRec.dPrice;
                    k.oe = pRec.bstrSource;
                    k.os = k.AskSize = pRec.lSize;
                    k.trade = k.ask;
                    k.size = k.AskSize;
                    break;
                case enumMarketSide.msBid:
                    k.bid = (decimal)pRec.dPrice;
                    k.be = pRec.bstrSource;
                    k.bs = k.BidSize = pRec.lSize;
                    k.trade = k.bid;
                    k.size = k.BidSize;
                    break;
            }
            //k.trade = k.hasAsk ? k.ask : k.bid;
            tl.newTick(k);
        }


        /// <summary>
        /// Submit a new order and if successful and if the o.id != 0, map the incoming TradeLink o.ID to the order Token returned in res
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        long tl_newSendOrderRequest(Order o)
        {
            test(o);
            string strType = "MBConst.VALUE_MARKET";
            int side = o.side ? MBConst.VALUE_BUY : MBConst.VALUE_SELL;
            //int tif = MBConst.VALUE_DAY;
            int tif = MBConst.VALUE_GTC;
            //TODO: need to modify type depending on the type of order
            int otype = MBConst.VALUE_MARKET;
            //debug(String.Format("new order isLimit:{0} isStop:{1} isMarket:{2}", o.isLimit, o.isStop, o.isMarket));
            if (o.isMarket)
            {
            	otype = MBConst.VALUE_MARKET;
            	strType = "MBConst.VALUE_MARKET";
            }
            else if (o.isLimit && o.isStop)
            {
            	otype = MBConst.VALUE_STOP_LIMIT;
            	strType = "MBConst.VALUE_STOP_LIMIT";
            }
            else if (o.isLimit)
            {
            	otype = MBConst.VALUE_LIMIT;
            	strType = "MBConst.VALUE_LIMIT";
            }
            else if (o.isStop)
            {
            	otype = MBConst.VALUE_STOP_MARKET;
            	strType = "MBConst.VALUE_STOP_MARKET";
            }
            else if (o.isTrail)
            {
            	otype = MBConst.VALUE_TRAILING_STOP;
            	strType = "MBConst.VALUE_TRAILING_STOP";
            }
            else
            {
            	strType = "UNKNOWN";
            }
            debug(String.Format("TradeLink order is type: {0}:{1}", otype, strType));
            if( o.comment != null && o.comment != "" )
            	debug(String.Format("Order has comment:{0}", o.comment));
            string route = "MBTX";
            int voltype = MBConst.VALUE_NORMAL;
            DateTime dt = new DateTime(0);
            string res = null;
            string token = null;
            MbtAccount m_account = getaccount(o.Account);
            bool good = m_OrderClient.Submit(side, o.UnsignedSize, o.symbol, (double)o.price, (double)o.stopp, tif, 0, otype, voltype, 0, m_account, route, "", 0, 0, dt, dt, 0, 0, 0, 0, 0, ref res);
            if (!good)
            {
            	debug(String.Format("The following order failed: {0}\nreason:{1}",o, res));
            }
            else
            {
            	token = res;
            	debug(String.Format("Order sent to server and waiting success: {0}\ninfo: {1}", o, token));
            	// if the order ID is not 0, map it to the token
            	if( o.id != 0 )
            	{
            		//get the broker Token and save the association
            		if( !tl2broker.ContainsKey(o.id) )
            		{
            			debug(String.Format("Mapping TL:{0} to token:{1}", o.id, token));
            			tl2broker.Add(o.id, token);
            		}
            		else
            		{
            			//the id was there so upate the token with it
            			debug(String.Format("WARNING! Updating TL:{0} with token:{1}", o.id, token));
            			tl2broker[o.id] = token;
            		}
            		if( !broker2tl.ContainsKey( token ) )
            		{
            			debug(String.Format("Mapping token:{0} to TL:{1}", token, o.id));
            			broker2tl.Add(token, o.id);
            		}
            		else
            		{
            			debug(String.Format("Warning: Updating token:{0} with TL:{1}", token, o.id));
            			broker2tl[token] = o.id; // this actually shouldn't be called...!
            		}
            		debug(String.Format("Mapped TL order {0} to token {1}", o.id, token));
            	}
            	else
            	{
            		debug("WARNING! Incoming TL order does not have an id. It will be generated.");
            	}
            }
            return (long)MessageTypes.OK;
        }

        MbtAccount getaccount(string name) { foreach (MbtAccount a in m_OrderClient.Accounts) if (a.Account == name) return a; return m_OrderClient.Accounts.DefaultAccount; }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.BROKERNAME);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            return f.ToArray();
        }

        void debug(string msg)
        {
        	//TODO: add a check for verbose debugging
            _log.GotDebug(msg);
            _dw.GotDebug(msg);
        }

        private void _loginbut_Click(object sender, EventArgs e)
        {
            m_ComMgr.DoLogin((int)_id.Value, _user.Text, _pass.Text, "");
        }

        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }
    }

    public static class MBConst
    {
		public const int tickDown			= 1;
		public const int tickEvenUp			= 0;
		// Buy/Sell action values
		public const int VALUE_BUY = 10000;
		public const int VALUE_SELL = 10001;
		public const int VALUE_SELLSHT = 10002;

		// Time-in-force values
		public const int VALUE_GTC = 10008; 		// mandatory for Forex;
		public const int VALUE_DAYPLUS = 10009; 		// ARCA & INET after hours;
		public const int VALUE_IOC = 10010; 		// OTC;
		public const int VALUE_DAY = 10011;

		// Capacity values
		public const int VALUE_AGENCY = 10020;
		public const int VALUE_PRINCIPAL = 10021;

		// T&S record type
		public const int VALUE_TS_NORMAL = 30030; 		// inside market hours;
		public const int VALUE_TS_FORMT = 30031; 		// outside market hours;

		// OrderType values
		public const int VALUE_DISCRETIONARY = 10043;
		public const int VALUE_LIMIT = 10030;
		public const int VALUE_LIMIT_CLOSE = 10057;
		public const int VALUE_LIMIT_OPEN = 10056;
		public const int VALUE_LIMIT_STOPMKT = 10064;
		public const int VALUE_LIMIT_TRAIL = 10054;
		public const int VALUE_LIMIT_TTO = 10050;
		public const int VALUE_MARKET = 10031;
		public const int VALUE_MARKET_CLOSE = 10039;
		public const int VALUE_MARKET_OPEN = 10038;
		public const int VALUE_MARKET_STOP = 10069;
		public const int VALUE_MARKET_TRAIL = 10055;
		public const int VALUE_MARKET_TTO = 10051;
		public const int VALUE_PEGGED = 10062;
		public const int VALUE_RESERVE = 10040;
		public const int VALUE_RSV_DISC = 10044;
		public const int VALUE_RSV_PEGGED = 10066;
		public const int VALUE_RSV_TTO = 10052;
		public const int VALUE_STOPLMT_STOP = 10072;
		public const int VALUE_STOPLMT_TRAIL = 10068;
		public const int VALUE_STOPLMT_TTO = 10067;
		public const int VALUE_STOPMKT_LIMIT = 10076;
		public const int VALUE_STOP_LIMIT = 10033;
		public const int VALUE_STOP_MARKET = 10032;
		public const int VALUE_STOP_TRAIL = 10065;
		public const int VALUE_STOP_TTO = 10053;
		public const int VALUE_TRAILING_STOP = 10034;
		public const int VALUE_TTO_ORDER = 10037;
		public const int VALUE_VWAP = 10063;

		// Alert severities
		public const int ALERT_VAL_CRIT = 30001;
		public const int ALERT_VAL_NORMAL = 30000;
		public const int ALERT_VAL_REINSTATE = 30002;
		public const int ALERT_VAL_UPDATECXN = 30003;

		// Alert types
		public const int ALERT_TYPE_ACCT = 30010;
		public const int ALERT_TYPE_INTERNAL = 30014;
		public const int ALERT_TYPE_LOGOFF = 30013;
		public const int ALERT_TYPE_ORDSRV = 30012;
		public const int ALERT_TYPE_QUOTESRV = 30015;
		public const int ALERT_TYPE_USER = 30011;

		// Customer types
		public const int VALUE_RETAIL = 30021;
		public const int VALUE_DEMO = 30022;

		// Volume types
		public const int VALUE_NORMAL = 10042;
		public const int VALUE_PART = 10046;
    }
}
