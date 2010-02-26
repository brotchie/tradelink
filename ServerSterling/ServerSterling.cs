using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using System.Threading;
using SterlingLib;
namespace SterServer
{
    public class ServerSterling : TLServer_WM
    {
        public const string PROGRAM ="ServerSterling";
        Thread _bw;
        // basic structures needed for operation
        STIEvents stiEvents;
        STIOrderMaint stiOrder;
        STIPosition stiPos;
        STIQuote stiQuote;
        STIBook stiBook;
        bool imbalance = false;
        PositionTracker pt = new PositionTracker();
        int _SLEEP = 50;
        public ServerSterling() : this(50) { }
        public ServerSterling(int sleepOnNodata)
        {
            _SLEEP = 50;
        }
        bool _connected = false;
        public bool isConnected { get { return _connected; } }
        public bool Start()
        {
            try
            {
                debug(Util.TLSIdentity());
                debug("Attempting to start: " + PROGRAM);
                // basic structures needed for operation
                stiEvents = new STIEvents();
                stiOrder = new STIOrderMaint();
                stiPos = new STIPosition();
                stiQuote = new STIQuote();
                stiBook = new STIBook();
                _bw = new Thread(new ParameterizedThreadStart(background));
                _runbg = true;
                _bw.Start();


                stiEvents.SetOrderEventsAsStructs(true);

                stiEvents.OnSTIOrderUpdate += new _ISTIEventsEvents_OnSTIOrderUpdateEventHandler(stiEvents_OnSTIOrderUpdate);
                stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
                stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
                stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
                stiQuote.OnSTIQuoteSnap += new _ISTIQuoteEvents_OnSTIQuoteSnapEventHandler(stiQuote_OnSTIQuoteSnap);
                stiPos.GetCurrentPositions();

                newAcctRequest += new StringDelegate(tl_newAcctRequest);
                newProviderName = Providers.Sterling;
                newSendOrderRequest += new OrderDelegate(tl_gotSrvFillRequest);
                newPosList += new PositionArrayDelegate(tl_gotSrvPosList);
                newRegisterStocks += new DebugDelegate(tl_RegisterStocks);
                newOrderCancelRequest += new UIntDelegate(tl_OrderCancelRequest);
                newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
                newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
                newImbalanceRequest += new VoidDelegate(tl_newImbalanceRequest);
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
                _connected = false;
                return false;
            }
            debug(PROGRAM + " started.");
            _connected = true;
            return _connected;
        }

        void tl_newImbalanceRequest()
        {
            // register for imbalance data
            stiQuote.RegisterForMdx(true);
            imbalance = true;
        }

        string tl_newAcctRequest()
        {
            return string.Join(",", accts.ToArray());
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            // message will be handled on main thread for com security
            _msgq.Write(new GenericMessage(t, msg));
            // we say ok for any supported messages
            switch (t)
            {
                case MessageTypes.SENDORDERPEGMIDPOINT:
                    return (long)MessageTypes.OK;
            }
            return (long)MessageTypes.UNKNOWN_MESSAGE;
        }

        MessageTypes[] tl_newFeatureRequest()
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
            f.Add(MessageTypes.HEARTBEAT);
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



