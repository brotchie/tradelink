using System;
using System.Collections.Generic;
using System.Text;
using VBRediClasses;
using TradeLink.API;
using TradeLink.Common;
using System.Threading;
namespace ServerRedi
{
    public class ServerRedi 
    {
        VBCacheClass _cc;
        VBOrderClass _oc;
        List<string> _accts = new List<string>();
        public string[] Accounts { get { return _accts.ToArray(); } set { _accts.Clear(); _accts.AddRange(value); } }
        string _userid;
        string _pwd;
        bool _conn = false;
        public bool isConnected { get { return _conn; } }
        const int MAXRECORD = 5000;
        RingBuffer<bool> _newsyms = new RingBuffer<bool>(5);
        RingBuffer<Order> _neworders = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<long> _newcancel = new RingBuffer<long>(MAXRECORD);
        VBCacheClass _positionCache;
        VBCacheClass _messageCache;
        List<long> _onotified = new List<long>(MAXRECORD);
        string _newsymlist = string.Empty;
        Thread _bw;
        int _SLEEP = 50;
        bool _bwgo = true;
        Dictionary<string, long> OrderIdDict;
        PositionTracker pt = new PositionTracker();
        List<string> accts = new List<string>();

        bool _papertrade = false;
        public bool isPaperTradeEnabled { get { return _papertrade; } set { _papertrade = value; } }
        PapertradeTracker ptt = new PapertradeTracker();
        bool _papertradebidask = false;
        public bool isPaperTradeUsingBidAsk { get { return _papertradebidask; } set { _papertradebidask = value; } }



        
        public ServerRedi(TLServer tls) : this(tls, 50) { }
        public ServerRedi(TLServer tls, int sleepvalue)
        {
            tl = tls;
            _bw = new Thread(new ParameterizedThreadStart(doqueues));
            tl.newProviderName = Providers.REDI;
            tl.newRegisterSymbols += new SymbolRegisterDel(tl_newRegisterSymbols);
            tl.newSendOrderRequest += new OrderDelegateStatus(ServerRedi_newSendOrderRequest);
            tl.newOrderCancelRequest += new LongDelegate(ServerRedi_newOrderCancelRequest);
            tl.newFeatureRequest += new MessageArrayDelegate(ServerRedi_newFeatureRequest);
            tl.newPosList += new PositionArrayDelegate(ServerRedi_gotSrvPosList);
            tl.newAcctRequest += new StringDelegate(ServerRedi_newAccountRequest);
        }

