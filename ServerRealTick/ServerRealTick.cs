using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalTrade.Toolkit;
using TalTrade.Toolkit.Domain;
using TalTrade.Toolkit.Domain.Livequote;
using TalTrade.Toolkit.Domain.Order;
using TalTrade.Toolkit.Domain.Position;
using TalTrade.Toolkit.Domain.Ticks;
using TalTrade.Toolkit.Domain.XPerms;
using TradeLink.API;
using TradeLink.Common;
using System.Threading;
using TalTrade.Toolkit.ClientAdapter;
using TalTrade.Toolkit;

namespace RealTickConnector
{
    public class ServerRealTick : TLServer_WM
    {
        private ToolkitApp _app;

        OrderCache _oc;
        PositionTable PST;
        private XPermsAccountsTable XPAT;
        private OrderTable ORD;
        private TicksTable TT;
        private LiveQuoteTable TBL;
        List<string> accts = new List<string>();
        const int MAXRECORD = 5000;
        RingBuffer<Order> _neworders = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<long> _newcancel = new RingBuffer<long>(MAXRECORD);
        RingBuffer<bool> _newsyms = new RingBuffer<bool>(5);

        string _newsymlist = string.Empty;
        System.ComponentModel.BackgroundWorker _bw;
        int _SLEEP = 50;
        bool _bwgo = true;

        PositionTracker pt = new PositionTracker();

        Dictionary<long, OrderRecord> _orSet = new Dictionary<long, OrderRecord>();

        bool _conn = false;
        public bool isConnected { get { return _conn; } }

        public ServerRealTick()
        {

            
            newProviderName = Providers.RealTick;
            _bw = new System.ComponentModel.BackgroundWorker();
            _bw.DoWork += new System.ComponentModel.DoWorkEventHandler(_bw_DoWork);
            newRegisterSymbols += new SymbolRegisterDel(ServerRealTick_newRegisterSymbols);
            newSendOrderRequest += new OrderDelegateStatus(ServerRealTick_newSendOrderRequest);
            newOrderCancelRequest += new LongDelegate(ServerRealTick_newOrderCancelRequest);
            newFeatureRequest += new MessageArrayDelegate(ServerRealTick_newFeatureRequest);
            newPosList += new PositionArrayDelegate(ServerRealTick_gotSrvPosList);
            newAcctRequest += new StringDelegate(ServerRealTick_newAcctRequest);
        }

        void _bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            while (_bwgo)
            {
                bool newsym = false;
                while (!_newsyms.isEmpty)
                {
                    _newsyms.Read();
                    newsym = true;
                }
                if (newsym)
                {
                    // get symbols
                    _numDisplayed = 0;
                    _numIgnored = 0;
                    debug("Subscribe request: " + _newsymlist);
                    if (!isConnected)
                    {
                        debug("not connected.");
                        return;
                    }

                    Basket b = BasketImpl.FromString(_newsymlist);
                    foreach (Security s in b)
                    {
                        try
                        {
                            TBL.WantData(TBL.TqlForBidAskTrade(s.Symbol, null), true, true);
                            TBL.Start();
                            //TT.WantData(TBL.TqlForBidAskTrade(s.Symbol, null), true, true);
                            //TT.Start();
                            _done.WaitOne(30000);
                        }
                        catch (Exception ex)
                        {
                            debug(s.Symbol + " error subscribing: " + ex.Message + ex.StackTrace);
                        }
                    }
                    debug(string.Format("DISPLAYED {0} TRADES AND IGNORED {1} QUOTES", _numDisplayed, _numIgnored));
                    debug("DONE.");
                    debug("registered: " + _newsymlist);
                }
                while (!_neworders.isEmpty)
                {
                    Order o = _neworders.Read();
                    var ob = new OrderBuilder(_oc);
                    ob.SetAccount(null, o.Account, null, null);
                    ob.SetBuySell(o.side ? OrderBuilder.BuySell.BUY : OrderBuilder.BuySell.SELL);
                    ob.SetExpiration(OrderBuilder.Expiration.DAY);
                    ob.SetRoute("NYS");
                    ob.SetSymbol(o.symbol, o.Exchange, OrderBuilder.SecurityType.STOCK);
                    if (o.size != 0)
                        ob.SetVolume(Math.Abs(o.size));
                    if (o.isMarket)
                    {
                        ob.SetPriceMarket();
                    }
                    if (o.isLimit)
                    {
                        ob.SetPriceLimit(new Price(o.price.ToString()));
                    }
                    if (o.isStop)
                    {
                        ob.SetPriceLimit(new Price(o.price.ToString()));
                    }
                    // ob.SetGoodFrom(DateTime.Now.AddMinutes(60));
                    currentOrderId = o.id;
                    state = OrderState.OrderPending;
                    _oc.SubmitOrder(ob);
                }
                while (!_newcancel.isEmpty)
                {
                    long id = _newcancel.Read();
                    if (_orSet.ContainsKey(id))
                    {
                        var cxl = new CancelBuilder(_orSet[id]);
                        try
                        {
                            _oc.SubmitCancel(cxl);
                        }
                        catch (Exception ee)
                        {
                            debug("ORDER CANCEL FAILED:" + ee.Message);
                        }

                        state = OrderState.CancelPending;
                        debug("ORDER CANCEL FINISHED");

                    }
                    else
                    {
                        debug("ORDER CANCEL FAILED, NOT EXIST THE Order.Id:" + id);
                    }

                }
                if (_newcancel.isEmpty && _neworders.isEmpty && _newsyms.isEmpty)
                    Thread.Sleep(_SLEEP);
            }
        }

