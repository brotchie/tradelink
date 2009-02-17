using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using SterlingLib;
using TradeLink.API;

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
        TLServer_WM tl = new TLServer_WM(TLTypes.LIVEBROKER);
        const string PROGRAM = "SterServer ";
        Timer tt = new Timer();


        public SterMain()
        {
            InitializeComponent();
            tt.Interval = 1000;
            tt.Enabled = true;
            tt.Tick += new EventHandler(tt_Tick);
            tt.Start();

            stiEvents.SetOrderEventsAsStructs(true);

            stiEvents.OnSTIOrderUpdate += new _ISTIEventsEvents_OnSTIOrderUpdateEventHandler(stiEvents_OnSTIOrderUpdate);
            stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
            stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
            stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
            stiQuote.OnSTIQuoteSnap += new _ISTIQuoteEvents_OnSTIQuoteSnapEventHandler(stiQuote_OnSTIQuoteSnap);

            tl.BrokerName = Brokers.Sterling;
            tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.gotSrvPosList += new PositionArrayDelegate(tl_gotSrvPosList);
            tl.RegisterStocks += new DebugDelegate(tl_RegisterStocks);
            tl.OrderCancelRequest += new UIntDelegate(tl_OrderCancelRequest);

            debug(PROGRAM + Util.TLSIdentity());
            FormClosing += new FormClosingEventHandler(SterMain_FormClosing);
        }


        void SterMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            stiQuote.DeRegisterAllQuotes();
        }



        void stiEvents_OnSTIOrderUpdateMsg(STIOrderUpdateMsg oSTIOrderUpdateMsg)
        {
            throw new NotImplementedException();
        }

        void stiEvents_OnSTIOrderConfirmMsg(STIOrderConfirmMsg oSTIOrderConfirmMsg)
        {
            throw new NotImplementedException();
        }



        void tl_RegisterStocks(string msg)
        {
            symquotes = msg;
            qc++;
        }
        int qc = 0;
        int qr = 0;
        string symquotes = "";

        uint or = 0;
        Dictionary<uint, string> idacct = new Dictionary<uint, string>();
        void tt_Tick(object sender, EventArgs e)
        {
            // orders
            while (or < oc)
            {
                STIOrder order = new STIOrder();
                if (or == oq.Length) or = 0;
                Order o = oq[or++];
                order.LmtPrice = (double)o.price;
                order.StpPrice = (double)o.stopp;
                order.Destination = o.Exchange;
                order.Side = o.side ? "B" : "S";
                order.Symbol = o.symbol;
                order.Quantity = o.UnsignedSize;
                order.Account = o.Account;
                order.Destination = o.Exchange;
                order.Tif = o.TIF;
                order.PriceType = o.isMarket ? STIPriceTypes.ptSTIMkt : (o.isLimit ? STIPriceTypes.ptSTILmt : STIPriceTypes.ptSTISvrStp);
                order.ClOrderID = o.id.ToString();
                int err = order.SubmitOrder();
                string tmp = "";
                if ((err==0) && (!idacct.TryGetValue(o.id, out tmp)))
                    idacct.Add(o.id,order.Account);
                if (err < 0)
                    debug("Error sending order: " + Util.PrettyError(tl.BrokerName, err) + o.ToString());
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
            if (ic > ir)
            {
                if (ir == idq.Length) ir = 0;
                uint number = idq[ir++];
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
        uint[] idq = new uint[MAXRECORD];
        uint ic = 0;
        uint ir = 0;
        void tl_OrderCancelRequest(uint number)
        {
            if (ic == idq.Length) ic = 0;
            idq[ic++] = number;
        }



        void stiEvents_OnSTITradeUpdate(ref structSTITradeUpdate t)
        {
            Trade f = new TradeImpl();
            f.symbol = t.bstrSymbol;
            f.Account = t.bstrAccount;
            f.id = Convert.ToUInt32(t.bstrClOrderId);
            f.xprice = (decimal)t.fExecPrice;
            f.xsize = t.nQuantity;
            long now = Convert.ToInt64(t.bstrUpdateTime);
            f.xsec = (int)(now % 100);
            long rem = (now - f.xsec) / 100;
            f.side = t.bstrSide == "B";
            f.xtime = (int)(rem % 10000);
            f.xdate = (int)((rem - f.xtime) / 10000);
            tl.newFill(f);
        }

        void stiEvents_OnSTIOrderUpdate(ref structSTIOrderUpdate structOrderUpdate)
        {
            Order o = new OrderImpl();
            o.symbol = structOrderUpdate.bstrSymbol;
            o.id = Convert.ToUInt32(structOrderUpdate.bstrClOrderId);
            o.size = structOrderUpdate.nQuantity;
            o.side = o.size > 0;
            o.price = (decimal)structOrderUpdate.fLmtPrice;
            o.stopp = (decimal)structOrderUpdate.fStpPrice;
            o.TIF = structOrderUpdate.bstrTif;
            o.Account = structOrderUpdate.bstrAccount;
            o.ex= structOrderUpdate.bstrDestination;
            tl.newOrder(o);

        }


        Position[] tl_gotSrvPosList(string account)
        {
            stiPos.RegisterForPositions();
            return new PositionImpl[0];
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
            k.sec = now % 100;
            k.time = (now - k.sec) / 100;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            tl.newTick(k);
        }

        void stiQuote_OnSTIQuoteSnap(ref structSTIQuoteSnap q)
        {
            TickImpl k = new TickImpl(q.bstrSymbol);
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
            k.sec = now % 100;
            k.time = (now - k.sec) / 100;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            tl.newTick(k);
        }

        const int MAXRECORD = 5000;
        Order[] oq = new OrderImpl[MAXRECORD];
        uint oc = 0;
        void tl_gotSrvFillRequest(Order o)
        {
            if (o.id ==0 ) o.id = (uint)(DateTime.Now.Ticks + acct.GetHashCode());
            oq[oc++] = o;
        }

        string acct = "";
        void stiPos_OnSTIPositionUpdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            
        }


        void debug(string msg)
        {
            if (msgbox.InvokeRequired)
                msgbox.Invoke(new DebugDelegate(debug), new object[] { msg });
            else
            {
                msgbox.Items.Add(msg);
                msgbox.SelectedIndex = msgbox.Items.Count - 1;
            }
        }
    }
}
