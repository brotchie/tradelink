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
        TradeLink_Client_WM c;
        TradeLink_Server_WM s;

        // counters used to test link events are working
        int ticks;
        int fills;
        int orders;
        int fillrequest;

        public TestTradeLink_WM() 
        {
            s = new TradeLink_Server_WM();
            c = new TradeLink_Client_WM("testtradelink",false);

            // register server events (so server can process orders)
            s.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);

            // setup client events
            c.gotFill += new FillDelegate(tlclient_gotFill);
            c.gotOrder += new OrderDelegate(tlclient_gotOrder);
            c.gotTick += new TickDelegate(tlclient_gotTick);

        }

        [Test]
        public void StartupTests()
        {
            // we're expecting this server type
            TLTypes expect = TLTypes.HISTORICALBROKER;
            // discover our states
            TLTypes FOUND = c.TLFound();
            bool CONNECTED = c.Mode(FOUND&expect,true,false);

            // should be able to connect to whatever server we find
            Assert.That(CONNECTED,"make sure you don't have TLServers running");

        }

        [Test]
        public void TickTests()
        {
            // havent' sent any ticks, so shouldn't have any counted
            Assert.That(ticks == 0, ticks.ToString());

            // have to subscribe to a stock to get notified on fills for said stock
            c.Subscribe(new MarketBasket(new Stock("TST")));

            //send a tick from the server
            Tick t = Tick.NewTrade("TST", 10, 100);
            s.newTick(t);

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
            int error = c.SendOrder(o);
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
            c.Subscribe(new MarketBasket(new Stock("TST")));

            // prepare and send an execution from client to server
            Trade t = new Trade("TST", 100, 300, DateTime.Now);
            s.newFill(t);

            // make sure client received and counted it
            Assert.That(fills == 1, fills.ToString());
        }

        // event handlers

        void tl_gotSrvFillRequest(Order o)
        {
            s.newOrder(o);
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
}
