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
        public virtual void GotTick(Tick tick)
        {
        }
        /// <summary>
        /// Called when new orders received
        /// track or respond to orders here, eg:
        /// this.MyOrders.Add(order);
        /// </summary>
        /// <param name="order"></param>
        public virtual void GotOrder(Order order)
        {
        }
        /// <summary>
        /// Called when orders are filled as trades.
        /// track or respond to trades here, eg:
        /// positionTracker.Adjust(fill);
        /// </summary>
        /// <param name="fill"></param>
        public virtual void GotFill(Trade fill)
        {
        }
        /// <summary>
        /// Called if a cancel has been processed
        /// </summary>
        /// <param name="cancelid"></param>
        public virtual void GotOrderCancel(uint cancelid)
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
        public virtual void D(string msg) { SendDebug(DebugImpl.Create(msg)); }
        /// <summary>
        /// short form of sendorder
        /// </summary>
        /// <param name="o"></param>
        public virtual void O(Order o) { SendOrder(o); }
        /// <summary>
        /// short form of sendcancel
        /// </summary>
        /// <param name="id"></param>
        public virtual void C(uint id) { SendCancel(id); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public void I(string indicators) { SendIndicators(indicators); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public void I(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicators(string.Join(",", s)); }
        /// <summary>
        /// short form of sendindicator
        /// </summary>
        /// <param name="indicators"></param>
        public void I(string[] indicators) { SendIndicators(string.Join(",", indicators)); }
        /// <summary>
        /// sends an order
        /// </summary>
        /// <param name="o"></param>
        public void sendorder(Order o) { SendOrder(o); }
        /// <summary>
        /// cancels an order (must have the id)
        /// </summary>
        /// <param name="id"></param>
        public void sendcancel(uint id) { SendCancel(id); }
        /// <summary>
        /// sends indicators as array of objects for later analysis
        /// </summary>
        /// <param name="indicators"></param>
        public void sendindicators(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicators(string.Join(",", s)); }
        /// <summary>
        /// send indicators as array of strings for later analysis
        /// </summary>
        /// <param name="indicators"></param>
        public void sendindicators(string[] indicators) { SendIndicators(string.Join(",", indicators)); }
        /// <summary>
        /// sends indicators as a comma seperated string (for later analsis)
        /// </summary>
        /// <param name="indicators"></param>
        public void sendindicators(string indicators) { SendIndicators(indicators); }
        /// <summary>
        /// sends a debug message about what your response is doing at the moment.
        /// </summary>
        /// <param name="msg"></param>
        public void senddebug(string msg) { SendDebug(DebugImpl.Create(msg)); }
        /// <summary>
        /// called when a position update is received (usually only when the response is initially loaded)
        /// </summary>
        /// <param name="p"></param>
        public virtual void GotPosition(Position p) { }

        string[] _inds = new string[0];
        string _name = "";
        string _full = "";
        bool _valid = true;

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
        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event StringParamDelegate SendIndicators;
    }
}
