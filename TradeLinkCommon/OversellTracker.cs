using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// prevent or adjust oversell/overcovers
    /// </summary>
    public class OversellTracker 
    {
        PositionTracker _pt;
        public string Name { get { return "OVERSELL"; } }
        public OversellTracker() : this(new PositionTracker()) { }
        public OversellTracker(PositionTracker pt)
        {
            _pt = pt;
        }

        int osa = 0;
        public int OversellsAvoided { get { return osa; } }

        bool _split = false;
        /// <summary>
        /// split oversells into two orders (otherwise, oversold portion is dropped)
        /// </summary>
        public bool Split { get { return _split; } set { _split = value; } }

        public event OrderDelegate SendOrderEvent;
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        public void sendorder(Order o)
        {
            // get original size
            int osize = o.size;
            int uosize = o.UnsignedSize;
            // get existing size
            int size = _pt[o.symbol].Size;
            // check for overfill/overbuy
            bool over = o.size * size < -1;
            // detect
            if (over)
            {
                // determine correct size
                int oksize = _pt[o.symbol].FlatSize;
                // adjust
                o.size = oksize;
                // send 
                sonow(o);
                // count
                osa++;
                // notify
                debug(o.symbol + " oversell detected: " + osize + ">" + size + " adjusted to: " + o.ToString());
                // see if we're splitting
                if (Split)
                {
                    // calculate new size
                    int nsize = Math.Abs(uosize - Math.Abs(oksize));
                    // adjust side
                    nsize *= (o.side ? 1 : -1);
                    // create order
                    Order newo = new OrderImpl(o);
                    newo.size = nsize;
                    // send
                    sonow(newo);
                    // notify
                    debug(o.symbol + " splitting oversell/overcover to 2nd order: " + newo);
                }
            }

        }

        void sonow(Order o)
        {
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }

        public void GotFill(Trade t)
        {
            _pt.Adjust(t);
        }

        public void GotPosition(Position p)
        {
            _pt.Adjust(p);
        }
        
    }
}
