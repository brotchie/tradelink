using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// tradelink servers allow tradelink clients to talk to any supported broker with common interface.
    /// this version of server supports communication with clients via windows messaging.
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class TLServer_WM : Form, TLServer
    {
        public Basket AllClientBasket
        {
            get
            {

                Basket b = new BasketImpl();
                for (int i = 0; i < stocks.Count; i++)
                    b.Add(BasketImpl.FromString(stocks[i]));
                return b;
            }
        }
        public bool SymbolSubscribed(string sym)
        {
            for (int i = 0; i < client.Count; i++)
            {
                if (client[i] == string.Empty) continue;
                else if (stocks[i].Contains(sym))
                        return true;
            }
            return false;
        }
        public string ClientName(int num) { return client[num]; }

        public string ClientSymbols(string client)
        {
            int cid = client.IndexOf(client);
            if (cid < 0) return string.Empty;
            return stocks[cid];
        }

        Providers _pn = Providers.Unknown;
        public Providers newProviderName { get { return _pn; } set { _pn = value; } }
        public event StringDelegate newAcctRequest;
        public event OrderDelegateStatus newSendOrderRequest;
        public event LongDelegate newOrderCancelRequest;
        public event PositionArrayDelegate newPosList;
        public event SymbolRegisterDel newRegisterSymbols;
        public event MessageArrayDelegate newFeatureRequest;
        public event UnknownMessageDelegate newUnknownRequest;
        public event UnknownMessageDelegateSource newUnknownRequestSource;
        public event VoidDelegate newImbalanceRequest;

        bool _noverb = true;
        /// <summary>
        /// toggle higher level of debugging
        /// </summary>
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        public string Version() { return Util.TLSIdentity(); }
        protected int MinorVer = 0;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }


        public TLServer_WM() : base()
        {
            MinorVer = Util.BuildFromRegistry(Util.PROGRAM);
            
            this.Text = WMUtil.GetUniqueWindow(WMUtil.SERVERWINDOW);
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.ShowInTaskbar = false;
            this.Hide();
            //WMUtil.SendMsg(Text, (IntPtr)WMUtil.HWND_BROADCAST, Handle, (int)MessageTypes.SERVERUP);
        }

        public void Start()
        {

        }
        public virtual void Stop()
        {
            debug("stopping server.");
            //WMUtil.SendMsg(Text, (IntPtr)WMUtil.HWND_BROADCAST, Handle, (int)MessageTypes.SERVERDOWN);
        }

        private void SrvDoExecute(string msg) // handle an order (= execute request)
        {
            if (this.InvokeRequired)
                this.Invoke(new DebugDelegate(SrvDoExecute), new object[] { msg });
            else
            {
                try
                {
                    Order o = OrderImpl.Deserialize(msg);
                    if (newSendOrderRequest != null) 
                        newSendOrderRequest(o); //request fill

                }
                catch (Exception ex)
                {
                    debug("Error unpacking order: " + msg + " " + ex.Message + ex.StackTrace);
                }
            }
        }


        delegate void tlneworderdelegate(OrderImpl o, bool allclients);
        public void newOrder(Order o) { newOrder(o, true); }
        public void newOrder(Order o, bool allclients)
        {
            if (this.InvokeRequired)
                this.Invoke(new tlneworderdelegate(newOrder), new object[] { o,allclients });
            else
            {
                for (int i = 0; i < client.Count; i++)
                    if ((client[i] != null) && (client[i] != "") && (stocks[i].Contains(o.symbol) || allclients))
                        WMUtil.SendMsg(OrderImpl.Serialize(o), MessageTypes.ORDERNOTIFY, Handle, client[i]);
            }
        }

        // server to clients
        /// <summary>
        /// Notifies subscribed clients of a new tick.
        /// </summary>
        /// <param name="tick">The tick to include in the notification.</param>
        public void newTick(Tick tick)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new TickDelegate(newTick), new object[] { tick });
                }
                catch (Exception) { }
            }
            else
            {
                if (!tick.isValid) return; // need a valid tick
                for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                    if ((client[i] != null) && (stocks[i].Contains(tick.symbol)))
                        WMUtil.SendMsg(TickImpl.Serialize(tick), hims[i],Handle, (int)MessageTypes.TICKNOTIFY );
            }
        }

        public void TLSend(string msg, MessageTypes type, int clientid)
        {
            TLSend(msg, type, hims[clientid]);

        }
        public void TLSend(string message, MessageTypes type, string clientname)
        {
            int id = client.IndexOf(clientname);
            if (id == -1) return;
            TLSend(message,type,hims[id]);
        }
        delegate void tlsenddel(string m, MessageTypes t, IntPtr dest);
        public void TLSend(string message, MessageTypes type, IntPtr dest)
        {
            if (InvokeRequired)
                Invoke(new tlsenddel(TLSend), new object[] { message, type,dest });
            else
            {
                WMUtil.SendMsg(message, dest, Handle, (int)type);
            }
        }

        delegate void tlnewfilldelegate(TradeImpl t, bool allclients);
        /// <summary>
        /// Notifies subscribed clients of a new execution.
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        public void newFill(Trade trade) { newFill(trade, true); }
        public void newFill(Trade trade, bool allclients)
        {
            if (this.InvokeRequired)
                this.Invoke(new tlnewfilldelegate(newFill), new object[] { trade,allclients });
            else
            {
                // make sure our trade is filled and initialized properly
                if (!trade.isValid) return;
                for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                    if ((client[i] != null) && (allclients || (stocks[i].Contains(trade.symbol))))
                        WMUtil.SendMsg(TradeImpl.Serialize(trade), MessageTypes.EXECUTENOTIFY, Handle, client[i]);
            }
        }

        public int NumClients { get { return client.Count; } }

        // server structures
        protected List<string> client = new List<string>();
        protected List<DateTime> heart = new List<DateTime>();
        protected List<string> stocks = new List<string>();
        protected List<string> index = new List<string>();
        protected List<IntPtr> hims = new List<IntPtr>();

        private string SrvStocks(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return ""; // client not registered
            return stocks[cid];
        }
        private string[] SrvGetClients() { return client.ToArray(); }
        void SrvRegIndex(string cname, string idxlist)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            else index[cid] = idxlist;
            SrvBeatHeart(cname);
        }

        void SrvRegClient(string cname)
        {
            if (client.IndexOf(cname) != -1) return; // already registered
            client.Add(cname);
            heart.Add(DateTime.Now);
            stocks.Add("");
            index.Add("");
            hims.Add(WMUtil.HisHandle(cname));
            SrvBeatHeart(cname);
            debug("registered: " + cname);
        }
        
        void SrvRegStocks(string cname, string stklist)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            stocks[cid] = stklist;
            SrvBeatHeart(cname);
            if (newRegisterSymbols != null) newRegisterSymbols(cname, stklist);
            debug(cname + " registered symbols: " + stklist);
        }

        void SrvClearStocks(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            stocks[cid] = "";
            SrvBeatHeart(cname);
            debug(cname + " cleared symbols.");
        }
        void SrvClearIdx(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            index[cid] = "";
            SrvBeatHeart(cname);
        }


        void SrvClearClient(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return; // don't have this client to clear him
            client.RemoveAt(cid);
            stocks.RemoveAt(cid);
            heart.RemoveAt(cid);
            index.RemoveAt(cid);
            hims.RemoveAt(cid);
            debug(him + " disconnected.");
        }

        void SrvBeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return; // this client isn't registered, ignore
            TimeSpan since = DateTime.Now.Subtract(heart[cid]);
            heart[cid] = DateTime.Now;
            debug(cname + " heartbeat.");
        }



        public void newCancel(long id)
        {
            newOrderCancel(id);
        }

        public void newOrderCancel(long orderid_being_cancled)
        {
                foreach (string c in client) // send order cancel notifcation to clients
                    TLSend(orderid_being_cancled.ToString(), MessageTypes.ORDERCANCELRESPONSE, c);
        }

        public void newImbalance(Imbalance imb)
        {
                for (int i = 0; i < client.Count; i++)
                    TLSend(ImbalanceImpl.Serialize(imb), MessageTypes.IMBALANCERESPONSE, hims[i]);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            TradeLinkMessage tlm = WMUtil.ToTradeLinkMessage(ref m);
            if (tlm == null)
            {
                base.WndProc(ref m);
                return;
            }
            string msg = tlm.body;

            long result = (long)MessageTypes.OK;
            switch (tlm.type)
            {
                case MessageTypes.ACCOUNTREQUEST:
                    if (newAcctRequest == null) break;
                    string accts = newAcctRequest();
                    TLSend(accts, MessageTypes.ACCOUNTRESPONSE, msg);
                    break;
                case MessageTypes.POSITIONREQUEST:
                    if (newPosList == null) break;
                    string [] pm = msg.Split('+');
                    if (pm.Length<2) break;
                    string client = pm[0];
                    string acct = pm[1];
                    Position[] list = newPosList(acct);
                    foreach (Position pos in list)
                        TLSend(PositionImpl.Serialize(pos), MessageTypes.POSITIONRESPONSE, client);
                    break;
                case MessageTypes.ORDERCANCELREQUEST:
                    {
                        long id = 0;
                        if (long.TryParse(msg,out id) && (newOrderCancelRequest != null))
                            newOrderCancelRequest(id);
                    }
                    break;
                case MessageTypes.SENDORDER:
                    SrvDoExecute(msg);
                    break;
                case MessageTypes.REGISTERCLIENT:
                    SrvRegClient(msg);
                    break;
                case MessageTypes.REGISTERSTOCK:
                    string[] m2 = msg.Split('+');
                    SrvRegStocks(m2[0], m2[1]);
                    break;
                case MessageTypes.CLEARCLIENT:
                    SrvClearClient(msg);
                    break;
                case MessageTypes.CLEARSTOCKS:
                    SrvClearStocks(msg);
                    break;
                case MessageTypes.HEARTBEATREQUEST:
                    SrvBeatHeart(msg);
                    break;
                case MessageTypes.BROKERNAME :
                    result = (long)newProviderName;
                    break;
                case MessageTypes.IMBALANCEREQUEST:
                    if (newImbalanceRequest != null) newImbalanceRequest();
                    break;
                case MessageTypes.FEATUREREQUEST:
                    string msf = "";
                    List<MessageTypes> f = new List<MessageTypes>();
                    f.Add(MessageTypes.HEARTBEATREQUEST);
                    f.Add(MessageTypes.CLEARCLIENT);
                    f.Add(MessageTypes.CLEARSTOCKS);
                    f.Add(MessageTypes.REGISTERCLIENT);
                    f.Add(MessageTypes.REGISTERSTOCK);
                    f.Add(MessageTypes.FEATUREREQUEST);
                    f.Add(MessageTypes.FEATURERESPONSE);
                    f.Add(MessageTypes.VERSION);
                    f.Add(MessageTypes.BROKERNAME);
                    List<string> mf = new List<string>();
                    foreach (MessageTypes t in f)
                    {
                        int ti = (int)t;
                        mf.Add(ti.ToString());
                    }
                    if (newFeatureRequest!=null)
                    {
                        MessageTypes[] f2 = newFeatureRequest();
                        foreach (MessageTypes t in f2)
                        {
                            int ti = (int)t;
                            mf.Add(ti.ToString());
                        }
                    }
                    msf = string.Join(",", mf.ToArray());
                    TLSend(msf,MessageTypes.FEATURERESPONSE,msg);
                    break;
                case MessageTypes.VERSION :
                    result = (long)MinorVer;
                    break;
                default:
                    if (newUnknownRequest != null)
                        result = newUnknownRequest(tlm.type,msg);
                    else
                        result = (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
                    break;
            }
            m.Result = (IntPtr)result;
        }





    }
}
