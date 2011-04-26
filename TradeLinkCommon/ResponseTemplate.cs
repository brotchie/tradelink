using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// Template for most typical response.  Inherit from this to create a symbol grey or black strategy.
    /// eg:
    /// public class MyStrategy : ResponseTemplate
    /// {
    /// }
    /// </summary>
    public class ResponseTemplate : Response
    {
        /// <summary>
        /// Called when new ticks are recieved
        /// here is where you respond to ticks, eg to populate a barlist
        /// this.MyBarList.newTick(tick);
        /// </summary>
        /// <param name="tick"></param>
        public virtual void GotTick(Tick k)
        {
        }
        /// <summary>
        /// Called when new orders received
        /// track or respond to orders here, eg:
        /// this.MyOrders.Add(order);
        /// </summary>
        /// <param name="order"></param>
        public virtual void GotOrder(Order o)
        {
        }
        /// <summary>
        /// Called when orders are filled as trades.
        /// track or respond to trades here, eg:
        /// positionTracker.Adjust(fill);
        /// </summary>
        /// <param name="fill"></param>
        public virtual void GotFill(Trade f)
        {
        }
        /// <summary>
        /// Called if a cancel has been processed
        /// </summary>
        /// <param name="cancelid"></param>
        public virtual void GotOrderCancel(long id)
        {

        }
        /// <summary>
        /// called when unknown message arrives.   
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public virtual void GotMessage(MessageTypes type, long source, long dest, long msgid, string request,ref string response)
        {
        }
        /// <summary>
        /// Call this to reset your response parameters.
        /// You might need to reset groups of indicators or internal counters.
        /// eg : MovingAverage = 0;
        /// </summary>
        public virtual void Reset()
        {
        }
        /// <summary>
        /// short form of senddebug
        /// </summary>
        /// <param name="msg"></param>
        public virtual void D(string msg) { SendDebugEvent(DebugImpl.Create(msg)); }
        /// <summary>
        /// short form of sendorder
        /// </summary>
        /// <param name="o"></param>
        public virtual void O(Order o) { o.VirtualOwner = ID;  SendOrderEvent(o,ID); }
        /// <summary>
        /// short form of sendcancel
        /// </summary>
        /// <param name="id"></param>
        public virtual void C(long id) { SendCancelEvent(id,ID); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void I(string indicators) { SendIndicatorsEvent(ID,indicators); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void I(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicatorsEvent(ID, string.Join(",", s)); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void I(string[] indicators) { SendIndicatorsEvent(ID, string.Join(",", indicators)); }
        /// <summary>
        /// sends an order
        /// </summary>
        /// <param name="o"></param>
        public virtual void sendorder(Order o) { o.VirtualOwner = ID;  SendOrderEvent(o,ID); }
        /// <summary>
        /// cancels an order (must have the id)
        /// </summary>
        /// <param name="id"></param>
        public virtual void sendcancel(long id) { SendCancelEvent(id,ID); }
        /// <summary>
        /// sends indicators as array of objects for later analysis
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void sendindicators(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicatorsEvent(ID, string.Join(",", s)); }
        /// <summary>
        /// send indicators as array of strings for later analysis
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void sendindicators(string[] indicators) { SendIndicatorsEvent(ID, string.Join(",", indicators)); }
        /// <summary>
        /// sends indicators as a comma seperated string (for later analsis)
        /// </summary>
        /// <param name="indicators"></param>
        public virtual void sendindicators(string indicators) { SendIndicatorsEvent(ID, indicators); }
        /// <summary>
        /// requests ticks for a basket of securities
        /// </summary>
        /// <param name="syms"></param>
        public virtual void sendbasket(string[] syms) { if (SendBasketEvent != null) SendBasketEvent(new BasketImpl(syms), ID); else senddebug("SendBasket not supported in this application."); }
        /// <summary>
        /// request ticks for a basket of securities
        /// </summary>
        /// <param name="syms"></param>
        public virtual void sendbasket(Basket syms)
        {
            if (SendBasketEvent != null)
                SendBasketEvent(syms, ID);
            else
                senddebug("SendBasket not supported in this application.");
        }
        /// <summary>
        /// requests ticks for basket of securities
        /// </summary>
        /// <param name="syms"></param>
        public virtual void SB(string[] syms) { sendbasket(syms); }

        public event TicketDelegate SendTicketEvent;
        /// <summary>
        /// send ticket
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="pw"></param>
        /// <param name="summary"></param>
        /// <param name="desc"></param>
        /// <param name="pri"></param>
        /// <param name="stat"></param>
        public virtual void sendticket(string space, string user, string pw, string summary, string desc, Priority pri, TicketStatus stat)
        {
            if (SendTicketEvent != null)
                SendTicketEvent(space, user, pw, summary, desc, pri, stat);
        }
        /// <summary>
        /// send ticket with default priority and status
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="pw"></param>
        /// <param name="summary"></param>
        /// <param name="desc"></param>
        public virtual void sendticket(string space, string user, string pw, string summary, string desc)
        {
            sendticket(space, user, pw, summary, desc, Priority.Normal, TicketStatus.New);
        }
        /// <summary>
        /// send ticket with default priority and status
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="pw"></param>
        /// <param name="summary"></param>
        /// <param name="desc"></param>
        public virtual void T(string space, string user, string pw, string summary, string desc)
        {
            sendticket(space, user, pw, summary, desc); 
        }

        /// <summary>
        /// sends a message
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="data"></param>
        public virtual void sendmessage(MessageTypes type, long msgid,string request,string response) { if (SendMessageEvent!=null) SendMessageEvent(type, (long)ID, 0,msgid,request,ref response); }
        public virtual void sendmessage(MessageTypes type, string data) { sendmessage(type, 0,data, string.Empty); }
        /// <summary>
        /// sends a debug message about what your response is doing at the moment.
        /// </summary>
        /// <param name="msg"></param>
        public virtual void senddebug(string msg) { if (SendDebugEvent!=null) SendDebugEvent(DebugImpl.Create(msg)); }
        /// <summary>
        /// clears the chart
        /// </summary>
        public virtual void sendchartlabel() { sendchartlabel(-1, 0,System.Drawing.Color.White); }
        /// <summary>
        /// draws a label with default color (violet)
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        /// <param name="text"></param>
        public virtual void sendchartlabel(decimal price, int time, string text) { if (SendChartLabelEvent != null) SendChartLabelEvent(price, time, text, System.Drawing.Color.Purple); }
        /// <summary>
        /// draws text directly on a point on chart
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        /// <param name="text"></param>
        public virtual void sendchartlabel(decimal price, int time, string text, System.Drawing.Color c) { if (SendChartLabelEvent != null) SendChartLabelEvent(price, time, text, c); }
        /// <summary>
        /// draws line with default color (orage)
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        public virtual void sendchartlabel(decimal price, int time) { sendchartlabel(price, time, null, System.Drawing.Color.Orange); }
        /// <summary>
        /// draws a line between this and previous point drawn
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        public virtual void sendchartlabel(decimal price, int time, System.Drawing.Color c) { sendchartlabel(price, time, null, c); }
        /// <summary>
        /// same as sendchartlabel
        /// </summary>
        public virtual void CL() { sendchartlabel(); }
        /// <summary>
        /// same as sendchartlabel
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        public virtual void CL(decimal price, int time, System.Drawing.Color c) { sendchartlabel(price, time, c); }
        /// <summary>
        /// same as sendchartlabel
        /// </summary>
        /// <param name="price"></param>
        /// <param name="time"></param>
        /// <param name="text"></param>
        public virtual void CL(decimal price, int time, string text, System.Drawing.Color c) { sendchartlabel(price, time, text, c); }

        /// <summary>
        /// called when a position update is received (usually only when the response is initially loaded)
        /// </summary>
        /// <param name="p"></param>
        public virtual void GotPosition(Position p) { }

        string[] _inds = new string[0];
        string _name = "";
        string _full = "";
        bool _valid = true;
        int _id = UNKNOWNRESPONSE;
        public const int UNKNOWNRESPONSE = int.MaxValue;
        /// <summary>
        /// numeric tag for this response used by programs that load responses
        /// </summary>
        public int ID { get { return _id; } set { _id = value; } }

        /// <summary>
        /// Whether response can be used or not
        /// </summary>
        public bool isValid { get { return _valid; } set { _valid = value; } }
        /// <summary>
        /// Names of the indicators used by your response.
        /// Length must correspond to actual indicator values send with SendIndicators event
        /// </summary>
        public string[] Indicators { get { return _inds; } set { _inds = value; } }
        /// <summary>
        /// Custom name of response set by you
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }
        /// <summary>
        /// Full name of this response set by programs (includes namespace)
        /// </summary>
        public string FullName { get { return _full; } set { _full = value; } }
        public event DebugFullDelegate SendDebugEvent;
        public event OrderSourceDelegate SendOrderEvent;
        public event LongSourceDelegate SendCancelEvent;
        public event ResponseStringDel SendIndicatorsEvent;
        public event MessageDelegate SendMessageEvent;
        public event BasketDelegate SendBasketEvent;
        public event ChartLabelDelegate SendChartLabelEvent;



        // helper stuff

        /// <summary>
        /// shutdown a response entirely, flat all positions and notify user
        /// </summary>
        /// <param name="_pt"></param>
        /// <param name="gt"></param>
        public void shutdown(PositionTracker _pt, GenericTrackerI gt)
        {
            if (!isValid) return;
            D("ShutdownTime");
            isValid = false;
            bool ShutdownFlat = _pt != null;
            bool usegt = gt != null;
            if (ShutdownFlat)
            {
                
                D("flatting positions at shutdown.");
                foreach (Position p in _pt)
                {
                    if (usegt && (gt.getindex(p.Symbol) < 0)) continue;
                    Order o = new MarketOrderFlat(p);
                    D("flat order: " + o.ToString());
                    sendorder(o);
                }
            }
        }
        /// <summary>
        /// flat a symbol and flag it to prevent it from trading in future
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="activesym"></param>
        /// <param name="_pt"></param>
        /// <param name="sendorder"></param>
        public static void shutdown(string sym, GenericTracker<bool> activesym, PositionTracker _pt, OrderDelegate sendorder) { shutdown(sym, activesym, _pt, sendorder, null, string.Empty); }
        /// <summary>
        /// flat a symbol and flag it to allow prevention of future trading with status
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="activesym"></param>
        /// <param name="_pt"></param>
        /// <param name="sendorder"></param>
        /// <param name="D"></param>
        public static void shutdown(string sym, GenericTracker<bool> activesym, PositionTracker _pt, OrderDelegate sendorder, DebugDelegate D) { shutdown(sym, activesym, _pt, sendorder, D, string.Empty); }
        /// <summary>
        /// flat a symbol and flag it to allow prevention of future trading with status and supplied reason
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="activesym"></param>
        /// <param name="_pt"></param>
        /// <param name="sendorder"></param>
        /// <param name="D"></param>
        /// <param name="reason"></param>
        public static void shutdown(string sym, GenericTracker<bool> activesym, PositionTracker _pt, OrderDelegate sendorder, DebugDelegate D, string reason)
        {
            if (!activesym[sym]) return;
            Order o = new MarketOrderFlat(_pt[sym]);
            if (D != null)
            {
                string r = reason == string.Empty ? string.Empty : " (" + reason + ")";
                D("symbol shutdown" + r + ", flat order: " + o.ToString());
            }
            sendorder(o);
            activesym[sym] = false;
        }
    }
}
