/*
 * Created by SharpDevelop.
 * User: josh
 * Date: 2/7/2008
 * Time: 7:21 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using TradeLib;

namespace box
{
	/// <summary>
	/// Moving-average box
	/// </summary>
	public class MAbox : TradeBox
	{
        // indicator identifiers
        const int imkt = 0;
        const int ibar = 1;
		public MAbox(NewsService ns) : base(ns)
		{
			Name = "MABox";
            // auto-money-management features provided by TradeBox
			this.ProfitTarget = 1;
			this.Stop = .5m;

            // demo of indicator tracking (used by other tools)
            this.ResetIndicators(2);
            this.SetIname(imkt, "Mkt*100");
            this.SetIname(ibar, "2BarMA*100");
		}
		protected override bool EnterLong()	{ return MAenter(); }
		protected override bool EnterShort() 	{ return MAenter(); }
		protected override bool Exit()			{ return MAexit(); }
		bool MAenter()
		{
            bool enter = false;

            decimal market = this.getMostRecentTrade();
            if (!this.bl.Has(2) || (market==0)) return enter;
			decimal MA = SMA.BarSMA(this.bl,BarInterval.Minute,2);

            // debug output (optional, sent if this.Debug field is true)
			this.D("market:"+market.ToString("N2")+" MA:"+MA.ToString("N2"));

            // expose indicators (optional, if used by other tools)
            this.Indicators = new int[] { (int)(market * 100), (int)(MA * 100) };

            // rules that determine how indicators lead to entry (or not)
			if (this.Side && (market>MA)) enter = true; // long entry
			if (!this.Side && (market<MA)) enter = true; // short entry
			return enter;
		}
		
		bool MAexit()
		{
            bool exit = false;
            decimal market = this.getMostRecentTrade();
            if (!this.bl.Has(4) || (market==0)) return exit;
			decimal MA = SMA.BarSMA(this.bl,BarInterval.Minute,4);

			this.D("market:"+market.ToString("N2")+" MA:"+MA.ToString("N2"));
			if (this.Side && (market<MA)) exit = true; // long exit if market crosses
			if (!this.Side && (market>MA)) exit = true; // short exit if the market crosses
			return exit;
		}
	}
}
