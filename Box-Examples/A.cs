/*
 * Created by SharpDevelop.
 * User: josh
 * Date: 2/5/2008
 * Time: 5:50 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using TradeLib;

namespace box
{
    /// <summary>
    /// An example of a strategy that buys and sells Asymetries around the open.
    /// 
    /// If the open and the low are close but the high is far away, sell the open when the market reapproaches it.
    /// If the open and high and close but the low is far away, buy the open when the market reapproaches it.
    /// </summary>
	public class A : Box 
	{
        public A(NewsService ns) : base(ns) 
        { 
            Version = "$Rev$"; 
            Name = "A"+CleanVersion; 
        }
        BarList bars;

        // we need high-low and open for this box
        decimal h{ get { return BarMath.HH(bars); } }
        decimal l { get { return BarMath.LL(bars); } }
        decimal o { get { return bars.Get(0).Open; } }
        decimal PL(decimal t) { if (PosSize == 0) return 0; return (PosSize > 0) ? t - AvgPrice : AvgPrice - t; }

        // here are the parameters that define how close we need to be to the open
        const decimal a = .06m;
        const decimal r = .37m;
        const decimal e = .1m;
        const decimal p = .2m;
        const decimal s = .11m;
        
        // here are the trading rules that implement our strategy's intention
        protected override int Read(Tick tick, BarList bl,BoxInfo bi)
        {
            bars = bl;
            if (!bl.Has(2)) return 0; // must have at least one one bar
            if (((h - o) > a) && ((o - l) > a)) { Shutdown("Not enough Asymetry to trade."); return 0; }
            if ((h - l) < r) return 0; // must have the range
            if (((h-o)<=a) && ((h-tick.trade)>e)) return MaxSize;
            if (((o-l)<=a) && ((tick.trade-l)>e)) return MaxSize*-1;
            if (PL(tick.trade)>p) return PosSize*-1;
            if (PL(tick.trade)>s) return PosSize*-1;
            return 0;
        }
    }
}
