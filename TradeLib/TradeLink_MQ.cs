using System;
using System.Collections.Generic;
using System.Messaging;
using System.Xml.Serialization;

namespace TradeLib
{


    /// <summary>
    /// TradeLink implemented ontop of Microsoft Message Queing
    /// </summary>
    public class TradeLink_MQ : TradeLinkClient
    {
        public event MessageDelegate GotMessage;
        public event TickDelegate gotTick;
        public event FillDelegate gotFill;


        // clients to server
        public void Register() { TLSend(TL2.REGISTERCLIENT); }
        public void Subscribe(MarketBasket mb) { TLSend(new LinkMessage(TL2.REGISTERSTOCK, mb)); }
        public void Disconnect() { TLSend(TL2.CLEARSTOCKS); TLSend(TL2.CLEARCLIENT); }
        public void Unsubscribe() { TLSend(TL2.CLEARSTOCKS); }
        public int HeartBeat() { return 1; } //TLSend(TL2.HEARTBEAT); }

        // server to clients
        public void newTick(Tick t)
        {
            if (t.sym == "") return; // can't process symbol-less ticks
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(t.sym)))
                    TLSend(client[i], new LinkMessage(TL2.TICKNOTIFY, t));
        }
        public void newFill(Trade t)
        {
            for (int i = 0; i < client.Count; i++) // send tick to each client that has subscribed to tick's stock
                if ((client[i] != null) && (stocks[i].Contains(t.symbol)))
                    TLSend(client[i], new LinkMessage(TL2.EXECUTENOTIFY, t));
        }




        // server structures
        private List<string> client = new List<string>();
        private List<DateTime> heart = new List<DateTime>();
        private List<string> stocks = new List<string>();
        public string Stocks(string him)
        {
            int cid = client.IndexOf(him);
            if (cid == -1) return ""; // client not registered
            return stocks[cid];
        }

        public TradeLink_MQ() : this(null) { }
        public TradeLink_MQ(string me)
        {
            if (me == null) { Inbound = "TradeLink"; return; }
            else Inbound = me;
            if (me != "TradeLink") Outbound = "TradeLink";
        }
        MessageQueue xmt = null;
        MessageQueue rec = null;
        string outb = "TradeLink";
        string inb = "TLC";

        string Inbound
        {
            get
            {
                return ".\\Private$\\" + inb;
            }
            set
            {
                inb = value;
                if (!MessageQueue.Exists(Inbound)) MessageQueue.Create(Inbound);
                rec = new MessageQueue(Inbound);
                BinaryMessageFormatter f = new BinaryMessageFormatter();
                //f.TopObjectFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                //f.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
                rec.Formatter = f;
                rec.ReceiveCompleted += new ReceiveCompletedEventHandler(gotMessage);
                rec.BeginReceive();
            }
        }
        string Me { get { return inb; } }
        string Him { get { return outb; } }
        public string Outbound
        {
            get
            {
                return ".\\Private$\\" + outb;
            }
            set
            {
                outb = value;
                if (!MessageQueue.Exists(Outbound)) MessageQueue.Create(Outbound);
                xmt = new MessageQueue(Outbound);
                BinaryMessageFormatter f = new BinaryMessageFormatter();
                f.TopObjectFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                f.TypeFormat = System.Runtime.Serialization.Formatters.FormatterTypeStyle.TypesWhenNeeded;
                xmt.Formatter = f;
            }
        }
        bool hasQueueEnds { get { return (inb != null) && (outb != null); } }
        void TLSend(TL2 type) { TLSend(Him, new LinkMessage(type)); }
        void TLSend(LinkMessage m) { TLSend(Him, m); }
        void TLSend(string to, LinkMessage m)
        {
            Outbound = to;
            if (!hasQueueEnds) return;
            m.From = Me;
            System.Messaging.Message sm = new System.Messaging.Message(m);
            sm.ResponseQueue = rec;
            sm.Formatter = new BinaryMessageFormatter();
            xmt.Send(sm);
        }

        void gotMessage(object sender, ReceiveCompletedEventArgs e)
        {
            TL2 mt = TL2.INFO;
            string from = "";
            LinkMessage gotmess = null;
            object m = null;
            gotmess = (LinkMessage)e.Message.Body;
            mt = gotmess.Type;
            m = gotmess.Body;
            from = gotmess.From;

            int cid = -1;
            Tick t = new Tick();

            try
            {
                switch (mt)
                {
                    // CLIENT MESSAGES
                    case TL2.TICKNOTIFY:
                        t = (Tick)m;
                        gotTick(t);
                        break;
                    case TL2.EXECUTENOTIFY:
                        Trade trade;
                        trade = (Trade)m;
                        gotFill(trade);
                        break;

                    // SERVER MESSAGES
                    case TL2.REGISTERCLIENT:
                        if (client.IndexOf(from) != -1) break; // we already had your client, ignore
                        client.Add(from);
                        stocks.Add("");
                        heart.Add(new DateTime());
                        break;
                    case TL2.REGISTERSTOCK:
                        cid = client.IndexOf(from);
                        if (cid == -1) break; // client not found
                        MarketBasket mb = (MarketBasket)m;
                        stocks[cid] = mb.ToString();
                        BeatHeart(from);
                        break;
                    case TL2.HEARTBEAT:
                        BeatHeart(from);
                        break;
                    case TL2.CLEARSTOCKS:
                        cid = client.IndexOf(from);
                        if (cid == -1) break;
                        stocks[cid] = null;
                        BeatHeart(from);
                        break;
                    case TL2.CLEARCLIENT:
                        cid = client.IndexOf(from);
                        if (cid == -1) break; // we don't have the client, nothing to do
                        client[cid] = null;
                        stocks[cid] = null;
                        heart[cid] = new DateTime();
                        break;
                }
            }
            catch (Exception ex) { string s = ex.Message; }
            if (this.GotMessage != null) GotMessage(mt, from); // send GotMessage event to any subscribers
            rec.BeginReceive(); // prepare to receive new messages
        }
        int BeatHeart(string cname)
        {
            int cid = client.IndexOf(cname);
            if (cid == -1) return -1; // this client isn't registered, ignore
            DateTime now = new DateTime();
            TimeSpan since = now.Subtract(heart[cid]);
            heart[cid] = now;
            return since.Seconds;
        }
        public int SendOrder(Order order)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RegIndex(IndexBasket ib)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void GoLive()
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void GoSim()
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }

    [Serializable]
    [XmlRoot("TLMessages", IsNullable = true)]
    public class LinkMessage
    {
        public LinkMessage(TL2 t, object m) { Type = t; Body = m; }
        public LinkMessage(TL2 t) { Type = t; }
        public LinkMessage(object m) : this(TL2.INFO, m) { }
        public LinkMessage() : this(TL2.INFO, null) { }
        private string from = "";
        [XmlElement("from")]
        public string From { get { return from; } set { from = value; } }
        private TL2 type = TL2.INFO;
        [XmlElement("type")]
        public TL2 Type { get { return type; } set { type = value; } }
        private object body = new object();
        [XmlElement("body")]
        public Object Body { get { return body; } set { body = value; } }
    }

}