        public override void Stop()
        {
            try
            {
                stiQuote.DeRegisterAllQuotes();
                stiBook = null;
                stiOrder = null;
                stiPos = null;
                stiEvents = null;
                stiQuote = null;
                _runbg = false;
                Thread.Sleep(250);
                if ((_bw.ThreadState != ThreadState.Aborted) || (_bw.ThreadState != ThreadState.Stopped))
                {
                    try
                    {
                        _bw.Abort();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                debug(ex.Message + ex.StackTrace);
            }
            base.Stop();
        }
        const int MAXRECORD = 5000;
        RingBuffer<Order> _orderq = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<uint> _cancelq = new RingBuffer<uint>(MAXRECORD);
        RingBuffer<bool> _symsq = new RingBuffer<bool>(5);
        RingBuffer<GenericMessage> _msgq = new RingBuffer<GenericMessage>(100);
        string symquotes = "";
        Dictionary<uint, string> idacct = new Dictionary<uint, string>();


        bool _runbg = false;
        void background(object param)
        {
            while (_runbg)
            {
                try
                {
                    // orders
                    while (!_orderq.isEmpty)
                    {
                        STIOrder order = new STIOrder();
                        Order o = _orderq.Read();
                        order.LmtPrice = (double)o.price;
                        order.StpPrice = (double)o.stopp;
                        order.Destination = o.Exchange;
                        order.Side = getside(o.symbol,o.side);
                        order.Symbol = o.symbol;
                        order.Quantity = o.UnsignedSize;
                        string acct = accts.Count > 0 ? accts[0] : "";
                        order.Account = o.Account != "" ? o.Account : acct;
                        order.Destination = o.Exchange != "" ? o.ex : "NYSE";
                        order.Tif = o.TIF;
                        order.PriceType = o.isMarket ? STIPriceTypes.ptSTIMkt : (o.isLimit ? STIPriceTypes.ptSTILmt : STIPriceTypes.ptSTISvrStp);
                        order.ClOrderID = o.id.ToString();
                        int err = order.SubmitOrder();
                        string tmp = "";
                        if ((err == 0) && (!idacct.TryGetValue(o.id, out tmp)))
                            idacct.Add(o.id, order.Account);
                        if (err < 0)
                            debug("Error sending order: " + Util.PrettyError(newProviderName, err) + o.ToString());
                        if (err == -1)
                            debug("Make sure you have set the account in sending program.");
                    }

                    // quotes
                    if (!_symsq.isEmpty)
                    {
                        _symsq.Read();
                        foreach (string sym in symquotes.Split(','))
                            stiQuote.RegisterQuote(sym, "*");
                    }

                    // cancels
                    if (!_cancelq.isEmpty)
                    {
                        uint number = _cancelq.Read();
                        string acct = "";
                        if (idacct.TryGetValue(number, out acct))
                        {
                            stiOrder.CancelOrder(acct, 0, number.ToString(), DateTime.Now.Ticks.ToString() + acct);
                            newOrderCancel(number);
                        }
                        else
                            debug("No record of id: " + number.ToString());
                    }

                    // messages
                    if (_msgq.hasItems)
                    {
                        GenericMessage gm = _msgq.Read();
                        switch (gm.Type)
                        {
                            case MessageTypes.SENDORDERPEGMIDPOINT:
                                {
                                    // create order
                                    STIOrder order = new STIOrder();
                                    // pegged 2 midmarket
                                    order.ExecInst = "M";
                                    // get order
                                    Peg2Midpoint o = Peg2Midpoint.Deserialize(gm.Request);
                                    if (!o.isValid) break;
                                    order.Symbol = o.symbol;
                                    order.PegDiff = (double)o.pegdiff;
                                    order.PriceType = STIPriceTypes.ptSTIPegged;
                                    bool side = o.size > 0;
                                    order.Side = getside(o.symbol, side);
                                    order.Quantity = Math.Abs(o.size);
                                    order.Destination = o.ex;
                                    order.ClOrderID = o.id.ToString();
                                    string acct = accts.Count > 0 ? accts[0] : "";
                                    order.Account = o.Account != "" ? o.Account : acct;
                                    int err = order.SubmitOrder();
                                    string tmp = "";
                                    if ((err == 0) && (!idacct.TryGetValue(o.id, out tmp)))
                                        idacct.Add(o.id, order.Account);
                                    if (err < 0)
                                        debug("Error sending order: " + Util.PrettyError(newProviderName, err) + o.ToString());
                                    if (err == -1)
                                        debug("Make sure you have set the account in sending program.");

                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    debug(ex.Message + ex.StackTrace);
                }
                if (_symsq.isEmpty && _orderq.isEmpty && _cancelq.isEmpty)
                    Thread.Sleep(_SLEEP);
            }
        }

        string getside(string symbol, bool side)
        {
            // use by and sell as default
            string r = side ? "B" : "S";
            // if we're flat or short and selling, mark as a short
            if ((pt[symbol].isFlat || pt[symbol].isShort) && !side)
                r = "T";
            // if short and buying, mark as cover
            else if (pt[symbol].isShort && side)
                r = "C";
            return r;
        }

        void tl_RegisterStocks(string msg)
        {
            symquotes = msg;
            _symsq.Write(true);
        }


        void tl_OrderCancelRequest(uint number)
        {
            _cancelq.Write(number);
        }



        void stiEvents_OnSTITradeUpdate(ref structSTITradeUpdate t)
        {
            Trade f = new TradeImpl();
            f.symbol = t.bstrSymbol;
            f.Account = t.bstrAccount;
            uint id = 0;
            if (uint.TryParse(t.bstrClOrderId, out id))
                f.id = id;
            f.xprice = (decimal)t.fExecPrice;
            f.xsize = t.nQuantity;
            long now = Convert.ToInt64(t.bstrUpdateTime);
            int xsec = (int)(now % 100);
            long rem = (now - xsec) / 100;
            f.side = t.bstrSide == "B";
            f.xtime = ((int)(rem % 10000)) * 100 + xsec;
            f.xdate = (int)((now - f.xtime) / 1000000);
            pt.Adjust(f);
            newFill(f);
        }

        List<uint> _onotified = new List<uint>(MAXRECORD);

        void stiEvents_OnSTIOrderUpdate(ref structSTIOrderUpdate structOrderUpdate)
        {
            Order o = new OrderImpl();
            o.symbol = structOrderUpdate.bstrSymbol;
            uint id = 0;
            if (!uint.TryParse(structOrderUpdate.bstrClOrderId, out id))
                id = (uint)structOrderUpdate.nOrderRecordId;
            // don't notify for same order more than once
            if (_onotified.Contains(id)) return;
            if (structOrderUpdate.bstrLogMessage.Contains("REJ"))
                debug(o.id+" "+structOrderUpdate.bstrLogMessage);
            o.id = id;
            o.size = structOrderUpdate.nQuantity;
            o.side = o.size > 0;
            o.price = (decimal)structOrderUpdate.fLmtPrice;
            o.stopp = (decimal)structOrderUpdate.fStpPrice;
            o.TIF = structOrderUpdate.bstrTif;
            o.Account = structOrderUpdate.bstrAccount;
            o.ex = structOrderUpdate.bstrDestination;
            long now = Convert.ToInt64(structOrderUpdate.bstrUpdateTime);
            int xsec = (int)(now % 100);
            long rem = (now - xsec) / 100;
            o.time = ((int)(rem % 10000)) * 100 + xsec;
            o.date = (int)((rem - o.time) / 10000);
            _onotified.Add(o.id);
            newOrder(o);

        }


        Position[] tl_gotSrvPosList(string account)
        {
            return pt.ToArray();
        }

        void stiQuote_OnSTIQuoteUpdate(ref structSTIQuoteUpdate q)
        {
            Tick k = new TickImpl(q.bstrSymbol);
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize / 100;
            k.os = q.nAskSize / 100;
            if (q.bstrExch != "*")
                k.ex = q.bstrExch;
            if (q.bstrBidExch != "*")
                k.be = q.bstrBidExch;
            if (q.bstrAskExch != "*")
                k.oe = q.bstrAskExch;
            int now = Convert.ToInt32(q.bstrUpdateTime);
            k.date = Util.ToTLDate(DateTime.Now);
            int sec = now % 100;
            k.time = now;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            newTick(k);
            if ((q.bValidMktImb == 0) || !imbalance) return;
            newImbalance(new ImbalanceImpl(k.symbol, k.ex, q.nMktImbalance, k.time, 0, 0, 0));

        }

        void stiQuote_OnSTIQuoteSnap(ref structSTIQuoteSnap q)
        {
            TickImpl k = new TickImpl();
            k.symbol = q.bstrSymbol;
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize / 100;
            k.os = q.nAskSize / 100;
            if (q.bstrExch != "*")
                k.ex = q.bstrExch;
            if (q.bstrBidExch != "*")
                k.be = q.bstrBidExch;
            if (q.bstrAskExch != "*")
                k.oe = q.bstrAskExch;
            int now = Convert.ToInt32(q.bstrUpdateTime);
            k.date = Util.ToTLDate(DateTime.Now);
            k.time = now;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            newTick(k);
        }
        IdTracker _idt = new IdTracker();
        void tl_gotSrvFillRequest(Order o)
        {

            if (o.id == 0) o.id = _idt.AssignId;
            _orderq.Write(o);
        }

        List<string> accts = new List<string>();
        void stiPos_OnSTIPositionUpdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            // symbol
            string sym = structPositionUpdate.bstrSym;
            // size
            int size = structPositionUpdate.nSharesBot - structPositionUpdate.nSharesSld + structPositionUpdate.nOpeningPosition;
            // price
            decimal price = Math.Abs((decimal)structPositionUpdate.fPositionCost / size);
            // closed pl
            decimal cpl = (decimal)structPositionUpdate.fReal;
            // account
            string ac = structPositionUpdate.bstrAcct;
            // build position
            Position p = new PositionImpl(sym, price, size, cpl, ac);
            // track it
            pt.NewPosition(p);
            // track account
            if (!accts.Contains(ac))
                accts.Add(ac);
        }

        void stiEvents_OnSTIOrderUpdateMsg(STIOrderUpdateMsg oSTIOrderUpdateMsg)
        {
            throw new NotImplementedException();
        }

        void stiEvents_OnSTIOrderConfirmMsg(STIOrderConfirmMsg oSTIOrderConfirmMsg)
        {
            throw new NotImplementedException();
        }



        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }

        public event DebugDelegate SendDebug;
    }
}
