using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using SterlingLib;

namespace SterServer
{
    public partial class SterMain : Form
    {
        // basic structures needed for operation
        STIEvents stiEvents = new STIEvents();
        STIOrderMaint stiOrder = new STIOrderMaint();
        STIPosition stiPos = new STIPosition();
        STIQuote stiQuote = new STIQuote();
        TradeLink_Server_WM tl = new TradeLink_Server_WM(TLTypes.LIVEBROKER);
        const string PROGRAM = "SterServer ";
        Timer tt = new Timer();


        public SterMain()
        {
            InitializeComponent();
            tt.Interval = 1000;
            tt.Enabled = true;
            tt.Tick += new EventHandler(tt_Tick);
            tt.Start();
            stiEvents.OnSTITradeUpdateMsg += new _ISTIEventsEvents_OnSTITradeUpdateMsgEventHandler(stiEvents_OnSTITradeUpdateMsg);
            stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
            stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
            stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
            
            tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.gotSrvPosList += new TradeLink_Server_WM.PositionArrayDelegate(tl_gotSrvPosList);
            tl.RegisterStocks += new DebugDelegate(tl_RegisterStocks);
            debug(PROGRAM + Util.TLSIdentity());
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
        void tt_Tick(object sender, EventArgs e)
        {
            // orders
            while (or < oc)
            {
                STIOrder order = new STIOrder();
                if (or == oq.Length) or = 0;
                Order o = oq[or++];
                order.Destination = o.Exchange;
                order.Side = o.side ? "B" : "S";
                order.Symbol = o.symbol;
                order.Quantity = o.UnSignedSize;
                order.Account = o.Account;
                order.Destination = o.Exchange;
                order.Tif = o.TIF;
                order.PriceType = o.isMarket ? STIPriceTypes.ptSTIMkt : (o.isLimit ? STIPriceTypes.ptSTILmt : STIPriceTypes.ptSTISvrStp);
                if (o.id!=0)
                    order.ClOrderID = o.id.ToString();
                int err = order.SubmitOrder();
                if (err < 0)
                    debug("Error sending order: " + Util.PrettyError(Brokers.Echo, err) + o.ToString());
                if (err == -1)
                    debug("Make sure you have set the account in sending program.");
            }

            // quotes
            if (qc > qr)
            {
                stiQuote.DeRegisterAllQuotes();
                foreach (string sym in symquotes.Split(','))
                {
                    string ex = sym.Length > 3 ? "NSDQ" : "NYSE";
                    stiQuote.RegisterQuote(sym, ex);
                }
                qr = qc;
            }

            
        }

        void stiEvents_OnSTITradeUpdateMsg(STITradeUpdateMsg c)
        {
            Order o = new Order();
            o.Account = c.Account;
            o.id = Convert.ToUInt32(c.ClOrderID);
            o.symbol = c.Symbol;
            o.TIF = c.Tif;
            o.price = (decimal)c.LmtPrice;
            o.stopp = (decimal)c.StpPrice;
            o.size = c.Quantity;
            o.Exchange = c.Destination;
            o.side = c.Side.Contains("B");
            DateTime now = DateTime.Parse(c.UpdateTime);
            o.date = Util.ToTLDate(now);
            o.time = Util.ToTLTime(now);
            o.sec = now.Second;
            tl.newOrder(o);
        }

        void stiEvents_OnSTITradeUpdate(ref structSTITradeUpdate t)
        {
            Trade f = new Trade();
            f.Account = t.bstrAccount;
            f.id = Convert.ToUInt16(t.bstrClOrderId);
            f.Currency = (Currency)Enum.Parse(typeof(Currency), t.bstrCurrency);
            f.xprice = (decimal)t.fExecPrice;
            f.xsize = t.nQuantity;
            DateTime now = DateTime.Parse(t.bstrUpdateTime);
            f.xdate = Util.ToTLDate(now);
            f.xtime = Util.ToTLTime(now);
            f.xsec = now.Second;
            tl.newFill(f);
        }

        Position[] tl_gotSrvPosList(string account)
        {
            stiPos.RegisterForPositions();
            return new Position[0];
        }

        void stiQuote_OnSTIQuoteUpdate(ref structSTIQuoteUpdate q)
        {
            Tick k = new Tick(q.bstrSymbol);
            k.bid = (decimal)q.fBidPrice;
            k.ask = (decimal)q.fAskPrice;
            k.bs = q.nBidSize;
            k.os = q.nAskSize;
            k.be = q.bstrBidExch;
            k.oe = q.bstrAskExch;
            DateTime now = DateTime.Parse(q.bstrUpdateTime);
            k.date = Util.ToTLDate(now);
            k.time = Util.ToTLTime(now);
            k.sec = now.Second;
            k.trade = (decimal)q.fLastPrice;
            k.size = q.nLastSize;
            tl.newTick(k);
        }
        const int MAXRECORD = 5000;
        Order[] oq = new Order[MAXRECORD];
        uint oc = 0;
        void tl_gotSrvFillRequest(Order o)
        {
            oq[oc++] = o;
        }

        string acct = "";
        void stiPos_OnSTIPositionUpdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            debug("got position "+structPositionUpdate.ToString());
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