        void ServerRealTick_newRegisterSymbols(string client, string symbols)
        {
            debug("Subscribe request: " + symbols);
            if (!isConnected)
            {
                debug("not connected.");
                return;
            }
            // save list of symbols to subscribe
            _newsymlist = AllClientBasket.ToString();
            // notify other thread to subscribe to them
            _newsyms.Write(true);
        }





        private void initTables()
        {
            _oc = new OrderCache(_app);
            _oc.OnOrder += _oc_OnOrder;
            _oc.Start();
            state = OrderState.ConnectionPending;

            PST = new PositionTable(_app);
            PST.WantData(PST.TqlForPositions(), true, false);
            PST.OnPosition += PST_OnPosition;
            PST.Start();

            XPAT = new XPermsAccountsTable(_app);
            XPAT.OnXPermsAccounts += XPAT_OnXPermsAccounts;
            XPAT.WantData(XPAT.TqlForXPermsAccounts(), true, false);
            XPAT.Start();

            TBL = new LiveQuoteTable(_app);
            TBL.OnData += TBL_OnData;

            _bw.RunWorkerAsync();

            _conn = true;
        }

        void TBL_OnData(object sender, TalTrade.Toolkit.Domain.DataEventArgs<LivequoteRecord> e)
        {
            foreach (var r in e)
            {
                try
                {
                    Tick t = new TickImpl(r.DispName);
                    if (r.Ask!=null)
                    {
                        t.ask =r.Ask.Value.DecimalValue;
                        t.os = r.Asksize.Value;
                        t.oe = r.Askexid;
                    }
                    if (r.Bid!=null)
                    {
                        t.bid = r.Bid.Value.DecimalValue;
                        t.bs = r.Bidsize.Value;
                        t.be = r.Bidexid;
                    }
                    if (r.Trdprc1!=null)
                    {
                        t.ex = r.Exchid;
                        t.trade = r.Trdprc1.Value.DecimalValue;
                        t.size = r.Trdvol1.Value;
                    }
                     t.time = Util.DT2FT(r.TrdDate.Value + r.Trdtim1.Value);
                    t.date =Util.ToTLDate(r.TrdDate.Value);
                    newTick(t);
                    _numDisplayed++;
                    _done.Set();
                }
                catch (Exception exc)
                {
                    debug(exc.Message);
                    _numIgnored++;
                }
            }
        }


        void XPAT_OnXPermsAccounts(object sender, TalTrade.Toolkit.Domain.DataEventArgs<XPermsAccountsRecord> e)
        {
            foreach (var r in e)
            {
                //var acc = string.Format("{0}.{1}.{2}.{3}", r.Bank, r.Branch, r.Customer, r.Deposit);
                if (!accts.Exists(t => t == r.Deposit))
                {
                    accts.Add(r.Deposit);
                }
            }
        }

