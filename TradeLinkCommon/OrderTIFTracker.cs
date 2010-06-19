using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// enforce time limits for orders
    /// </summary>
    public class TIFTracker
    {

        
        List<long> _id = new List<long>();
        List<int> _tifs = new List<int>();
        public TIFTracker() : this(new IdTracker()) { }
        public TIFTracker(IdTracker id)
        {
            _idt = id;
        }
        public event LongDelegate SendCancel;
        public event OrderDelegate SendOrder;
        public event DebugDelegate SendDebug;
        int _tif = 0;
        public int DefaultTif { get { return _tif; } set { _tif = value; } }
        IdTracker _idt = null;
        Dictionary<string, int> _symidx = new Dictionary<string, int>();
        Dictionary<long, int> _ididx = new Dictionary<long, int>();
        List<int> _senttime = new List<int>();

        const int NOIDX = -1;
        int _lasttime = 0;

        int symidx(string sym)
        {
            int idx = NOIDX;
            if (_symidx.TryGetValue(sym, out idx))
                return idx;
            return idx;
        }

        void checktifs()
        {
            // can't check until time is set
            if (_lasttime == 0) return;
            // check time
            for (int i = 0; i < _senttime.Count; i++)
            {
                int diff = Util.FTDIFF(_senttime[i], _lasttime);
                if (diff >= _tifs[i])
                {
                    debug("Tif expired for: " + _id[i]);
                    if (SendCancel != null)
                        SendCancel(_id[i]);
                    else
                        debug("SendCancel unhandled! can't enforce TIF!");
                }
            }
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
        public void newTick(Tick k)
        {
            if (k.time > _lasttime)
            {
                _lasttime = k.time;
                checktifs();
            }
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
                _symidx.Add(o.symbol, _tifs.Count);
                _ididx.Add(o.id, _tifs.Count);
                _senttime.Add(_lasttime);
                _tifs.Add(_tif);
                _id.Add(o.id);
            }
            // pass order along if required
            if (SendOrder != null)
                SendOrder(o);
        }



    }
}
