using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TradeLib
{
    public class TradeLink_Server_WM : Form, TradeLinkServer
    {
        public TradeLink_Server_WM() : this(false) { }
        public TradeLink_Server_WM(string servername) : base()
        {
            this.Text = servername;
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.ShowInTaskbar = false;
            this.Hide();
        }
        public TradeLink_Server_WM(bool liveserver) : this(liveserver ? WMUtil.LIVEWINDOW : WMUtil.SIMWINDOW) { }

        public event OrderDelegate gotSrvFillRequest;
        Dictionary<string, decimal> chighs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> clows = new Dictionary<string, decimal>();
        Dictionary<string, Position> cpos = new Dictionary<string, Position>();

        private void SrvDoExecute(string msg)
        {
            Order o = Order.Deserialize(msg);
            if (gotSrvFillRequest != null) gotSrvFillRequest(o);
            for (int i = 0; i < client.Count; i++)
                WMUtil.SendMsg(msg, TL2.ORDERNOTIFY, Handle,client[i]);
        }

        public void newIndexTick(Index itick)
        {
            if (itick.Name == "") return;
            for (int i = 0; i < index.Count; i++)
                if ((client[i] != null) && (index[i].Contains(itick.Name)))
                    WMUtil.SendMsg(itick.Serialize(), TL2.TICKNOTIFY, Handle,client[i]);
        }

        // server to clients
        /// <summary>
        /// Notifies subscribed clients of a new tick.
        /// </summary>
        /// <param name="tick">The tick to include in the notification.</param>
        public void newTick(Tick tick)
        {
            if (tick.sym == "") return; // can't process symbol-less ticks
            if (tick.isTrade)
            {
                if (!highs.ContainsKey(tick.sym)) { highs.Add(tick.sym, tick.trade); lows.Add(tick.sym, tick.trade); }
                if (tick.trade > highs[tick.sym]) highs[tick.sym] = tick.trade;
                if (tick.trade < lows[tick.sym]) lows[tick.sym] = tick.trade;
            }

            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(tick.sym)))
                    WMUtil.SendMsg(tick.Serialize(), TL2.TICKNOTIFY, Handle,client[i]);
        }

        Dictionary<string, Position> SrvPos = new Dictionary<string, Position>();

        /// <summary>
        /// Notifies subscribed clients of a new execution.
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        public void newFill(Trade trade)
        {
            if (trade.symbol == "") return; // can't process symbol-less trades
            if (!SrvPos.ContainsKey(trade.symbol)) SrvPos.Add(trade.symbol, new Position(trade));
            else SrvPos[trade.symbol].Adjust(trade);
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(trade.symbol)))
                {
                    string msg = trade.Serialize();
                    WMUtil.SendMsg(msg, TL2.EXECUTENOTIFY, Handle,client[i]);
                }
        }

        public int NumClients { get { return client.Count; } }

        // server structures

        private Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
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
                case TL2.GETSIZE:
                    if (SrvPos.ContainsKey(msg))
                        result = SrvPos[msg].Size;
                    else result = 0;
                    break;
                case TL2.AVGPRICE:
                    if (SrvPos.ContainsKey(msg))
                        result = WMUtil.pack(SrvPos[msg].AvgPrice);
                    else result = 0;
                    break;
                case TL2.NDAYHIGH:
                    if (highs.ContainsKey(msg))
                        result = WMUtil.pack(highs[msg]);
                    else result = 0;
                    break;
                case TL2.NDAYLOW:
                    if (lows.ContainsKey(msg))
                        result = WMUtil.pack(lows[msg]);
                    else result = 0;
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
                default:
                    result = (long)TL2.FEATURE_NOT_IMPLEMENTED;
                    break;
            }
            m.Result = (IntPtr)result;
        }
    }
}
