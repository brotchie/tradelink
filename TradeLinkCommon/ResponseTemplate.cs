using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Common
{
    // A response is the most generic type of response you can have
    // responses will work in all the TradeLink programs (gauntlet/asp/kadina)
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

        public virtual void D(string msg) { SendDebug(DebugImpl.Create(msg)); }
        public virtual void O(Order o) { SendOrder(o); }
        public virtual void C(uint id) { SendCancel(id); }
        public void I(string indicators) { SendIndicators(indicators); }
        public void I(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicators(string.Join(",", s)); }
        public void I(string[] indicators) { SendIndicators(string.Join(",", indicators)); }

        public void sendorder(Order o) { SendOrder(o); }
        public void sendcancel(uint id) { SendCancel(id); }
        public void sendindicators(object[] indicators) { string[] s = new string[indicators.Length]; for (int i = 0; i < indicators.Length; i++) s[i] = indicators[i].ToString(); SendIndicators(string.Join(",", s)); }
        public void sendindicators(string[] indicators) { SendIndicators(string.Join(",", indicators)); }
        public void sendindicators(string indicators) { SendIndicators(indicators); }
        public void senddebug(string msg) { SendDebug(DebugImpl.Create(msg)); }
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
