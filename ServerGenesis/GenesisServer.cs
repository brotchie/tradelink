using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using GTAPINet;

namespace ServerGenesis
{
    public class GenesisServer : TLServer_WM
    {
        // genesis objects
        List<GTStock> _stk = new List<GTStock>();
        GTSession m_session;


        // tradelink objects
        bool _valid = false;
        string[] _sym = new string[0];

        public event DebugFullDelegate SendDebug;

        public bool isValid { get { return _valid; } }

        public GenesisServer()
            : base()
        {
            // tradelink events
            newAcctRequest += new StringDelegate(tl_newAcctRequest);
            newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            newOrderCancelRequest += new UIntDelegate(tl_newOrderCancelRequest);
            newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
            newProviderName = Providers.Genesis;
            newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            // set provider
            newProviderName = Providers.Genesis;


            // genesis init
            m_session = new GTSession();

            Load += new EventHandler(GenesisServer_Load);





        }

        void GenesisServer_Load(object sender, EventArgs e)
        {
            if (GTSession.gtInitialize(GTSession.GTAPI_VERSION, IntPtr.Zero) != 0)
                return;
            m_session.CreateSessionWindow(this.Handle, 0);
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            switch (t)
            {
                default:
                    break;
            }
            return (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
        }

        void tl_newSendOrderRequest(Order o)
        {
            GTSession.GTOrder32 order = new GTSession.GTOrder32();
            order.szStock = o.symbol;
            //order.place = o.ex;
            order.szAccountID = o.Account;
            order.chSide = o.side ? 'B' : 'S';
            order.dblPrice = (double)o.price;
            order.dblStopLimitPrice = (double)o.stopp;
            order.dwShare = o.UnsignedSize;
            // this should be order id
            order.dwUserData = o.id == 0 ? OrderImpl.Unique : o.id;
            // send order
            GTStock stock = m_session.GetStock(o.symbol);
            int err = stock.PlaceOrder(order);
            if (err != 0)
            {
                debug("error " + err.ToString() + " sending: " + o.ToString());
            }
        }

        void tl_newRegisterStocks(string msg)
        {
            // make sure logged in
            if (!isValid) return;
            // get symbols
            string[] sym = msg.Split(',');
            // look for new symbols
            foreach (string s in sym)
                if (!havestock(s)) // if it's new
                {
                    // get a subscription
                    GTStock m_stock1 = m_session.CreateStock(s);
                    // hook events
                    m_stock1.HookExecMsgOpenPosition(new GTStock.PFNOnExecMsgOpenPosition(OnStockExecMsgOpenPosition));
                    m_stock1.HookExecMsgTrade(new GTStock.PFNOnExecMsgTrade(OnStockExecMsgTrade));
                    m_stock1.HookExecMsgPending(new GTStock.PFNOnExecMsgPending(OnStockExecMsgPending));
                    m_stock1.HookExecMsgSending(new GTStock.PFNOnExecMsgSending(OnStockExecMsgSending));
                    m_stock1.HookExecMsgCancel(new GTStock.PFNOnExecMsgCancel(OnStockExecMsgCancel));
                    m_stock1.HookExecMsgReject(new GTStock.PFNOnExecMsgReject(OnStockExecMsgReject));
                    m_stock1.HookExecMsgRejectCancel(new GTStock.PFNOnExecMsgRejectCancel(OnStockExecMsgRejectCancel));
                    m_stock1.HookSendingOrder(new GTStock.PFNOnSendingOrder(OnStockSendingOrder));
                    m_stock1.HookGotQuotePrint(new GTStock.PFNOnGotQuotePrint(OnStockExecMsgPrint));
                    m_stock1.HookGotQuoteLevel2(new GTStock.PFNOnGotQuoteLevel2(OnStockGotQuoteLevel2));
                    m_stock1.HookOnBestAskPriceChanged(new GTStock.PFNOnBestAskPriceChanged(OnStockGotBestAsk));
                    m_stock1.HookOnBestBidPriceChanged(new GTStock.PFNOnBestBidPriceChanged(OnStockGotBestBid));
                    // save subscription
                    _stk.Add(m_stock1);
                }
        }

        bool havestock(string sym) { foreach (string s in _sym) if (s == sym) return true; return false; }

        void tl_newOrderCancelRequest(uint number)
        {
            // make sure logged in
            if (!isValid) return;
            m_session.CancelTicket((int)number);
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.VERSION);
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            return f.ToArray();
        }


