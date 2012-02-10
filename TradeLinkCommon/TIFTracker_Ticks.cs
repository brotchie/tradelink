using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// enforce time limits for orders
    /// (in tick counts rather than seconds)
    /// </summary>
    public class TIFTracker_Ticks : GenericTracker<long>, GotTickIndicator, GenericTrackerLong, SendOrderIndicator, SendCancelIndicator, GotCancelIndicator, GotFillIndicator
    {

        public void GotTick(Tick k) { newTick(k); }
        public long getvalue(int idx) { return this[idx]; }
        public long getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, long v) { this[idx] = v; }

        List<long> _id = new List<long>();
        List<int> _tifs = new List<int>();
        public TIFTracker_Ticks() : this(new IdTracker()) { }
        public TIFTracker_Ticks(IdTracker id)
        {
            _idt = id;
            NewTxt += new TextIdxDelegate(TIFTracker_Ticks_NewTxt);
        }

        void TIFTracker_Ticks_NewTxt(string txt, int idx)
        {
            _ticks.addindex(txt, 0);
        }
        public event LongDelegate SendCancelEvent;
        public event OrderDelegate SendOrderEvent;
        public event DebugDelegate SendDebugEvent;
        int _tif = 0;
        public int DefaultTif { get { return _tif; } set { _tif = value; } }
        IdTracker _idt = null;

        Dictionary<long, int> _ididx = new Dictionary<long, int>();
        List<int> _ticksatsent = new List<int>();
        GenericTracker<int> _ticks = new GenericTracker<int>();

        const int NOIDX = -1;
        

        /// <summary>
        /// gets tif in seconds for a given order id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int gettif(long id) { return _tifs[_ididx[id]]; }

        public long[] Orderids { get { return _id.ToArray(); } }

        void checktifs(int sidx)
        {
            // check tick count

            // get current  tickcount
            int nowticks = _ticks[sidx];

            for (int i = 0; i < _ticksatsent.Count; i++)
            {

                // skip if no order id
                if (_id[i] == 0)
                    continue;
                // skip if order hasn't been acknowledged
                if (!orderack[i])
                    continue;
                // get tif (in tick counts)
                int tif = _tifs[i];
                // ignore if canceled/filled no tif
                if (tif == 0)
                    continue;
                // get ticks since sent
                int org = _ticksatsent[i];
                // get difference
                int diff = Math.Abs(nowticks - org);
                // get ticks now
                if (diff >= tif)
                {
                    debug(sym[i]+" tif expired for: " + _id[i] + " at tick count: " + diff + " >= tif: " + tif);
                    if (SendCancelEvent != null)
                        SendCancelEvent(_id[i]);
                    else
                        debug("TifTracker_Ticks.SendCancel unhandled! can't enforce TIF!");
                }
            }
        }

        

        public void GotOrder(Order o)
        {
            // see if we know about this order
            int oidx = NOIDX;
            // skip if we don't
            if (!_ididx.TryGetValue(o.id, out oidx))
                return;
            // mark it as acked
            orderack[oidx] = true;
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
            debug(sym[idx] + " cancel received for tif(ticks) order: " + id);
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
                debug(sym + " completely filled tif order: " + id);
            }
            else
                debug(sym + " partial fill tif order: " + id);


        }

        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        public void newTick(Tick k)
        {
            // see if we're ignoring quotes
            if (IgnoreQuotes && !k.isTrade)
                return;
            // see if we're tracking this symbol
            int idx = getindex(k.symbol);
            // skip if not
            if (idx < 0)
                return;
            // otherwise count the tick
            _ticks[idx]++;
            // enforce tifs
            checktifs(idx);

        }

        // hold symbol for each order
        List<string> sym = new List<string>();
        List<int> symidx = new List<int>();
        // hold sent and filled size for each order
        List<int> sentsize = new List<int>();
        List<int> filledsize = new List<int>();
        // hold order acknowledgement status for each order
        List<bool> orderack = new List<bool>();

        /// <summary>
        /// whether to ignore quotes when counting ticks
        /// (set to true to support IOC-style orders where cancel is sent if not filled on first trade)
        /// </summary>
        public bool IgnoreQuotes = false;
        /// <summary>
        /// turns ignorequotes to true
        /// </summary>
        public void IOCMode() { IgnoreQuotes = true; }

        public void SendOrder(Order o)
        {
            // get tif from order
            int tif = 0;
            if (int.TryParse(o.TIF, out tif))
                SendOrderTIF(o, tif);
            else
                SendOrderTIF(o, DefaultTif);
        }

        public void SendOrderTIF(Order o, int TIF)
        {
            

            // make sure it has an id
            if (o.id == 0)
            {
                o.id = _idt.AssignId;
                debug(o.symbol + " no order id for tif, assigning: " + o.id+" order: "+o.ToString());
            }

            // see if we have an index for this order
            int idx = NOIDX;
            // add it if we don't
            if (!_ididx.TryGetValue(o.id, out idx))
            {
                debug(o.symbol+" tracking tif for: " + o.id+" order: "+o.ToString());
                // update per symbol index
                int sidx = getindex(o.symbol);
                if (sidx < 0)
                    sidx = addindex(o.symbol, o.id);
                else
                    this[sidx] = o.id;
                
                // update per order indicies
                _ididx.Add(o.id, _tifs.Count);
                orderack.Add(false);
                symidx.Add(sidx);
                // get tick count at present for symbol
                _ticksatsent.Add(_ticks[sidx]);
                _tifs.Add(TIF);
                _id.Add(o.id);
                sym.Add(o.symbol);
                sentsize.Add(o.UnsignedSize);
                filledsize.Add(0);
            }
            else // this shouldn't really happen
            {
                debug(o.symbol+" possible duplicate order id: " + o.id + " resetting tif count based on: "+o.ToString());
                _ticksatsent[idx] = _ticks[o.symbol];
                sentsize[idx] = o.UnsignedSize;
                filledsize[idx] = 0;
                sym[idx] = o.symbol;
                symidx[idx] = getindex(o.symbol);
                orderack[idx] = false;
            }



            // pass order along if required
            if (SendOrderEvent != null)
                SendOrderEvent(o);
        }



    }
}
