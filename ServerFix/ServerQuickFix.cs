using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;
using QuickFix;
namespace ServerFix
{
    public class ServerQuickFix : QuickFix.Application
    {
        public void fromAdmin(QuickFix.Message __p1, SessionID __p2)
        {
        }
        public void fromApp(QuickFix.Message message, SessionID sessionID)
        {
            // receiving messages
            Symbol sym = new Symbol();
            message.getField(sym);
            Tick k = new TickImpl(sym.getValue());
			
			{
            // bid
            BidPx bp = new BidPx();
            BidSize bs = new BidSize();
            k.bid = (decimal)bp.getValue();
            k.bs = (int)message.getField(bs).getValue();
			}
			
			{
            // ask
            OfferPx op = new OfferPx();
            OfferSize os = new OfferSize();
            k.ask = (decimal)op.getValue();
            k.os = (int)message.getField(os).getValue();
			}
			
			{
            // last
            Price price = new Price();
            message.getField(price);
            k.trade = (decimal)price.getValue();
			}
			
            tl.newTick(k);
            //ClOrdID clOrdID = new ClOrdID();
            //message.getField(clOrdID);
        }
        public void toApp(QuickFix.Message __p1, SessionID __p2)
        {
            // sending messages
        }
        public void onCreate(SessionID __p1)
        {
            debug("session created" + __p1.getSenderCompID() + " " + __p1.getTargetCompID());
        }
        public void onLogon(SessionID __p1)
        {
            _val = true;
        }
        public void onLogout(SessionID __p1)
        {
            _val = false;
        }
        public void toAdmin(QuickFix.Message __p1, SessionID __p2)
        {
        }
        /*
        public override void onMessage(QuickFix42.NewOrderSingle message, QuickFix.SessionID sessionID)
        {
          ClOrdID clOrdID = new ClOrdID;
          message.get(clOrdID);
          ClearingAccount clearingAccount = new ClearingAccount();
          message.get(clearingAccount);
        }
                public override void onMessage(QuickFix42.OrderCancelRequest message, QuickFix.SessionID sessionID)
        {
          ClOrdID clOrdID = new ClOrdID;
          message.get(clOrdID);
          // compile time error!! field not defined for OrderCancelRequest
          ClearingAccount clearingAccount = new ClearingAccount();
          message.get(clearingAccount);
        }
         */
        bool _val = false;
        public bool isValid { get { return _val; } }
        string _fixid = "FIX.4.2";
        /// <summary>
        /// fix id
        /// </summary>
        public string FIXID { get { return _fixid; } set { _fixid = value; } }
        string _sender = string.Empty;
        public string SenderCompId { get { return _sender; } set { _sender = value; } }
        string _target = string.Empty;
        public string TargetCompId { get { return _target; } set { _target = value; } }
        string _setpath = string.Empty;
        public ServerQuickFix(TLServer tls, string settingpath)
        {
            tl = tls;
            _setpath = settingpath;
            tl.Start();
            tl.newOrderCancelRequest += new LongDelegate(ServerQuickFix_newOrderCancelRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(ServerQuickFix_newSendOrderRequest);
            tl.newFeatureRequest += new MessageArrayDelegate(ServerQuickFix_newFeatureRequest);
        }
        MessageTypes[] ServerQuickFix_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.TICKNOTIFY);
            return f.ToArray();
        }
        const string CANCEL = "D";
        long ServerQuickFix_newSendOrderRequest(Order o)
        {
            QuickFix.Message message = new QuickFix.Message();
            QuickFix.Message.Header header = message.getHeader();
            header.setField(new BeginString(FIXID));
            header.setField(new SenderCompID(SenderCompId));
            header.setField(new TargetCompID(TargetCompId));
            header.setField(new MsgType(CANCEL));
            message.setField(new OrigClOrdID(o.id.ToString()));
            message.setField(new ClOrdID(o.id.ToString()));
            message.setField(new Symbol(o.symbol));
            message.setField(new Side(o.side ? Side.BUY : Side.SELL));
            //message.setField(new Text("Cancel My Order!"));
            Session.sendToTarget(message);
            return 0;
            
        }
        void ServerQuickFix_newOrderCancelRequest(long val)
        {
            debug("order canceling not presently implemented");
        }
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        SocketAcceptor acceptor;
        public TLServer tl;
        public bool Start(string user, string pw)
        {
            try
            {
                SessionSettings settings = new SessionSettings(_setpath);
                Application application = this;
                FileStoreFactory storeFactory = new FileStoreFactory(settings);
                FileLogFactory logFactory = new FileLogFactory(settings);
                MessageFactory messageFactory = new DefaultMessageFactory();
                acceptor = new SocketAcceptor
                  (application, storeFactory, settings, logFactory /*optional*/, messageFactory);
                acceptor.start();
                _val = true;
                return true;
            }
            catch (Exception ex)
            {
                debug("exception starting server: " + ex.Message + ex.StackTrace);
                _val = false;
            }
            return false;
        }
        public void Stop()
        {
            try
            {
                _val = false;
                acceptor.stop();
            }
            catch { }
        }
    }
}
