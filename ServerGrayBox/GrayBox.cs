using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using Microsoft.Win32;


using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;
using GBQUOTESLib;

class DestinationStrings
{
public const string STR_DEST_LSTK = "LSTK";
public const string STR_DEST_NYSE = "NYSE";
public const string STR_DEST_MILL = "MILL";
public const string STR_DEST_EDGA = "EDGA";
public const string STR_DEST_EDGX = "EDGX";
public const string STR_DEST_CSFB = "CSFB";
public const string STR_DEST_XBOS = "XBOS";
public const string STR_DEST_DTTX = "DTTX";
public const string STR_DEST_NSXS = "NSXS";
public const string STR_DEST_CBSX = "CBSX";
public const string STR_DEST_NQPX = "NQPX";
public const string STR_DEST_FLOW = "FLOW";
public const string STR_DEST_ATTN = "ATTN";
public const string STR_DEST_ARCA = "ARCA";
public const string STR_DEST_BELZ = "BELZ";
public const string STR_DEST_HDOT = "HDOT";
public const string STR_DEST_TRAC = "TRAC";
public const string STR_DEST_INET = "INET";
public const string STR_DEST_BRUT = "BRUT";
public const string STR_DEST_BTRD = "BTRD";
public const string STR_DEST_SOES = "SOES";
public const string STR_DEST_RASH = "RASH";
public const string STR_DEST_BATS = "BATS";
}



//for Checking GrayBox instance
public delegate bool CallBackPtr(int hwnd, int lParam);
public delegate void SetCallBack(string msg);






namespace GbTLServer
{
    public class GrayBox
    {
        //Thread related
        Thread m_bgThread;

        //Tradelink related
        TLServer tl;
        PositionTracker pt = new PositionTracker();


        ListBox lst_box;
        Button btn_connect;

        //for Checking GrayBox instance
        long msg_id = 1;
        private CallBackPtr callBackPtr;
        static bool m_bGrayBoxRunning = false;

        //For quotes
        private GBQUOTESLib.GBStockManager gbStockMgr = null;
        //private GBStock gbStock = null;
        private GBLevel1 gbLevel = null;
        public DebugWindow dbg_cntrl = new DebugWindow();
        
        //For Storing order information
        Dictionary<int, Order> OrderList = new Dictionary<int, Order>();

    
        const int MAXRECORD = 5000;
        RingBuffer<Order> _orderq = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<long> _cancelq = new RingBuffer<long>(MAXRECORD);
        RingBuffer<bool> _symsq = new RingBuffer<bool>(5);
        RingBuffer<GenericMessage> _msgq = new RingBuffer<GenericMessage>(100);
        string symquotes = "";
        Dictionary<long, int> dictOrderIdBBoxId = new Dictionary<long, int>();
       

        public GrayBox(TLServer tls, ListBox lstbox, Button btnConnect)
        {
            tl = tls;
            lst_box = lstbox;
            btn_connect = btnConnect;


            

            //ConnectQuotesServer();        

            
        }
        bool _runbg = false;
        bool _verbosedebug = true;
        public bool VerboseDebugging { get { return _verbosedebug; } set { _verbosedebug = value; } }
        IdTracker _idt = new IdTracker();
        bool _ignoreoutoforderticks = true;
        public bool IgnoreOutOfOrderTicks { get { return _ignoreoutoforderticks; } set { _ignoreoutoforderticks = value; } }
        int _fixorderdecimalplace = 2;
        public int FixOrderDecimalPlace { get { return _fixorderdecimalplace; } set { _fixorderdecimalplace = value; } }



        int _SLEEP = 50;
        int _ORDERSLEEP = 1;
        int _CANCELWAIT = 1000;

        RingBuffer<string> removesym = new RingBuffer<string>(1000);


