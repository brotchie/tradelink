
using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel; // required for ParamPrompt

namespace Responses
{
    /// <summary>
    /// An example of a strategy that buys and sells Asymetries around the open.
    /// 
    /// If the open and the low are close but the high is far away, sell the open when the market reapproaches it.
    /// If the open and high and close but the low is far away, buy the open when the market reapproaches it.
    /// </summary>
	public class OpenAsymetry : ResponseTemplate
	{
        public OpenAsymetry()
        {
            // enable prompting of user for parameters
            ParamPrompt.Popup(this);
        }

        // here are parameters to prompt for and their descriptions
        [Description("minimum asymmetry allowed for entry")]
        public decimal MinAsymetry { get { return a; } set { a = value; } }
        [Description("minimum range required for asymmetry test")]
        public decimal MinRange { get { return r; } set { r = value; } }
        [Description("how close market must be to asymmetry before entry")]
        public decimal AnticipateDistance { get { return d; } set { d = value; } }
        [Description("profit target in points")]
        public decimal Profit { get { return p; } set { p = value; } }
        [Description("stop loss in points")]
        public decimal Stop { get { return s; } set { s = value; } }
        [Description("entry size for trade")]
        public int EntrySize { get { return e; } set { e = value; } }


        // here are the defaults for the above parameters
        decimal a = .06m;  
        decimal r = .37m; 
        decimal d = .1m; 
        decimal p = .2m; 
        decimal s = -.11m; 
        int e = 100;
        
        // enable bar tracking
        BarListTracker blt = new BarListTracker();
        // enable position tracking
        PositionTracker pt = new PositionTracker();

        public override void  GotTick(Tick tick)
        {
            // enable bars for all ticks that come in
            blt.newTick(tick);

            // if we don't have one bar yet, wait for more ticks until continuing
            if (!blt[tick.symbol].Has(1)) return; 

            // get high
            decimal h = Calc.HH(blt[tick.symbol]);
            // get low
            decimal l = Calc.LL(blt[tick.symbol]);
            // get open
            decimal o = blt[tick.symbol][0].Open;

            // asymmetry calculations...

            // if there is no asymetry in this market, shutdown this response
            if (((h - o) > a) && ((o - l) > a))
            {
                isValid = false;
                return;
            }
            // if the range for the market is too small, wait
            if ((h - l) < r) return; 
            // if the market gapped down after opening, 
            // but now is turning around... go long
            if (((h - o) <= a) && ((h - tick.trade) > e))
                sendorder(new BuyMarket(tick.symbol, EntrySize));
            // if market gapped up after open, 
            // but now is turning around to go negative... go short
            if (((o - l) <= a) && ((tick.trade - l) > e))
                sendorder(new SellMarket(tick.symbol, EntrySize));

            // get open pl in points
            decimal points = Calc.OpenPT(tick.trade, pt[tick.symbol]);
            // if we hit our profit or loss target, stop trading
            if ((points > p) || (points < s))
            {
                // flat us
                sendorder(new MarketOrderFlat(pt[tick.symbol]));
                // don't trade anymore
                isValid = false;
            }
        }
    }
}
