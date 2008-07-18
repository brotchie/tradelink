using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TradeLib
{
    public class TradeLink_Server_WM : Form, TradeLinkServer
    {
        public delegate decimal DecimalStringDelegate(string s);
        public delegate int IntStringDelegate(string s);
        public delegate string StringDelegate();
        public event DecimalStringDelegate gotSrvAcctOpenPLRequest;
        public event DecimalStringDelegate gotSrvAcctClosedPLRequest;
        public event StringDelegate gotSrvAcctRequest;
        public event DecimalStringDelegate PositionPriceRequest;
        public event IntStringDelegate PositionSizeRequest;
        public event DecimalStringDelegate DayHighRequest;
        public event DecimalStringDelegate DayLowRequest;
        public event OrderDelegate gotSrvFillRequest;
        public event UIntDelegate OrderCancelRequest;

        public TradeLink_Server_WM() : this(TLTypes.HISTORICALBROKER) { }
        public TradeLink_Server_WM(string servername) : base()
        {
            this.Text = servername;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.ShowInTaskbar = false;
            this.Hide();
        }
        public TradeLink_Server_WM(TLTypes servertype) : 
            this(servertype==TLTypes.LIVEBROKER? WMUtil.LIVEWINDOW :
                (servertype == TLTypes.SIMBROKER ? WMUtil.SIMWINDOW : (servertype== TLTypes.HISTORICALBROKER? WMUtil.REPLAYWINDOW : WMUtil.TESTWINDOW))) { }

        private void SrvDoExecute(string msg) // handle an order (= execute request)
        {
            if (this.InvokeRequired)
                this.Invoke(new DebugDelegate(SrvDoExecute), new object[] { msg });
            else
            {
                Order o = Order.Deserialize(msg);
                if (gotSrvFillRequest != null) gotSrvFillRequest(o); //request fill
            }
        }

        public void newIndexTick(Index itick)
        {
            if (this.InvokeRequired)
                this.Invoke(new IndexDelegate(newIndexTick), new object[] { itick });
            else
            {
                if (!itick.isValid) return;
                for (int i = 0; i < index.Count; i++)
                    if ((client[i] != null) && (index[i].Contains(itick.Name)))
                        WMUtil.SendMsg(itick.Serialize(), TL2.TICKNOTIFY, Handle, client[i]);
            }
        }

        delegate void tlneworderdelegate(Order o, bool allclients);
        public void newOrder(Order o) { newOrder(o, false); }
        public void newOrder(Order o, bool allclients)
        {
            if (this.InvokeRequired)
                this.Invoke(new tlneworderdelegate(newOrder), new object[] { o,allclients });
            else
            {
                for (int i = 0; i < client.Count; i++)
                    if ((client[i] != null) && (client[i] != "") && (stocks[i].Contains(o.symbol) || allclients))
                        WMUtil.SendMsg(o.Serialize(), TL2.ORDERNOTIFY, Handle, client[i]);
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
                this.Invoke(new TickDelegate(newTick), new object[] { tick });
            else
            {
                if (!tick.isValid) return; // need a valid tick
                for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                    if ((client[i] != null) && (stocks[i].Contains(tick.sym)))
                        WMUtil.SendMsg(tick.Serialize(), TL2.TICKNOTIFY, Handle, client[i]);
            }
        }

        delegate void tlnewfilldelegate(Trade t, bool allclients);
        /// <summary>
        /// Notifies subscribed clients of a new execution.
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        public void newFill(Trade trade) { newFill(trade, false); }
        public void newFill(Trade trade,bool allclients)
        {
            if (this.InvokeRequired)
                this.Invoke(new tlnewfilldelegate(newFill), new object[] { trade,allclients });
            else
            {
                // make sure our trade is filled and initialized properly
                if (!trade.isValid || !trade.isFilled) return;
                for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                    if ((client[i] != null) && ((stocks[i].Contains(trade.symbol) || allclients)))
                        WMUtil.SendMsg(trade.Serialize(), TL2.EXECUTENOTIFY, Handle, client[i]);
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

        public void newOrderCancel(long orderid_being_cancled)
        {
            foreach (string c in client) // send order cancel notifcation to clients
                WMUtil.SendMsg(orderid_being_cancled.ToString(), TL2.ORDERCANCELRESPONSE, Handle, c);
        }


        protected override void WndProc(ref Message m)
        {
            TradeLinkMessage tlm = WMUtil.ToTradeLinkMessage(ref m);
            if (tlm == null)
            {
                base.WndProc(ref m);
                return;
            }
            string msg = tlm.body;

            long result = (long)TL2.OK;
            switch (tlm.type)
            {
                case TL2.ACCOUNTOPENPL:
                    if (gotSrvAcctOpenPLRequest == null) break;
                    result = WMUtil.pack(gotSrvAcctOpenPLRequest(msg));
                    break;
                case TL2.ACCOUNTCLOSEDPL:
                    if (gotSrvAcctClosedPLRequest == null) break;
                    result = WMUtil.pack(gotSrvAcctClosedPLRequest(msg));
                    break;
                case TL2.ACCOUNTREQUEST:
                    if (gotSrvAcctRequest == null) break;
                    WMUtil.SendMsg(gotSrvAcctRequest(), TL2.ACCOUNTRESPONSE, Handle, msg);
                    break;
                case TL2.ORDERCANCELREQUEST:
                    if (OrderCancelRequest != null)
                        OrderCancelRequest(Convert.ToUInt32(msg));
                    break;
                case TL2.GETSIZE:
                    if (PositionSizeRequest != null)
                        result = PositionSizeRequest(msg);
                    break;
                case TL2.AVGPRICE:
                    if (PositionPriceRequest != null)
                        result = WMUtil.pack(PositionPriceRequest(msg));
                    break;
                case TL2.NDAYHIGH:
                    if (DayHighRequest!=null)
                        result = WMUtil.pack(DayHighRequest(msg));
                    break;
                case TL2.NDAYLOW:
                    if (DayLowRequest!=null)
                        result = WMUtil.pack(DayLowRequest(msg));
                    break;
                case TL2.SENDORDER:
                    SrvDoExecute(msg);
                    break;
                case TL2.REGISTERCLIENT:
                    SrvRegClient(msg);
                    break;
                case TL2.REGISTERSTOCK:
                    string[] m2 = msg.Split('+');
                    SrvRegStocks(m2[0], m2[1]);
                    break;
                case TL2.REGISTERINDEX:
                    string[] ib = msg.Split('+');
                    SrvRegIndex(ib[0], ib[1]);
                    break;
                case TL2.CLEARCLIENT:
                    SrvClearClient(msg);
                    break;
                case TL2.CLEARSTOCKS:
                    SrvClearStocks(msg);
                    break;
                case TL2.HEARTBEAT:
                    SrvBeatHeart(msg);
                    break;
                case TL2.BROKERNAME :
                    result = (long)Brokers.TradeLinkSimulation;
                    break;
                case TL2.FEATUREREQUEST:
                    TL2[] f = GetFeatures();
                    List<string> mf = new List<string>();
                    foreach (TL2 t in f)
                    {
                        int ti = (int)t;
                        mf.Add(ti.ToString());
                    }
                    string msf = string.Join(",", mf.ToArray());
                    WMUtil.SendMsg(msf, TL2.FEATURERESPONSE, Handle, msg);
                    break;
                default:
                    result = (long)TL2.FEATURE_NOT_IMPLEMENTED;
                    break;
            }
            m.Result = (IntPtr)result;
        }

        TL2[] GetFeatures()
        {
            List<TL2> f = new List<TL2>();

            f.Add(TL2.ACCOUNTCLOSEDPL);
            f.Add(TL2.ACCOUNTOPENPL);
            f.Add(TL2.ACCOUNTREQUEST);
            f.Add(TL2.ACCOUNTRESPONSE);
            f.Add(TL2.AVGPRICE);
            f.Add(TL2.BROKERNAME);
            f.Add(TL2.CLEARCLIENT);
            f.Add(TL2.CLEARSTOCKS);
            f.Add(TL2.EXECUTENOTIFY);
            f.Add(TL2.FEATUREREQUEST);
            f.Add(TL2.FEATURERESPONSE);
            f.Add(TL2.GETSIZE);
            f.Add(TL2.HEARTBEAT);
            f.Add(TL2.NDAYHIGH);
            f.Add(TL2.NDAYLOW);
            f.Add(TL2.OPENPRICE);
            f.Add(TL2.ORDERCANCELREQUEST);
            f.Add(TL2.ORDERCANCELRESPONSE);
            f.Add(TL2.ORDERNOTIFY);
            f.Add(TL2.POSCLOSEDPL);
            f.Add(TL2.POSOPENPL);
            f.Add(TL2.REGISTERCLIENT);
            f.Add(TL2.REGISTERINDEX);
            f.Add(TL2.REGISTERSTOCK);
            f.Add(TL2.SENDORDER);
            f.Add(TL2.TICKNOTIFY);
            f.Add(TL2.TRADENOTIFY);
            return f.ToArray();
        }


    }
}
