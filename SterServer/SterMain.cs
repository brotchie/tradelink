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

        public SterMain()
        {
            InitializeComponent();
            stiEvents.OnSTITradeUpdateMsg += new _ISTIEventsEvents_OnSTITradeUpdateMsgEventHandler(stiEvents_OnSTITradeUpdateMsg);
            stiEvents.OnSTITradeUpdate += new _ISTIEventsEvents_OnSTITradeUpdateEventHandler(stiEvents_OnSTITradeUpdate);
            stiPos.OnSTIPositionUpdate += new _ISTIPositionEvents_OnSTIPositionUpdateEventHandler(stiPos_OnSTIPositionUpdate);
            stiQuote.OnSTIQuoteUpdate += new _ISTIQuoteEvents_OnSTIQuoteUpdateEventHandler(stiQuote_OnSTIQuoteUpdate);
            tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.gotSrvPosList += new TradeLink_Server_WM.PositionArrayDelegate(tl_gotSrvPosList);
            
            debug(PROGRAM + Util.TLSIdentity());
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

        void tl_gotSrvFillRequest(Order o)
        {
            STIOrder order = new STIOrder();
            order.Symbol = o.symbol;
            order.Quantity = o.UnSignedSize;
            order.Account = o.Account;
            order.Destination = o.Exchange;
            order.Tif = o.TIF;
            order.PriceType = o.isMarket ? STIPriceTypes.ptSTIMkt : (o.isLimit ? STIPriceTypes.ptSTILmt : STIPriceTypes.ptSTISvrStp);
            order.ClOrderID = o.id.ToString();
            int err = order.SubmitOrder();
            if (err < 0)
                debug("Error sending order: " + Util.PrettyError(Brokers.Echo, err) + o.ToString());

        }

        void stiPos_OnSTIPositionUpdate(ref structSTIPositionUpdate structPositionUpdate)
        {
            debug("got position");
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
