using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;
using System.Threading;
using SterlingLib;
using System.Xml.Serialization;
using System.Xml;

namespace SterServer
{

    public class ServerSterling 
    {
        public const string PROGRAM ="ServerSterling";
        Thread _bw;
        // basic structures needed for operation
        STIEvents stiEvents;
        STIOrderMaint stiOrder;
        STIAcctMaint stiMaint;
        STIPosition stiPos;
        STIQuote stiQuote;
        STIBook stiBook;
        STIApp stiApp;
        TLServer tl;

        OversellTracker ost;

        REGSHO_ShortTracker sho = new REGSHO_ShortTracker();

        bool _ignoreoutoforderticks = true;
        public bool IgnoreOutOfOrderTicks { get { return _ignoreoutoforderticks; } set { _ignoreoutoforderticks = value; } }
        int _fixorderdecimalplace = 2;
        public int FixOrderDecimalPlace { get { return _fixorderdecimalplace; } set { _fixorderdecimalplace = value; } }

        bool _papertrade = false;
        public bool isPaperTradeEnabled { get { return _papertrade; } set { _papertrade = value; } }
        PapertradeTracker ptt = new PapertradeTracker();
        bool _papertradebidask = false;
        public bool isPaperTradeUsingBidAsk { get { return _papertradebidask; } set { _papertradebidask = value; } }


        PositionTracker pt = new PositionTracker();
        int _SLEEP = 50;
        int _ORDERSLEEP = 1;
        int _CANCELWAIT = 1000;
        public int CancelWait { get { return _CANCELWAIT; } set { _CANCELWAIT = value; } }
        bool _supportcover = true;
        public bool CoverEnabled { get { return _supportcover; } set { _supportcover = value; } }
        
        public ServerSterling(TLServer tls, int sleepOnNodata, int sleepAfterOrder, DebugDelegate deb)
        {
            SendDebug = deb;
            tl = tls;
            _SLEEP = 50;
            _ORDERSLEEP = sleepAfterOrder;
        }

