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
        public int EntrySize { get { return _entrysize; } set { _entrysize= value; } }
        [Description("Default bar interval for this response.")]
        public BarInterval Interval { get { return _barinterval; } set { _barinterval = value; } }

        // this function is called the constructor, because it sets up the response
        // it is run before all the other functions, and has same name as my response.
        public SMAResponse() : this(true) { }
        public SMAResponse(bool prompt)
        {
            // enable prompting of system parameters to user,
            // so they do not have to recompile to change things
            ParamPrompt.Popup(this,!prompt);

            // only build bars for user's interval
            blt = new BarListTracker(Interval);

            // only calculate on new bars
            blt.GotNewBar += new SymBarIntervalDelegate(blt_GotNewBar);


            // set our indicator names, in case we import indicators into R
            // or excel, or we want to view them in gauntlet or kadina
            Indicators = new string[] { "SMA" };
        }

        void blt_GotNewBar(string symbol, int interval)
        {
            // lets do our entries.  

            // calculate the SMA using closign prices
            decimal SMA = Calc.Avg(blt[symbol].Close());

            // if our current price is above SMA, buy
            if (blt[symbol].RecentBar.Close > SMA)
                sendorder(new BuyMarket(symbol, EntrySize));
            // otherwise if it's less than SMA, sell
            if (blt[symbol].RecentBar.Close < SMA)
                sendorder(new SellMarket(symbol, EntrySize));

            // this way we can debug our indicators during development
            // indicators are sent in the same order as they are named above
            sendindicators(SMA.ToString());
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

            // ignore anything that is not a trade
            if (!tick.isTrade) return;

            // if we made it here, we have enough bars and we have a trade.

            // exits are processed first, lets see if we have our total profit
            if (Calc.OpenPL(tick.trade, pt[tick.symbol]) + pt[tick.symbol].ClosedPL > TotalProfitTarget)
            {
                // if we hit our target, flat our position
                sendorder(new MarketOrderFlat(pt[tick.symbol]));
                // shut us down
                isValid = false;
                // don't process anything else after this (entries, etc)
                return;
            }

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
