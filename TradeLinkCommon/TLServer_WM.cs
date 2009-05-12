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
    public class TLServer_WM : Form, TradeLinkServer
    {
        public event StringDelegate newAcctRequest;
        public event DecimalStringDelegate newPositionPriceRequest;
        public event IntStringDelegate newPositionSizeRequest;
        public event DecimalStringDelegate newDayHighRequest;
        public event DecimalStringDelegate newDayLowRequest;
        public event OrderDelegate newSendOrderRequest;
        public event UIntDelegate newOrderCancelRequest;
        public event PositionArrayDelegate newPosList;
        public event DebugDelegate newRegisterStocks;
        public event MessageArrayDelegate newFeatureRequest;
        public event UnknownMessageDelegate newUnknownRequest;
        public event VoidDelegate newImbalanceRequest;
        public event IntDelegate DOMRequest;

        public string Version() { return Util.TLSIdentity(); }
        protected int MinorVer = 0;

        public TLServer_WM() : this(TLTypes.HISTORICALBROKER) { }
        public TLServer_WM(string servername) : base()
        {
            MinorVer = Util.BuildFromFile(Util.TLProgramDir + @"\VERSION.txt");
            this.Text = servername;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.ShowInTaskbar = false;
            this.Hide();
        }
        public TLServer_WM(TLTypes servertype) : 
            this(servertype==TLTypes.LIVEBROKER? WMUtil.LIVEWINDOW :
                (servertype == TLTypes.SIMBROKER ? WMUtil.SIMWINDOW : (servertype== TLTypes.HISTORICALBROKER? WMUtil.REPLAYWINDOW : WMUtil.TESTWINDOW))) { }

        private void SrvDoExecute(string msg) // handle an order (= execute request)
        {
            if (this.InvokeRequired)
                this.Invoke(new DebugDelegate(SrvDoExecute), new object[] { msg });
            else
            {
                Order o = OrderImpl.Deserialize(msg);
                if (newSendOrderRequest != null) newSendOrderRequest(o); //request fill
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
                        WMUtil.SendMsg(TickImpl.Serialize(tick), MessageTypes.TICKNOTIFY, Handle, client[i]);
            }
        }

        public void TLSend(string message, MessageTypes type, string client)
        {
            if (client == "") return;
            WMUtil.SendMsg(message, type, Handle, client);
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
        private List<string> client = new List<string>();
        private List<DateTime> heart = new List<DateTime>();
        private List<string> stocks = new List<string>();
        private List<string> index = new List<string>();

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
            SrvBeatHeart(cname);
        }
        
        void SrvRegStocks(string cname, string stklist)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            stocks[cid] = stklist;
            SrvBeatHeart(cname);
            if (newRegisterStocks != null) newRegisterStocks(stklist);
        }

        void SrvClearStocks(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            stocks[cid] = "";
            SrvBeatHeart(cname);
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
        }

        void SrvBeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return; // this client isn't registered, ignore
            TimeSpan since = DateTime.Now.Subtract(heart[cid]);
            heart[cid] = DateTime.Now;
        }

        void SrvDOMReq(string cname, int depth)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return;
            SrvBeatHeart(cname);
            DOMRequest(depth);
        }

        public void newOrderCancel(long orderid_being_cancled)
        {
            foreach (string c in client) // send order cancel notifcation to clients
                WMUtil.SendMsg(orderid_being_cancled.ToString(), MessageTypes.ORDERCANCELRESPONSE, Handle, c);
        }

        public void newImbalance(Imbalance imb)
        {
            for (int i = 0; i < client.Count; i++)
                WMUtil.SendMsg(ImbalanceImpl.Serialize(imb), MessageTypes.IMBALANCERESPONSE, Handle, client[i]);
        }

        public Providers newProviderName = Providers.TradeLink;

        protected override void WndProc(ref Message m)
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
                    WMUtil.SendMsg(newAcctRequest(), MessageTypes.ACCOUNTRESPONSE, Handle, msg);
                    break;
                case MessageTypes.POSITIONREQUEST:
                    if (newPosList == null) break;
                    string [] pm = msg.Split('+');
                    if (pm.Length<2) break;
                    string client = pm[0];
                    string acct = pm[1];
                    Position[] list = newPosList(acct);
                    foreach (Position pos in list)
                        WMUtil.SendMsg(PositionImpl.Serialize(pos), MessageTypes.POSITIONRESPONSE, Handle, client);
                    break;
                case MessageTypes.ORDERCANCELREQUEST:
                    if (newOrderCancelRequest != null)
                        newOrderCancelRequest(Convert.ToUInt32(msg));
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
                case MessageTypes.HEARTBEAT:
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
                    f.Add(MessageTypes.HEARTBEAT);
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
                    WMUtil.SendMsg(msf, MessageTypes.FEATURERESPONSE, Handle, msg);
                    break;
                case MessageTypes.VERSION :
                    result = (long)MinorVer;
                    break;
                case MessageTypes.DOMREQUEST:
                    string[] dom = msg.Split('+');
                    SrvDOMReq(dom[0], int.Parse(dom[1]) );
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