        int _minlotsize = 1;
        public int MinLotSize { get { return _minlotsize; } set { _minlotsize = value; } }
        bool _xmlquotes = false;
        public bool UseXmlMode { get { return _xmlquotes; } set { _xmlquotes = value; debug("xml mode: " + (_xmlquotes ? "on" : "off")); } }
        bool _connected = false;
        public bool isConnected { get { return _connected; } }
        bool _verbosedebug = false;
        public bool VerboseDebugging { get { return _verbosedebug; } set { _verbosedebug = value; } }
        bool _ossplit = false;
        public bool OversellSplit { get { return _ossplit; } set { _ossplit = value; } }
        public bool Start()
        {
            try
            {
                if (_connected) return true;
                ost = new OversellTracker(pt,_idt);
                ost.MinLotSize = MinLotSize;
                ost.Split = OversellSplit;
                ost.SendOrderEvent += new OrderDelegate(ost_SendOrderEvent);
                ost.SendDebugEvent += new DebugDelegate(ost_SendDebugEvent);
                ost.SendCancelEvent += new LongDelegate(reqcancel);
                ost.VerboseDebugging = VerboseDebugging;
                sho.SendDebugEvent+=new DebugDelegate(v);
                sho.DefaultAccount = Account;
                sho.VerboseDebugging = VerboseDebugging;
                
                debug(Util.TLSIdentity());
                debug("Attempting to start: " + PROGRAM);
                // basic structures needed for operation
                stiEvents = new STIEvents();
                stiOrder = new STIOrderMaint();
                stiPos = new STIPosition();
                stiQuote = new STIQuote();
                stiBook = new STIBook();
                stiApp = new STIApp();
                stiMaint = new STIAcctMaint();
                _bw = new Thread(new ParameterizedThreadStart(background));
                _runbg = true;
                _bw.Start();
                ptt.GotCancelEvent+=new LongDelegate(newcancel);
                ptt.GotFillEvent += new FillDelegate(tl.newFill);
                ptt.GotOrderEvent+=new OrderDelegate(tl.newOrder);
                ptt.SendDebugEvent += new DebugDelegate(ptt_SendDebugEvent);
                ptt.UseBidAskFills = isPaperTradeUsingBidAsk;

                stiEvents.OnSTIShutdown += new _ISTIEventsEvents_OnSTIShutdownEventHandler(stiEvents_OnSTIShutdown);
                stiEvents.SetOrderEventsAsStructs(true);
                stiApp.SetModeXML(UseXmlMode);

                stiEvents.OnSTIOrderUpdate += new _ISTIEventsEvents_OnSTIOrderUpdateEventHandler(stiEvents_OnSTIOrderUpdate);
                stiEvents.OnSTIOrderUpdateXML += new _ISTIEventsEvents_OnSTIOrderUpdateXMLEventHandler(stiEvents_OnSTIOrderUpdateXML);
                stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
                stiEvents.OnSTITradeUpdateXML += new _ISTIEventsEvents_OnSTITradeUpdateXMLEventHandler(stiEvents_OnSTITradeUpdateXML);
                stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
                stiPos.OnSTIPositionUpdateXML += new _ISTIPositionEvents_OnSTIPositionUpdateXMLEventHandler(stiPos_OnSTIPositionUpdateXML);
                stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
                stiQuote.OnSTIQuoteSnap += new _ISTIQuoteEvents_OnSTIQuoteSnapEventHandler(stiQuote_OnSTIQuoteSnap);
                stiEvents.OnSTIOrderRejectMsg += new _ISTIEventsEvents_OnSTIOrderRejectMsgEventHandler(stiEvents_OnSTIOrderRejectMsg);
                stiEvents.OnSTIOrderRejectXML += new _ISTIEventsEvents_OnSTIOrderRejectXMLEventHandler(stiEvents_OnSTIOrderRejectXML);
                stiEvents.OnSTIOrderReject += new _ISTIEventsEvents_OnSTIOrderRejectEventHandler(stiEvents_OnSTIOrderReject);
                stiQuote.OnSTIQuoteUpdateXML += new _ISTIQuoteEvents_OnSTIQuoteUpdateXMLEventHandler(stiQuote_OnSTIQuoteUpdateXML);
                stiQuote.OnSTIQuoteSnapXML += new _ISTIQuoteEvents_OnSTIQuoteSnapXMLEventHandler(stiQuote_OnSTIQuoteSnapXML);

                Array acctlist = new string[0];
                stiMaint.GetAccountList(ref acctlist);
                if (acctlist.Length > 0)
                {
                    foreach (string au in acctlist)
                        Accounts = new string[] { au};


                }
                stiPos.GetCurrentPositions();
                

                tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
                tl.newProviderName = Providers.Sterling;
                tl.newSendOrderRequest += new OrderDelegateStatus(tl_gotSrvFillRequest);
                tl.newPosList += new PositionArrayDelegate(tl_gotSrvPosList);
                tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
                tl.newOrderCancelRequest += new LongDelegate(tl_OrderCancelRequest);
                tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
                tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
                tl.newImbalanceRequest += new VoidDelegate(tl_newImbalanceRequest);
                tl.SendDebugEvent += new DebugDelegate(tl_SendDebugEvent);
                
                string trader = stiApp.GetTraderName().ToUpper();
                debug("trader: " + trader);
                if (!accts.Contains(trader))
                {
                    accts.Add(trader);
                    debug("accounts: " + string.Join(",", Accounts));
                }

                debug("VerboseDebugging: " + (VerboseDebugging ? "ON" : "disabled."));
                debug("PaperTrade: " + (isPaperTradeEnabled ? "ON" : "disabled."));
                debug("OversellSplit: " + (OversellSplit ? "ON" : "disabled."));
                debug("CoverEnabled: " + (CoverEnabled? "ON" : "disabled."));
                debug("RegSHOShorts: " + (RegSHOShorts ? "ON" : "disabled."));
                debug("ServerStops: " + (UseServerStops ? "ON" : "disabled."));
                debug("SendCancelOnRejects: "+(SendCancelOnReject ? "ON" : "disabled."));
                debug("SendCancelOnError: " + (SendCancelOnError? "ON" : "disabled."));
                debug("UseSubscribedSymbolForNotify: " + (UseSubscribedSymbolForNotify? "ON" : "disabled."));
                
                
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

        void reqcancel(long id)
        {
            v("marking id: " + id + " for cancelation.");
            _cancelq.Write(id);
        }




        void ost_SendDebugEvent(string msg)
        {
            debug(msg);
        }

        void ost_SendOrderEvent(Order o)
        {
            if (AutoSubscribeOrderSymbol)
            {
                bool needsub = true;
                // see if we have this symbol
                foreach (Security s in tl.AllClientBasket)
                    if (s.Symbol == o.symbol)
                    {
                        needsub = false;
                        break;
                    }
                if (needsub)
                {
                    // add symbol
                    if (symquotes.Length > 0)
                        symquotes += "," + o.symbol;
                    else
                        symquotes = o.symbol;
                    // mark symbol as added
                    _symsq.Write(true);
                    
                }
            }
            _orderq.Write(o);
            _receivedorder = true;
        }

        void ptt_SendDebugEvent(string msg)
        {
            if (!isPaperTradeEnabled)
                return;
            v("papertrade: " + msg);
        }

        void tl_SendDebugEvent(string message)
        {
            if (!tl.VerboseDebugging)
                return;
            debug("From Server: " + message);
        }

        void stiEvents_OnSTIOrderRejectXML(ref string bstrOrder)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTIOrderReject));
                structSTIOrderReject q = (structSTIOrderReject)xs.Deserialize(new System.IO.StringReader(bstrOrder));
                doupdatereject(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing reject: " + bstrOrder);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void stiPos_OnSTIPositionUpdateXML(ref string bstrPosition)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTIPositionUpdate));
                structSTIPositionUpdate q = (structSTIPositionUpdate)xs.Deserialize(new System.IO.StringReader(bstrPosition));
                dopositionupdate(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing position: " + bstrPosition);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void stiEvents_OnSTIOrderUpdateXML(ref string bstrOrder)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTIOrderUpdate));
                structSTIOrderUpdate q = (structSTIOrderUpdate)xs.Deserialize(new System.IO.StringReader(bstrOrder));
                doorderupdate(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing order: " + bstrOrder);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void stiEvents_OnSTITradeUpdateXML(ref string bstrTrade)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTITradeUpdate));
                structSTITradeUpdate q = (structSTITradeUpdate)xs.Deserialize(new System.IO.StringReader(bstrTrade));
                dofillupdate(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing fill: " + bstrTrade);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void stiQuote_OnSTIQuoteSnapXML(ref string bstrQuote)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTIQuoteSnap));
                structSTIQuoteSnap q = (structSTIQuoteSnap)xs.Deserialize(new System.IO.StringReader(bstrQuote));
                dosnap(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing quote: " + bstrQuote);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void stiQuote_OnSTIQuoteUpdateXML(ref string bstrQuote)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(SterlingLib.structSTIQuoteUpdate));
                structSTIQuoteUpdate q = (structSTIQuoteUpdate)xs.Deserialize(new System.IO.StringReader(bstrQuote));
                doquote(ref q);
            }
            catch (Exception ex)
            {
                debug("Error deserializing quote: " + bstrQuote);
                debug(ex.Message + ex.StackTrace);
            }

        }

        RingBuffer<string> removesym = new RingBuffer<string>(1000);
        void tl_newRegisterSymbols(string client, string symbols)
        {
            if (VerboseDebugging)
                debug("client subscribe request received: " + symbols);
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
            _symsq.Write(true);
        }

        void stiEvents_OnSTIShutdown()
        {
            debug("Sterling Trader Pro was closed or exited.");
        }

        void stiEvents_OnSTIOrderReject(ref structSTIOrderReject structOrderReject)
        {
            if (UseXmlMode) return;
            doupdatereject(ref structOrderReject);
        }

        bool _cancelonrej = false;
        public bool SendCancelOnReject { get { return _cancelonrej; } set { _cancelonrej = value; } }

        bool _cancelonerr = false;
        public bool SendCancelOnError { get { return _cancelonerr; } set { _cancelonerr = value; } }

        void doupdatereject(ref structSTIOrderReject structOrderReject)
        {
            debug("reject: " + structOrderReject.bstrClOrderId + " reason: " + structOrderReject.nRejectReason + " " + sterrejectpretty(structOrderReject.nRejectReason) + " additional info: " + structOrderReject.bstrText);
            if (SendCancelOnReject)
            {
                long cancelid = 0;
                if (long.TryParse(structOrderReject.bstrClOrderId, out cancelid))
                {
                    v("sending cancel ack for rejected id: " + cancelid);
                    newcancel(cancelid);
                }
            }
        }

        void doupdatereject(STIOrderRejectMsg oSTIOrderRejectMsg)
        {
            debug("reject: " + oSTIOrderRejectMsg.ClOrderID + " reason: " + oSTIOrderRejectMsg.RejectReason.ToString());
            if (SendCancelOnReject)
            {
                long cancelid = 0;
                if (long.TryParse(oSTIOrderRejectMsg.ClOrderID, out cancelid))
                {
                    v("sending cancel ack for rejected id: " + cancelid);
                    newcancel(cancelid);
                }
            }
        }

        void stiEvents_OnSTIOrderRejectMsg(STIOrderRejectMsg oSTIOrderRejectMsg)
        {
            if (UseXmlMode) return;
            doupdatereject(oSTIOrderRejectMsg);
        }

        string sterrejectpretty(string rint)
        {
            int ri = -1;
            if (int.TryParse(rint, out ri))
                return sterrejectpretty(ri);
            return "unknown reject error";
        }
        string sterrejectpretty(int r)
        {
            try
            {
                rejectmessages rm = (rejectmessages)r;
                return rm.ToString();
            }
            catch (Exception)
            {
                return "unknown reject error";
            }
        }

        enum rejectmessages
        {
            rrSTIUnknown = 0,
            rrSTIUnknownPid =1,
            rrSTIInvalidPassword,
            rrSTIAccessDenied,
            rrSTINotFound,
            rrSTICannotRoute,
            rrSTIPendingCancel,
            rrSTIPendingReplace,
            rrSTIOrderClosed,
            rrSTICannotCreate,
            rrSTIDupeClOrdId,
            rrSTINoSeqNoAvailable,
            rrSTIInvalidAcct,
            rrSTIInvalidDest_OrNotEnabledForDest,
            rrSTIError,
            rrSTIDupeSeqNo,
            rrSTINoChange,
            rrSTIInvalidSeqNo,
            rrSTIInvalidQty,
            rrSTITLTC_TooLateToCancel,
            rrSTIShareLimit,
            rrSTIDollarLimit,
            rrSTIBuyingPower,
            rrSTITenSecRule,
            rrSTINotSupported,
            rrSTIDupeAcct,
            rrSTIInvalidGroupId,
            rrSTIDupeStation,
            rrSTIPosTradingLmt,
            rrSTITltcMoc_TooLateCancelMOC,
            rrSTIHardToBorrow,
            rrSTIVersion,
            rrSTIDupeLogin,
            rrSTIInvalidSym,
            rrSTINxRules,
            rrSTIBulletNotRequired,
            rrSTIMocMktImb,
            rrSTINx30SecRule,
            rrSTIEasyToBorrowOnly,
            rrSTIStaleOrder,
            rrSTILast,
        }

        bool _lastimbalance = false;
        bool _imbalance = false;
        void tl_newImbalanceRequest()
        {
            debug("Issued new imbalance request in Server.");
            _imbalance = true;
        }

        string tl_newAcctRequest()
        {
            return string.Join(",", accts.ToArray());
        }

        long tl_newUnknownRequest(MessageTypes t, string msg)
        {
            if (VerboseDebugging)
                debug("got message: " + t.ToString() + " " + msg);
            // message will be handled on main thread for com security
            _msgq.Write(new GenericMessage(t, msg));
            // we say ok for any supported messages
            switch (t)
            {
                case MessageTypes.SENDORDERPEGMIDPOINT:
                    return (long)MessageTypes.OK;
            }

            debug("Message type " + t.ToString() + ":" + msg + " was unsupported.");
            return (long)MessageTypes.UNKNOWN_MESSAGE;
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.HALTRESUME);
            f.Add(MessageTypes.INDICATION);
            f.Add(MessageTypes.LIVEDATA);
            f.Add(MessageTypes.LIVETRADING);
            f.Add(MessageTypes.SIMTRADING);
            f.Add(MessageTypes.ORDERCANCELREQUEST);
            f.Add(MessageTypes.ORDERCANCELRESPONSE);
            f.Add(MessageTypes.OK);
            f.Add(MessageTypes.BROKERNAME);
            f.Add(MessageTypes.CLEARCLIENT);
            f.Add(MessageTypes.CLEARSTOCKS);
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.HEARTBEATREQUEST);
            f.Add(MessageTypes.ORDERNOTIFY);
            f.Add(MessageTypes.EXECUTENOTIFY);
            f.Add(MessageTypes.REGISTERCLIENT);
            f.Add(MessageTypes.REGISTERSTOCK);
            f.Add(MessageTypes.TICKNOTIFY);
            f.Add(MessageTypes.VERSION);
            f.Add(MessageTypes.IMBALANCEREQUEST);
            f.Add(MessageTypes.IMBALANCERESPONSE);
            f.Add(MessageTypes.POSITIONREQUEST);
            f.Add(MessageTypes.POSITIONRESPONSE);
            f.Add(MessageTypes.ACCOUNTREQUEST);
            f.Add(MessageTypes.ACCOUNTRESPONSE);
            f.Add(MessageTypes.SENDORDER);
            f.Add(MessageTypes.SENDORDERSTOP);
            f.Add(MessageTypes.SENDORDERMARKET);
            f.Add(MessageTypes.SENDORDERLIMIT);
            f.Add(MessageTypes.SENDORDERTRAIL);
            f.Add(MessageTypes.SENDORDERPEGMIDPOINT);
            f.Add(MessageTypes.SENDORDERMARKETONCLOSE);
            return f.ToArray();
        }



        public void Stop()
        {
            try
            {
                _runbg = false;
                stiQuote.DeRegisterAllQuotes();
                stiBook = null;
                stiOrder = null;
                stiPos = null;
                stiEvents = null;
                stiQuote = null;
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
            if (tl != null)
                tl.Stop();

        }
        const int MAXRECORD = 5000;
        RingBuffer<Order> _orderq = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<long> _cancelq = new RingBuffer<long>(MAXRECORD);
        RingBuffer<bool> _symsq = new RingBuffer<bool>(5);
        RingBuffer<GenericMessage> _msgq = new RingBuffer<GenericMessage>(100);
        string symquotes = "";
        Dictionary<long, string> idacct = new Dictionary<long, string>();

        bool _autocap = false;
        public bool AutoCapAccounts { get { return _autocap; } set { _autocap = value; } }

        /// <summary>
        /// gets or sets default account
        /// </summary>
        public string Account 
        { 
            get { return accts.Count > 0 ? accts[0] : string.Empty; } 
            set 
            { 
                string a = AutoCapAccounts ? value.ToUpper() : value;
                if (!accts.Contains(a))
                    accts.Insert(0, a); 
                debug("default account: " + Account); 
            } 
        }

        /// <summary>
        /// gets or sets all accounts to advertise
        /// </summary>
        public string[] Accounts 
        { 
            get 
            { 
                return accts.ToArray(); 
            } 
            set 
            {
                accts.Clear();
                foreach (string a in value)
                {
                    string av = AutoCapAccounts ? a.ToUpper() : a;
                    if (!accts.Contains(av) && (av!=string.Empty))
                        accts.Add(av);
                }
                debug("Accounts: " + string.Join(",", accts.ToArray())); 
            } 
        }

        bool _useserverstops = true;
        public bool UseServerStops { get { return _useserverstops; } set { _useserverstops = value; } }

        bool _autosetunsetid = true;

        public bool AutosetUnsetId { get { return _autosetunsetid; } set { _autosetunsetid = value; } }

        bool _autosubosym = true;
        public bool AutoSubscribeOrderSymbol { get { return _autosubosym; } set { _autosubosym = value; } }

        int _postsubscribewait = 100;
        public int PostSymSubscribeWait { get { return _postsubscribewait; } set { _postsubscribewait = value; } }

        bool _usesubscribedsym = true;
        public bool UseSubscribedSymbolForNotify { get { return _usesubscribedsym; } set { _usesubscribedsym = value; } }

        string[] syms { get { return symquotes.Split(','); } }
        Dictionary<string, int> ssym2longsymidx = new Dictionary<string, int>();

        string getfullsymbolname(string ssym)
        {
            int idx = getlongsymbolidx(ssym);
            if ((idx >= 0) && (idx < syms.Length))
            {
                return syms[idx];
            }
            return ssym;
        }

        int getlongsymbolidx(string ssym)
        {
            int idx;
            if (ssym2longsymidx.TryGetValue(ssym, out idx))
                return idx;
            return -1;

        }

        bool hasssymbol(string ssym)
        {
            return getlongsymbolidx(ssym) != -1;
        }

        string getinstrument(Security sec)
        {
            
            switch (sec.Type)
            {
                case SecurityType.OPT:
                    return "O";
                case SecurityType.FUT:
                    return "F";
                case SecurityType.CASH:
                case SecurityType.FOX:
                    return "X";
                default:
                    return "E";
            }
        }

        bool _runbg = false;
        void background(object param)
        {
            while (_runbg)
            {
                try
                {
                    // new quotes
                    if (!_symsq.isEmpty)
                    {
                        _symsq.Read();
                        // clear index
                        ssym2longsymidx.Clear();
                        for (int i = 0; i<syms.Length; i++)
                        {
                            string sym = syms[i];
                            // get security
                            Security sec = SecurityImpl.Parse(sym);
                            if (sec.hasDest)
                                stiQuote.RegisterQuote(sec.Symbol_Spaces,sec.DestEx);
                            else if (sec.Type== SecurityType.OPT)
                                stiQuote.RegisterQuote(sec.Symbol_Spaces, "O");
                            else
                                stiQuote.RegisterQuote(sec.Symbol_Spaces, "*");
                            // save relationship
                            if (!ssym2longsymidx.ContainsKey(sec.Symbol_Spaces))
                                ssym2longsymidx.Add(sec.Symbol_Spaces, i);
                            else
                            {
                                v(sec.Symbol_Spaces + " replacing index: " + ssym2longsymidx[sec.Symbol_Spaces] + " with: " + i);
                                ssym2longsymidx[sec.Symbol_Spaces] = i;
                            }

                        }
                        // wait moment for quotes to load
                        Thread.Sleep(PostSymSubscribeWait);
                    }

                    // orders
                    while (!_orderq.isEmpty)
                    {
                        STIOrder order = new STIOrder();
                        Order o = _orderq.Read();
                        if (VerboseDebugging)
                            debug("client order received: " + o.ToString());
                        if ((o.id == 0) && AutosetUnsetId)
                        {
                            o.id = _idt.AssignId;
                        }
                        if (isPaperTradeEnabled)
                        {
                            ptt.sendorder(o);
                        }
                        else
                        {
                            o.price = Math.Round(o.price, FixOrderDecimalPlace);
                            o.stopp = Math.Round(o.stopp, FixOrderDecimalPlace);

                            if (o.ex == string.Empty)
                                o.ex = o.symbol.Length > 3 ? "NSDQ" : "NYSE";
                            order.Destination = o.Exchange;
                            order.Side = getside(o);
                            Security sec = SecurityImpl.Parse(o.symbol);
                            order.Instrument = getinstrument(sec);
                            order.Symbol = sec.Symbol_Spaces;
                            order.Quantity = o.UnsignedSize;
                            string acct = Account != string.Empty ? Account : string.Empty;
                            order.Account = o.Account != string.Empty ? o.Account : acct;
                            order.Destination = o.Exchange != "" ? o.ex : "NYSE";
                            bool close = o.ValidInstruct == OrderInstructionType.MOC;
                            bool pegged = (o.ValidInstruct >= OrderInstructionType.PEG2MID) && (o.ValidInstruct <= OrderInstructionType.PEG2BST);
                            order.Tif = tif2tif(o.TIF);
                            if (!pegged)
                            {
                                order.LmtPrice = (double)o.price;
                                order.StpPrice = (double)o.stopp;
                            }
                            if (close)
                            {
                                if (o.isMarket)
                                    order.PriceType = STIPriceTypes.ptSTIMktClo;
                                else if (o.isLimit)
                                    order.PriceType = STIPriceTypes.ptSTILmtClo;
                                else
                                    order.PriceType = STIPriceTypes.ptSTIClo;
                            }
                            else if (pegged)
                            {
                                order.PriceType = STIPriceTypes.ptSTIPegged;
                                if (o.price<=0)
                                    order.PegDiff = (double)o.price;
                                else
                                    order.LmtPrice = (double)o.price;
                                if (o.ValidInstruct== OrderInstructionType.PEG2BST)
                                    order.ExecInst = "T";
                                else if (o.ValidInstruct== OrderInstructionType.PEG2MID)
                                    order.ExecInst = "M";
                                else if (o.ValidInstruct== OrderInstructionType.PEG2MKT)
                                    order.ExecInst = "P";
                                else if (o.ValidInstruct== OrderInstructionType.PEG2PRI)
                                    order.ExecInst = "R";
                            }
                            else if (o.isMarket)
                                order.PriceType = STIPriceTypes.ptSTIMkt;
                            else if (o.isLimit && o.isStop)
                            {
                                if (UseServerStops)
                                    order.PriceType = STIPriceTypes.ptSTISvrStpLmt;
                                else
                                    order.PriceType = STIPriceTypes.ptSTILmtStp;
                            }
                            else if (o.isLimit)
                                order.PriceType = STIPriceTypes.ptSTILmt;
                            else if (o.isStop)
                            {
                                if (UseServerStops)
                                    order.PriceType = STIPriceTypes.ptSTISvrStp;
                                else
                                {
                                    order.PriceType = STIPriceTypes.ptSTILmtStp;
                                }
                            }
                            else if (o.isTrail)
                                order.PriceType = STIPriceTypes.ptSTITrailStp;
                            // sterling options need some information provided twice
                            if (sec.Type == SecurityType.OPT)
                            {
                                if (SecurityImpl.ParseOptionOSI(sec.Symbol_Spaces, ref sec, v))
                                {
                                    order.Maturity = sec.Date.ToString();
                                    order.PutCall = sec.isCall ? "C" : "P";
                                    order.StrikePrice = (double)sec.Strike;
                                    order.Underlying = sec.Symbol;
                                    string symbol = order.Symbol;
                                    // if we're flat or going with our side, it's an open
                                    if (pt[symbol, o.Account].isFlat || (pt[symbol, o.Account].isLong == o.side))
                                    {
                                        v(o.symbol + " marking option order as open: " + o.ToString() + " from pos: " + pt[symbol, o.Account]);
                                        order.OpenClose = "O";
                                    }
                                    // if going against our side, it's a close
                                    else if (!pt[symbol, o.Account].isFlat && (pt[symbol, o.Account].isLong == o.side))
                                    {
                                        v(o.symbol + " marking option order as close: " + o.ToString() + " from pos: " + pt[symbol, o.Account]);
                                        order.OpenClose = "C";
                                    }
                                }
                                else
                                    debug("error parsing option osi: " + o.symbol + " on order: " + o.ToString());
                            }
                            order.ClOrderID = o.id.ToString();
                            int err = order.SubmitOrder();
                            if (VerboseDebugging)
                                debug("client order sent: " + order.ClOrderID);
                            string tmp = "";
                            if ((err == 0) && (!idacct.TryGetValue(o.id, out tmp)))
                            {
                                // save account/id relationship for canceling
                                idacct.Add(o.id, order.Account);
                                // wait briefly between orders
                                Thread.Sleep(_ORDERSLEEP);
                            }
                            // if order was sent and we're following short regulations, start tracking this order
                            if ((err == 0) && RegSHOShorts)
                            {
                                // keep track that we sent the order
                                sho.GotOrder(o);
                            }
                            if (err < 0)
                            {
                                debug("Error sending order: " + Util.PrettyError(tl.newProviderName, err) + o.ToString());
                                if (err == -1)
                                    debug("Make sure you have set the account in sending program.");
                                if (SendCancelOnError)
                                {
                                    v("sending cancel ack for error on order id: " + o.id);
                                    newcancel(o.id);
                                }

                            }
                        }
                    }

                    
                    // old quotes
                    while (removesym.hasItems)
                    {
                        string rem = removesym.Read();
                        stiQuote.DeRegisterQuote(rem, "*");
                    }

                    // cancels
                    if (!_cancelq.isEmpty)
                    {
                        long number = _cancelq.Read();
                        if (isPaperTradeEnabled)
                        {
                            ptt.sendcancel(number);
                        }
                        else
                        {
                            string acct = "";
                            if (idacct.TryGetValue(number, out acct))
                            {
                                // get unique cancel id
                                long cancelid = _idt.AssignId;
                                // save cancel to order id relationship
                                _cancel2order.Add(cancelid, number);
                                bool isman;
                                // see if it's a manual order
                                if (!ismanorder.TryGetValue(number, out isman))
                                    isman = false;
                                // send cancel
                                if (isman) // manual orders use nOrderRercordId
                                    stiOrder.CancelOrder(acct, (int)number, null, cancelid.ToString());
                                else
                                    stiOrder.CancelOrder(acct, 0, number.ToString(), cancelid.ToString());
                                if (VerboseDebugging)
                                    debug("client cancel requested: " + number.ToString() + " " + cancelid.ToString());
                            }
                            else
                                debug("No record of order id: " + number.ToString());
                            // see if empty yet
                            if (_cancelq.hasItems)
                                Thread.Sleep(_CANCELWAIT);
                        }
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
                                    if (VerboseDebugging)
                                        debug("client P2M order: " + o.ToString());
                                    order.Symbol = o.symbol;
                                    order.PegDiff = (double)o.pegdiff;
                                    order.PriceType = STIPriceTypes.ptSTIPegged;
                                    bool side = o.size > 0;
                                    order.Side = getside(order.Symbol, side);
                                    order.Quantity = Math.Abs(o.size);
                                    order.Destination = o.ex;
                                    order.ClOrderID = o.id.ToString();
                                    order.Tif = "D";
                                    string acct = Account != string.Empty ? Account : string.Empty;
                                    order.Account = o.Account != string.Empty ? o.Account : acct;
                                    int err = order.SubmitOrder();
                                    string tmp = "";
                                    if ((err == 0) && (!idacct.TryGetValue(o.id, out tmp)))
                                        idacct.Add(o.id, order.Account);
                                    if (err < 0)
                                        debug("Error sending order: " + Util.PrettyError(tl.newProviderName, err) + o.ToString());
                                    if (err == -1)
                                        debug("Make sure you have set the account in sending program.");

                                }
                                break;
                        }
                    }

                    if (_lastimbalance != _imbalance)
                    {
                        _lastimbalance = _imbalance;
                        // register for imbalance data
                        stiQuote.RegisterForAllMdx(true);
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

        string tif2tif(string incoming)
        {
            if (incoming == string.Empty)
                return "D";

            if ((incoming == "OPG") || (incoming == "OPN"))
            {
                return "O";
            }
            else if (incoming == "MOC")
            {
                return "D";
            }
            return "D";
        }

        Dictionary<long, long> _cancel2order = new Dictionary<long, long>(MAXRECORD);

        

        bool _regshoshort = false;
        public bool RegSHOShorts { get { return _regshoshort; } set { _regshoshort = value; } }

        string getside(string symbol, bool side)
        {
            // use by and sell as default
            string r = side ? "B" : "S";
            if (CoverEnabled)
            {
                // if we're flat or short and selling, mark as a short
                if ((pt[symbol, Account].isFlat || pt[symbol, Account].isShort) && !side)
                {
                    r = "T";
                }
                // if short and buying, mark as cover
                else if (pt[symbol, Account].isShort && side)
                {
                    r = "C";
                }
            }
            return r;
        }
        string getside(Order o)
        {
            bool side = o.side;
            string symbol = o.symbol;
            // use by and sell as default
            string r = side ? "B" : "S";
            if (CoverEnabled)
            {
                // if we're flat or short and selling, mark as a short
                if ((pt[symbol,o.Account].isFlat || pt[symbol, o.Account].isShort) && !side)
                {
                    v(o.symbol + " marking order as short: " + o.ToString()+ " pos: "+pt[symbol,o.Account]);
                    r = "T";
                }
                // if short and buying, mark as cover
                else if (pt[symbol,o.Account].isShort && side)
                {
                    v(o.symbol + " marking order as cover: " + o.ToString() + " pos: " + pt[symbol, o.Account]);
                    r = "C";
                }
            }
            if (RegSHOShorts)
            {
                // see if order needs short marking (versus sell)
                if (sho.isOrderShort(o))
                {
                    v(o.symbol + " marking order as regsho short: " + o);
                    r = "T";
                }
                else
                    v(o.symbol + " not a regsho short order: " + o);

            }
            return r;
        }

        void tl_RegisterStocks(string msg)
        {

        }


        void tl_OrderCancelRequest(long number)
        {
            v("processing incoming cancel id: " + number);
            ost.sendcancel(number);
        }

        void dofillupdate(ref structSTITradeUpdate t)
        {
            Trade f = new TradeImpl();
            string ssym = t.bstrSymbol;
            if (UseSubscribedSymbolForNotify)
            {
                int idx = getlongsymbolidx(ssym);
                // check for error
                if ((idx < 0) || (idx >= syms.Length))
                {
                    debug(ssym + " fill ack error identifying symbol idx: " + idx + " symbols: " + string.Join(",", syms));
                    f.symbol = ssym;
                }
                else
                    f.symbol = syms[idx];
            }
            else
                f.symbol = ssym;
            
            f.Account = t.bstrAccount;
            long id = 0;
            if (long.TryParse(t.bstrClOrderId, out id))
                f.id = id;
            else
                f.id = t.nOrderRecordId;
            f.xprice = (decimal)t.fExecPrice;
            f.xsize = t.nQuantity;
            long now = Convert.ToInt64(t.bstrUpdateTime);
            int xsec = (int)(now % 100);
            long rem = (now - xsec) / 100;
            f.side = t.bstrSide == "B";
            f.xtime = ((int)(rem % 10000)) * 100 + xsec;
            f.xdate = (int)((now - f.xtime) / 1000000);
            f.ex = t.bstrDestination;
            pt.Adjust(f);
            if (RegSHOShorts)
                sho.GotFill(f);
            tl.newFill(f);
            if (VerboseDebugging)
                debug("new trade sent: " + f.ToString() +" pos: "+pt[f.symbol,f.Account]);
        }

        void stiEvents_OnSTITradeUpdate(ref structSTITradeUpdate t)
        {
            if (UseXmlMode) return;
            dofillupdate(ref t);
        }

        List<long> _onotified = new List<long>(MAXRECORD);

        void stiEvents_OnSTIOrderUpdate(ref structSTIOrderUpdate structOrderUpdate)
        {
            if (UseXmlMode) return;
            doorderupdate(ref structOrderUpdate);
        }

        void v(string msg)
        {
            if (VerboseDebugging)
            {
                debug(msg);
            }
        }

        Dictionary<long, bool> ismanorder = new Dictionary<long, bool>();

        void newcancel(long id)
        {
            if (VerboseDebugging)
                debug("cancel received for: " + id);
            if (RegSHOShorts)
                sho.GotCancel(id);
            tl.newCancel(id);

        }

        void doorderupdate(ref structSTIOrderUpdate structOrderUpdate)
        {
            STIOrderStatus stat = (STIOrderStatus)structOrderUpdate.nOrderStatus;
            Order o = new OrderImpl();
            string ssym = structOrderUpdate.bstrSymbol;
            if (UseSubscribedSymbolForNotify)
            {
                int idx = getlongsymbolidx(ssym);
                // check for error
                if ((idx < 0) || (idx>=syms.Length))
                {
                    debug(ssym + " order ack error identifying symbol idx: " + idx + " symbols: " + string.Join(",", syms));
                    o.symbol = ssym;
                }
                else
                    o.symbol = syms[idx];
            }
            else
                o.symbol = ssym;
            long id = 0;
            // see if the order id is unknown
            if (!long.TryParse(structOrderUpdate.bstrClOrderId, out id))
            {
                // see if we know the sterling id (sometimes sterling doesn't send full client id)
                if (sterid2tlid.ContainsKey(structOrderUpdate.nOrderRecordId))
                {
                    id = sterid2tlid[structOrderUpdate.nOrderRecordId];

                }
                else
                {
                    // use the norderrecordid as our order id
                    id = (long)structOrderUpdate.nOrderRecordId;

                    // ensure this is not a secondary notification of same order
                    string tmp;
                    if (!idacct.TryGetValue(id, out tmp))
                    {

                        // save the id
                        debug("manual order: " + id + " " + structOrderUpdate.bstrAccount);
                        idacct.Add(id, structOrderUpdate.bstrAccount);
                        ismanorder.Add(id, true);
                    }
                }
            }
            // if this is a cancel notification, pass along
            if (stat == STIOrderStatus.osSTICanceled)
            {
                // if it's a cancel, we'll have cancel id rather than order id
                // get new id
                long orderid = 0;
                if (_cancel2order.TryGetValue(id, out orderid))
                {
                    newcancel(orderid);

                }
                else if (sterid2tlid.ContainsKey(structOrderUpdate.nOrderRecordId))
                {
                    newcancel(sterid2tlid[structOrderUpdate.nOrderRecordId]);
                }
                else
                {
                    debug("exchange_or_user cancel sent with unknown id: " + id);
                    string clid = structOrderUpdate.bstrClOrderId == null ? string.Empty : structOrderUpdate.bstrClOrderId;
                    v("order information.  clid:" + clid + " status: " + stat.ToString() + " nrecid: " + structOrderUpdate.nOrderRecordId + " other: " + Util.DumpObjectProperties(structOrderUpdate));

                }
                return;
            }
            else if (stat == STIOrderStatus.osSTIPendingCancel)
            {
                string clid = structOrderUpdate.bstrClOrderId == null ? string.Empty : structOrderUpdate.bstrClOrderId;
                v("order information.  clid:" + clid + " status: " + stat.ToString() + " nrecid: " + structOrderUpdate.nOrderRecordId + " other: " + Util.DumpObjectProperties(structOrderUpdate));
                return;
            }
            // don't notify for same order more than once
            if (_onotified.Contains(id))
            {
                string clid = structOrderUpdate.bstrClOrderId == null ? string.Empty : structOrderUpdate.bstrClOrderId;
                v("order information.  clid:" + clid + " status: " + stat.ToString() + " nrecid: " + structOrderUpdate.nOrderRecordId+" other: "+Util.DumpObjectProperties(structOrderUpdate));
                return;
            }
            if (structOrderUpdate.bstrLogMessage.Contains("REJ"))
                debug(id+" "+structOrderUpdate.bstrLogMessage);
            o.id = id;
            o.size = structOrderUpdate.nQuantity;
            o.side = structOrderUpdate.bstrSide == "B";
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
            v("order acknowledgement: " + o.ToString()+" status: "+stat.ToString()+" id: "+id+" nrecid: "+structOrderUpdate.nOrderRecordId);
            updateidmap(structOrderUpdate.nOrderRecordId, o.id);
            if (RegSHOShorts)
                sho.GotOrder(o);
            tl.newOrder(o);
            
        }

        Dictionary<int, long> sterid2tlid = new Dictionary<int, long>();
        Dictionary<long, int> tl2sterid = new Dictionary<long, int>();

        void updateidmap(int sterid, long tlid)
        {
            long id;
            int nrec;
            // do sterling map first
            if (sterid2tlid.TryGetValue(sterid, out id))
            {
                if (id != tlid)
                {
                    debug("unexpected id change, sterid: " + sterid + " tlid: " + id + " newtlid: " + tlid);
                    sterid2tlid[sterid] = tlid;
                }
            }
            else // new id
            {
                sterid2tlid.Add(sterid, tlid);
                tl2sterid.Add(tlid, sterid);
            }

            // then check tl map (generally not necessary)
            if (tl2sterid.TryGetValue(tlid, out nrec))
            {
                if (nrec != sterid)
                {
                    debug("unexpected id change, tlid: " + tlid + " sterid: " + nrec+ " newsterid: " + sterid);
                    tl2sterid[tlid] = sterid;
                }
            }
        }



        Position[] tl_gotSrvPosList(string account)
        {
            // otherwise return positions as sterling sees them
            return pt.ToArray();
        }

        int _lasttime = 0;

        object locker = new object();
        void doquote(ref structSTIQuoteUpdate q)
        {
            string ssym = q.bstrSymbol;
            string sym = getfullsymbolname(ssym);
            Tick k = new TickImpl(sym);
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize / 100;
            k.os = q.nAskSize / 100;
            k.ex = GetExPretty(q.bstrExch);
            k.be = GetExPretty(q.bstrBidExch);
            k.oe = GetExPretty(q.bstrAskExch);
            int now = Convert.ToInt32(q.bstrUpdateTime);
            k.date = Util.ToTLDate(DateTime.Now);
            //int sec = now % 100;
            k.time = now;

            // we don't want to simply return on out-of-order ticks because it'll prevent processing
            // of the mdx messages further in this function.
            if (!IgnoreOutOfOrderTicks || (k.time > _lasttime))
            {
                _lasttime = k.time;
                k.trade = (decimal)q.fLastPrice;
                k.size = q.nLastSize;
                // execute orders if papertrade is enabled
                if (isPaperTradeEnabled)
                    ptt.newTick(k);
                // notify clients of tick
                if (!_imbalance || (_imbalance && k.isValid))
                    tl.newTick(k);
            }

            /////////////////////////
            // MDX Processing
            /////////////////////////
            if (q.nMdxMsgType == 1)
            {
                if (VerboseDebugging)
                    debug(q.bstrUpdateTime
                    + "  Received Regulatory Imbalance for: " + q.bstrSymbol
                    + "  ValidIntradayMarketImb: " + q.bValidIntradayMktImb
                    + "  ValidMktImb: " + q.bValidMktImb
                    + "  Imbalance: " + q.nImbalance
                    + "  iMktImbalance: " + q.nIntradayMktImbalance
                    + "  MktImbalance: " + q.nMktImbalance);

                int time;
                if (int.TryParse(q.bstrUpdateTime, out time))
                {
                    Imbalance imb = new ImbalanceImpl(q.bstrSymbol, GetExPretty(q.bstrExch), q.nImbalance, time, 0, 0, 0);
                    tl.newImbalance(imb);
                }
            }
            else if (q.nMdxMsgType == 2)
            {
                if (VerboseDebugging)
                    debug(q.bstrUpdateTime
                    + "  Received Informational Imbalance for: " + q.bstrSymbol
                    + "  ValidIntradayMarketImb: " + q.bValidIntradayMktImb
                    + "  ValidMktImb: " + q.bValidMktImb
                    + "  Imbalance: " + q.nImbalance
                    + "  iMktImbalance: " + q.nIntradayMktImbalance
                    + "  MktImbalance: " + q.nMktImbalance);

                int time;
                if (int.TryParse(q.bstrUpdateTime, out time))
                {
                    Imbalance imb = new ImbalanceImpl(q.bstrSymbol, GetExPretty(q.bstrExch), 0, time, 0, 0, q.nIntradayMktImbalance);
                    tl.newImbalance(imb);
                }
            }
            else if (q.nMdxMsgType == 3)
            {
                if (VerboseDebugging)
                    debug(q.bstrUpdateTime
                    + "  Received Halt/Delay for: " + q.bstrSymbol
                    + "  Status: " + q.bstrHaltResumeStatus
                    + "  Reason: " + q.bstrHaltResumeReason);

                int time;
                if (int.TryParse(q.bstrUpdateTime, out time))
                {
                    HaltResume h = new HaltResumeImpl(q.bstrSymbol, GetExPretty(q.bstrExch), time, q.bstrHaltResumeStatus, q.bstrHaltResumeReason);
                    for (int clientNumber = 0; clientNumber < tl.NumClients; clientNumber++)
                        tl.TLSend(HaltResumeImpl.Serialize(h), MessageTypes.HALTRESUME, clientNumber);
                }
            }
            else if (q.nMdxMsgType == 4)
            {
                if (VerboseDebugging)
                    debug(q.bstrUpdateTime
                    + "  Received Indication for: " + q.bstrSymbol
                    + "  ValidIndicators: " + q.bValidIndicators
                    + "  IndicatorHigh: " + q.fIndicatorHigh
                    + "  IndicatorLow: " + q.fIndicatorLow);

                int time;
                if (int.TryParse(q.bstrUpdateTime, out time))
                {
                    Indication ind = new IndicationImpl(q.bstrSymbol, GetExPretty(q.bstrExch), time, q.bValidIndicators, (decimal)q.fIndicatorHigh, (decimal)q.fIndicatorLow);
                    for (int clientNumber = 0; clientNumber < tl.NumClients; clientNumber++)
                        tl.TLSend(IndicationImpl.Serialize(ind), MessageTypes.INDICATION, clientNumber);
                }
            }

        }

        void stiQuote_OnSTIQuoteUpdate(ref structSTIQuoteUpdate q)
        {
            if (UseXmlMode) return;
            doquote(ref q);

        }

        void dosnap(ref structSTIQuoteSnap q)
        {
            TickImpl k = new TickImpl();
            k.symbol = q.bstrSymbol;
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize / 100;
            k.os = q.nAskSize / 100;
            k.ex = GetExPretty(q.bstrExch);
            k.be = GetExPretty(q.bstrBidExch);
            k.oe = GetExPretty(q.bstrAskExch);
            int now = Convert.ToInt32(q.bstrUpdateTime);
            k.date = Util.ToTLDate(DateTime.Now);
            k.time = now;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            tl.newTick(k);
        }

        void stiQuote_OnSTIQuoteSnap(ref structSTIQuoteSnap q)
        {
            if (UseXmlMode) return;
            dosnap(ref q);
        }
        IdTracker _idt = new IdTracker();
        long tl_gotSrvFillRequest(Order o)
        {

            if (o.id == 0) o.id = _idt.AssignId;
            ost.sendorder(o);
            return (long)MessageTypes.OK;
        }

        bool _posupdatelimit = true;
        public bool LimitPositionUpdates { get { return _posupdatelimit; } set { _posupdatelimit = value; } }
        bool _receivedorder = false;

        void dopositionupdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            // symbol
            string ssym = structPositionUpdate.bstrSym;
            string sym = ssym;
            if (UseSubscribedSymbolForNotify)
            {
                sym = getfullsymbolname(ssym);
            }
            if (!UseSubscribedSymbolForNotify || (sym == ssym))
            {
                // only do for non-equities
                if (structPositionUpdate.bstrInstrument == "O")
                    sym = ssym + " "+SecurityType.OPT;
                else if (structPositionUpdate.bstrInstrument == "X")
                    sym = ssym + " " + SecurityType.CASH;
                else if (structPositionUpdate.bstrInstrument == "F")
                    sym = ssym + " " + SecurityType.FUT;
            }

            // size
            int size = (structPositionUpdate.nSharesBot - structPositionUpdate.nSharesSld) + structPositionUpdate.nOpeningPosition;
            // price
            decimal price = size == 0 ? 0 :Math.Abs((decimal)structPositionUpdate.fPositionCost / size);
            // closed pl
            decimal cpl = (decimal)structPositionUpdate.fReal;
            // account
            string ac = structPositionUpdate.bstrAcct;
            // build position
            Position p = new PositionImpl(sym, price, size, cpl, ac);
            // track it
            bool adjust = false;
            if (_posupdatelimit && _receivedorder)
                ;
            else
            {
                pt.NewPosition(p);
                adjust = true;
            }
            if (RegSHOShorts)
                sho.GotPosition(p);
            // track account
            if (!accts.Contains(ac))
                accts.Add(ac);
            if (VerboseDebugging)
                debug("new position recv: " + p.ToString()+" info: "+pt[p.Symbol,ac]+" acct: "+ac);
        }

        List<string> accts = new List<string>();
        void stiPos_OnSTIPositionUpdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            if (UseXmlMode)
                return;
            dopositionupdate(ref structPositionUpdate);
        }

        void stiEvents_OnSTIOrderUpdateMsg(STIOrderUpdateMsg oSTIOrderUpdateMsg)
        {
            throw new NotImplementedException();
        }

        void stiEvents_OnSTIOrderConfirmMsg(STIOrderConfirmMsg oSTIOrderConfirmMsg)
        {
            throw new NotImplementedException();
        }

        public string GetExPretty(string val)
        {
            return GetExType(val).ToString();
        }

        public STEREXCH GetExType(string val)
        {
            try
            {
                char c = val.ToCharArray(0, 1)[0];
                int ascii = (int)c;
                return (STEREXCH)ascii;
            }
            catch { }
            return STEREXCH.NONE;
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }

        public event DebugDelegate SendDebug;
    }

    public enum STEREXCH
    {
        NONE = -1,
        AMEX = 65,
        BSTN = 66,
        CNCI = 67,
        MWST = 77,
        NYSE = 78,
        PACE = 79,
        NSDS = 83,
        NSDT = 84,
        NSDQ = 81,
        CBOE = 87,
        PSE = 88,
        CMPO = 79,
        CMPE = 42,
    }

}
