using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    public class OffsetTracker
    {
        OffsetInfo _default = new OffsetInfo();
        string[] _ignore = new string[0];
        public OffsetInfo DefaultOffset { get { return _default; } set { _default = value; } }
        public string[] IgnoreSyms { get { return _ignore; } set { _ignore = value; } }
        bool _hasevents = false;
        public event OrderDelegate SendOffset;
        public event UIntDelegate SendCancel;
        PositionTracker _pt = new PositionTracker();
        uint _nextid = OrderImpl.Unique;

        public OffsetTracker() { }
        public OffsetTracker(uint InitialOffsetId)
        {
            _nextid = InitialOffsetId;
        }




        void doupdate(string sym)
        {
            // is update ignored?
            if (IgnoreUpdate(sym)) return;
            // get our offset values
            OffsetInfo off = GetOffset(sym);
            // cancel existing profits
            cancel(off.ProfitId);
            // cancel existing stops
            cancel(off.StopId);
            // get new profit
            Order profit = Calc.PositionProfit(_pt[sym], off.ProfitDist, off.ProfitPercent);
            // if it's valid, send and track it
            if (profit.isValid)
            {
                profit.id = _nextid++;
                off.ProfitId = profit.id;
                SendOffset(profit);
            }
            // get new stop
            Order stop = Calc.PositionStop(_pt[sym], off.StopDist, off.StopPercent);
            // if it's valid, send and track
            if (stop.isValid)
            {
                stop.id = _nextid++;
                off.StopId = stop.id;
                SendOffset(stop);
            }
            // make sure new offset info is reflected
            SetOffset(sym, off);


        }

        void cancel(uint id) { if (id != 0) SendCancel(id); }

        bool HasEvents()
        {
            if (_hasevents) return true;
            if ((SendCancel == null) || (SendOffset == null))
                throw new Exception("You must define targets for SendCancel and SendOffset events.");
            _hasevents = true;
            return _hasevents;
        }

        bool IgnoreUpdate(string sym) 
        { 
            foreach (string isym in _ignore) 
                if (sym == isym) 
                    return true; 
            return false; 
        }

        uint ProfitId(string sym)
        {
            OffsetInfo val;
            // if we have an offset, return the id
            if (_offvals.TryGetValue(sym, out val))
                return val.ProfitId;
            // no offset id
            return 0;
        }

        uint StopId(string sym)
        {
            OffsetInfo val;
            // if we have offset, return it's id
            if (_offvals.TryGetValue(sym, out val))
                return val.StopId;
            // no offset id
            return 0;
        }

        /// <summary>
        /// must send new positions here (eg from GotPosition on Response)
        /// </summary>
        /// <param name="p"></param>
        public void UpdatePosition(Position p)
        {
            // update position
            _pt.Adjust(p);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(p.Symbol);
        }

        /// <summary>
        /// must send new fills here (eg call from Response.GotFill)
        /// </summary>
        /// <param name="t"></param>
        public void UpdatePosition(Trade t)
        {
            // update position
            _pt.Adjust(t);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(t.symbol);

        }

        OffsetInfo GetOffset(string sym)
        {
            OffsetInfo val;
            // see if we have a custom offset
            if (_offvals.TryGetValue(sym, out val))
                return val;
            // otherwise use default
            return _default;
        }

        void SetOffset(string sym, OffsetInfo off)
        {
            OffsetInfo v;
            if (_offvals.TryGetValue(sym, out v))
                _offvals[sym] = off;
            else
                _offvals.Add(sym, off);
        }

        public void GotCancel(uint id)
        {
            // find any matching orders and reflect them as canceled
            foreach (string sym in _offvals.Keys)
            {
                if (_offvals[sym].StopId == id)
                    _offvals[sym].StopId = 0;
                else if (_offvals[sym].ProfitId == id)
                    _offvals[sym].ProfitId = 0;
            }

        }
        // track offset ids
        Dictionary<string, uint> _profitids = new Dictionary<string, uint>();
        Dictionary<string, uint> _stopids = new Dictionary<string, uint>();
        // per-symbol offset values
        Dictionary<string, OffsetInfo> _offvals = new Dictionary<string, OffsetInfo>();
        

    }

    public class OffsetInfo
    {
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent)
        {
            ProfitDist = profitdist;
            StopDist = stopdist;
            ProfitPercent = profitpercent;
            StopPercent = stoppercent;
        }
        public OffsetInfo(decimal profitdist, decimal stopdist) : this(profitdist,stopdist,1,1) {}
        public OffsetInfo() : this(0,0,1,1) {}
        public uint ProfitId;
        public uint StopId;
        public decimal ProfitDist;
        public decimal StopDist;
        public decimal ProfitPercent;
        public decimal StopPercent;
    }
}
