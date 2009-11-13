using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    public class TIFTracker
    {

        uint this[string symbol] 
        { 
            get 
            {
              int idx = symidx(symbol);
              if (idx==NOIDX) return _tif;
              return _tifs[idx];
            }
        set
        {
            int idx = symidx(symbol);
            if (idx == NOIDX)
            {
                _symidx.Add(symbol, _tifs.Count);
                _tifs.Add(value);
            }
            else
                _tifs[idx] = value;
        }
        }
        public TIFTracker() : this(new IdTracker()) { }
        public TIFTracker(IdTracker id)
        {
            _idt = id;
        }
        public event UIntDelegate SendCancel;
        public event OrderDelegate SendOrder;
        public event DebugDelegate SendDebug;
        uint _tif = 300;
        public uint DefaultTif { get { return _tif; } set { _tif = value; } }
        IdTracker _idt = null;
        Dictionary<string, int> _symidx = new Dictionary<string, int>();
        Dictionary<uint, int> _ididx = new Dictionary<uint, int>();
        List<int> _senttime = new List<int>();
        List<uint> _tifs = new List<uint>(); 
        List<uint> _id = new List<uint>();
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
        public void SendOrderTIF(Order o)
        {
            // update time
            if ((o.time != 0) && (o.time > _lasttime))
                _lasttime = o.time;
            if ((o.time==0) && (_lasttime==0))
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