        string tl_newAcctRequest()
        {
            return m_session.GetAccount32().szAccountName;
        }

        void debug(string msg)
        {
            if (SendDebug != null)
            {
                SendDebug(DebugImpl.Create(msg));
            }
        }


        public void Start(string user, string password, string data1, int data2)
        {
            if ((user == null) || (user == string.Empty)) return;
            try
            {
                /*
                // setting up session callbacks
                m_session.HookExecMsgChat(new GTSession.PFNOnExecMsgChat(OnSessionExecMsgChat));
                m_session.HookExecMsgErrMsg(new GTSession.PFNOnExecMsgErrMsg(OnSessionExecMsgErrMsg));
                m_session.HookExecConnected(new GTSession.PFNOnExecConnected(OnExecConnected));
                m_session.HookExecDisconnected(new GTSession.PFNOnExecDisconnected(OnExecDisconnected));
                m_session.HookExecMsgState(new GTSession.PFNOnExecMsgState(OnSessionExecMsgState));
                m_session.HookQuoteConnected(new GTSession.PFNOnQuoteConnected(OnQuoteConnected));
                m_session.HookQuoteDisconnected(new GTSession.PFNOnQuoteDisconnected(OnQuoteDisconnected));
                m_session.HookExecMsgLoggedin(new GTSession.PFNOnExecMsgLoggedin(OnExecMsgLoggedin));
                m_session.HookGotLevel2Text(new GTSession.PFNOnGotLevel2Text(OnGotLevel2Text));
                m_session.HookGotQuoteText(new GTSession.PFNOnGotQuoteText(OnGotQuoteText));
                m_session.HookLevel2Connected(new GTSession.PFNOnLevel2Connected(OnLevel2Connected));
                m_session.HookLevel2Disconnected(new GTSession.PFNOnLevel2Disconnected(OnLevel2Disconnected));
                 

                // login
                GTSession.GTSession32 session32 = m_session.GetSession32();
                GTSession.GTime32 time32 = m_session.GetTime32();
                GTSession.GTSetting32 setting32 = m_session.GetSetting32();
                 **/

                string xip = Properties.Settings.Default.executeip;
                ushort xpt = Properties.Settings.Default.executeport;
                string qip = Properties.Settings.Default.quoteip;
                ushort qpt = Properties.Settings.Default.quoteport;
                string dip = Properties.Settings.Default.l2ip;
                ushort dpt = Properties.Settings.Default.l2port;
                GTSession.gtSetExecAddress(m_session, xip, xpt);
                GTSession.gtSetQuoteAddress(m_session, qip, qpt);
                GTSession.gtSetLevel2Address(m_session, dip, dpt);

                int err = GTSession.gtLogin(m_session, user, password);
                if (err == 0)
                {
                    debug("login succeeded.");
                    _valid = true;
                }
                else
                {
                    _valid = false;
                    debug("login failed.");
                }

            }
            catch (Exception ex)
            {
                debug(ex.Message);
                debug(ex.StackTrace);
                debug("login failed.");
            }
            
        }

        protected virtual int OnExecConnected(UInt32 hSession)
        {

            return 0;
        }
        protected virtual int OnExecDisconnected(UInt32 hSession)
        {

            return 0;
        }
        protected virtual int OnQuoteConnected(UInt32 hSession)
        {

            return 0;
        }
        protected virtual int OnQuoteDisconnected(UInt32 hSession)
        {
            return 0;
        }

        protected virtual int OnLevel2Connected(UInt32 hSession)
        {

            return 0;
        }

        protected virtual int OnLevel2Disconnected(UInt32 hSession)
        {

            return 0;
        }

        protected virtual int OnExecMsgLoggedin(UInt32 hSession)
        {
            debug("logged in");
            return 0;
        }

        protected virtual int OnSessionExecMsgChat(UInt32 hSession, GTSession.GTChat32 chat)
        {

            return 0;
        }

        protected virtual int OnSessionExecMsgState(UInt32 hSession, GTSession.GTServerState32 state)
        {
            //message.Items.Insert(0, state.szServer + ": " + state.nConnect);
            return 0;
        }

