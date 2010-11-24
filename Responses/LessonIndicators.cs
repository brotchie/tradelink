using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;

namespace Responses
{
    public class LessonIndicatorSimple : ResponseTemplate
    {
        BarListTracker _blt = new BarListTracker();
        int MINBARS = 10;
        decimal MAXVOL = .5m;
        public override void GotTick(Tick tick)
        {
            // create bars from ticks
            _blt.newTick(tick);
            // make sure we have enough bars our indicator
            if (!_blt[tick.symbol].Has(MINBARS)) return;
            // get highs from our bar
            decimal [] highs = _blt[tick.symbol].High();
            // get lows
            decimal [] lows = _blt[tick.symbol].Low();
            // compute high low ranges
            decimal [] hlrange = Calc.Subtract(highs,lows);
            // compute average range
            decimal avghl = Calc.Avg(hlrange);
            // debug it
            senddebug(avghl.ToString());
            // extra credit
            if (avghl > MAXVOL)
                senddebug(tick.symbol + " exceeded max volatility " + tick.time);
            // display it (gauntlet and kadina only)
            sendindicators(new string[] { tick.time.ToString(), avghl.ToString("N2") } );
            // plot it (in kadina only)
            sendchartlabel(avghl+_blt[tick.symbol].RecentBar.Close, tick.time);
        }

        public LessonIndicatorSimple()
        {
            Indicators = new string[] { "Time","AVGHL" };
        }
    }

    public class LessonIndicatorComplex : ResponseTemplate
    {
        ComplexSpreadIndicator _csi = new ComplexSpreadIndicator();
        decimal BIGSPREAD = .2m;
        public override void GotTick(Tick tick)
        {
            // update our indicator
            _csi.GotTick(tick);
            // verify our indicator has enough data
            if (!_csi.hasSpread(tick.symbol)) return;
            // when we have enough data, we can now get spread
            decimal spread = _csi[tick.symbol];
            // debug it
            if (spread> BIGSPREAD)
                senddebug(tick.symbol+" too big to trade "+tick.time);
            // display our spread(gauntlet and kadina only)
            sendindicators(new string[] { tick.time.ToString(), spread.ToString("N2") });
        }

        public LessonIndicatorComplex()
        {
            Indicators = new string[] { "Time", "Spread" };
        }
    }

    /// <summary>
    /// keeps track of spreads for many symbols
    /// </summary>
    public class ComplexSpreadIndicator : GotTickIndicator
    {
        // some dictionaries which hold bid and ask for each symbol
        Dictionary<string, decimal> _bid = new Dictionary<string, decimal>();
        Dictionary<string, decimal> _ask = new Dictionary<string, decimal>();
        // allows us to test if we have a full spread
        public bool hasSpread(string sym) 
        { 
            // make sure we have bid and ask values
            return _bid.ContainsKey(sym) && _ask.ContainsKey(sym);
        }
        // tracks spreads from ticks
        public void GotTick(Tick k)
        {
            // create a temp value so we can quickly query dictionaries
            decimal v = 0;
            // if we have a bid
            if (k.hasBid)
                // if it's not our first bid
                if (_bid.TryGetValue(k.symbol, out v))
                    _bid[k.symbol] = k.bid; // we can update it
                else // otherwise we must add it
                    _bid.Add(k.symbol,k.bid);
            // do same for ask
            if (k.hasAsk)
                if (_ask.TryGetValue(k.symbol, out v))
                    _ask[k.symbol] = k.ask;
                else 
                    _ask.Add(k.symbol,k.ask);
        }
        /// <summary>
        /// obtains current spread, or zero for no spread
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Spread(string sym)
        {
            return hasSpread(sym) ? _ask[sym] - _bid[sym] : 0;
        }
        /// <summary>
        /// obtains current spread
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal this[string sym] { get { return Spread(sym); } }

    }
}
