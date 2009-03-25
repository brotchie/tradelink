using System;
using System.Collections.Generic;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel; 

namespace Responses
{
    public class SMAResponse : ResponseTemplate
    {
        // parameters of this system
        [Description("Total Profit Target")]
        public decimal TotalProfitTarget { get { return _totalprofit; } set { _totalprofit = value; } }
        [Description("Entry size when signal is found")]
        public int EntrySize { get { return _entrysize; } set { _entrysize = value; } }
        [Description("Default bar interval for this response.")]
        public BarInterval Interval { get { return _barinterval; } set { _barinterval = value; } }
        [Description("Bars to use in calculating simple moving average (SMA).")]
        public int BarsBack { get { return _barsback; } set { _barsback = value; } }

        // this function is called the constructor, because it sets up the response
        // it is run before all the other functions, and has same name as my response.
        public SMAResponse() : this(true) { }
        public SMAResponse(bool prompt)
        {
            // enable prompting of system parameters to user,
            // so they do not have to recompile to change things
            ParamPrompt pp = new ParamPrompt(this);
            // show prompt to user
            pp.ShowDialog();

            // make sure default interval is used when tracking bars
            blt.DefaultInterval = Interval;

            // set our indicator names, in case we import indicators into R
            // or excel, or we want to view them in gauntlet or kadina
            Indicators = new string[] { "SMA" };
        }

        // turn on bar tracking
        BarListTracker blt = new BarListTracker();
        // turn on position tracking
        PositionTracker pt = new PositionTracker();

        // got tick is called whenever this strategy receives a tick
        public override void GotTick(Tick tick)
        {
            // apply bar tracking to all ticks that enter
            blt.newTick(tick);
            // make sure we have minimum number of bars,
            // otherwise just wait for next tick before going forward
            if (!blt[tick.symbol].Has(BarsBack)) return;

            // ignore anything that is not a trade
            if (!tick.isTrade) return;

            // if we made it here, we have enough bars and we have a trade.

            // exits are processed first, lets see if we have our total profit
            if (Calc.OpenPL(tick.trade, pt[tick.symbol]) + pt[tick.symbol].ClosedPL > TotalProfitTarget)
            {
                // if we hit our target, flat our position
                sendorder(new MarketOrderFlat(pt[tick.symbol]));
                // don't process anything else after this (entries, etc)
                return;
            }
            
            // if we are here, we haven't hit our profit target.
            // lets do our entries.  

            // first lets calculate the moving average.

            // we need the closing prices for the requested # of bars
            decimal[] prices = Calc.Closes(blt[tick.symbol], BarsBack);

            // then lets calculate the SMA
            decimal SMA = Calc.Avg(prices);

            // if our current price is above SMA, buy
            if (tick.trade > SMA)
                sendorder(new BuyMarket(tick.symbol, EntrySize));
            // otherwise if it's less than SMA, sell
            if (tick.trade < SMA)
                sendorder(new SellMarket(tick.symbol, EntrySize));

            // this way we can debug our indicators during development
            // indicators are sent in the same order as they are named above
            sendindicators(SMA.ToString());

        }

        public override void GotFill(Trade fill)
        {
            // make sure every fill is tracked against a position
            pt.Adjust(fill);
        }

        public override void GotPosition(Position p)
        {
            // make sure every position set at strategy startup is tracked
            pt.Adjust(p);
        }

        // these variables "hold" the parameters set by the user above
        // also they are the defaults that show up first
        int _barsback = 2;
        BarInterval _barinterval = BarInterval.FiveMin;
        int _entrysize = 100;
        decimal _totalprofit = 200;
    }

    /// <summary>
    /// this is the same as SMAResponse, except it runs without prompting user
    /// </summary>
    public class SMAResponseAuto : SMAResponse
    {
        public SMAResponseAuto() : base(false) { }
    }
}
