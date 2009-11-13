using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Responses
{
    public class LessonParamPrompt : ResponseTemplate
    {
        BarListTracker _blt = new BarListTracker();
        PositionTracker _pt = new PositionTracker();
        int MINBARS = 10;
        [Description("Minimum bars this strategy needs to trade.")]
        public int MinBars { get { return MINBARS; } set { MINBARS = value; } }
        decimal MAXVOL = .5m;
        [Description("Maximum average HL range allowed for trading.")]
        public decimal MaxVolatility { get { return MAXVOL; } set { MAXVOL = value; } }
        public override void GotTick(Tick tick)
        {
            // create bars from ticks
            _blt.newTick(tick);
            // make sure we have enough bars our indicator
            if (!_blt[tick.symbol].Has(MINBARS)) return;
            // get highs from our bar
            decimal[] highs = _blt[tick.symbol].High();
            // get lows
            decimal[] lows = _blt[tick.symbol].Low();
            // compute high low ranges
            decimal[] hlrange = Calc.Subtract(highs, lows);
            // compute average range
            decimal avghl = Calc.Avg(hlrange);
            // ignore volatile symbols
            if (avghl > MAXVOL) return;
            // compute MA
            decimal ma = Calc.Avg(_blt[tick.symbol].Close());
            // trading rule
            if (_pt[tick.symbol].isFlat && (_blt[tick.symbol].RecentBar.Close > ma))
                sendorder(new BuyMarket(tick.symbol, 100));
            // exit rule
            if (_pt[tick.symbol].isLong && (_blt[tick.symbol].RecentBar.Close < ma))
                sendorder(new MarketOrderFlat(_pt[tick.symbol]));

        }

        public override void GotFill(Trade fill)
        {
            _pt.Adjust(fill);
        }
        public override void GotPosition(Position p)
        {
            _pt.Adjust(p);
        }
    }
}
