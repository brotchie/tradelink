using System;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    /// <summary>
    /// This response will always enter long on every tick, unless he's already in a position.
    /// 
    /// If he's in a position, he will exit at pre-defined stop and loss targets.  (then get back in)
    /// 
    /// Used for testing applications that use responsees.  (Quotopia, Gauntlet, etc)
    /// </summary>
    public class AlwaysEnter : ResponseTemplate
    {
        PositionTracker pt = new PositionTracker();
        public AlwaysEnter() : base() 
        {
            Name = "AlwaysEnter";
        }

        public override void GotTick(TradeLink.API.Tick tick)
        {
            // ignore quotes
            if (!tick.isTrade) return;
            // get current position
            Position p = pt[tick.symbol];
            // if we're flat, enter
            if (p.isFlat)
            {
                D("entering long");
                O(new BuyMarket(tick.symbol, 1));
            }
            // otherwise if we're up 10/th of a point, flat us
            else if (Calc.OpenPT(tick.trade, p) > .1m)
            {
                D("hit profit target");
                O(new MarketOrderFlat(p));
            }
        }

        public override void GotFill(TradeLink.API.Trade fill)
        {
            // keep track of position
            pt.Adjust(fill);
        }

        public override void GotPosition(TradeLink.API.Position p)
        {
            // keep track of position
            pt.Adjust(p);
        }
    }
}