        void background(object param)
        {
            while (_runbg)
            {
                try
                {
                    // orders
                    while (!_orderq.isEmpty)
                    {
                        //STIOrder order = new STIOrder();
                        Order o = _orderq.Read();
                        if (VerboseDebugging)
                            debug("client order received: " + o.ToString());
                        if (o.id == 0)
                        {
                            o.id = _idt.AssignId;
                        }

                        o.price = Math.Round(o.price, FixOrderDecimalPlace);
                        o.stopp = Math.Round(o.stopp, FixOrderDecimalPlace);

                      
                        //order.LmtPrice = (double)o.price;
                        //order.StpPrice = (double)o.stopp;
                        if (o.ex == string.Empty)
                            o.ex = o.symbol.Length > 3 ? "NSDQ" : "NYSE";

                        int err = (int)SendOrder(o);

                        
                        if (VerboseDebugging)
                            debug("client order sent: ");
                        string tmp = "";
                        if ((err == 0))
                        {
                            // save account/id relationship for canceling
                            //idacct.Add(o.id, order.Account);
                            // wait briefly between orders
                            Thread.Sleep(_ORDERSLEEP);
                        }
                        if (err < 0)
                            debug("Error sending order: " + Util.PrettyError(tl.newProviderName, err) + o.ToString());
                        if (err == -1)
                            debug("Make sure you have set the account in sending program.");
                    }

                    // new quotes
                    if (!_symsq.isEmpty)
                    {
                        _symsq.Read();
                        foreach (string sym in symquotes.Split(','))
                        {
                            //stiQuote.RegisterQuote(sym, "*");
                            GetQuote(sym);
                        }
                    }
                    // old quotes
                    while (removesym.hasItems)
                    {
                        string rem = removesym.Read();
                       // stiQuote.DeRegisterQuote(rem, "*");
                    }

                    // cancels
                    if (!_cancelq.isEmpty)
                    {
                        long orderid = _cancelq.Read();
                        int BBoxId = -1;
                        dictOrderIdBBoxId.TryGetValue(orderid, out BBoxId);
                        if(BBoxId != -1) CancelOrder(BBoxId);
                        //string acct = "";
                        //if (idacct.TryGetValue(number, out acct))
                        //{
                        //    // get unique cancel id
                        //    long cancelid = _canceltracker.AssignId;
                        //    // save cancel to order id relationship
                        //    _cancel2order.Add(cancelid, number);
                        //    // send cancel
                        //    stiOrder.CancelOrder(acct, 0, number.ToString(), cancelid.ToString());
                        //    if (VerboseDebugging)
                        //        debug("client cancel requested: " + number.ToString() + " " + cancelid.ToString());
                        //}
                        //else
                        //    debug("No record of id: " + number.ToString());
                        // see if empty yet
                        if (_cancelq.hasItems)
                            Thread.Sleep(_CANCELWAIT);
                    }

                    //// messages
                    //if (_msgq.hasItems)
                    //{
                    //    GenericMessage gm = _msgq.Read();
                    //    switch (gm.Type)
                    //    {
                    //        case MessageTypes.SENDORDERPEGMIDPOINT:
                    //            {
                    //                // create order
                    //                STIOrder order = new STIOrder();
                    //                // pegged 2 midmarket
                    //                order.ExecInst = "M";
                    //                // get order
                    //                Peg2Midpoint o = Peg2Midpoint.Deserialize(gm.Request);
                    //                if (!o.isValid) break;
                    //                if (VerboseDebugging)
                    //                    debug("client P2M order: " + o.ToString());
                    //                order.Symbol = o.symbol;
                    //                order.PegDiff = (double)o.pegdiff;
                    //                order.PriceType = STIPriceTypes.ptSTIPegged;
                    //                bool side = o.size > 0;
                    //                order.Side = getside(o.symbol, side);
                    //                order.Quantity = Math.Abs(o.size);
                    //                order.Destination = o.ex;
                    //                order.ClOrderID = o.id.ToString();
                    //                order.Tif = "D";
                    //                string acct = Account != string.Empty ? Account : string.Empty;
                    //                order.Account = o.Account != string.Empty ? o.Account : acct;
                    //                int err = order.SubmitOrder();
                    //                string tmp = "";
                    //                if ((err == 0) && (!idacct.TryGetValue(o.id, out tmp)))
                    //                    idacct.Add(o.id, order.Account);
                    //                if (err < 0)
                    //                    debug("Error sending order: " + Util.PrettyError(tl.newProviderName, err) + o.ToString());
                    //                if (err == -1)
                    //                    debug("Make sure you have set the account in sending program.");

                    //            }
                    //            break;
                    //    }
                    //}

                    //if (_lastimbalance != _imbalance)
                    //{
                    //    _lastimbalance = _imbalance;
                    //    // register for imbalance data
                    //    stiQuote.RegisterForAllMdx(true);
                    //}
                }
                catch (Exception ex)
                {
                    debug(ex.Message + ex.StackTrace);
                }
                if (_symsq.isEmpty && _orderq.isEmpty && _cancelq.isEmpty)
                    Thread.Sleep(_SLEEP);
            }
        }

