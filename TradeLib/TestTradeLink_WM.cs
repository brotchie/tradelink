using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using TradeLib;
using System.Windows.Forms;

namespace TestTradeLib
{
    [TestFixture]
    public class TestTradeLink_WM
    {
        // each side of our "link"
        ServerTest st = new ServerTest();
        ClientTest ct = new ClientTest();
        // counters used to test link events are working
        int ticks;
        int fills;
        int orders;
        int fillrequest;
        TLTypes FOUND;
        bool CONNECTED = false;

        public TestTradeLink_WM() 
        {
            // register server events (so server can process orders)
            st.tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);

            // setup client events
            ct.tl.gotFill += new FillDelegate(tlclient_gotFill);
            ct.tl.gotOrder += new OrderDelegate(tlclient_gotOrder);
            ct.tl.gotTick += new TickDelegate(tlclient_gotTick);

            FOUND = st.tl.TLFound();
            CONNECTED = ct.tl.Mode(FOUND,false,false);

        }

        [Test]
        public void StartupTests()
        {
            // discover our server out there
            Assert.That(FOUND == TLTypes.SIMBROKER, "make sure you don't have TLServers running");

            // should be able to connect to whatever server we find
            Assert.That(CONNECTED,"make sure you don't have TLServers running");

        }

        [Test]
        public void TickTests()
        {
            // havent' sent any ticks, so shouldn't have any counted
            Assert.That(ticks == 0, ticks.ToString());

            // have to subscribe to a stock to get notified on fills for said stock
            ct.tl.Subscribe(new MarketBasket(new Stock("TST")));

            //send a tick from the server
            Tick t = Tick.NewTrade("TST", 10, 100);
            st.tl.newTick(t);

            // make sure the client got it
            Assert.That(ticks == 1, ticks.ToString());
        }

        [Test]
        public void OrderTests()
        {
            // no orders yet
            Assert.That(orders == 0, orders.ToString());
            // no fill requests yet
            Assert.That(fillrequest == 0, fillrequest.ToString());

            // client wants to buy 100 TST at market
            Order o = new Order("TST", 100);
            // if it works it'll return zero
            int error = ct.tl.SendOrder(o);
            Assert.That(error==0,error.ToString());
            // client should have received notification that an order entered his account
            Assert.That(orders == 1, orders.ToString());
            // server should have gotten a request to fill an order
            Assert.That(fillrequest == 1, fillrequest.ToString());
        }

        [Test]
        public void FillTests()
        {
            // no executions yet
            Assert.That(fills == 0, fills.ToString());

            // have to subscribe to a stock to get notified on fills for said stock
            ct.tl.Subscribe(new MarketBasket(new Stock("TST")));

            // prepare and send an execution from client to server
            Trade t = new Trade("TST", 100, 300, DateTime.Now);
            st.tl.newFill(t);

            // make sure client received and counted it
            Assert.That(fills == 1, fills.ToString());
        }

        // event handlers

        void tl_gotSrvFillRequest(Order o)
        {
            fillrequest++;
        }

        void tlclient_gotTick(Tick t)
        {
            ticks++;

        }

        void tlclient_gotOrder(Order o)
        {
            orders++;
        }

        void tlclient_gotFill(Trade t)
        {
            fills++;
        }
    }


    // examples of TradeLink+Form binding

    public partial class ServerTest : Form
    {
        // normally you don't want this class to be public, just easier for the test
        public TradeLink_WM tl = new TradeLink_WM();
        public ServerTest() 
        {
            Text = TradeLink_WM.SIMWINDOW; // identify this window as a broker simulator
            tl.Me = Text;
            tl.MeH = Handle;
            Hide();
        }
        protected override void WndProc(ref Message m)
        {
            tl.GotWM_Copy(ref m);
            base.WndProc(ref m);
        }
    }

    public partial class ClientTest : Form
    {
        // normally make this class non-public
        public TradeLink_WM tl = new TradeLink_WM();
        public ClientTest()
        {
            Text = "clienttest"; // name doesn't matter so long as it's unique
            tl.Me = Text;
            tl.MeH = Handle;
            Hide();
        }
        protected override void WndProc(ref Message m)
        {
            tl.GotWM_Copy(ref m);
            base.WndProc(ref m);
        }
    }
}