        public event DebugDelegate SendDebug;
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }

        void PST_OnPosition(object sender, TalTrade.Toolkit.Domain.DataEventArgs<PositionRecord> e)
        {
            if (e.Count == 0)
            {
                debug("GOT NO POSITIONS FOR TODAY");
            }
            else
            {
                debug("GOT TODAY'S POSITIONS");
                foreach (PositionRecord pos in e)
                {
                    var p = new PositionImpl(pos.DispName, decimal.Parse(pos.AverageLong.Value.ToString()), Convert.ToInt32(pos.Longpos.GetValueOrDefault()));
                    pt.NewPosition(p);

                    //debug("  --> {0}.{1}.{2}.{3}",
                    //    pos.Bank,
                    //    pos.Branch,
                    //    pos.Customer,
                    //    pos.Deposit);

                    //debug("      Symbol {0}, AcctType {1}",
                    //    pos.DispName, pos.AcctType);
                    //debug("      Intraday Long = {0}@{1}",
                    //    pos.Longpos, pos.AverageLong);
                    //debug("      Intraday Short = {0}@{1}",
                    //    pos.Shortpos, pos.AverageShort);
                }
            }
        }

        private string ServerRealTick_newAcctRequest()
        {
            var r = String.Empty;
            if (accts.Count > 0)
            {
                r = string.Join(",", accts.ToArray());
            }
            return r;
        }

        private Position[] ServerRealTick_gotSrvPosList(string account)
        {
            return pt.ToArray();
        }

        private MessageTypes[] ServerRealTick_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.OK);
            f.Add(MessageTypes.BROKERNAME);
            f.Add(MessageTypes.CLEARCLIENT);
            f.Add(MessageTypes.CLEARSTOCKS);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.HEARTBEATREQUEST);
            f.Add(MessageTypes.HEARTBEATRESPONSE);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.VERSION);
            f.Add(MessageTypes.IMBALANCEREQUEST);
            f.Add(MessageTypes.IMBALANCERESPONSE);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.SENDORDERPEGMIDPOINT);
            return f.ToArray();
        }

        public void ServerRealTick_newOrderCancelRequest(long val)
        {
            _newcancel.Write(val);
        }

        IdTracker _idt = new IdTracker();
        public long currentOrderId;
        public long ServerRealTick_newSendOrderRequest(Order o)
        {
            if (o.id == 0) o.id = _idt.AssignId;
            _neworders.Write(o);


            return (long)MessageTypes.OK;
        }
        enum OrderState { ConnectionPending, OrderPending, CancelPending, OrderFinished, ConnectionDead };
        private static OrderState state = OrderState.ConnectionDead;
        void _oc_OnOrder(object sender, DataEventArgs<OrderRecord> e)
        {
            if (isConnected)
            {
                if (state != OrderState.ConnectionDead)
                {
                    foreach (var o in e)
                    {
                        if (o.Type == "UserSubmitOrder" && state == OrderState.OrderPending)
                        {
                            if (o.CurrentStatus == "COMPLETED")
                            {
                                var order = new OrderImpl();
                                order.id = currentOrderId;
                                long now = Util.ToTLDate(DateTime.Now);
                                int xsec = (int)(now % 100);
                                long rem = (now - xsec) / 100;
                                order.time = ((int)(rem % 10000)) * 100 + xsec;
                                order.date = (int)((rem - order.time) / 10000);
                                order.symbol = o.DispName;
                                order.side = o.Buyorsell == "BUY" ? true : false;
                                order.size = o.Volume.Value;
                                order.price = o.Price.Value.DecimalValue;
                                order.stopp = o.StopPrice.Value.DecimalValue;
                                order.Account = o.Deposit;
                                order.ex = o.Exchange;
                                newOrder(order);
                                state = OrderState.OrderFinished;
                                _orSet[currentOrderId] = o;
                            }
                        }
                        if (o.Type == "ExchangeTradeOrder")
                        {
                            Trade f = new TradeImpl();
                            f.symbol = o.DispName;
                            f.Account = o.TraderId;

                            foreach (var order in _orSet)
                            {
                                if (order.Value.OrderTag == o.OrderTag)
                                {
                                    f.id = order.Key;
                                }
                            }
                            f.xprice = o.Price.Value.DecimalValue;
                            f.xsize = o.Volume ?? 0;
                            f.ex = o.Exchange;
                            if (o.Buyorsell == "BUY")
                                f.side = true;
                            if (o.Buyorsell == "SALE")
                                f.side = false;
                            long now = Util.ToTLDate(DateTime.Now);
                            int xsec = (int)(now % 100);
                            long rem = (now - xsec) / 100;
                            f.xtime = ((int)(rem % 10000)) * 100 + xsec;
                            f.xdate = (int)((now - f.xtime) / 1000000);
                            PST.Start();
                            pt.Adjust(f);
                            newFill(f);
                            state = OrderState.OrderFinished;
                        }

                        if (o.Type == "UserSubmitCancel" && state == OrderState.CancelPending)
                        {
                            if (o.CurrentStatus == "DELETED")
                            {
                                foreach (var order in _orSet)
                                {
                                    if (order.Value.OrderTag == o.OrderTag)
                                    {
                                        newOrderCancel(order.Key);
                                        state = OrderState.OrderFinished;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        long _numDisplayed = 0;
        long _numIgnored = 0;
        System.Threading.AutoResetEvent _done = new System.Threading.AutoResetEvent(false);



        public bool Start()
        {
            try
            {
                if (isConnected)
                    return true;
                _app = new ClientAdapterToolkitApp();
                initTables();
                return true;
            }
            catch (Exception ee)
            {
                debug(ee.Message+ee.StackTrace);
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                TBL.Stop();
                _bwgo = false;
                _bw.CancelAsync();
                
                
            }
            catch { }
            
        }
    }
}