        private void ConnectGrayboxEvents()
        {
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderConfirmEventMy += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderConfirmEventMy);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderExecuteEventMy += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderExecuteEventMy);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelConfirmEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelConfirmEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelErrorEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelErrorEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelExecEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelExecEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderErrorEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderErrorEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxDisConnectedEvent += new GrayBoxAPI.BBEventHandler(this.DisconnectEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxConnectedEvent += new GrayBoxAPI.BBEventHandler(this.ConnectEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxConnectionErrorEvent += new GrayBoxAPI.BBEventHandler(this.ConnectErrorEvent);
        }

        private void ConnectTradeLinkEvents()
        {
            // tradelink bindings
            tl.newProviderName = Providers.GrayBox;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_newSendOrderRequest);
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
        }

        private void ConnectEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
           debug("Connected to Graybox");
           btn_connect.Enabled = false;
        }
        private void ConnectErrorEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            debug("Cannot connect to Graybox please check");
        }
        private void DisconnectEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            debug("Graybox disconnected. Please restart this application.");
            GrayBoxAPI.BlackBox.instance().DisConnect();
            MessageBox.Show("Graybox disconnected. This application will close.", "Graybox TL Server", MessageBoxButtons.OK,MessageBoxIcon.Warning);
            lst_box.FindForm().Close();

            m_bGrayBoxRunning = false;
            btn_connect.Enabled = true;
        }

    

        public void Start()
        {
            m_bgThread = new Thread(new ParameterizedThreadStart(background));
            m_bgThread.IsBackground = true;
            _runbg = true;
            m_bgThread.Start();

            ConnectTradeLinkEvents();
            ConnectGrayboxEvents();

            debug("Graybox TL Server Initialized");
            debug("Click 'Connect' to make connection with Graybox");

        }

        public void Stop()
        {
            DisconnectServer();
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            return 0;
        }

        Position[] tl_newPosList(string account)
        {
            return pt.ToArray();
        }
        void tl_newOrderCancelRequest(long val)
        {
            _cancelq.Write(val);
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ISSHORTABLE);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.SENDORDER);
         

            debug("New feature request");
            return f.ToArray();

        }

        string tl_newAcctRequest()
        {
            return ("1001010");
        }

        void BlackBox_GrayBoxOrderConfirmEventMy(object ob, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
            Order o = new OrderImpl();
            OrderList.TryGetValue(e.BBXid, out o);
            tl.newOrder(o);
            string orderside = o.side ? "BUY" : "SELL";
            debug("Order confirmed " + o.id + " " + o.symbol + " " + orderside + " " + o.size);
        }
        void BlackBox_GrayBoxOrderExecuteEventMy(object ob, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
            Order o = new OrderImpl();
            OrderList.TryGetValue(e.BBXid, out o);

            Position p = new PositionImpl( o.symbol, (decimal) e.Price, e.Quantity, (decimal) e.Price, o.Account);

            string orderside = o.side ? "BUY" : "SELL";
            debug("Order executed " + o.id + " " + o.symbol + " " + orderside + " " + o.size);

            pt.NewPosition(p);
           
        }
        void BlackBox_GrayBoxOrderCancelConfirmEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
        }

        void BlackBox_GrayBoxOrderCancelErrorEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
        }

        void BlackBox_GrayBoxOrderCancelExecEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
        }

        void BlackBox_GrayBoxOrderErrorEvent(object o, GrayBoxAPI.BBEventArgs e)
        {
            UpdateOrderStatus(e);
        }
      

        private void UpdateOrderStatus(GrayBoxAPI.BBEventArgs e)
        {
            
        }

        public bool GetconnectionStatus()
        {
            Ping pPing = new Ping();
            try
            {
                PingReply reply = pPing.Send(ReadGrayBoxRegValues(strConnectionKey, strPrimaryQuotesKey),5000);
                if(reply.Status  == IPStatus.Success)
                    return true;

            }
            catch{}
            return false;
        }

        public bool ConnectQuotesServer(string strUser, string strPasswd, string strBRSQ, string strQuoteSrv, string strBookSrv)
        {
            try
            {
                if(!ValidateGrayboxInstallation()) return false;
                GBQUOTESLib.GBServer gbServer = new GBQUOTESLib.GBServerClass();
                gbServer.Initialize(strUser, strPasswd, "", strQuoteSrv, strBookSrv);
                gbStockMgr = (GBQUOTESLib.GBStockManager)gbServer.GetStockManager("");
                
                bool bConnect = GrayBoxAPI.BlackBox.instance().Connect();
                if (bConnect)
                {
                    debug("Connected to Graybox");
                }
                else
                {
                    debug("Not connected to Graybox");
                    return false;
                }

                return true;
            }
            catch
            {
                debug("Graybox connection error");
                return false;
            }
        }

        public bool ValidateGrayboxInstallation()
        {
                callBackPtr = new CallBackPtr(Report);
                EnumWindows(callBackPtr, 0);

                debug("Checking Graybox installation ... ");

                if (m_bGrayBoxRunning == false)
                {
                    //Finding Graybox path
                    debug(" Graybox instance ... ");

                    string grayboxpath = GetGrayboxPath();
                    if (grayboxpath == null)
                    {
                        debug("Graybox not installed");
                        debug("Please verify Graybox installation");
                        return false;
                    }
                    else
                    {
                        debug("Starting Graybox ...");
                        grayboxpath =  grayboxpath + "\\Graybox.exe";
                        Process prcsGrayBox = new Process();
                        prcsGrayBox.StartInfo.FileName = grayboxpath;
                        prcsGrayBox.Start();
                        debug("Please login to graybox and reconnect again. Connection aborted.");
                        return false;
                    }
                    //return false;
                    
                    //MessageBox.Show("Please start Graybox before connecting. Connection aborted. Reconnect again");
                    //return false;
                }

                return true;
        }

        public void DisconnectServer()
        {
            bool bConnect = GrayBoxAPI.BlackBox.instance().DisConnect();


            debug("Graybox disconnected");
        }

        public bool IsConnected()
        {
            return GrayBoxAPI.BlackBox.instance().isConnected();
        }



        void tl_newRegisterSymbols(string client, string symbols)
        {
            ////test();
            //string[] syms = tl.AllClientBasket.ToString().Split(',');
            ////m_Quotes.UnadviseAll(this);
            //for (int i = 0; i < syms.Length; i++)
            //{
            //    if (syms[i].Contains("."))
            //    {
            //        //we can reasonably assume this is an options request
            //        //m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfOptions);
            //    }
            //    else if (syms[i].Contains("/"))
            //    {
            //        //we know (or can at least reasonably assume) this is forex
            //        //advise only level1 bid-ask quotes
            //       // m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelOne);
            //        //m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelTwo);
            //    }
            //    else
            //    {
            //        //probably equity, advise time and sales
            //        //m_Quotes.AdviseSymbol(this, syms[i], ((int)enumQuoteServiceFlags.qsfTimeAndSales));
            //        GetQuote(syms[i]);
                    
            //    }
            //}
            //debug("Symbol Registered  " + syms[syms.Length - 1]);
            // 
           
            // get original basket
            Basket org = BasketImpl.FromString(symquotes);
            // if we had something before, check if something was removed
            if (org.Count > 0)
            {
                Basket rem = BasketImpl.Subtract(org, tl.AllClientBasket);
                foreach (Security s in rem)
                    removesym.Write(s.Symbol);
            }
            symquotes = tl.AllClientBasket.ToString();
            if (VerboseDebugging)
                debug("client subscribe request received: " + symquotes);
            _symsq.Write(true);
            
        }
        Dictionary<string, GBStock> m_StockDict = new Dictionary<string, GBStock>();
        Dictionary<string, GBLevel1> m_Level1Dict = new Dictionary<string, GBLevel1>();
        private void GetQuote(string strSymbol)
        {
            
            //if (null != gbStock)
            //{
            //    try
            //    {
            //        gbStockMgr.ReleaseStock(gbStock);
            //    }
            //    catch(Exception ex)
            //    {
            //    }
            //    gbStock = null;
                
            //}



           
            try
            {
                GBStock l_gbStock;
                l_gbStock = (GBStock)gbStockMgr.GetStock(strSymbol, "");
                m_StockDict.Add(strSymbol, l_gbStock);
                if (l_gbStock == null)
                    return;
                GBLevel1 l_gbLevel = (GBLevel1)l_gbStock.GetLevel1();
                m_Level1Dict.Add(strSymbol, l_gbLevel);
                l_gbLevel.GBEvent += new IGBQuoteEventVtbl_GBEventEventHandler(InsideMktEvent_GBEvent);
                l_gbLevel.SubscribeCallback("Inside Market", 1);
            }
            catch (Exception ex)
            {
                //debug(ex.Message.ToString());
            }
            //Thread.Sleep(10000);
            //          MessageBox.Show("after GetStock");

            

            ;



        }

        void InsideMktEvent_GBEvent(object pISrc, string bstrSymbol, string bstrHint, object pIEventData)
        {
            GBInsideMkt insideMkt = new GBInsideMktClass();
            insideMkt = (GBInsideMkt)pIEventData;

          

            Tick k = new TickImpl(bstrSymbol);
            k.bid = (decimal)insideMkt.BidPrice;
            k.ask = (decimal)insideMkt.AskPrice;
            k.bs = insideMkt.BidVolume / 100;
            k.os = insideMkt.AskVolume / 100;
            k.date = Util.ToTLDate(DateTime.Now);
            k.time = Util.ToTLTime(DateTime.Now); 
           
            tl.newTick(k);
            

        }

        long tl_newSendOrderRequest(Order o)
        {
            if(!m_bGrayBoxRunning)
            {
                debug("Graybox got disconnected");
                return (long)MessageTypes.BROKERSERVER_NOT_FOUND;

            }

            if (o.id == 0) o.id = _idt.AssignId;
            _orderq.Write(o);
            return (long)MessageTypes.OK;

            
        }

        private string GetDestinationID(string strDest)
        {
	      
            if(strDest == "") return "";
            
            string DestId = "";

            switch (strDest)
            {
                case DestinationStrings.STR_DEST_ATTN:
                    DestId = GrayBoxAPI.DestId.ATTN.ToString();
                    break;

                case DestinationStrings.STR_DEST_ARCA:
                    DestId = GrayBoxAPI.DestId.ARCA.ToString();
                    break;
                case DestinationStrings.STR_DEST_BELZ:
                    DestId = GrayBoxAPI.DestId.BELZ.ToString();
                    break;

                case DestinationStrings.STR_DEST_HDOT:
                    DestId = GrayBoxAPI.DestId.HDOT.ToString();
                    break;
                case DestinationStrings.STR_DEST_TRAC:
                    DestId = GrayBoxAPI.DestId.TRAC.ToString();
                    break;
                case DestinationStrings.STR_DEST_INET:
                    DestId = GrayBoxAPI.DestId.INET.ToString();
                    break;
                case DestinationStrings.STR_DEST_BRUT:
                    DestId = GrayBoxAPI.DestId.BRUT.ToString();
                    break;
                case DestinationStrings.STR_DEST_BTRD:
                    DestId = GrayBoxAPI.DestId.BTRD.ToString();
                    break;
                case DestinationStrings.STR_DEST_SOES:
                    DestId = GrayBoxAPI.DestId.SOES.ToString();
                    break;
                case DestinationStrings.STR_DEST_RASH:
                    DestId = 84.ToString(); ;
                    break;
                case DestinationStrings.STR_DEST_BATS:
                    DestId = 87.ToString(); ;
                    break;
                case DestinationStrings.STR_DEST_NYSE:
                    DestId = GrayBoxAPI.DestId.NYSE.ToString();
                    break;
                case DestinationStrings.STR_DEST_EDGA:
                    DestId = 67.ToString(); ;
                    break;
                case DestinationStrings.STR_DEST_EDGX:
                    DestId = 69.ToString(); ;
                    break;
                case DestinationStrings.STR_DEST_CSFB:
                    DestId = 89.ToString(); ;
                    break;
                case DestinationStrings.STR_DEST_XBOS:
                    DestId = 88.ToString();
                    break;
                case DestinationStrings.STR_DEST_MILL:
                    DestId = 77.ToString();
                    break;
                case DestinationStrings.STR_DEST_DTTX:
                    DestId = 90.ToString();
                    break;
                case DestinationStrings.STR_DEST_NSXS:
                    DestId = 72.ToString();
                    break;
                case DestinationStrings.STR_DEST_CBSX:
                    DestId = 71.ToString();
                    break;
                case DestinationStrings.STR_DEST_NQPX:
                    DestId = 50.ToString();
                    break;
                case DestinationStrings.STR_DEST_FLOW:
                    DestId = 51.ToString();
                    break;
                default:
                    DestId = GrayBoxAPI.DestId.NONE.ToString();
                    break;


            }

                    return DestId;

        	
        }

        private long SendOrder(Order o)
        {
            string side = o.side ? GrayBoxAPI.OrderSide.BUY : GrayBoxAPI.OrderSide.SELL;


            string Ordertype = GrayBoxAPI.OrderType.MARKET;
            //debug(String.Format("new order isLimit:{0} isStop:{1} isMarket:{2}", o.isLimit, o.isStop, o.isMarket));
            if (o.isMarket)
            {
                Ordertype = GrayBoxAPI.OrderType.MARKET;

            }
            else if (o.isLimit && o.isStop)
            {
                Ordertype = GrayBoxAPI.OrderType.STOPLIMIT;

            }
            else if (o.isLimit)
            {
                Ordertype = GrayBoxAPI.OrderType.LIMIT;

            }
            else if (o.isStop)
            {
                Ordertype = GrayBoxAPI.OrderType.STOP;

            }
            else if (o.isTrail)
            {
                Ordertype = GrayBoxAPI.OrderType.NOTYPE;

            }
            else
            {
                //strType = "UNKNOWN";
            }
            string route = "";
            if (o.Exchange.Trim() == "XBOS" || o.Exchange.Trim() == "CSFB")
                route = 68.ToString();
            else
                route = 78.ToString();
            

            debug("New order received" + o.id);

            if (o.price == 0)
            {
                debug("Order price is not correct" + o.price);
            }

            if (o.Exchange == "")
            {
                debug("Order Exchange is not correct" + o.Exchange);
            }

            int BBoxID = -1;
            try
            {
                BBoxID = GrayBoxAPI.BlackBox.instance().Order(side.Trim(),
                                             Ordertype.Trim(),
                                               o.symbol.Trim(),
                                               o.UnsignedSize,
                                               o.UnsignedSize,
                                               (double)o.price,
                                               route,
                                               GetDestinationID(o.Exchange.Trim().ToUpper()),
                                                o.TIF,
                                               o.time,
                                               (double)o.stopp);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in order : " + ex.Message, "Graybox TL Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (long)MessageTypes.UNKNOWN_ERROR;
            }
            debug("Order sent to graybox. Order Id : " + BBoxID);


            OrderList.Add(BBoxID, o);
            dictOrderIdBBoxId.Add(o.id, BBoxID);



            return (long)MessageTypes.OK;
        }
        private long CancelOrder(int BBoxId)
        {
            try
            {
                 GrayBoxAPI.BlackBox.instance().CancleOrderUsingBBID(BBoxId);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in canceling order : " + ex.Message, "Graybox TL Server", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return (long)MessageTypes.UNKNOWN_ERROR;
            }
            return (long)MessageTypes.OK;
        }

       

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            [In] IntPtr hProcess,
            [Out] out bool wow64Process
        );

        [DllImport("user32.dll")]
        public static extern int EnumWindows(CallBackPtr callPtr, int lPar);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private static bool is64BitProcess = (IntPtr.Size == 8);
        private static bool is64BitOperatingSystem = is64BitProcess || InternalCheckIsWow64();


        public static bool InternalCheckIsWow64()
        {
            if ((Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process p = Process.GetCurrentProcess())
                {
                    bool retVal;
                    if (!IsWow64Process(p.Handle, out retVal))
                    {
                        return false;
                    }
                    return retVal;
                }
            }
            else
            {
                return false;
            }
        }

        private string strPathKey = "Software\\Online Investment\\Graybox\\Logon";
        private string strPathKey2 = "";
        private string strConnectionKey = "Software\\Online Investment\\Graybox\\Connection";
        private string strPrimaryQuotesKey = "XQuote IP";
        private string strPrimaryBooksKey = "XBook IP";
        private string strSecondaryQuotesKey = "Secondary Quote Server";
        private string strSecondaryBooksKey = "Secondary Book";
        private string strQuotesPort = "XQuote Port";
        private string strBooksPort = "XBook Port";
        private string strLastUserID = "LastUserID";

        RegistryKey baseKey = Registry.CurrentUser;

                   

       
        public static bool Report(int hwnd, int lParam)
        {
            StringBuilder sb = new StringBuilder(1024);

            IntPtr hwndptr = new IntPtr(hwnd);

            GetWindowText(hwndptr, sb, sb.Capacity);
            string window_title = sb.ToString();
            if (window_title.Contains("Graybox"))
            {
                if (window_title.Equals("Graybox TL Server"))
                {
                }
                else
                {
                    m_bGrayBoxRunning = true;
                    return false;
                }

            }
            //Console.WriteLine("Window name is " + window_title);
            return true;
        }

        public string ReadGrayBoxRegValues(string strPath,string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseKey;
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(strPath);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    string strRetval = sk1.GetValue(KeyName.ToUpper()).ToString();
                    return strRetval;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    //ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }

        public string ReadGrayBoxRegValues(RegistryKey baseKey,string strPath, string KeyName)
        {
            // Opening the registry key
            RegistryKey rk = baseKey;

          
            
            // Open a subKey as read-only
            RegistryKey sk1 = rk.OpenSubKey(strPath,false);
            // If the RegistrySubKey doesn't exist -> (null)
            if (sk1 == null)
            {
               
                return null;
            }
            else
            {
                try
                {
                    // If the RegistryKey exists I get its value
                    // or null is returned.
                    string strRetval = sk1.GetValue(KeyName.ToUpper()).ToString();
                    return strRetval;
                }
                catch (Exception e)
                {
                    // AAAAAAAAAAARGH, an error!
                    //ShowErrorMessage(e, "Reading registry " + KeyName.ToUpper());
                    return null;
                }
            }
        }
   

        private string GetServerDetails(string strServerKey,string strPort)
        {
            string strServer = ReadGrayBoxRegValues(strConnectionKey, strServerKey); 
            if(strServer != null)
            {
                string strPortVal = ReadGrayBoxRegValues(strConnectionKey, strPort);
                if (strPortVal != null)
                {
                    strServer = strServer + "," + strPortVal;
                    return strServer;
                }
                else
                    return null;
            }
            else
                return null;
            
        }

        public string[] GetQuotesServer()
        {
            string[] strServers = new string[2];
            strServers[0] = GetServerDetails(strPrimaryQuotesKey, strQuotesPort);
            strServers[1] = GetServerDetails(strSecondaryQuotesKey, strQuotesPort);
            return strServers;
        }

      

        public string[] GetBookServer()
        {
            string[] strServers = new string[2];
            strServers[0] = GetServerDetails(strPrimaryBooksKey, strBooksPort);
            strServers[1] = GetServerDetails(strSecondaryBooksKey, strBooksPort);
            return strServers;
        }

        public string GetLastUserID()
        {
            return ReadGrayBoxRegValues(strPathKey, strLastUserID);
            
        }

        public string GetGrayboxPath()
        {
             if(is64BitOperatingSystem)
                strPathKey2 = "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{1C5646AC-5CD4-42B1-BB49-6AD7522775C8}";
            else
                strPathKey2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{1C5646AC-5CD4-42B1-BB49-6AD7522775C8}";

            string path = ReadGrayBoxRegValues(Registry.LocalMachine, strPathKey2, "InstallLocation");
            return path;
        }


        void debug(string msg)
        {
            if (lst_box.InvokeRequired == true)
            {
                SetCallBack d = new SetCallBack(SetTextinListBox);
                lst_box.Parent.Invoke(d, new Object[] { msg });
            }
            else
            {
                SetTextinListBox(msg);
            }
        }

        private void SetTextinListBox(string msg)
        {
            lst_box.Items.Add(msg_id.ToString() + "  " + msg);
               msg_id++;
        }

    }
      
}
