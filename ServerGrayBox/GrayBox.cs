using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;
using GBQUOTESLib;
using System.Runtime.InteropServices;
using Microsoft.Win32; 

public delegate bool CallBackPtr(int hwnd, int lParam);






namespace TradeLinkTest
{
    class GrayBox
    {
        TLServer tl;
        ListBox lst_box;
        long msg_id = 1;
        private CallBackPtr callBackPtr;
        static bool m_bGrayBoxRunning = false;

        private string strPathKey = "Software\\Online Investment\\Graybox\\Logon";
        private string strConnectionKey = "Software\\Online Investment\\Graybox\\Connection";
        private string strPrimaryQuotesKey = "XQuote IP";
        private string strPrimaryBooksKey = "XBook IP";
        private string strSecondaryQuotesKey = "Secondary Quote Server";
        private string strSecondaryBooksKey = "Secondary Book";
        private string strQuotesPort = "XQuote Port";
        private string strBooksPort = "XBook Port";
        private string strLastUserID = "LastUserID";



        private GBQUOTESLib.GBStockManager gbStockMgr = null;
        private GBStock gbStock = null;
        private GBLevel1 gbLevel = null;
        public DebugWindow dbg_cntrl = new DebugWindow();
        PositionTracker pt = new PositionTracker();
       



        Dictionary<int,Order> OrderList = new Dictionary<int,Order>();

        RegistryKey baseKey = Registry.CurrentUser;

        [DllImport("user32.dll")]
        public static extern int EnumWindows(CallBackPtr callPtr, int lPar);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

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
                    return sk1.GetValue(KeyName.ToUpper()).ToString();
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

            return ReadGrayBoxRegValues(strPathKey, "LayoutFile");
        }

      

       

        public GrayBox(TLServer tls, ListBox lstbox)
        {
            tl = tls;
            lst_box = lstbox;
            

            // tradelink bindings
            tl.newProviderName = Providers.GrayBox;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_newSendOrderRequest);
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);

            //debug("Trdelink APIs Connected");
           

            GrayBoxAPI.BlackBox.instance().GrayBoxOrderConfirmEventMy += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderConfirmEventMy);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderExecuteEventMy += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderExecuteEventMy);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelConfirmEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelConfirmEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelErrorEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelErrorEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderCancelExecEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderCancelExecEvent);
            GrayBoxAPI.BlackBox.instance().GrayBoxOrderErrorEvent += new GrayBoxAPI.BBEventHandler(BlackBox_GrayBoxOrderErrorEvent);
            debug("Graybox TL Server Initialized");
            debug("Click 'Connect' to make connection with Graybox");

            //ConnectQuotesServer();        

            
        }

        void debug(string msg)
        {
            lst_box.Items.Add(msg_id.ToString() + "  " + msg);
            msg_id++;
        }

        void Start()
        {
        }

        void Stop()
        {
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
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);

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
                    debug("Connected to Graybox");
                else
                {
                    debug("Not connected to Graybox");
                    return false;
                }

                return true;
            }
            catch(Exception ex)
            {
                debug("Graybox connection error: " + ex.Message);
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

                    string grayboxpath = ReadGrayBoxRegValues(strPathKey,"LayoutFile");
                    if (grayboxpath == null)
                    {
                        debug("Graybox not installed");
                        debug("Please verify Graybox installation");
                        return false;
                    }
                    else
                    {
                        debug("Starting Graybox ...");
                        grayboxpath =  grayboxpath.Replace("defaultlayout.xml", "Graybox.exe");
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
            //test();
            string[] syms = tl.AllClientBasket.ToString().Split(',');
            //m_Quotes.UnadviseAll(this);
            for (int i = 0; i < syms.Length; i++)
            {
                if (syms[i].Contains("."))
                {
                    //we can reasonably assume this is an options request
                    //m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfOptions);
                }
                else if (syms[i].Contains("/"))
                {
                    //we know (or can at least reasonably assume) this is forex
                    //advise only level1 bid-ask quotes
                   // m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelOne);
                    //m_Quotes.AdviseSymbol(this, syms[i], (int)enumQuoteServiceFlags.qsfLevelTwo);
                }
                else
                {
                    //probably equity, advise time and sales
                    //m_Quotes.AdviseSymbol(this, syms[i], ((int)enumQuoteServiceFlags.qsfTimeAndSales));
                    GtQuote(syms[i]);
                    
                }
            }
            debug("Symbol Registered  " + syms[syms.Length - 1]);
            
        }

        private void GtQuote(string strSymbol)
        {
            
            if (null != gbStock)
            {
                try
                {
                    gbStockMgr.ReleaseStock(gbStock);
                }
                catch(Exception ex)
                {
                }
                gbStock = null;
                
            }

            

           
            try
            {

                gbStock = (GBStock)gbStockMgr.GetStock(strSymbol, "");
            }
            catch (Exception ex)
            {
                //debug(ex.Message.ToString());
            }
            //Thread.Sleep(10000);
            //          MessageBox.Show("after GetStock");

            if (gbStock == null)
                return;

            gbLevel = (GBLevel1)gbStock.GetLevel1();
            gbLevel.GBEvent += new IGBQuoteEventVtbl_GBEventEventHandler(InsideMktEvent_GBEvent);
            gbLevel.SubscribeCallback("Inside Market", 1);



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
            string  side = o.side ? GrayBoxAPI.OrderSide.BUY : GrayBoxAPI.OrderSide.SELL;
           
            
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
            
            string route = "None";

            debug("New order received" + o.id);

            if (o.price == 0)
            {
                debug("Order price is not correct" + o.price);
            }

            if (o.Exchange == "")
            {
                debug("Order Exchange is not correct" + o.Exchange);
            }

            int BBoxID = GrayBoxAPI.BlackBox.instance().Order(side.Trim(),
                                         Ordertype.Trim(),
                                           o.symbol.Trim(),
                                           o.UnsignedSize,
                                           o.UnsignedSize,
                                           (double)o.price,
                                           route,
                                           o.Exchange.Trim(),
                                            o.TIF,
                                           o.time,
                                           (double)o.stopp);

             debug("Order sent to graybox. Order Id : " + BBoxID);
           

            OrderList.Add(BBoxID, o);


            
            return (long)MessageTypes.OK;
        }
    }
}
