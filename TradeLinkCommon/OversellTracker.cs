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
        public OversellTracker() : this(new PositionTracker(),new IdTracker()) { }
        IdTracker _idt;
        public OversellTracker(PositionTracker pt, IdTracker idt)
        {
            _pt = pt;
            _idt = idt;
        }

        bool _verbosedebug = false;
        public bool VerboseDebugging { get { return _verbosedebug; } set { _verbosedebug = value; } }

        protected virtual void v(string msg)
        {
            if (!_verbosedebug)
                return;
            debug(msg);
        }

        int minlotsize = 1;
        public int MinLotSize { get { return minlotsize; } set { minlotsize = value; } }

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

        Dictionary<long, long> _orgid2splitid = new Dictionary<long, long>();

        public event LongDelegate SendCancelEvent;

        void cancel(long id)
        {
            if (SendCancelEvent != null)
                SendCancelEvent(id);
            else
                debug("Can't cancel: " + id + " as no SendCancelEvent handler is defined!");
        }

        /// <summary>
        /// ensure that if splits are enabled, cancels for the original order also are copied to the split
        /// </summary>
        /// <param name="id"></param>
        public void sendcancel(long id)
        {
            try
            {
                long cancelsplit = 0;
                // cancel original
                cancel(id);
                // cancel split if it exists
                if (_orgid2splitid.TryGetValue(id, out cancelsplit))
                {
                    debug("cancel received on original order: " + id + ", copying cancel to split: " + cancelsplit);
                    cancel(cancelsplit);
                }
                else
                    v("cancel did not match split order, passed along cancel: " + id);
            }
            catch (Exception ex)
            {
                debug("error encountered processing cancel: " + id + " err: " + ex.Message + ex.StackTrace);
            }

        }

        /// <summary>
        /// track and correct oversells (either by splitting into two orders, or dropping oversell)
        /// </summary>
        /// <param name="o"></param>
        public void sendorder(Order o)
        {
            // get original size
            int osize = o.size;
            int uosize = o.UnsignedSize;
            // get existing size
            int size = _pt[o.symbol].Size;
            int upsize = _pt[o.symbol].UnsignedSize;
            // check for overfill/overbuy
            bool over = (o.size * size < -1) && (o.UnsignedSize > Math.Abs(size)) && (upsize >= MinLotSize);
            // detect
            if (over)
            {
                // determine correct size
                int oksize = _pt[o.symbol].FlatSize;
                // adjust
                o.size = Calc.Norm2Min(oksize,MinLotSize);
                // send 
                sonow(o);
                // count
                osa++;
                // notify
                debug(o.symbol + " oversell detected on pos: "+size+" order adjustment: " + osize + "->" + size + " " + o.ToString());
                // see if we're splitting
                if (Split)
                {
                    // calculate new size
                    int nsize = Calc.Norm2Min(Math.Abs(uosize - Math.Abs(oksize)),MinLotSize);
                    // adjust side
                    nsize *= (o.side ? 1 : -1);
                    // create order
                    Order newo = new OrderImpl(o);
                    newo.size = nsize;
                    newo.id = _idt.AssignId;
                    if (_orgid2splitid.ContainsKey(o.id))
                        _orgid2splitid[o.id] = newo.id;
                    else
                        _orgid2splitid.Add(o.id, newo.id);
                    // send
                    if (nsize!=0)
                        sonow(newo);
                    // notify
                    debug(o.symbol + " splitting oversell/overcover: "+o.ToString()+" to 2nd order: " + newo);
                }
            }
            else
                sonow(o);

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
