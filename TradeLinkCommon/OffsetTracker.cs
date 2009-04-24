using System;
using System.Collections.Generic;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// allows automatic sending of profit targets and stop orders for a set of positions.
    /// automatically manages partial fills.
    /// </summary>
    public class OffsetTracker
    {
        OffsetInfo _default = new OffsetInfo();
        string[] _ignore = new string[0];
        /// <summary>
        /// default offset used by the tracker, in the event no custom offset is set. eg ot["IBM"] = new OffsetInfo();
        /// </summary>
        public OffsetInfo DefaultOffset { get { return new OffsetInfo(_default); } set { value.ProfitId = 0; value.StopId = 0; _default = value; } }
        bool _ignoredefault = false;
        /// <summary>
        /// ignore symbols by default.   if true... a symbol has no custom offset defined will be ignored (regardless of ignore list).  the default is false.
        /// </summary>
        public bool IgnoreDefault { get { return _ignoredefault; } set { _ignoredefault = value; } }
        /// <summary>
        /// always ignore these symbols.   this list is only in affect when IgnoreDefault is false.
        /// </summary>
        public string[] IgnoreSyms { get { return _ignore; } set { _ignore = value; } }
        bool _hasevents = false;
        public event OrderDelegate SendOffset;
        public event UIntDelegate SendCancel;
        PositionTracker _pt = new PositionTracker();
        /// <summary>
        /// a position tracker you can reuse 
        /// </summary>
        public PositionTracker PositionTracker { get { return _pt; } set { _pt = value; } }
        public OffsetTracker() { }
        public IdTracker _ids = new IdTracker();
        /// <summary>
        /// id tracker used by offsettracker, you can reuse in other apps you use OT.
        /// </summary>
        public IdTracker Ids { get { return _ids; } set { _ids = value; } }
        public OffsetTracker(uint initialid) : this(new IdTracker(initialid)) { }
        public OffsetTracker(IdTracker tracker)
        {
            _ids = tracker;
        }

        /// <summary>
        /// clear single custom offset
        /// </summary>
        /// <param name="sym"></param>
        public void ClearCustom(string sym) { _offvals.Remove(sym); }
        /// <summary>
        /// clear all custom offsets
        /// </summary>
        public void ClearCustom() { _offvals.Clear(); }

        int _cancelpause = 10;
        /// <summary>
        /// sleep briefly after sending cancel orders to allow them to process.
        /// </summary>
        public int CancelSleep { get { return _cancelpause; } set { _cancelpause = value; } }

        void doupdate(string sym) { doupdate(sym, false); }
        void doupdate(string sym, bool poschange)
        {
            // is update ignored?
            if (IgnoreUpdate(sym)) return;
            // get our offset values
            OffsetInfo off = GetOffset(sym);
            // see if we have profit
            if (off.hasProfit)
            {
                // cancel existing profits
                cancel(off.ProfitId);
                // wait a moment to allow cancel to be received
                System.Threading.Thread.Sleep(_cancelpause);
            }
            // see if we have stop
            if (off.hasStop)
            {
                // cancel existing stops
                cancel(off.StopId);
                // wait a moment to allow cancel to be received
                System.Threading.Thread.Sleep(_cancelpause);
            }

            if (!off.hasProfit)
            {
                // get new profit
                Order profit = Calc.PositionProfit(_pt[sym], off.ProfitDist, off.ProfitPercent, off.NormalizeSize, off.MinimumLotSize);
                // if it's valid, send and track it
                if (profit.isValid)
                {
                    profit.id = Ids.AssignId;
                    off.ProfitId = profit.id;
                    SendOffset(profit);
                }
            }
            if (!off.hasStop)
            {
                // get new stop
                Order stop = Calc.PositionStop(_pt[sym], off.StopDist, off.StopPercent, off.NormalizeSize, off.MinimumLotSize);
                // if it's valid, send and track
                if (stop.isValid)
                {
                    stop.id = Ids.AssignId;
                    off.StopId = stop.id;
                    SendOffset(stop);
                }
            }
            // make sure new offset info is reflected
            SetOffset(sym, off);


        }

        bool hascustom(string symbol) { OffsetInfo oi; return _offvals.TryGetValue(symbol, out oi); }

        void cancel(OffsetInfo offset) { cancel(offset.ProfitId); cancel(offset.StopId); }
        void cancel(uint id) { if (id != 0) SendCancel(id); }
        /// <summary>
        /// cancels all offsets (profit+stops) for given side
        /// </summary>
        /// <param name="side"></param>
        public void CancelAll(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel all offsets for side
                if (p.isLong==side)
                    cancel(GetOffset(p.Symbol));
            }
        }
        /// <summary>
        /// cancels all offsets for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelAll(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol==sym)
                    cancel(GetOffset(sym));
            }
            
        }

        /// <summary>
        /// cancels only profit orders for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelProfit(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol == sym)
                    cancel(GetOffset(sym).ProfitId);
            }
        }

        /// <summary>
        /// cancels only stops for symbol
        /// </summary>
        /// <param name="sym"></param>
        public void CancelStop(string sym)
        {
            foreach (Position p in _pt)
            {
                // if sym matches, cancel all offsets
                if (p.Symbol == sym)
                    cancel(GetOffset(sym).StopId);
            }
        }

        /// <summary>
        /// cancel profits for side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelProfit(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel profits for side
                if (p.isLong == side)
                    cancel(GetOffset(p.Symbol).ProfitId);
            }
        }

        /// <summary>
        /// cancel stops for a side (long is true, false is short)
        /// </summary>
        /// <param name="side"></param>
        public void CancelStop(bool side)
        {
            foreach (Position p in _pt)
            {
                // make sure we're not flat
                if (p.isFlat) continue;
                // if side matches, cancel stops for side
                if (p.isLong == side)
                    cancel(GetOffset(p.Symbol).StopId);
            }
        }

        /// <summary>
        /// cancels all tracked offsets
        /// </summary>
        public void CancelAll()
        {
            foreach (string sym in _offvals.Keys)
                cancel(GetOffset(sym));
        }


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
            // if updates are ignored by default
            if (_ignoredefault) // see if we have custom offset
                return !hascustom(sym);
            // otherwise see if it's specifically ignored
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
        public void Adjust(Position p)
        {
            // update position
            _pt.Adjust(p);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(p.Symbol,true);
        }

        /// <summary>
        /// must send new fills here (eg call from Response.GotFill)
        /// </summary>
        /// <param name="t"></param>
        public void Adjust(Trade t)
        {
            // update position
            _pt.Adjust(t);
            // do we have events?
            if (!HasEvents()) return;
            // do update
            doupdate(t.symbol,true);

        }

        /// <summary>
        /// obtain curretn offset information for a symbol.
        /// if no custom value has been set, use default
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public OffsetInfo this[string symbol] { get { return GetOffset(symbol); } set { SetOffset(symbol, value); } }

        OffsetInfo GetOffset(string sym)
        {
            OffsetInfo val;
            // see if we have a custom offset
            if (_offvals.TryGetValue(sym, out val))
                return val;
            // otherwise use default
            return DefaultOffset;
        }

        void SetOffset(string sym, OffsetInfo off)
        {
            OffsetInfo v;
            if (_offvals.TryGetValue(sym, out v))
                _offvals[sym] = off;
            else
                _offvals.Add(sym, off);
        }
        /// <summary>
        /// should be called from GotCancel, when cancels arrive from broker.
        /// </summary>
        /// <param name="id"></param>
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
        /// <summary>
        /// should be called from GotTick, when ticks arrive.
        /// If cancels are not processed on fill updates, they will be resent each tick until they are processed.
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            // update the offsets for this tick's symbol
            doupdate(k.symbol);
        }

        // track offset ids
        Dictionary<string, uint> _profitids = new Dictionary<string, uint>();
        Dictionary<string, uint> _stopids = new Dictionary<string, uint>();
        // per-symbol offset values
        Dictionary<string, OffsetInfo> _offvals = new Dictionary<string, OffsetInfo>();
        

    }

    /// <summary>
    /// instructions used to control offset amounts and distances.
    /// </summary>
    public class OffsetInfo
    {
        /// <summary>
        /// copy an existing offset to this one
        /// </summary>
        /// <param name="copy"></param>
        public OffsetInfo(OffsetInfo copy) : this(copy.ProfitDist, copy.StopDist, copy.ProfitPercent, copy.StopPercent, copy.NormalizeSize, copy.MinimumLotSize) { }
        /// <summary>
        /// create an offset instruction
        /// </summary>
        /// <param name="profitdist">in cents</param>
        /// <param name="stopdist">in cents</param>
        /// <param name="profitpercent">in percent (eg .1 = 10%)</param>
        /// <param name="stoppercent">in percent (eg .1 = 10%)</param>
        /// <param name="NormalizeSize">true or false</param>
        /// <param name="MinSize">minimum lot size when normalize size is true</param>
        public OffsetInfo(decimal profitdist, decimal stopdist, decimal profitpercent, decimal stoppercent, bool NormalizeSize, int MinSize)
        {
            ProfitDist = profitdist;
            StopDist = stopdist;
            ProfitPercent = profitpercent;
            StopPercent = stoppercent;
            this.NormalizeSize = NormalizeSize;
            MinimumLotSize = MinSize;
        }
        public bool NormalizeSize = false;
        public int MinimumLotSize = 1;
        public OffsetInfo(decimal profitdist, decimal stopdist) : this(profitdist,stopdist,1,1,false, 1) {}
        public OffsetInfo() : this(0,0,1,1,false,1) {}
        public uint ProfitId = 0;
        public uint StopId = 0;
        public decimal ProfitDist = 0;
        public decimal StopDist = 0;
        public decimal ProfitPercent = 1;
        public decimal StopPercent = 1;
        public bool hasProfit { get { return ProfitId != 0; } }
        public bool hasStop { get { return StopId != 0; } }

    }
}
