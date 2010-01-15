using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using SterlingLib;
using TradeLink.API;
using TradeLink.AppKit;

namespace SterServer
{
    public partial class SterMain : Form
    {
        // basic structures needed for operation
        STIEvents stiEvents = new STIEvents();
        STIOrderMaint stiOrder = new STIOrderMaint();
        STIPosition stiPos = new STIPosition();
        STIQuote stiQuote = new STIQuote();
        STIBook stiBook = new STIBook();
        TLServer_WM tl = new TLServer_WM();
        public const string PROGRAM = "SterServer ";
        Timer tt = new Timer();
        bool imbalance = false;
        PositionTracker pt = new PositionTracker();
        DebugControl _dc = new DebugControl(true);
        public SterMain()
        {
            InitializeComponent();
            _dc.Parent = this;
            _dc.Dock = DockStyle.Fill;
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("report", new EventHandler(report));
            tt.Interval = 10;
            tt.Enabled = true;
            tt.Tick += new EventHandler(tt_Tick);
            tt.Start();

            stiEvents.SetOrderEventsAsStructs(true);

            stiEvents.OnSTIOrderUpdate += new _ISTIEventsEvents_OnSTIOrderUpdateEventHandler(stiEvents_OnSTIOrderUpdate);
            stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
            stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
            stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
            stiQuote.OnSTIQuoteSnap += new _ISTIQuoteEvents_OnSTIQuoteSnapEventHandler(stiQuote_OnSTIQuoteSnap);
            stiPos.GetCurrentPositions();

            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
            tl.newProviderName = Providers.Sterling;
            tl.newSendOrderRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.newPosList += new PositionArrayDelegate(tl_gotSrvPosList);
            tl.newRegisterStocks += new DebugDelegate(tl_RegisterStocks);
            tl.newOrderCancelRequest += new UIntDelegate(tl_OrderCancelRequest);
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newUnknownRequest += new UnknownMessageDelegate(tl_newUnknownRequest);
            tl.newImbalanceRequest += new VoidDelegate(tl_newImbalanceRequest);
            debug(PROGRAM + Util.TLSIdentity());
            FormClosing += new FormClosingEventHandler(SterMain_FormClosing);
        }

        void report(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _msgs.ToString(), null, new AssemblaTicketWindow.LoginSucceedDel(success), false);
        }

        StringBuilder _msgs = new StringBuilder();
        void success(string u, string p)
        {
            _msgs = new StringBuilder();
        }

        void debug(string msg)
        {
            _dc.GotDebug(msg);
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
            return f.ToArray();
        }


        void SterMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                stiQuote.DeRegisterAllQuotes();
            }
            catch (Exception)
            {
                // incase stering was already closed 
            }
        }



        void stiEvents_OnSTIOrderUpdateMsg(STIOrderUpdateMsg oSTIOrderUpdateMsg)
        {
            throw new NotImplementedException();
        }

        void stiEvents_OnSTIOrderConfirmMsg(STIOrderConfirmMsg oSTIOrderConfirmMsg)
        {
            throw new NotImplementedException();
        }

        const int MAXRECORD = 5000;
        RingBuffer<Order> _orderq = new RingBuffer<Order>(MAXRECORD);
        RingBuffer<uint> _cancelq = new RingBuffer<uint>(MAXRECORD);


        void tl_RegisterStocks(string msg)
        {
            symquotes = msg;
            qc++;
        }
        int qc = 0;
        int qr = 0;
        string symquotes = "";

        Dictionary<uint, string> idacct = new Dictionary<uint, string>();
        void tt_Tick(object sender, EventArgs e)
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
                    order.Side = o.side ? "B" : "S";
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
                        debug("Error sending order: " + Util.PrettyError(tl.newProviderName, err) + o.ToString());
                    if (err == -1)
                        debug("Make sure you have set the account in sending program.");
                }

                // quotes
                if (qc > qr)
                {
                    foreach (string sym in symquotes.Split(','))
                        stiQuote.RegisterQuote(sym, "*");
                    qr = qc;
                }

                // cancels
                if (!_cancelq.isEmpty)
                {
                    uint number = _cancelq.Read();
                    string acct = "";
                    if (idacct.TryGetValue(number, out acct))
                    {
                        stiOrder.CancelOrder(acct, 0, number.ToString(), DateTime.Now.Ticks.ToString() + acct);
                        tl.newOrderCancel(number);
                    }
                    else
                        debug("No record of id: " + number.ToString());
                }
            }
            catch (Exception ex)
            {
                debug(ex.Message+ex.StackTrace);
            }


            
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
            if (!uint.TryParse(t.bstrClOrderId, out id))
                f.id = id;
            f.xprice = (decimal)t.fExecPrice;
            f.xsize = t.nQuantity;
            long now = Convert.ToInt64(t.bstrUpdateTime);
            int xsec = (int)(now % 100);
            long rem = (now - xsec) / 100;
            f.side = t.bstrSide == "B";
            f.xtime = ((int)(rem % 10000))*100+xsec;
            f.xdate = (int)((now - f.xtime) / 1000000);
            tl.newFill(f);
        }

        void stiEvents_OnSTIOrderUpdate(ref structSTIOrderUpdate structOrderUpdate)
        {
            Order o = new OrderImpl();
            o.symbol = structOrderUpdate.bstrSymbol;
            uint id = 0;
            if (!uint.TryParse(structOrderUpdate.bstrClOrderId, out id))
                id = (uint)structOrderUpdate.nOrderRecordId;
            o.id = id;
            o.size = structOrderUpdate.nQuantity;
            o.side = o.size > 0;
            o.price = (decimal)structOrderUpdate.fLmtPrice;
            o.stopp = (decimal)structOrderUpdate.fStpPrice;
            o.TIF = structOrderUpdate.bstrTif;
            o.Account = structOrderUpdate.bstrAccount;
            o.ex= structOrderUpdate.bstrDestination;
            long now = Convert.ToInt64(structOrderUpdate.bstrUpdateTime);
            int xsec = (int)(now % 100);
            long rem = (now - xsec) / 100;
            o.time = ((int)(rem % 10000)) * 100 + xsec;
            o.date = (int)((rem - o.time) / 10000);
            tl.newOrder(o);

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
            if (q.bstrExch!="*")
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
            tl.newTick(k);
            if ((q.bValidMktImb==0)||!imbalance) return;
            tl.newImbalance(new ImbalanceImpl(k.symbol, k.ex, q.nMktImbalance, k.time, 0, 0,0));
           
        }

        void stiQuote_OnSTIQuoteSnap(ref structSTIQuoteSnap q)
        {
            TickImpl k = new TickImpl();
            k.symbol = q.bstrSymbol;
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize/100;
            k.os = q.nAskSize/100;
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
            tl.newTick(k);
        }
        IdTracker _idt = new IdTracker();
        void tl_gotSrvFillRequest(Order o)
        {

            if (o.id == 0) o.id = _idt.AssignId ;
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



    }
}