        protected virtual int OnSessionExecMsgErrMsg(UInt32 hSession, GTSession.GTErrMsg32 errmsg)
        {
            string strMsg = GTSession.GetErrorMessage(errmsg);

            return 0;
        }
        protected virtual int OnGotLevel2Text(UInt32 hSession, GTSession.GTQuoteText32 text)
        {

            return 0;
        }
        protected virtual int OnGotQuoteText(UInt32 hSession, GTSession.GTQuoteText32 text)
        {

            return 0;
        }
        protected virtual int OnStockExecMsgOpenPosition(UInt32 hStock, GTSession.GTOpenPosition32 openposition)
        {
            return 0;
        }
        protected virtual int OnStockExecMsgTrade(UInt32 hStock, GTSession.GTTrade32 trade)
        {
            // map a trade object
            Trade t = new TradeImpl(trade.szStock, (decimal)trade.dblExecPrice, trade.nExecShares);
            // map remaining fields
            t.Account = trade.szAccountID;
            t.id = (uint)trade.dwTicketNo;
            t.side = trade.chExecSide == 'B';
            t.xdate = trade.nExecDate;
            t.xtime = trade.nExecTime;
            // notify clients
            newFill(t);

            return 0;
        }
        protected virtual int OnStockExecMsgPending(UInt32 hStock, GTSession.GTPending32 pending)
        {


            return 0;
        }
        protected virtual int OnStockExecMsgSending(UInt32 hStock, GTSession.GTSending32 pending)
        {


            return 0;
        }
        protected virtual int OnStockExecMsgPrint(UInt32 hStock, GTSession.GTPrint32 print)
        {




            return 0;
        }
        protected virtual int OnStockExecMsgCancel(UInt32 hStock, GTSession.GTCancel32 cancel)
        {
            // notify clients of cancel
            newOrderCancel(cancel.dwTicketNo);
            return 0;
        }

        protected virtual int OnStockExecMsgReject(UInt32 hStock, GTSession.GTReject32 reject)
        {

            string str = ((reject.chPendSide == 'B') ? "Bid" : (reject.chPendSide == 'S') ? "Ask" : "Short sell")
                + " " + reject.szStock + " " + reject.nRejectShares + " @" + reject.dblPendPrice + "  Rejected : " + reject.szRejectReason;

            debug(str);

            return 0;
        }

        protected virtual int OnStockExecMsgRejectCancel(UInt32 hStock, GTSession.GTRejectCancel32 reject)
        {
            string str = ((reject.chPendSide == 'B') ? "Bid" : (reject.chPendSide == 'S') ? "Ask" : "Short sold")
                + " " + reject.szStock + " @" + reject.dblPendPrice + "  Rejected : " + reject.szRejectReason;

            debug(str);



            return 0;
        }

        protected virtual int OnStockSendingOrder(UInt32 hStock, GTSession.GTSending32 pending)
        {
            // get order
            Order o = new OrderImpl(pending.szStock, pending.nEntryShares);
            o.id = (uint)pending.dwTicketNo;
            o.side = pending.chEntrySide == 'B';
            //o.TIF = pending.nEntryTIF;
            o.Account = pending.szAccountID;
            o.date = pending.nEntryDate;
            o.time = pending.nEntryTime;
            // is this limit v stop?
            char type = pending.chPriceIndicator;
            // not sure about these two
            o.price = (decimal)pending.dblEntryPrice;
            o.stopp = (decimal)pending.dblEntryStopLimitPrice;
            // notify clients
            newOrder(o);
            return 0;
        }

        protected virtual int OnStockGotQuoteLevel2(UInt32 hStock, GTSession.GTLevel232 level232)
        {
            return 0;

            GTStock stock = new GTStock(hStock);

            if (stock.GetAskLevel2Count() > 0 && stock.GetBidLevel2Count() > 0)
                if (stock.GetAskLevel2Item(0).dblPrice < stock.GetBidLevel2Item(0).dblPrice)
                {
                    //string str = "L1:" + level132.szStock + " " + level132.dblBidPrice + " " + level132.dblAskPrice;
                    string str = "BID: " + stock.GetBidLevel2Item(0).dblPrice + " " + "ASK: " + stock.GetAskLevel2Item(0).dblPrice;
                    //listMsg.Items.Insert(0, str);
                }
            return 0;
        }

        protected virtual int OnStockGotBestBid(UInt32 hStock)
        {
            return 0;
        }

        protected virtual int OnStockGotBestAsk(UInt32 hStock)
        {
            return 0;
        }


        public void Stop()
        {
            // release objects and subscriptions as needed

            try
            {
                GTSession.gtLogout(m_session);
                m_session.DestroySessionWindow();
            }
            catch (Exception ex) { debug(ex.Message + ex.StackTrace); }
        }
    }
}