        bool _noverb = true;
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; tl.VerboseDebugging = value; } }
        void v(string msg)
        {
            if (_noverb) return;
            debug(msg);
        }

        void tl_newRegisterSymbols(string client, string symbols)
        {
            debug("Subscribe request: " + symbols);
            if (!isConnected)
            {
                debug("not connected.");
                return;
            }
            
            // save list of symbols to subscribe
            _newsymlist = tl.AllClientBasket.ToString();
            // notify other thread to subscribe to them
            _newsyms.Write(true);
        }
        void ServerRedi_newOrderCancelRequest(long val)
        {
            v("order cancel request received for: " + val);
            _newcancel.Write(val);
        }
        Position[] ServerRedi_gotSrvPosList(string account)
        {
            v("position list request received for account: " + account);
            return pt.ToArray();
        }
        string ServerRedi_newAccountRequest()
        {
            v("accounts request received.");
            return string.Join(",", accts.ToArray());
        }
        /*
        bool isMsgTableOpen = false;
         *                             if (isMsgTableOpen)
                            {
                                _cc.VBRevokeObject(ref se);
                            }
                            _cc.VBSubmit(ref isMsgTableOpen, ref se);
         */
        object vret;
        public string Account { get { return _accts.Count>0 ? _accts[0] : string.Empty; } set { if (!_accts.Contains(value)) _accts.Add(value); } }

        string gettiftype(Order o)
        {
            string type = "Invalid";
            switch (o.ValidInstruct)
            {
                case OrderInstructionType.DAY:
                    type = "Day";
                    break;
                case OrderInstructionType.GTC:
                    type = "Gtc";
                    break;
                case OrderInstructionType.OPG:
                    type = "OPG";
                    break;
                case OrderInstructionType.MOC:
                    type = "MOC";
                    break;
            }

            if (type == "Invalid")
                debug(o.symbol + " error unknown/invalid tif type for: " + o);

            return type;
        }

        string getpricetype(Order o)
        {
            string type = "Invalid";
            if (o.isStop && o.isLimit)
                type = "Stop Limit";
            else if (o.isStop)
                type = "Stop";
            else if (o.isLimit)
                type = "Limit";
            else if (o.isMarket)
                type = "Market";

            if (o.ValidInstruct != OrderInstructionType.Invalid)
            {
                if (o.ValidInstruct == OrderInstructionType.MOC)
                    type = "Market Close";
            }

            if (type == "Invalid")
                debug(o.symbol + " error unknown/invalid price type for: " + o);

            return type;

        }

        void doqueues(object obj)
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
                    Basket b = BasketImpl.FromString(_newsymlist);
                    object err = null;
                    foreach (Security s in b)
                    {
                        try
                        {
                            string se= string.Empty;
                            if (vret == null)
                            {
                                vret = _cc.VBRediCache.Submit("L1", "true", ref err);
                                checkerror(ref err, "submit");
                            }
                            _cc.VBRediCache.AddWatch(0, s.Symbol, string.Empty, ref err);
                            checkerror(ref err,"watch");
                        }
                        catch (Exception ex)
                        {
                            debug(s.Symbol + " error subscribing: " + ex.Message + ex.StackTrace);
                        }
                    }
                    debug("registered: " + _newsymlist);
                }
                while (!_neworders.isEmpty)
                {                  
                    Order o = _neworders.Read();
                    v("received order: " + o.ToString());
                    if (o.Account == string.Empty)
                        o.Account = Account;

                    if (isPaperTradeEnabled)
                    {
                        ptt.sendorder(o);
                    }
                    else
                    {
                        RediLib.ORDER rediOrder = new RediLib.ORDERClass();


                        // get any current position in symbol
                        Position p = pt[o.symbol];
                        // determine if a sell order should be a long exit or a short entry
                        string side = !o.side && !p.isLong ? "Sell Short" : (o.side ? "Buy" : "Sell");

                        rediOrder.Account = o.Account;
                        rediOrder.UserID = _userid;
                        rediOrder.Password = _pwd;

                        rediOrder.TIF = gettiftype(o);
                        rediOrder.Side = side;
                        rediOrder.Symbol = o.symbol;
                        if (o.ex == string.Empty)
                            o.ex = o.symbol.Length > 3 ? "ARCA" : "NYSE";
                        rediOrder.Exchange = o.ex;
                        rediOrder.Quantity = o.UnsignedSize;
                        rediOrder.Price = o.price.ToString();
                        rediOrder.PriceType = getpricetype(o);

                        rediOrder.StopPrice = o.stopp;
                        rediOrder.Memo = o.comment;
                        rediOrder.Warning = false;
                        object err = null;
                        object transId = null;
                        bool IsSuccessful = rediOrder.Submit2(ref transId, ref err);
                        if (IsSuccessful)
                        {
                            v("successfully sent order: " + o.ToString());
                            object err1 = null;
                            _messageCache.VBRediCache.AddWatch(1, o.symbol, "", ref err1);

                            if (!(err1 == null))
                                debug("FAILED open of Table! Table : Message  Error:" + err1.ToString());
                            else
                                v("successfully watching symbol: " + o.symbol);

                        }
                        else
                        {
                            v("error sending order: " + o.ToString());
                            if (!(err == null))
                                debug("order submission was not successful: " + err.ToString());
                        }
                    }
                }                
                while (!_newcancel.isEmpty)
                {
                    long id = _newcancel.Read();
                    v("received cancel request: " + id);
                    if (isPaperTradeEnabled)
                    {
                        ptt.sendcancel(id);
                    }
                    else
                    {
                        object err = null;
                        _messageCache.VBRediCache.Cancel(id, ref err);
                        if (err != null)
                            v("error canceling id: " + id);
                        else
                            v("cancel request sent for: " + id);
                    }
                }
                if (_newcancel.isEmpty && _neworders.isEmpty && _newsyms.isEmpty)
                    Thread.Sleep(_SLEEP);
            }
            
        }
        void MessageCache_CacheEvent(int action, int row)
        {
            switch (action)
            {
                case 1: //CN_Submit                    
                    break;
                case 4: //CN_Insert
                    {
                        try 
                        {
                            int i = row;
                            int err = 0;
                            object cv = null;                            
                            string orderReferenceNumber = String.Empty;
                            Order o = new OrderImpl();
                            _messageCache.VBGetCell(row, "SYMBOL", ref cv, ref err);
                            if (!(cv == null))
                            {
                                o.symbol = cv.ToString();
                            }
                            _messageCache.VBGetCell(row, "SIDE", ref cv, ref err);
                            if (!(cv == null))
                            {
                                v("order side: "+cv.ToString());
                                if (cv.ToString() == "BUY")
                                {
                                    o.side = true;
                                }
                                else if (cv.ToString() == "SELL")
                                {
                                    o.side = false;
                                }
                                else if (cv.ToString().Contains("SHORT"))
                                {
                                    o.side = false;
                                }
                            }
                            _messageCache.VBGetCell(row, "QUANTITY", ref cv, ref err);
                            if (!(cv == null))
                            {
                                o.size = int.Parse(cv.ToString());
                            }
                            _messageCache.VBGetCell(row, "PRICE", ref cv, ref err);
                            if (!(cv == null))
                            {
                                o.price = decimal.Parse(cv.ToString());
                            }
                            _messageCache.VBGetCell(row, "STOPPRICE", ref cv, ref err);
                            if (!(cv == null))
                            {
                                o.stopp = decimal.Parse(cv.ToString());
                            }
                            _messageCache.VBGetCell(row, "ACCOUNT", ref cv, ref err);
                            if (!(cv == null))
                            {
                                o.Account = cv.ToString();
                            }
                            _messageCache.VBGetCell(row, "BRSEQ", ref cv, ref err);
                            if (!(cv == null))
                            {
                                orderReferenceNumber = cv.ToString();
                            }
                            _messageCache.VBGetCell(row, "Status", ref cv, ref err);                            
                            if (!(cv == null))
                            {
                                if (cv.ToString() == "Open")
                                {
                                    o.id = row;                                    
                                    long now = Util.ToTLDate(DateTime.Now);
                                    int xsec = (int)(now % 100);
                                    long rem = (now - xsec) / 100;
                                    o.time = ((int)(rem % 10000)) * 100 + xsec;
                                    o.date = (int)((rem - o.time) / 10000);
                                    o.size = o.side ? o.UnsignedSize : o.UnsignedSize * -1;
                                    OrderIdDict.Add(orderReferenceNumber, (long)row);
                                    if (_onotified.Contains((int)row)) return;
                                    _onotified.Add(o.id);
                                    tl.newOrder(o);
                                    v("order ack received and sent: " + o.ToString());
                                }
                                else if (cv.ToString() == "Canceled")
                                {
                                    long id = OrderIdDict[orderReferenceNumber];
                                    tl.newCancel(id);
                                    v("order cancel ack received and sent: " + id);
                                }
                                else if (cv.ToString() == "Complete")
                                {
                                    Trade f = new TradeImpl();
                                    _messageCache.VBGetCell(row, "SYMBOL", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        f.symbol = cv.ToString();
                                    }
                                    _messageCache.VBGetCell(row, "ACCOUNT", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        f.Account = cv.ToString();
                                    }
                                    _messageCache.VBGetCell(row, "BRSEQ", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        long id = 0;
                                        if (OrderIdDict.TryGetValue(cv.ToString(), out id))
                                            f.id = id;
                                        else
                                            f.id = _idt.AssignId;
                                        f.id = id;
                                    }
                                    _messageCache.VBGetCell(row, "EXECQUANTITY", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        f.xsize = int.Parse(cv.ToString());
                                    }
                                    _messageCache.VBGetCell(row, "EXECPRICE", ref cv, ref err);
                                    if (cv != null)
                                    {
                                        f.xprice = decimal.Parse(cv.ToString());
                                    }
                                    else
                                    {
                                        v(f.symbol + " error getting EXECPRICE, err: " + err+" retrying...");
                                        _messageCache.VBGetCell(row, "EXECPRICE", ref cv, ref err);
                                        if (cv != null)
                                        {
                                            f.xprice = decimal.Parse(cv.ToString());
                                        }
                                        else
                                        {
                                            v(f.symbol + " error getting EXECPRICE, err: " + err + " retrying new method...");
                                            _messageCache.VBGetCell(row, "EXECVALUE", ref cv, ref err);
                                            bool ok = false;
                                            decimal val = 0;
                                            int usize = Math.Abs(f.xsize);
                                            if (cv != null)
                                            {
                                                
                                                if (decimal.TryParse(cv.ToString(), out val))
                                                {
                                                    
                                                    if ((val != 0) && (usize != 0))
                                                    {
                                                        ok = true;
                                                        f.xprice = val / usize;
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                            }
                                            if (!ok)
                                                v(f.symbol + " error inferring EXECPRICE, usize: " + usize + " execval: " + val);

                                        }
                                    }


                                    _messageCache.VBGetCell(row, "EXCHANGE", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        f.ex = cv.ToString();
                                    }
                                    else
                                    _messageCache.VBGetCell(row, "EXECSIDE", ref cv, ref err);
                                    if (!(cv == null))
                                    {
                                        if (cv.ToString().Contains("BUY"))
                                        {
                                            f.side = true;
                                        }
                                        else if (cv.ToString().Contains("SELL") 
                                            || cv.ToString().Contains("SHORT"))
                                        {
                                            f.side = false;
                                        }
                                        else
                                            v("invalid fill side: " + cv.ToString());
                                    }
                                    f.xtime = Util.ToTLDate();
                                    f.xdate = Util.ToTLTime();
                                    Object objErr = null;
                                    _positionCache.VBRediCache.AddWatch(2, string.Empty, f.Account, ref objErr);
                                    if (f.isValid)
                                    {
                                        pt.Adjust(f);
                                        tl.newFill(f);
                                        v("fill ack received and sent: " + f.ToString());
                                    }
                                    else
                                        debug("ignoring invalid fill: " + f.ToString());
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            debug(ex.Message+ex.StackTrace);
                        }
                    }
                    break;
                case 5: //CN_Update
                    {
                        try
                        {
                            int i = row;
                            int err = 0;
                            object cv = null;
                            string orderStatus = null;
                            _messageCache.VBGetCell(row, "Status", ref cv, ref err);
                            if (!(cv == null))
                            {
                                orderStatus = cv.ToString();
                            }
                            if (orderStatus == "Complete")
                            {
                                Trade f = new TradeImpl();
                                _messageCache.VBGetCell(row, "SYMBOL", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    f.symbol = cv.ToString();
                                }
                                _messageCache.VBGetCell(row, "ACCOUNT", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    f.Account = cv.ToString();
                                }
                                _messageCache.VBGetCell(row, "BRSEQ", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    long id = 0;
                                    if (OrderIdDict.TryGetValue(cv.ToString(), out id))
                                        f.id = id;
                                    else
                                        f.id = _idt.AssignId;
                                    f.id = id;
                                }
                                _messageCache.VBGetCell(row, "EXECPRICE", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    f.xprice = decimal.Parse(cv.ToString());
                                }
                                _messageCache.VBGetCell(row, "EXECQUANTITY", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    f.xsize = int.Parse(cv.ToString());
                                }
                                _messageCache.VBGetCell(row, "EXCHANGE", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    f.ex = cv.ToString();
                                }
                                _messageCache.VBGetCell(row, "SIDE", ref cv, ref err);
                                if (!(cv == null))
                                {
                                    if (cv.ToString() == "BUY")
                                    {
                                        f.side = true;
                                    }
                                    else if (cv.ToString() == "SELL")
                                    {
                                        f.side = false;
                                    }
                                }                                
                                long now = Util.ToTLDate(DateTime.Now);
                                int xsec = (int)(now % 100);
                                long rem = (now - xsec) / 100;                                
                                f.xtime = ((int)(rem % 10000)) * 100 + xsec;
                                f.xdate = (int)((now - f.xtime) / 1000000);
                                Object objErr = null;
                                _positionCache.VBRediCache.AddWatch(2, string.Empty, f.Account, ref objErr);
                                if (f.isValid)
                                {
                                    pt.Adjust(f);
                                    tl.newFill(f);
                                    v("fill ack received and sent: " + f.ToString());
                                }
                                else
                                    debug("ignoring invalid fill: " + f.ToString());
                            }
                            if (orderStatus == "Partial")
                            {
                            }
                        }
                        catch (Exception exc)
                        {
                            debug(exc.Message);
                        }
                    }
                    break;
                case 8: //CN_Remove
                    break;
            }
        }
        void PositionCache_CacheEvent(int action, int row)
        {
            int err = 0;
            object cv = new object();
            decimal dv = 0;            
            String symbol = string.Empty;
            decimal pl = 0.00M;
            decimal value = 0.00M;
            int size = 0;
            string acct = string.Empty;
            if (action == 1)
            {
                try
                {
                    v("for position submit row: " + row.ToString());
                    int i = row != 0 ? row - 1 : row;                    
                    _positionCache.VBGetCell(i, "SYMBOL", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        symbol = cv.ToString();
                    }
                    _positionCache.VBGetCell(i, "PANDL", ref cv, ref err);
                    if (!(cv == null))
                    {
                        if (decimal.TryParse(cv.ToString(), out dv))
                            pl = dv;
                    }
                    _positionCache.VBGetCell(i, "ACCOUNT", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        acct = cv.ToString();
                    }
                    _positionCache.VBGetCell(i, "SYMBOLPOSITION", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        int val;
                        if (int.TryParse(cv.ToString(), out val))
                            size = val;
                    }
                    _positionCache.VBGetCell(i, "ACCOUNTSYMBOLVALUE", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        if (decimal.TryParse(cv.ToString(), out dv))
                            value = dv;
                    }
                    Position p = new PositionImpl(symbol, value, size, pl, acct);
                    pt.NewPosition(p);
                    v("new position: " + p.ToString());
                    // track account
                    if (!accts.Contains(acct))
                        accts.Add(acct);
                    
                }
                catch (Exception exc)
                {
                    debug(exc.Message);
                }
            }
            else
            {
                try
                {
                    v("for position submit row: " + row.ToString());
                    int i = row;
                    _positionCache.VBGetCell(i, "SYMBOL", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        symbol = cv.ToString();
                    }
                    _positionCache.VBGetCell(i, "PANDL", ref cv, ref err);
                    if (!(cv == null))
                    {
                        if (decimal.TryParse(cv.ToString(), out dv))
                            pl = dv;
                    }
                    _positionCache.VBGetCell(i, "ACCOUNT", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        acct = cv.ToString();
                    }
                    _positionCache.VBGetCell(i, "SYMBOLPOSITION", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        int val;
                        if (int.TryParse(cv.ToString(), out val))
                            size = val;
                    }
                    _positionCache.VBGetCell(i, "ACCOUNTSYMBOLVALUE", ref cv, ref err);
                    v("getcellerr: " + err.ToString());
                    if (!(cv == null))
                    {
                        if (decimal.TryParse(cv.ToString(), out dv))
                            value = dv;
                    }
                    Position p = new PositionImpl(symbol, value, size, pl, acct);
                    pt.NewPosition(p);
                    v(p.ToString());
                    // track account
                    if (!accts.Contains(acct))
                        accts.Add(acct);
                }
                catch (Exception exc)
                {
                    debug(exc.Message);
                }
            }
        }

        IdTracker _idt = new IdTracker();
        long ServerRedi_newSendOrderRequest(Order o)
        {
            if (o.id == 0) o.id = _idt.AssignId;
            v("received order request: " + o.ToString());
            _neworders.Write(o);
            return (long)MessageTypes.OK;
        }
        MessageTypes[] ServerRedi_newFeatureRequest()
        {
            v("received feature request.");
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
        void checkerror(ref object err, string context)
        {
            if (err != null)
                v(context+" result: "+err.ToString());
            err = null;
        }
        // respond to redi events
        void VBRediCache_CacheEvent(int action, int row)
        {
            int err = 0;
            object cv = new object();
            decimal dv = 0;
            int iv = 0;
            switch (action)
            {
                case 1: //CN_SUBMIT
                    {
                        try
                        {
                            v("for submit row: " + row.ToString() + " action: " + action.ToString());
                            int i = row != 0 ? row - 1 : row;
                            Tick k = new TickImpl();
                            _cc.VBGetCell(i, "SYMBOL", ref cv, ref err);
                            v("getcellerr: " + err.ToString());
                            if (!(cv == null))
                            {
                                k.symbol = cv.ToString();
                            }
                            _cc.VBGetCell(i, "LastPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                    k.trade = dv;
                            }
                            _cc.VBGetCell(i, "BidPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                    k.bid = dv;
                            }
                            _cc.VBGetCell(i, "AskPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                    k.ask = dv;
                            }
                            _cc.VBGetCell(i, "LastSize", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (int.TryParse(cv.ToString(), out iv))
                                    k.size = iv;
                            }
                            _cc.VBGetCell(i, "BidSize", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (int.TryParse(cv.ToString(), out iv))
                                    k.bs = iv;
                            }
                            _cc.VBGetCell(i, "AskSize", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (int.TryParse(cv.ToString(), out iv))
                                    k.os = iv;
                            }                            
                            k.date = Util.ToTLDate(DateTime.Now);
                            k.time = Util.DT2FT(DateTime.Now);
                            // fill papertrade orders first
                            if (isPaperTradeEnabled)
                                ptt.newTick(k);
                            tl.newTick(k);
                        }
                        catch (Exception exc)
                        {
                            debug(exc.Message);
                        }
                    }
                    break;
                    /*
                case 4 : //CN_INSERT
                    break;
                     * */
                case 5: // CN_UPDATE
                    {
                        try
                        {
                            if (TickDebugVerbose)
                                v("for update row: " + row.ToString() + " action: " + action.ToString());
                            int i = row;                            
                            Tick k = new TickImpl();
                            _cc.VBGetCell(i, "SYMBOL", ref cv, ref err);
                            if (TickDebugVerbose)
                                v("getcellerr: " + err.ToString());                            
                            if (!(cv == null))
                            {
                                k.symbol = cv.ToString();
                            }
                            _cc.VBGetCell(i, "LastPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                {
                                    k.trade = dv;
                                    k.size = 1; // this code is just to make everything else working, i.e. Last, High and Low.
                                }
                            }
                            _cc.VBGetCell(i, "BidPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                    k.bid = dv;
                            }
                            _cc.VBGetCell(i, "AskPrice", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (decimal.TryParse(cv.ToString(), out dv))
                                    k.ask = dv;
                            }
                            //_cc.VBGetCell(i, "LastSize", ref cv, ref err);
                            //if (!(cv == null))
                            //{
                            //    if (int.TryParse(cv.ToString(), out iv))
                            //        k.size = iv;
                            //}
                            _cc.VBGetCell(i, "BidSize", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (int.TryParse(cv.ToString(), out iv))
                                    k.bs = iv;
                            }
                            _cc.VBGetCell(i, "AskSize", ref cv, ref err);
                            if (!(cv == null))
                            {
                                if (int.TryParse(cv.ToString(), out iv))
                                    k.os = iv;
                            }
                            k.date = Util.ToTLDate(DateTime.Now);
                            k.time = Util.DT2FT(DateTime.Now);                            
                            tl.newTick(k);
                        }
                        catch (Exception exc)
                        {
                            debug(exc.Message);
                        }
                    }
                    break;
                    /*
                case 7: // CN_REMOVING
                    break;
                case 8 : // CN_REMOVED
                    break;
                     */
            }
        }

        bool _tickdebugverb = false;
        public bool TickDebugVerbose { get { return _tickdebugverb; } set { _tickdebugverb = value; } }

        public event DebugDelegate SendDebug;
        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public TLServer tl;
        public bool Start(string user,string pw, string accnt)
        {
            try
            {
                debug(Util.TLSIdentity());
                debug("Attempting to start ");
                _cc = new VBCacheClass();
                _oc = new VBOrderClass();
                _cc.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(VBRediCache_CacheEvent);
                _cc.VBRediCache.UserID = user;
                _cc.VBRediCache.Password = pw;
                _messageCache = new VBCacheClass();
                _messageCache.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(MessageCache_CacheEvent);
                _messageCache.VBRediCache.UserID = user;
                _messageCache.VBRediCache.Password = pw;
                object err1 = null;
                _messageCache.VBRediCache.Submit("Message", "true", ref err1);
                _positionCache = new VBCacheClass();
                _positionCache.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(PositionCache_CacheEvent);
                _positionCache.VBRediCache.UserID = user;
                _positionCache.VBRediCache.Password = pw;
                object err2 = null;
                _messageCache.VBRediCache.Submit("Position", "true", ref err2);
                //app.OrderAck += new RediLib.EApplication_OrderAckEventHandler(app_OrderAck);
                _userid = user;
                _pwd = pw;
                Account = accnt;
                OrderIdDict = new Dictionary<string, long>();
                ptt.GotCancelEvent += new LongDelegate(tl.newCancel);
                ptt.GotFillEvent += new FillDelegate(tl.newFill);
                ptt.GotOrderEvent += new OrderDelegate(tl.newOrder);
                ptt.SendDebugEvent += new DebugDelegate(ptt_SendDebugEvent);
                ptt.UseBidAskFills = isPaperTradeUsingBidAsk;
                _bw.Start();
                if (isPaperTradeEnabled)
                    debug("Papertrading enabled.");
            }
            catch (Exception ex)
            {
                debug("error starting ");
                debug(ex.Message + ex.StackTrace);
                debug("Did you forget to login to Redi?");
                _conn = false;
                return false;
            }
            debug("Started successfully.");
            debug("User: " + _cc.VBRediCache.UserID);
            _conn = true;
            return true;
        }

        void ptt_SendDebugEvent(string msg)
        {
            if (!isPaperTradeEnabled)
                return;
            v("papertrade: " + msg);
        }        
        public void Stop()
        {
            try
            {
                _bwgo = false;
                string b = string.Empty;
                _cc.VBRevokeObject(ref b);
            }
            catch { }
            tl.Stop();
        }

    }
}
