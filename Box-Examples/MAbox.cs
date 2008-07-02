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
	public class MAbox : BlackBoxEasy
	{
        // indicator identifiers
        const int imkt = 0;
        const int ibar = 1;
		public MAbox() : base()
		{
			Name = "MABox";
            // auto-money-management features provided by BlackBoxEasy
			this.ProfitTarget = 1;
			this.Stop = .5m;

            // demo of indicator tracking (used by other tools)
            // we will push the values of these indicators so they can be retrieved elsewhere
            this.ResetIndicators(2);
            this.SetIname(imkt, "Mkt");
            this.SetIname(ibar, "2BarMA");
		}
        // where to find our entry rules... in this case we're defining entry rules the same for long and short
		protected override bool EnterLong()	{ return MAenter(); }
		protected override bool EnterShort() 	{ return MAenter(); }

        // where to find our exit rules
		protected override bool Exit()			{ return MAexit(); }

        // we define our entry rules here
		bool MAenter()
		{
            // we'll only enter if this value is changed to true
            bool enter = false;

            decimal market = this.getMostRecentTrade();
            if (!this.bl.Has(2) || (market==0)) return enter;
            // get our MA
			decimal MA = SMA.BarSMA(this.bl,BarInterval.Minute,2);

            // debug output (optional, sent if this.Debug field is true)
			this.D("market:"+market.ToString("N2")+" MA:"+MA.ToString("N2"));

            // expose indicators (optional, if used by other tools)
            this.Indicators = new object[] { market, MA };

            // rules that determine how indicators lead to entry (or not)
			if (this.Side && (market>MA)) enter = true; // long entry
			if (!this.Side && (market<MA)) enter = true; // short entry
			return enter;
		}
		
        // our exit rules
		bool MAexit()
		{
            // we'll only exit if this value is changed to true
            bool exit = false;

            decimal market = this.getMostRecentTrade();
            // we want a minimum number of bars
            if (!this.bl.Has(4) || (market==0)) return exit;
			decimal MA = SMA.BarSMA(this.bl,BarInterval.Minute,4);

			this.D("market:"+market.ToString("N2")+" MA:"+MA.ToString("N2"));

            // rules to determine how we can exit a position
			if (this.Side && (market<MA)) exit = true; // long exit if market crosses
			if (!this.Side && (market>MA)) exit = true; // short exit if the market crosses
			return exit;
		}
	}
}
