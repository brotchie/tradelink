using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// enforce time limits for orders (in seconds)
    /// </summary>
    public class TIFTracker : GenericTracker<long>, GotTickIndicator, GenericTrackerLong, SendOrderIndicator, SendCancelIndicator, GotCancelIndicator, GotFillIndicator
    {

        public void GotTick(Tick k) { newTick(k); }
        public long getvalue(int idx) { return this[idx]; }
        public long getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, long v) { this[idx] = v; }

        List<long> _id = new List<long>();
        List<int> _tifs = new List<int>();
        public TIFTracker() : this(new IdTracker()) { }
        public TIFTracker(IdTracker id)
        {
            _idt = id;
        }
        public event LongDelegate SendCancelEvent;
        public event OrderDelegate SendOrderEvent;
        public event DebugDelegate SendDebugEvent;
        int _tif = 0;
        public int DefaultTif { get { return _tif; } set { _tif = value; } }
        IdTracker _idt = null;

        Dictionary<long, int> _ididx = new Dictionary<long, int>();
        List<int> _senttime = new List<int>();

        const int NOIDX = -1;
        int _lasttime = 0;

        /// <summary>
        /// gets tif in seconds for a given order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int gettif(long id) { return _tifs[_ididx[id]]; }

        public long[] Orderids { get { return _id.ToArray(); } }

        void checktifs()
        {
            // can't check until time is set
            if (_lasttime == 0) return;
            // check time
            for (int i = 0; i < _senttime.Count; i++)
            {
                // skip if order has been canceled/filled
                if (_id[i] == 0)
                    continue;
                // skip if tif is zero
                int tif = _tifs[i];
                if (tif == 0)
                    continue;
                int senttime = _senttime[i];
                int diff = Util.FTDIFF(senttime, _lasttime);
                if (diff >= tif)
                {
                    debug("Tif expired for: " + _id[i]+" at time: "+_lasttime+" from: "+_senttime+" secs: "+diff);
                    if (SendCancelEvent != null)
                        SendCancelEvent(_id[i]);
                    else
                        debug("SendCancel unhandled! can't enforce TIF!");
                }
            }
        }

        public void GotCancel(long id)
        {
            int idx = -1;
            // not our id
            if (!_ididx.TryGetValue(id, out idx))
                return;
            // mark as canceled
            _tifs[idx] = 0;
            // get symbol
            debug(sym[idx] + " cancel received for tif(sec) order: " + id);
        }

        public void GotFill(Trade fill)
        {
            long id = fill.id;
            int idx = -1;
            // not our id
            if (!_ididx.TryGetValue(id, out idx))
                return;
            // mark as filled
            filledsize[idx] += Math.Abs(fill.xsize);
            // get symbol
            string sym = fill.symbol;
            // see if completed
            if (filledsize[idx] == sentsize[idx])
            {
                // mark as done
                _tifs[idx] = 0;
                debug(sym + " completely filled: " + id);
            }
            else
                debug(sym + " partial fill: " + id);


        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        public void newTick(Tick k)
        {
            if (k.time > _lasttime)
            {
                _lasttime = k.time;
                checktifs();
            }
        }

        List<string> sym = new List<string>();
        List<int> sentsize = new List<int>();
        List<int> filledsize = new List<int>();

        public void SendOrder(Order o)
        {
            // get tif from order
            int tif = 0;
            if (int.TryParse(o.TIF, out tif))
                SendOrderTIF(o, tif);
            else
                SendOrderTIF(o, 0);
        }

        public void SendOrderTIF(Order o, int TIF)
        {
            // update time
            if ((o.time != 0) && (o.time > _lasttime))
                _lasttime = o.time;
            if ((o.time == 0) && (_lasttime == 0))
            {
                debug("No time available!  Can't enforce tif!");
                return;
            }
            // check existing tifs
            checktifs();
            // make sure it has an id
            if (o.id == 0)
            {
                o.id = _idt.AssignId;
            }
            // see if we have an index
            int idx = NOIDX;
            // add it if we don't
            if (!_ididx.TryGetValue(o.id, out idx))
            {
                debug("tracking tif for: " + o.id + " " + o.symbol);
                int sidx = getindex(o.symbol);
                if (sidx < 0)
                    addindex(o.symbol, o.id);
                else
                    this[sidx] = o.id;
                _ididx.Add(o.id, _tifs.Count);
                _senttime.Add(_lasttime);
                _tifs.Add(_tif);
                _id.Add(o.id);
                sym.Add(o.symbol);
                sentsize.Add(o.UnsignedSize);
                filledsize.Add(0);
            }
            // pass order along if required
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }



    }
}
