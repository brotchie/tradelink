using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;
using daslibrary;

namespace ServerDAS
{
    public partial class DASServerMain : AppTracker
    {
        
         private bool LOGGEDIN;
       // AmeritradeBrokerAPI api = new AmeritradeBrokerAPI();
        const string APIVER = "1";
        TLServer tl = new TLServer_WM();
        public const string PROGRAM = "ServerDAS BETA";
        Log _log = new Log(PROGRAM);
        bool ok { get { 
            return true; 
            //api.LoggedIn; 
        } }
        Dictionary<long, long> idmap = new Dictionary<long, long>();

        private SocketOrderServer socketOrderServer;
        private SocketQuoteServer socketQuoteServer;
        private int traderid;

        public static DebugWindow _dw = new DebugWindow();

        private List<long> ls = new List<long>();

        int accid = 0;

        void debug(string msg)
        {
            _log.GotDebug(msg);
            _dw.GotDebug(msg);

        }
        
        public DASServerMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
           
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(DASServerMain_FormClosing);
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tl = new TradeLink.Common.TLServer_WM();
            else
                tl = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);


            // bindings
            tl.newProviderName = Providers.DAS;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegateStatus(tl_gotSrvFillRequest);
            //tl.newAcctRequest += new StringDelegate(tl_gotSrvAcctRequest);
            tl.newOrderCancelRequest += new LongDelegate(tl_newOrderCancelRequest);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            tl.newRegisterSymbols +=new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newPosList += new PositionArrayDelegate(tl_newPosList);

            debug(Util.TLSIdentity());
            debug("Attempting to start: " + PROGRAM);

