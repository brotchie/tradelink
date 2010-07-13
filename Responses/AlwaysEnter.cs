using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;

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
        public AlwaysEnter() : base() 
        {
            Name = "AlwaysEnter";
        }

        public override void GotTick(TradeLink.API.Tick tick)
        {
            // track tick
            _kt.newTick(tick);
            // get current position
            Position p = _pt[tick.symbol];

            // if we're flat, enter
            if (p.isFlat)
            {
                D(tick.symbol+": entering long");
                O(new MarketOrder(tick.symbol, EntrySize));
            }
            else 
            {
                // get most recent tick data
                Tick k = _kt[tick.symbol];
                // estimate our exit price
                decimal exitprice = UseQuotes 
                    ? (k.hasAsk && p.isLong ? k.ask 
                    : (k.hasBid && p.isShort ? k.bid : 0))
                    : (k.isTrade ? k.trade : 0);
                // assuming we could estimate an exit, see if our exit would hit our target
                if ((exitprice != 0) && (Calc.OpenPT(exitprice, p) > ProfitTarget))
                {
                    D("hit profit target");
                    O(new MarketOrderFlat(p));
                }
                
                
            }
        }

        PositionTracker _pt = new PositionTracker();
        TickTracker _kt = new TickTracker();

        public override void GotFill(TradeLink.API.Trade fill)
        {
            // keep track of position
            D(fill.symbol + " fill: " + fill.ToString());
            _pt.Adjust(fill);
        }

        public override void GotPosition(TradeLink.API.Position p)
        {
            // keep track of position
            D(p.Symbol + " pos: " + p.ToString());
            _pt.Adjust(p);
        }

        int _entrysize = 100;
        [Description("size used when entering positions.  Negative numbers would be short entries.")]
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }
        decimal _profittarget = .01m;
        [Description("profit target in dollars when position is exited")]
        public decimal ProfitTarget { get { return _profittarget; } set { _profittarget = value; } }
        bool _usequotes = false;
        [Description("whether bid/ask is used to determine profitability, otherwise last trade is used")]
        public bool UseQuotes { get { return _usequotes; } set { _usequotes = value; } }

    }

    /// <summary>
    /// allows user to control alwaysenter parameters through graphical pop-up interface
    /// </summary>
    public class AlwaysEnter_Parameters : AlwaysEnter
    {
        public override void Reset()
        {
            ParamPrompt.Popup(this, true, false);
            base.Reset();
        }
    }
}
