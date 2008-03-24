using System;
using System.Messaging;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TradeLib
{
    public delegate void MessageDelegate(TL2 msgid, string source);
    public delegate void TickDelegate(Tick t);
    public delegate void FillDelegate(Trade t);
    public delegate void IndexDelegate(Index idx);
    public delegate void StockDelegate(Stock stock);
    public delegate void SrvFillRequest(Order order);

    public class TLServerNotFound : Exception { }

    [Serializable] [XmlRoot("TLMessages",IsNullable=true)]
    public class LinkMessage
    {
        public LinkMessage(TL2 t, object m) { Type = t; Body = m; }
        public LinkMessage(TL2 t) { Type = t; }
        public LinkMessage(object m) : this(TL2.INFO, m) { }
        public LinkMessage() : this(TL2.INFO, null) { }
        private string from = "";
        [XmlElement("from")]
        public string From { get { return from; } set { from = value; } }
        private TL2 type = TL2.INFO;
        [XmlElement("type")]
        public TL2 Type { get { return type; } set { type = value; } }
        private object body = new object();
        [XmlElement("body")]
        public Object Body { get { return body; } set { body = value; } }
    }

    public class TradeLink_MQ : TradeLink
    {
        // clients that want notifications for subscribed stocks can override these methods
        public event MessageDelegate GotMessage;
        public event TickDelegate gotTick;
        public event FillDelegate gotFill;

        // clients to server
        public override void Register() { TLSend(TL2.REGISTERCLIENT); }
        public override void Subscribe(MarketBasket mb) { TLSend(new LinkMessage(TL2.REGISTERSTOCK,mb)); }
        public override void Disconnect() { TLSend(TL2.CLEARSTOCKS); TLSend(TL2.CLEARCLIENT); }
        public override void Unsubscribe() { TLSend(TL2.CLEARSTOCKS); }
        public override int HeartBeat() { return 1; } //TLSend(TL2.HEARTBEAT); }

        // server to clients
        public void newTick(Tick t) 
        {
            if (t.sym == "") return; // can't process symbol-less ticks
            for (int i = 0;i<client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i]!=null) && (stocks[i].Contains(t.sym))) 
                    TLSend(client[i],new LinkMessage(TL2.TICKNOTIFY,t)); 
        }
        public void newFill(Trade t) 
        {
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(t.symbol)))
                    TLSend(client[i],new LinkMessage(TL2.EXECUTENOTIFY,t)); 
        }




        // server structures
        private List<string> client = new List<string>();
        private List<DateTime> heart = new List<DateTime>();
        private List<string> stocks = new List<string>();
        public string Stocks(string him)
        {
                int cid = client.IndexOf(him);
                if (cid == -1) return ""; // client not registered
                return stocks[cid];
        }

        public TradeLink_MQ() : this(null) { }
        public TradeLink_MQ(string me)
        {
            if (me == null) { Inbound = "TradeLink"; return; }
            else Inbound = me;
            if (me != "TradeLink") Outbound = "TradeLink";
        }
        MessageQueue xmt = null;
        MessageQueue rec = null;
        string outb = "TradeLink";
        string inb = "TLC";

        string Inbound 
        { 
            get 
            { 
                return ".\\Private$\\" + inb; 
            } 
            set 
            { 
                inb = value;
                if (!MessageQueue.Exists(Inbound)) MessageQueue.Create(Inbound);
                rec = new MessageQueue(Inbound);
                BinaryMessageFormatter f = new BinaryMessageFormatter();
                //f.TopObjectFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                //f.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
                rec.Formatter = f;
                rec.ReceiveCompleted += new ReceiveCompletedEventHandler(gotMessage);
                rec.BeginReceive();
            } 
        }
        string Me { get { return inb; } }
        string Him { get { return outb; } }
        public string Outbound 
        { 
            get 
            { 
                return ".\\Private$\\" + outb; 
            } 
            set 
            { 
                outb = value;
                if (!MessageQueue.Exists(Outbound)) MessageQueue.Create(Outbound);
                xmt = new MessageQueue(Outbound);
                BinaryMessageFormatter f = new BinaryMessageFormatter();
                f.TopObjectFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                f.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
                xmt.Formatter = f;
            } 
        }
        bool hasQueueEnds { get { return (inb != null) && (outb != null); } }
        void TLSend(TL2 type) { TLSend(Him, new LinkMessage(type)); }
        void TLSend(LinkMessage m) { TLSend(Him, m); }
        void TLSend(string to, LinkMessage m)
        {
            Outbound = to;
            if (!hasQueueEnds) return;
            m.From = Me;
            System.Messaging.Message sm = new System.Messaging.Message(m);
            sm.ResponseQueue = rec;
            sm.Formatter = new BinaryMessageFormatter();
            xmt.Send(sm);
        }

        void gotMessage(object sender, ReceiveCompletedEventArgs e)
        {
            TL2 mt = TL2.INFO;
            string from = "";
            LinkMessage gotmess = null;
            object m = null;
            gotmess = (LinkMessage)e.Message.Body;
            mt = gotmess.Type;
            m = gotmess.Body;
            from = gotmess.From;
            
            int cid = -1;
            Tick t = new Tick();
            
            try
            {
                switch (mt)
                {
                    // CLIENT MESSAGES
                    case TL2.TICKNOTIFY:
                        t = (Tick)m;
                        gotTick(t);
                        break;
                    case TL2.EXECUTENOTIFY:
                        Trade trade;
                        trade = (Trade)m;
                        gotFill(trade);
                        break;

                    // SERVER MESSAGES
                    case TL2.REGISTERCLIENT:
                        if (client.IndexOf(from) != -1) break; // we already had your client, ignore
                        client.Add(from);
                        stocks.Add("");
                        heart.Add(new DateTime());
                        break;
                    case TL2.REGISTERSTOCK:
                        cid = client.IndexOf(from);
                        if (cid == -1) break; // client not found
                        MarketBasket mb = (MarketBasket)m;
                        stocks[cid] = mb.ToString();
                        BeatHeart(from);
                        break;
                    case TL2.HEARTBEAT:
                        BeatHeart(from);
                        break;
                    case TL2.CLEARSTOCKS:
                        cid = client.IndexOf(from);
                        if (cid == -1) break;
                        stocks[cid] = null;
                        BeatHeart(from);
                        break;
                    case TL2.CLEARCLIENT:
                        cid = client.IndexOf(from);
                        if (cid == -1) break; // we don't have the client, nothing to do
                        client[cid] = null;
                        stocks[cid] = null;
                        heart[cid] = new DateTime();
                        break;
                }
            }
            catch (Exception ex) { string s = ex.Message; }
            if (this.GotMessage!=null) GotMessage(mt, from); // send GotMessage event to any subscribers
            rec.BeginReceive(); // prepare to receive new messages
        }
        int BeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return -1; // this client isn't registered, ignore
            DateTime now = new DateTime();
            TimeSpan since = now.Subtract(heart[cid]);
            heart[cid] = now;
            return since.Seconds;
        }
    }

    public class TradeLink_WM : TradeLink
    {
        const string myver = "2.0";
        const string build = "$Rev: 992 $";
        public string Ver { get { return myver + "." + Util.CleanVer(build); } }

        // clients that want notifications for subscribed stocks can override these methods
        public event MessageDelegate GotMessage;
        public event TickDelegate gotTick;
        public event FillDelegate gotFill;
        public event IndexDelegate gotIndexTick;
        public event SrvFillRequest gotSrvFillRequest;

        private IntPtr meh = IntPtr.Zero;
        private IntPtr himh = IntPtr.Zero;
        private string hiswindow = "TradeLinkServer";
        private string mywindow = "TradeLinkClient";
        public TradeLink_WM() { }
        private IntPtr MyHandle { set { meh = value; } get { return meh; } }
        private IntPtr HimHandle { get { return himh; } }
        public string Him 
        { 
            get 
            { 
                return hiswindow; 
            } 
            set 
            { 
                hiswindow = value; 
                himh = HisHandle(hiswindow); 
            } 
        }
        public string Me 
        { 
            get { return mywindow; }
            set
            {
                mywindow = value; 
            } 
        }
        public IntPtr MeH { get { return meh; } set { meh = value; } }
        const string SIMWINDOW = "TL-ANVIL-SIMU";
        const string LIVEWINDOW = "TL-ANVIL-LIVE";
        const string REPLAYWINDOW = "TradeLink Replay";

        public bool Mode(TLTypes mode) { return Mode(mode,false, true); }
        public bool Mode(TLTypes mode, bool showarning) { return Mode(mode, false, showarning); }
        public bool Mode(TLTypes mode, bool throwexceptions, bool showwarning)
        {
            bool HandleExceptions = !throwexceptions;
            switch (mode)
            {
                case TLTypes.LIVEANVIL:
                    if (HandleExceptions)
                    {
                        try
                        {
                            GoLive();
                        }
                        catch (TLServerNotFound)
                        {
                            
                            if (showwarning)
                                MessageBox.Show("No Live broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                            return false;
                        }
                    }
                    else GoLive();
                    break;
                case TLTypes.SIMANVIL:
                    if (HandleExceptions)
                    {
                        try
                        {

                            GoSim();
                        }
                        catch (TLServerNotFound)
                        {
                            if (showwarning)
                                MessageBox.Show("No simulation broker instance was found.  Make sure broker application + TradeLink server is running.", "TradeLink server not found");
                            return false;
                        }
                    }
                    else GoSim();
                    break;
                case TLTypes.TLREPLAY:
                    if (HandleExceptions)
                    {
                        try
                        {
                            GoHist();
                        }
                        catch (TLServerNotFound)
                        {
                            if (showwarning)
                                MessageBox.Show("TradeLink Replay not found. Please start TradeLink Replay.", "TradeLink Server not found");
                            return false;
                        }
                    }
                    else GoHist();
                    break;
            }
            return true;
        }


        /// <summary>
        /// Make's TL client use historical server
        /// </summary>
        public void GoHist() { Disconnect(); Him = REPLAYWINDOW; Register(); }
        /// <summary>
        /// Makes TL client use Anvil LIVE server (anvil must be logged in and TradeLink loaded)
        /// </summary>
        public void GoLive() { Disconnect(); Him = LIVEWINDOW; Register(); }

        /// <summary>
        /// Makes TL client use Anvil Simulation mode (anvil must be logged in and TradeLink loaded)
        /// </summary>
        public void GoSim() { Disconnect(); Him = SIMWINDOW; Register(); }

        public void GoSrv() { Me = REPLAYWINDOW; }
        protected long TLSend(TL2 type) { return TLSend(type, ""); }
        protected long TLSend(TL2 type, string m)
        {
            if (meh == IntPtr.Zero) throw new Exception("TradeLink client names or handle not defined: " + Me + "(" + meh.ToInt32() + ")");
            else if (himh == IntPtr.Zero) throw new TLServerNotFound();
            long res = SendMsg(m, himh, meh, (int)type);
            return res;
        }
        /// <summary>
        /// Sends the order.
        /// </summary>
        /// <param name="o">The oorder</param>
        /// <returns>Zero if succeeded, anvil error code otherwise.</returns>
        public int SendOrder(Order o)
        {
            if (o == null) return 0;
            string m = o.symbol+","+(o.side ? "B" : "S")+","+Math.Abs(o.size)+","+o.price+","+o.stopp+","+o.comment+",";
            return (int)TLSend(TL2.SENDORDER, m);
        }

        /// <summary>
        /// Today's high
        /// </summary>
        /// <param name="sym">The symbol.</param>
        /// <returns></returns>
        public decimal DayHigh(string sym) { return unpack(TLSend(TL2.NDAYHIGH,sym)); }
        public decimal FastHigh(string sym) 
        { 
            try 
            {
                return chighs[sym];
            }
            catch (KeyNotFoundException)
            {
                decimal high = DayHigh(sym);
                if (high!=0) chighs[sym] = high;
                return high;
            }
        }
        /// <summary>
        /// Today's low
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns></returns>
        public decimal DayLow(string sym) { return unpack(TLSend(TL2.NDAYLOW, sym)); }
        public decimal FastLow(string sym)
        {
            try
            {
                return clows[sym];
            }
            catch (KeyNotFoundException)
            {
                decimal low = DayLow(sym);
                if (low!=0) clows[sym] = low;
                return low;
            }
        }
        /// <summary>
        /// Today's closing price (zero if still open)
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns></returns>
        public decimal DayClose(string sym) { return unpack(TLSend(TL2.CLOSEPRICE, sym)); }
        /// <summary>
        /// yesterday's closing price (day)
        /// </summary>
        /// <param name="sym">the symbol</param>
        /// <returns></returns>
        public decimal YestClose(string sym) { return unpack(TLSend(TL2.YESTCLOSE, sym)); }
        /// <summary>
        /// Gets opening price for this day
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>decimal</returns>
        public decimal DayOpen(string sym) { return unpack(TLSend(TL2.OPENPRICE, sym)); }
        /// <summary>
        /// Gets position size
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>signed integer representing position size in shares</returns>
        public int PosSize(string sym) { return (int)TLSend(TL2.GETSIZE, sym); }
        /// <summary>
        /// Returns average price for a position
        /// </summary>
        /// <param name="sym">The symbol</param>
        /// <returns>decimal representing average price</returns>
        public decimal AvgPrice(string sym) { return unpack(TLSend(TL2.AVGPRICE,sym)); }

        public Position FastPos(string sym)
        {
            try
            {
                return cpos[sym];
            }
            catch (KeyNotFoundException)
            {
                Position p = new Position(AvgPrice(sym),PosSize(sym));
                cpos.Add(sym, p);
                return p;
            }
        }

        public override void Disconnect()
        {
            try
            {
                TLSend(TL2.CLEARSTOCKS, mywindow);
                TLSend(TL2.CLEARCLIENT, mywindow);
            }
            catch (TLServerNotFound) { }
        }

        public override void Register()
        {
            TLSend(TL2.REGISTERCLIENT, mywindow);
        }

        public override void Subscribe(MarketBasket mb)
        {
            TLSend(TL2.REGISTERSTOCK,mywindow + "+" + mb.ToString());
        }

        public void RegIndex(IndexBasket ib)
        {
            TLSend(TL2.REGISTERINDEX, mywindow + "+" + ib.ToString());
        }

        public override void Unsubscribe()
        {
            TLSend(TL2.CLEARSTOCKS,mywindow);
        }

        public override int HeartBeat()
        {
            return (int)TLSend(TL2.HEARTBEAT, mywindow);
        }

        //SendMessage
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(System.IntPtr hwnd, int msg, int wparam, ref COPYDATASTRUCT lparam);
        const int WM_COPYDATA = 0x004A;

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string ClassName, string WindowName);

        public static IntPtr FindClient(string name) { return FindWindow(null, name); }

        public static bool Found(string name) { return (FindClient(name) != IntPtr.Zero); }


        public TLTypes TLFound()
        {
            TLTypes f = TLTypes.NONE;
            if (Found(SIMWINDOW)) f |= TLTypes.SIMANVIL;
            if (Found(LIVEWINDOW)) f |= TLTypes.LIVEANVIL;
            if (Found(REPLAYWINDOW)) f |= TLTypes.TLREPLAY;
            return f;
        }

        /*
        //TL1.0 dll wrappage
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern int getSize(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getAvgPrice(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getDayHigh(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getDayLow(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getDayOpen(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getDayClose(string symbol,string windowname);
        [DllImport("c:///program files/tradelink/eTradeLink.dll")]
        static extern decimal getYestClose(string symbol,string windowname);
        */





        static IntPtr HisHandle(string WindowName) 
        {
            IntPtr p = IntPtr.Zero;
            try
            {
                p = FindWindow(null, WindowName);
            }
            catch (NullReferenceException) { }
            return p;
        }

        struct COPYDATASTRUCT
        {
            public int dwData;
            public int cbData;
            public IntPtr lpData;
        }

        /// <summary>
        /// Sends the MSG from source window to destination window, using WM_COPYDATA.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messagetype">The messagetype.</param>
        /// <param name="destinationwindow">The destinationwindow.</param>
        /// <returns></returns>
        long SendMsg(string message, TL2 messagetype, string destinationwindow)
        {
            IntPtr him = HisHandle(destinationwindow);
            return SendMsg(message, him, MyHandle, (int)messagetype);
        }
        /// <summary>
        /// Sends the MSG.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="desthandle">The desthandle.</param>
        /// <param name="sourcehandle">The sourcehandle.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        static long SendMsg(string str,System.IntPtr desthandle, System.IntPtr sourcehandle, int type)
        {
            if ((desthandle == IntPtr.Zero) || (sourcehandle == IntPtr.Zero)) return -1; // fail on invalid handles
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds.dwData = type;
            str = str + '\0';
            cds.cbData = str.Length+1;

            System.IntPtr pData = Marshal.AllocCoTaskMem(str.Length);
            pData = Marshal.StringToCoTaskMemAnsi(str);

            cds.lpData = pData;

            IntPtr res = SendMessage(desthandle, WM_COPYDATA, (int)sourcehandle, ref cds);
            Marshal.FreeCoTaskMem(pData);
            return res.ToInt64();
        }

        /// <summary>
        /// This method must be called from the WndProc method of your applications form.
        /// </summary>
        /// <param name="m">The WndProc-provided message.</param>
        public void GotWM_Copy(ref System.Windows.Forms.Message mymess)
        {
            if (mymess.Msg == WM_COPYDATA)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds = (COPYDATASTRUCT)Marshal.PtrToStructure(mymess.LParam, typeof(COPYDATASTRUCT));
                long result = 0;
                if (cds.cbData > 0)
                {
                    TL2 type = (TL2)cds.dwData;
                    string msg = Marshal.PtrToStringAnsi(cds.lpData);
                    if (Me == REPLAYWINDOW) // we're a server
                    {
                        switch (type)
                        {
                            case TL2.GETSIZE:
                                if (SrvPos.ContainsKey(msg))
                                    result = SrvPos[msg].Size;
                                else result = 0;
                                break;
                            case TL2.AVGPRICE:
                                if (SrvPos.ContainsKey(msg))
                                    result = pack(SrvPos[msg].AvgPrice);
                                else result = 0;
                                break;
                            case TL2.NDAYHIGH:
                                if (highs.ContainsKey(msg))
                                    result = pack(highs[msg]);
                                else result = 0;
                                break;
                            case TL2.NDAYLOW:
                                if (lows.ContainsKey(msg))
                                    result = pack(lows[msg]);
                                else result = 0;
                                break;
                            case TL2.SENDORDER:
                                SrvDoExecute(msg);
                                break;
                            case TL2.REGISTERCLIENT:
                                SrvRegClient(msg);
                                break;
                            case TL2.REGISTERSTOCK:
                                string[] m = msg.Split('+');
                                SrvRegStocks(m[0], m[1]);
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
                            default:
                                result = 0;
                                break;
                        }
                    }
                    else // we're a client
                    {
                        string[] r = msg.Split(',');
                        switch (type)
                        {
                            case TL2.TICKNOTIFY:
                                Tick t = new Tick();
                                t.sym = r[(int)f.symbol];
                                if (Index.isIdx(t.sym))
                                {
                                    // we got an index update
                                    Index i = Index.Deserialize(msg);
                                    if (gotIndexTick != null) gotIndexTick(i);
                                    break;
                                }
                                int date = Convert.ToInt32(r[(int)f.date]);
                                int time = Convert.ToInt32(r[(int)f.time]);
                                int sec = Convert.ToInt32(r[(int)f.sec]);
                                // trade,size,tex,bid,ask,bs,as,be,as
                                if (r[(int)f.trade].Equals("0")) t.SetQuote(date, time, sec, Convert.ToDecimal(r[(int)f.bid]), Convert.ToDecimal(r[(int)f.ask]), Convert.ToInt32(r[(int)f.bidsize]), Convert.ToInt32(r[(int)f.asksize]), r[(int)f.bidex], r[(int)f.askex]);
                                else t.SetTrade(date, time, sec, Convert.ToDecimal(r[(int)f.trade]), Convert.ToInt32(r[(int)f.tsize]), r[(int)f.tex]);

                                if (t.isTrade)
                                {
                                    try
                                    {
                                        if (t.trade > chighs[t.sym]) chighs[t.sym] = t.trade;
                                        if (t.trade < clows[t.sym]) clows[t.sym] = t.trade;
                                    }
                                    catch (KeyNotFoundException)
                                    {
                                        decimal high = DayHigh(t.sym);
                                        decimal low = DayLow(t.sym);
                                        chighs.Add(t.sym, high);
                                        if (low == 0) low = 640000;
                                        clows.Add(t.sym, low);
                                    }
                                }
                                if (gotTick != null) gotTick(t);
                                break;
                            case TL2.EXECUTENOTIFY:
                                // date,time,symbol,side,size,price,comment
                                Trade tr = new Trade(r[(int)xf.sym], Convert.ToBoolean(r[(int)xf.side]), Convert.ToInt32(r[(int)xf.size]), 0, 0, r[(int)xf.desc], Convert.ToInt32(r[(int)xf.time]), Convert.ToInt32(r[(int)xf.date]));
                                tr.Fill(Convert.ToDecimal(r[(int)xf.price]));
                                tr.xsec = Convert.ToInt32(r[(int)xf.sec]);
                                try
                                {
                                    cpos[tr.symbol] = new Position(tr.symbol, AvgPrice(tr.symbol), PosSize(tr.symbol));
                                }
                                catch (KeyNotFoundException)
                                {
                                    cpos.Add(tr.symbol, new Position(tr.symbol, AvgPrice(tr.symbol), PosSize(tr.symbol)));
                                }
                                if (gotFill != null) gotFill(tr);
                                break;
                        }
                        if (GotMessage != null) GotMessage(type, hiswindow);
                        result = 0;
                    }
                }
                mymess.Result = (IntPtr)result;
            }
        }

        Dictionary<string, decimal> chighs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> clows = new Dictionary<string, decimal>();
        Dictionary<string, Position> cpos = new Dictionary<string, Position>();

        private void SrvDoExecute(string msg)
        {
            string[] r = msg.Split(',');
            Order o = new Order(r[0], (r[1].Contains("B")), Convert.ToInt32(r[2]), Convert.ToDecimal(r[3]), Convert.ToDecimal(r[4]), r[5], 0, 0);
            if (gotSrvFillRequest != null) gotSrvFillRequest(o);
        }

        public void newIndexTick(Index itick)
        {
            if (itick.Name == "") return;
            for (int i = 0; i < index.Count; i++)
                if ((client[i] != null) && (index[i].Contains(itick.Name)))
                    SendMsg(Index.Serialize(itick), TL2.TICKNOTIFY, client[i]);
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
                    SendMsg(tick.toTLmsg(), TL2.TICKNOTIFY, client[i]);
        }

        Dictionary<string, Position> SrvPos = new Dictionary<string, Position>();

        /// <summary>
        /// Notifies subscribed clients of a new execution.
        /// </summary>
        /// <param name="trade">The trade to include in the notification.</param>
        public void newFill(Trade trade)
        {
            if (trade.symbol=="") return; // can't process symbol-less trades
            if (!SrvPos.ContainsKey(trade.symbol)) SrvPos.Add(trade.symbol, new Position(trade));
            else SrvPos[trade.symbol].Adjust(trade);
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(trade.symbol)))
                    SendMsg(trade.TLmsg(),TL2.EXECUTENOTIFY,client[i]);
        }

        // server structures
        private Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        private List<string> client = new List<string>();
        private List<DateTime> heart = new List<DateTime>();
        private List<string> stocks = new List<string>();
        private List<string> index = new List<string>();

        public string SrvStocks(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return ""; // client not registered
            return stocks[cid];
        }
        public string[] SrvGetClients() { return client.ToArray(); }
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
            if (cid == -1) return;
            stocks[cid] = "";
            client[cid] = "";
        }

        void SrvBeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return; // this client isn't registered, ignore
            TimeSpan since = DateTime.Now.Subtract(heart[cid]);
            heart[cid] = DateTime.Now;
        }
        decimal unpack(long i)
        {
            if (i == 0) return 0;
            int w = (int)(i >> 16);
            int f = (int)(i - (w << 16));
            decimal dec = (decimal)w;
            decimal frac = (decimal)f;
            frac /= 1000;
            dec += frac;
            return dec;
        }
        long pack(decimal d)
        {
            int whole = (int)Math.Truncate(d);
            int frac = (int)(d - whole);
            long packed = (whole << 16) + frac;
            return packed;
        }





        enum xf // execution message fields from TL server
        {
            date = 0,
            time,
            sec,
            sym,
            side,
            size,
            price,
            desc
        }
        enum f
        { // tick message fields from TL server
            symbol = 0,
            date,
            time,
            sec,
            trade,
            tsize,
            tex,
            bid,
            ask,
            bidsize,
            asksize,
            bidex,
            askex,
        }
    }


    [Flags]
    public enum TLTypes
    {
        NONE = 0,
        LIVEANVIL = 1,
        SIMANVIL = 2,
        TLREPLAY = 4,
    }

    public enum TL2
    {
        SENDORDER = 1,
        AVGPRICE,
        POSOPENPL,
        POSCLOSEDPL,
        POSLONGPENDSHARES,
        POSSHORTPENDSHARES,
        LRPBID,
        LRPASK,
        POSTOTSHARES,
        LASTTRADE,
        LASTSIZE,
        NDAYHIGH,
        NDAYLOW,
        INTRADAYHIGH,
        INTRADAYLOW,
        OPENPRICE,
        CLOSEPRICE,
        ISSIMULATION = 25,
        GETSIZE,
        YESTCLOSE,
        TICKNOTIFY = 100,
        EXECUTENOTIFY,
        REGISTERCLIENT,
        REGISTERSTOCK,
        CLEARSTOCKS,
        CLEARCLIENT,
        HEARTBEAT,
        INFO,
        QUOTENOTIFY,
        TRADENOTIFY_NOTUSED, 
        REGISTERINDEX,
        DAYRANGE,
    }

    public abstract class TradeLink
    {
        public static string PrettyAnvilError(int errorcode)
        {
            string[] e = { "SO_OK", "SO_NO_ACCOUNT", "SO_NO_SERVER_CONNECTION", "SO_STOCK_NOT_INITIALIZED", "SO_BUYING_POWER_EXCEEDED", "SO_SIZE_ZERO", "SO_INCORRECT_PRICE", "SO_INCORRECT_SIDE", "SO_NO_BULLETS_FOR_CHEAP_STOCK", "SO_NO_SHORTSELL_FOR_CHEAP_STOCK", "SO_NO_ONOPENORDER_FOR_NASDAQ_STOCK", "SO_NO_ONCLOSEORDER_FOR_NASDAQ_STOCK", "SO_NO_ONCLOSEORDER_AGAINST_IMBALANCE_AFTER_1540", "SO_NO_ONCLOSEORDER_AFTER_1600", "SO_NO_SIZEORDER_FOR_NON_NASDAQ_STOCK", "SO_NO_STOPORDER_FOR_NASDAQ_STOCK", "SO_MAX_ORDER_SIZE_EXCEEDED", "SO_MAX_POSITION_SIZE_EXCEEDED", "SO_MAX_POSITION_VALUE_EXCEEDED", "SO_TRADING_LOCKED", "SO_TRADING_HISTORY_NOT_LOADED", "SO_NO_SOES_ORDER_WHEN_MARKET_CLOSED", "SO_NO_SDOT_ORDER_WHEN_MARKET_CLOSED", "SO_MAX_OPEN_POSITIONS_EXCEEDED", "SO_MAX_POSITION_PENDING_ORDERS_EXCEEDED", "SO_MAX_TOTAL_SHARES_EXCEEDED", "SO_MAX_TRADED_SHARES_EXCEEDED", "SO_AMEX_ORDER_EXECUTION_BLOCKED", "SO_NYSE_ORDER_EXECUTION_BLOCKED", "SO_NASDAQ_ORDER_EXECUTION_BLOCKED", "SO_ARCA_ORDER_EXECUTION_BLOCKED", "SO_MAX_LOSS_EXCEEDED", "SO_MAX_LOSS_PER_STOCK_EXCEEDED", "SO_MAX_OPEN_LOSS_PER_STOCK_EXCEEDED", "SO_NYSE_ODD_LOT_VIOLATION", "SO_SHORT_EXEMPT_NOT_INSTITUTIONAL", "SO_SELL_SIZE_GREATER_THAN_POSITION", "SO_NO_SHORT_BEFORE_SELL_COVER_POSITION", "SO_SHORT_CAN_EXECUTE_BEFORE_SELL", "SO_SAME_PRICE_VENUE_OVERSELL", "SO_STAGING_TICKET_EXCEEDED", "SO_DESTINATION_NOT_RECOGNIZED", "SO_HIT_OWN_ORDERS", "SO_SHORT_SELL_VIOLATION", "SO_POTENTIAL_OVERSELL" };
            return e[errorcode];
        }


        // the tradelink API (tradelink2)
        public abstract void Disconnect();
        public abstract void Register();
        public abstract void Subscribe(MarketBasket mb);
        public abstract void Unsubscribe();
        public abstract int HeartBeat();

    }


}

