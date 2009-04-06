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
using MBTCOMLib;
using MBTORDERSLib;
using MBTQUOTELib;


namespace ServerMB
{
    public partial class ServerMBMain : Form, IMbtQuotesNotify
    {
        TLServer_WM tl = new TLServer_WM();
        static MBTCOMLib.MbtComMgr m_ComMgr;
        static MBTORDERSLib.MbtOrderClient m_OrderClient;
        static MbtQuotes m_Quotes;
        PositionTracker pt = new PositionTracker();
        bool showmessage = false;
        public ServerMBMain()
        {
            InitializeComponent();
            _msg.SendToBack();
            m_ComMgr = new MBTCOMLib.MbtComMgrClass();
            m_ComMgr.SilentMode = true;
            m_ComMgr.EnableSplash(false);
            m_OrderClient = m_ComMgr.OrderClient;
            m_Quotes = m_ComMgr.Quotes;
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("Messages", new EventHandler(rightmessage));

            // tradelink bindings
            tl.newProviderName = Providers.MBTrading;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
            tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            tl.newOrderCancelRequest += new UIntDelegate(tl_newOrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);


            // mb bindings
            m_ComMgr.OnLogonSucceed += new IMbtComMgrEvents_OnLogonSucceedEventHandler(m_ComMgr_OnLogonSucceed);
            m_ComMgr.OnLogonDeny += new IMbtComMgrEvents_OnLogonDenyEventHandler(m_ComMgr_OnLogonDeny);
            m_OrderClient.OnSubmit += new _IMbtOrderClientEvents_OnSubmitEventHandler(m_OrderClient_OnSubmit);
            m_OrderClient.OnRemove += new _IMbtOrderClientEvents_OnRemoveEventHandler(m_OrderClient_OnRemove);
            m_OrderClient.OnPositionUpdated += new _IMbtOrderClientEvents_OnPositionUpdatedEventHandler(m_OrderClient_OnPositionUpdated);


            FormClosing += new FormClosingEventHandler(ServerMBMain_FormClosing);

        }

        void m_ComMgr_OnLogonDeny(string bstrReason)
        {
            debug("login denied: " + bstrReason);
        }

        void m_ComMgr_OnLogonSucceed()
        {
            debug("login successful");
            BackColor = Color.Green;
        }


        void rightmessage(object sender, EventArgs e)
        {
            showmessage = !showmessage;
            if (showmessage)
            {
                _msg.Visible = true;
                _msg.BringToFront();
                Invalidate(true);
            }
            else
            {
                _msg.Visible = false;
                _msg.SendToBack();
                Invalidate(true);
            }
        }



        void ServerMBMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            try
            {
                m_Quotes.Disconnect();
                m_OrderClient.Disconnect();
            }
            catch (Exception) { }
        }


        Position[] tl_newPosList(string account)
        {
            test();
            int num = m_OrderClient.Positions.Count;
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
            }
            return posl;
        }

        void m_OrderClient_OnPositionUpdated(MbtPosition pPos)
        {
            
        }



        void m_OrderClient_OnRemove(MbtOpenOrder pOrd)
        {
            uint num = Convert.ToUInt32(pOrd.OrderNumber);
            tl.newOrderCancel((long)num);
        }


        void m_OrderClient_OnSubmit(MbtOpenOrder pOrd)
        {
            OrderImpl o = new OrderImpl(pOrd.Symbol,pOrd.Quantity);
            o.side = pOrd.BuySell == MBConst.VALUE_BUY;
            o.price = (decimal)pOrd.Price;
            o.stopp = (decimal)pOrd.StopLimit;
            o.TIF = pOrd.TimeInForce == MBConst.VALUE_DAY ? "DAY" : "GTC";
            o.time = Util.ToTLTime(pOrd.UTCDateTime);
            o.date = Util.ToTLDate(pOrd.UTCDateTime);
            o.sec = pOrd.UTCDateTime.Second;
            o.trail = (decimal)pOrd.TrailingOffset;
            //o.ex = pOrd.Route;
            o.id = Convert.ToUInt32(pOrd.OrderNumber);
            tl.newOrder(o);


        }
        bool test() { if (m_ComMgr.IsConnected) return true; debug("message rejected, must login first."); return false; } 
        string tl_newAcctRequest()
        {
            test();
            int num = m_OrderClient.Accounts.Count;
            string[] accts = new string[num];
            for (int i = 0; i < num; i++)
                accts[i] = m_OrderClient.Accounts[i].Account;
            return string.Join(",", accts);
        }

        void tl_newOrderCancelRequest(uint number)
        {
            test();
            string res = null;
            m_OrderClient.Cancel(number.ToString(), ref res);
        }

        void tl_newRegisterStocks(string msg)
        {
            test();
            string [] syms = msg.Split(',');
            m_Quotes.UnadviseAll(this);
            for (int i = 0; i < syms.Length; i++)
                m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfTimeAndSales);
        }

        void MBTQUOTELib.IMbtQuotesNotify.OnOptionsData(ref OPTIONSRECORD pRec)
        {
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
        void MBTQUOTELib.IMbtQuotesNotify.OnQuoteData(ref QUOTERECORD pQuote)
        {
        }
        void MBTQUOTELib.IMbtQuotesNotify.OnLevel2Data(ref LEVEL2RECORD pRec)
        {
        }



        void tl_newSendOrderRequest(Order o)
        {
            test();
            int side = o.side ? MBConst.VALUE_BUY : MBConst.VALUE_SELL;
            int tif = MBConst.VALUE_DAY;
            if (o.TIF=="GTC") tif = MBConst.VALUE_GTC;
            int otype = MBConst.VALUE_MARKET;
            if (o.isLimit) otype = MBConst.VALUE_LIMIT;
            string route = "MBTX";
            int voltype = MBConst.VALUE_NORMAL;
            DateTime dt = new DateTime(0);
            string res = null;
            MbtAccount m_account = getaccount(o.Account);
            bool good = m_OrderClient.Submit(side, o.UnsignedSize, o.symbol, (double)o.price, (double)o.stopp, tif, 0, otype, voltype, 0, m_account, route, "", 0, 0, dt, dt, 0, 0, 0, 0, 0, ref res);
        }

        MbtAccount getaccount(string name) { foreach (MbtAccount a in m_OrderClient.Accounts) if (a.Account == name) return a; return m_OrderClient.Accounts.DefaultAccount; }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
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
            return f.ToArray();
        }

        void debug(string msg)
        {
            if (_msg.InvokeRequired)
                Invoke(new DebugDelegate(debug), new object[] { msg });
            else
            {
                _msg.Items.Add(DateTime.Now.ToShortTimeString() + " " + msg);
                _msg.SelectedIndex = _msg.Items.Count - 1;
            }
        }

        private void _loginbut_Click(object sender, EventArgs e)
        {
            m_ComMgr.DoLogin((int)_id.Value, _user.Text, _pass.Text, "");
        }
    }

    public static class MBConst
    {
        public const int tickEvenUp = 0;
        public const int tickDown = 1;
        public const int VALUE_BUY = 10000;
        public const int VALUE_SELL = 10001;
        public const int VALUE_LIMIT = 10030;
        public const int VALUE_MARKET = 10031;
        public const int VALUE_DAY = 10011;
        public const int VALUE_GTC = 10008;
        public const int VALUE_NORMAL = 10042;
    }
}