            //api.OnStatusChange += new Axtdaactx.ITDAAPICommEvents_OnStatusChangeEventHandler(api_OnStatusChange);
            ////api.OnL1Quote +=  rs_LevelOneStreaming = new AmeritradeBrokerAPI.RequestState();
            ////api.rs_LevelOneStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_LevelOneStreaming_TickWithArgs);
            ////api.rs_ActivesStreaming = new AmeritradeBrokerAPI.RequestState();
            ////api.rs_ActivesStreaming.TickWithArgs += new AmeritradeBrokerAPI.EventHandlerWithArgs(rs_ActivesStreaming_TickWithArgs);
            //api.OnL1Quote += new Axtdaactx.ITDAAPICommEvents_OnL1QuoteEventHandler(api_LevelOneStreaming);
            //doLogin();
        }

        void tl_newRegisterSymbols(string client, string symbols)
        {
            
            debug("got symbol request: " + client + " for: " + symbols);
            Basket mb = tl.AllClientBasket;
            try
            {
                socketQuoteServer.WLSTRemoveWatchListonL1();
            }
            catch (Exception ex)
            {
                debug("DAS exception on removing watch list for symbols: " + symbols+ " err: " + ex.Message + ex.StackTrace);
            }
           
            //Close_Connections(false);
            foreach (Security s in mb)
            {
                try
                {
                    socketQuoteServer.WLSTAddWatch(s.Symbol);
                    socketQuoteServer.WLSTAddWatch(s.Symbol, 2);
                    v(s.Symbol + " watching l1, l2 data.");
                }
                catch (Exception ex)
                {
                    debug("DAS exception on registering symbols: " + s.Symbol + " err: " + ex.Message + ex.StackTrace);
                }
          
            }
            

        }

        Position[] tl_newPosList(string account)
        {
            /*
            AmeritradeBrokerAPI.ATradeArgument brokerAcctPosArgs = new AmeritradeBrokerAPI.ATradeArgument();
            brokerAcctPosArgs.oPositions = new List<AmeritradeBrokerAPI.Positions>();
            //api.TD_getAcctBalancesAndPositions(_user.Text, _pass.Text, AmeritradeBrokerAPI.SOURCEID, APIVER, ref brokerAcctPosArgs.oCashBalances, ref brokerAcctPosArgs.oPositions);
            Position[] plist = new Position[brokerAcctPosArgs.oPositions.Count];
            int count = 0;
            foreach (AmeritradeBrokerAPI.Positions oPosition in brokerAcctPosArgs.oPositions)
            {
                decimal price = 0;
                decimal.TryParse(oPosition.AveragePric, out price);
                int size = 0;
                int.TryParse(oPosition.Quantity, out size);
                Position p = new PositionImpl(oPosition.StockSymbol, price, size);
                plist[count++] = p;
            }
            return plist;
             */
            int count = 0;
            List<itemPosition> litem = new List<itemPosition>();
            try
            {
                litem = socketOrderServer.sitemPosition.Finditems(accid);
            }
            catch (Exception ex)
            {
                debug("DAS connector error on requesting positions: " + ex.Message + ex.StackTrace);
            }
            if (litem.Count == 0) return null;
            Position[] plist = new Position[litem.Count];
            foreach (itemPosition itemp in litem)
            {
                
                Position p = new PositionImpl(itemp.msecsym, Convert.ToDecimal(itemp.mavgcost),Convert.ToInt32(itemp.mqty) );
                plist[count++] = p;

            }
            return plist;


        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            //switch (t)
            //{
            //  case MessageTypes.ISSHORTABLE:
            //       return api.TD_IsShortable(msg) ? 1 : 0;
            //}
            return (long)MessageTypes.FEATURE_NOT_IMPLEMENTED;
        }

        void tl_newOrderCancelRequest(long number)
        {

            int orderid = Convert.ToInt32(number);

            
            
                if (orderid == 0)
                {
                    v("ignoring cancel for invalid id: 0");
                    return ;
                }


                if (!socketOrderServer.sitemOrder.sordermap.ContainsKey(Convert.ToInt32(number)))
                {
                    v("ignoring cancel for unknown order: "+number);
                    return ;
                }

                itemOrder porder = socketOrderServer.sitemOrder.sordermap[orderid];

                if (!porder.IsOpenOrder()) 
                {
                    v("ignoring cancel for closed order, tlid: "+number+" sym: "+porder.msecsym+" ord: "+porder.ToString());
                    return ;
                }

                if (porder.mlvsqty > 0)
                {
                    try
                    {
                        iManager im = new iManager(socketOrderServer);
                        im.Send_CancelOrder(porder, 0);
                        v("requested cancel for: " + number + " sym: " + porder.msecsym + " ord: " + porder.ToString());
                    }
                    catch (Exception ex)
                    {
                        debug("DAS error canceling order: " + number + " err: " + ex.Message + ex.StackTrace);
                    }

                    tl.newCancel(number);

                }
                else
                    v("ignoring cancel for order with zero size, tlid: " + number + " sym: " + porder.msecsym + " ord: " + porder.ToString());

               
                
        }

        void DASServerMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            debug("stopping: " + PROGRAM);
            _log.Stop();
            Properties.Settings.Default.Save();
            //if (ok)
              // api.Logout();
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);       
            f.Add(MessageTypes.ISSHORTABLE);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.LIVEDATA);
            

            return f.ToArray();

        }

        long tl_gotSrvFillRequest(Order o)

        {
            //if (!ok)
            //{
            //    debug("not logged in.");
            //    return (long)MessageTypes.SYMBOL_NOT_LOADED;
            //}
           // string action = o.side ? "buy" : "sell";
            string otype = o.isLimit ? "limit" : "market";


            if (o.id == 0)
                o.id = OrderImpl.Unique;

            //if (o.ex.ToUpper().Contains("ARCA"))
            //    route = "ecn_arca";
            //else if (o.ex.ToUpper().Contains("INET"))
            //    route = "inet";

         



            itemOrder newOrder = new itemOrder();

            newOrder.mtrid = traderid;
            newOrder.msecsym = o.symbol.ToUpper();
            newOrder.mstatus = 0;
            newOrder.morderid = 0;

            newOrder.mexchange = Convert.ToByte(o.ex);

            if (o.side)
            {
                newOrder.mstatus |= 1 << 6;
            }
            else
            {
                int ty = 1;
            }

            if (o.isMarket)
            {
                newOrder.mstatus |= 1 << 9;
            }
            

            if (o.isLimit)
            {
                newOrder.mprice = Convert.ToDouble(o.price);
            }

            if (o.isStop)
            {
                newOrder.mstatus |= 1 << 10;               
                newOrder.mstopprice = Convert.ToDouble(o.stopp);
            }


            int err = 0;

            try
            {
                // itemAccount itemA = socketOrderServer.sitemAccount.FindItemByName(Session["acct_num"].ToString());
                
                foreach (int i in socketOrderServer.sitemAcct_Access.GetAccountIDs(traderid))
                {
                    accid = i;
                    //break;
                }

                itemAccount itemA = socketOrderServer.sitemAccount.FindItem(accid);
                newOrder.maccid = itemA.mitemifo.maccid;
                newOrder.mbrid = itemA.mitemifo.mbrid;
                newOrder.mrrno = itemA.mitemifo.mrrno;



                newOrder.mtmforce = 65535;  //Day Order

                newOrder.mroute = "AUTO"; ;
                newOrder.mqty = Math.Abs(o.size);

                string errMsg = "";
                long morig = 0; 

                

                err = itemOrder.LSendOrder(newOrder, ref errMsg, true, socketOrderServer, ref morig);
                if ((err != 0) || ((errMsg != null) && (errMsg != string.Empty)))
                {
                    debug("error occured sending order: " + o.ToString() + " err: " + Util.PrettyError(Providers.DAS, err));
                }
                else
                    v(o.symbol + " successfully sent: " + o.ToString() + " morig: " + morig);
                ls.Add(morig);

            }

            catch (Exception ex)
            {
                debug("DAS exception occured sending order: "+o.ToString()+" err: "+ex.Message+ex.StackTrace);
                return (long)MessageTypes.INVALID_ORDERSIZE;
            }
           
            return (long)err;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            
            string user_id = textBoxID.Text.Trim().ToUpper();

            try
            {
                socketOrderServer = new SocketOrderServer();
                socketOrderServer.eventPool.Subscribe("OrderNotify", new EventHandler<OrderArgs>(OrderNotifyHandler));       
                socketOrderServer.eventPool.Subscribe("CancelResponse", new EventHandler<OrderArgs>(CancelResponseHandler));
                socketOrderServer.eventPool.Subscribe("Trade", new EventHandler<TradeArgs>(TradeHandler));

                // socketOrderServer.eventPool.Subscribe("ExecuteNotify", new EventHandler<OrderArgs>(ExecuteNotifyHandler));
              //  socketOrderServer.eventPool.Subscribe("Order", new EventHandler<OrderArgs>(OrderHandler));
              //  socketOrderServer.eventPool.Subscribe("OrderModify", new EventHandler<OrderArgs>(OrderModifyHandler));
                socketOrderServer.Connect(Properties.Settings.Default.OrderserverAddress, Properties.Settings.Default.OrderserverPort);
                socketOrderServer.PkgLongin(user_id, textBoxPwd.Text.Trim(), Properties.Settings.Default.OrderserverAddress);

                int timeout = 0;
                while (timeout <= 90000)  // wait one minute
                {
                    timeout += 1000;
                    System.Threading.Thread.Sleep(1000);
                    if (socketOrderServer.m_clientSocket != null) break;
                }

                itemTrader itemtrader = socketOrderServer.sitemTrader.FindItemByName(user_id);
                traderid = itemtrader.mtrid;

                socketQuoteServer = new SocketQuoteServer(socketOrderServer);
                socketQuoteServer.eventPool.Subscribe("Lv1", new EventHandler<Lv1Args>(Lv1Handler));
                
                socketQuoteServer.Connect(Properties.Settings.Default.QuoteserverAddress, Properties.Settings.Default.QuoteserverPort);
                socketQuoteServer.PkgLogin(user_id, textBoxPwd.Text.Trim(), Properties.Settings.Default.QuoteserverAddress);
                labelMessage.Text = "login successfully.";
                debug("successful login: " + user_id);
            }
            catch (Exception e1)
            {
                debug("DAS connector login exception: " + e1.Message + e1.StackTrace);
                labelMessage.Text = " Login fail." + e1.Message;
            }

        }

        private void CancelResponseHandler(object sender, OrderArgs e)
        {
            
            tl.newCancel(e.ItemOrder.morderid);
            v(e.ItemOrder.msecsym+" received cancel ack for: " + e.ItemOrder.morderid);

        }


        private void TradeHandler(object sender, TradeArgs e)
        {


            itemTrade itrade = e.ItemTrade;




           // Order o = new OrderImpl(iorder.msecsym, iorder.IsBuyOrder(), iorder.mqty, Convert.ToDecimal(iorder.mprice), Convert.ToDecimal(iorder.mstopprice), "", iorder.mc_date, iorder.mc_date, iorder.morderid);
            Trade trade = new TradeImpl();
            trade.symbol = itrade.msecsym;
            itemOrder lorder =socketOrderServer.sitemOrder.FindItem(itrade.morderid);
            if (lorder== null) return; 
              trade.side = lorder.IsBuyOrder();
            trade.xprice = Convert.ToDecimal(itrade.mprice);
            trade.xsize = itrade.mqty;
            DateTime mdate = ComFucs.GetDate(itrade.mm_date);
            trade.Account = "";
            trade.xdate = mdate.Day + mdate.Month * 100 + mdate.Year * 10000;
            trade.xtime = mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000;
            tl.newFill(trade);
            v(trade.symbol+" received fill ack for: " + trade.ToString());
        }

        private void OrderNotifyHandler(object sender, OrderArgs e)
        {
            itemOrder iorder = e.ItemOrder;
            // if (!ls.Contains(iorder.morigtkn)) return;
            DateTime mdate = ComFucs.GetDate(iorder.mm_date);
            Order o = new OrderImpl(iorder.msecsym, iorder.IsBuyOrder(), iorder.mqty, Convert.ToDecimal(iorder.mprice), Convert.ToDecimal(iorder.mstopprice), "",
                        mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000, mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000, iorder.morderid);

            tl.newOrder(o);
            v(o.symbol + " received order ack for: " + o.ToString());
        }

        /*
        private void ExecuteNotifyHandler(object sender, OrderArgs e)
        {


            itemOrder iorder = e.ItemOrder;

           

            
            Order o = new OrderImpl(iorder.msecsym, iorder.IsBuyOrder(), iorder.mqty, Convert.ToDecimal(iorder.mprice), Convert.ToDecimal(iorder.mstopprice), "", iorder.mc_date, iorder.mc_date, iorder.morderid);
            Trade trade = new TradeImpl();
            trade.symbol = iorder.msecsym;
            trade.side = iorder.IsBuyOrder();
            trade.xprice = Convert.ToDecimal(iorder.mprice);
            trade.xsize = iorder.mqty;
            DateTime mdate = ComFucs.GetDate(iorder.mm_date);

            trade.xdate = mdate.Day+ mdate.Month*100+mdate.Year*10000;
            trade.xtime = mdate.Second + mdate.Minute*100+ mdate.Hour*10000 ;
            tl.newFill(trade);
        }

       

        private void OrderHandler(object sender, OrderArgs e)
        {
            

                itemOrder iorder = e.ItemOrder;
                // if (!ls.Contains(iorder.morigtkn)) return;
                DateTime mdate = ComFucs.GetDate(iorder.mm_date);
                Order o = new OrderImpl(iorder.msecsym, iorder.IsBuyOrder(), iorder.mqty, Convert.ToDecimal(iorder.mprice), Convert.ToDecimal(iorder.mstopprice), "",
                            mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000, mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000, iorder.morderid);

                tl.newOrder(o);
            


        }


        private void OrderModifyHandler(object sender, OrderArgs e)
        {
            
                if ((e.ItemOrder.mstatus | 0x0004) != 0)
                {
                  //  tl.newCancel(e.ItemOrder.morderid);
                }
                if ((e.ItemOrder.mstatus | 0x0002) != 0)
                {
                    itemOrder iorder = e.ItemOrder;
                    Order o = new OrderImpl(iorder.msecsym, iorder.IsBuyOrder(), iorder.mqty, Convert.ToDecimal(iorder.mprice), Convert.ToDecimal(iorder.mstopprice), "", iorder.mc_date, iorder.mc_date, iorder.morderid);
                    Trade trade = new TradeImpl();
                    trade.symbol = iorder.msecsym;
                    trade.side = iorder.IsBuyOrder();
                    trade.xprice = Convert.ToDecimal(iorder.mprice);
                    trade.xsize = iorder.mqty;
                    DateTime mdate = ComFucs.GetDate(iorder.mm_date);
                    trade.xdate = mdate.Day + mdate.Month * 100 + mdate.Year * 10000;
                    trade.xtime = mdate.Second + mdate.Minute * 100 + mdate.Hour * 10000;
                    tl.newFill(trade);
                }
            


        }
         * */


        private void Lv1Handler(object sender, Lv1Args e)
        {
           
            DateTime DT = new DateTime(1970, 1, 1);
            
            Tick t = new TickImpl();
            t.date = Util.ToTLDate(DateTime.Now);
            t.time = Util.DT2FT(DateTime.Now);
            t.symbol = e.TheIssuIfo.secsym;
            t.bid = Convert.ToDecimal(e.TheIssuIfo.l1_BidPrice);
            t.ask = Convert.ToDecimal(e.TheIssuIfo.l1_AskPrice);
            t.ex = e.TheIssuIfo.PrimExch.ToString();
            t.trade = Convert.ToDecimal(e.TheIssuIfo.l1_lastPrice);
            t.size = e.TheIssuIfo.l1_volume;
            t.bs = e.TheIssuIfo.l1_BidSize;
            t.os = e.TheIssuIfo.l1_AskSize;
            t.ex = e.TheIssuIfo.PrimExch.ToString();
           
            tl.newTick(t);

        }

        bool _noverb = true;

        void v(string msg)
        {
            if (_noverb)
                return;
            debug(msg);
        }

        private void _verbose_CheckedChanged(object sender, EventArgs e)
        {
            string notmsg = _verbose.Checked ? "Verbose: on" : "Verbose: disabled.";
            _verbose.Text = notmsg;
            _verbose.Invalidate();
            _noverb = !_verbose.Checked;
            debug(notmsg);

        }

        private void toginfo_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

    }
}
