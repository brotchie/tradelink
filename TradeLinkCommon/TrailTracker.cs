using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// simulate a trailing stop for a number of positions
    /// </summary>
    public class TrailTracker
    {
        PositionTracker _pt = null;
        /// <summary>
        /// position tracker used by this component
        /// </summary>
        public PositionTracker pt { get { return _pt; } set { _pt = value; } }
        /// <summary>
        /// creates trail tracker (with it's own position tracker)
        /// </summary>
        public TrailTracker() : this(new PositionTracker()) { }
        /// <summary>
        /// creates a trail tracker from an existing position tracker component
        /// </summary>
        /// <param name="pt"></param>
        public TrailTracker(PositionTracker pt)
        {
            _pt = pt;
        }
        bool _trailbydefault = true;
        /// <summary>
        /// gets or sets the trail amount for a given symbol
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public OffsetInfo this[string sym] 
        {
            get
            {
                // get index
                int idx = symidx(sym);
                // if not there, get default
                if (idx == NOSYM) return _trailbydefault ? new OffsetInfo(_defaulttrail) : new OffsetInfo(0,0);
                // otherwise return whats there
                return _trail[idx];
            }
            set 
            {
                // get index
                int idx = symidx(sym);
                // if not there, save this info
                if (idx == NOSYM)
                {
                    _symidx.Add(sym, _trail.Count);
                    _trail.Add(value);
                    _ref.Add(0);
                }
                else // save it
                    _trail[idx] = value;

            } 
        }
        /// <summary>
        /// whether trailing stops are created by default for any symbol seen
        /// </summary>
        public bool TrailByDefault { get { return _trailbydefault; } set { _trailbydefault = value; } }
        OffsetInfo _defaulttrail = new OffsetInfo(0, 0);
        /// <summary>
        /// when TrailByDefault is enabled, default trail amount is used for symbols that do not have a trail configured
        /// </summary>
        public OffsetInfo DefaultTrail { get { return _defaulttrail; } set { _defaulttrail = value; } }
        Dictionary<string, int> _symidx = new Dictionary<string, int>();
        List<OffsetInfo> _trail = new List<OffsetInfo>();
        List<decimal> _ref = new List<decimal>();
        const int NOSYM = -1;
        int symidx(string sym)
        {
            int idx = NOSYM;
            if (_symidx.TryGetValue(sym, out idx))
                return idx;
            return NOSYM;
        }
        bool _valid = true;
        /// <summary>
        /// set to true if trailing stop are used, false if not
        /// </summary>
        public bool isValid { get { return _valid; } set { _valid = value; } }
        /// <summary>
        /// must pass ticks as received to this function, in order to have trailing stops executed at proper time.
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            // see if we're turned on
            if (!isValid) return;
            // see if we can exit when trail is broken
            if (SendOrder == null) return;
            // see if we have anything to trail against
            if (_pt[k.symbol].isFlat) return;
            // only care about trades
            if (!k.isTrade) return;
            // get index for symbol
            int idx = symidx(k.symbol);
            // setup parameters
            OffsetInfo trail = null;
            decimal refp = 0;
            // see if we trail this symbol
            if ((idx == NOSYM) && _trailbydefault)
            {
                // get parameters
                idx = _trail.Count;
                refp = k.trade;
                trail = new OffsetInfo(_defaulttrail);
                // save them
                _symidx.Add(k.symbol, idx);
                _ref.Add(refp);
                // just in case user is modifying on seperate thread
                lock (_trail)
                {
                    _trail.Add(trail);
                }
            }
            else if ((idx == NOSYM) && !_trailbydefault)
                return;
            else
            {
                // get parameters
                refp = _ref[idx];
                // just in case user tries to modify on seperate thread
                lock (_trail)
                {
                    trail = _trail[idx];
                }
            }

            // see if we need to update ref price
            if ((refp == 0)
                || (_pt[k.symbol].isLong && (refp < k.trade))
                || (_pt[k.symbol].isShort && (refp > k.trade)))
            {
                // update
                refp = k.trade;
                // save it
                _ref[idx] = refp;
            }

            // see if we broke our trail
            if (Math.Abs(refp - k.trade) > trail.StopDist)
            {
                // send flat order
                SendOrder(new MarketOrderFlat(_pt[k.symbol], trail.StopPercent, trail.NormalizeSize, trail.MinimumLotSize));
            }





        }

        public event OrderDelegate SendOrder;

        /// <summary>
        /// this must be called once per position tracker, for each position update.
        /// if you are using your own position tracker with this trailing stop(eg from offset tracker, or somewhere else)
        /// you only need to adjust it once, so if you adjust it directly you don't need to call again here.
        /// </summary>
        /// <param name="p"></param>
        public void Adjust(Position p)
        {
            _pt.Adjust(p);
        }

        /// <summary>
        /// this must be called once per position tracker, for each position update.
        /// if you are using your own position tracker with this trailing stop(eg from offset tracker, or somewhere else)
        /// you only need to adjust it once, so if you adjust it directly you don't need to call again here.
        /// </summary>
        /// <param name="fill"></param>
        public void Adjust(Trade fill)
        {
            _pt.Adjust(fill);
        }
    }
}
