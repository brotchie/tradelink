using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel; // required for param prompt

namespace Responses
{
    /// <summary>
    /// Grey response that will monitor existing position,
    /// and automatically exit for you if market goes against you. 
    /// Essentially a smarter stop-loss.
    /// </summary>
    public class GreyExit : ResponseTemplate
    {

        // here's the exit size we allow the user to choose at startup

        [DescriptionAttribute("% of our trade to exit when MA is crossed")]
        public decimal ExitSizePercent { get { return exitpercent; } set { exitpercent = value; } }
        [DescriptionAttribute("Interval to use in MA calculation")]
        public BarInterval Interval { get { return _int; } set { _int = value; } }
        [DescriptionAttribute("Number of bars to use in SMA.")]
        public int BarsBack { get { return _bb; } set { _bb = value; } }

        public GreyExit()
        {

            // here's how we prompt for parameters
            ParamPrompt.Popup(this);
            
            // set default bar interval based on user's selection
            blt.DefaultInterval = Interval;

            // definae names for our indicators
            Indicators = new string[] { "SMA", "Crossed?" };


        }

        // enable bar tracking
        BarListTracker blt = new BarListTracker();
        // enable position tracking
        PositionTracker pt = new PositionTracker();

        // GotTick is called everytime a new quote or trade occurs
        public override void  GotTick(Tick tick)
        {
            // make sure every tick has bars
            blt.newTick(tick);

            // if we don't have enough bars, wait for more ticks
            if (!blt[tick.symbol].Has(BarsBack)) return;

            // if we don't have a trade, wait to calculate indicators here
            if (!tick.isTrade) return;

            // this is a grey box that manages exits, so wait until we have a position
            if (pt[tick.symbol].isFlat) return;

            // calculate the MA from closing bars
            decimal MA = Calc.Avg(Calc.Closes(blt[tick.symbol], BarsBack));

            // if we're short, a cross is when market moves above MA
            // if we're long, cross is when market goes below MA
            bool cross = pt[tick.symbol].isShort ? (tick.trade > MA) : (tick.trade < MA);

            // if we have a cross, then flat us for the requested size
            if (cross)
                sendorder(new MarketOrderFlat(pt[tick.symbol],exitpercent));

            // notify gauntlet and kadina about our moving average and cross
            sendindicators(new string[] { MA.ToString(), cross.ToString() } );
           
        }

        // called whenever a trade occurs
        public override void GotFill(Trade fill)
        {
            // make sure positions reflect all trades that occur
            pt.Adjust(fill);
        }
        public override void GotPosition(Position p)
        {
            // make sure all positions existing at startup are reflected
            pt.Adjust(p);
        }

        // these are the default values of our user parameters above
        private decimal exitpercent = 0;
        private decimal MA = 0;
        BarInterval _int = BarInterval.FiveMin;
        bool _above = true; // trigger when crosses above or below
        bool _autoabove = false; // set above automatically based on position
        int _bb = 2; // bars back

    }
}
