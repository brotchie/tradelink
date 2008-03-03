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
		public MAbox(NewsService ns) : base(ns)
		{
			Name = "MABox";
			this.ProfitTarget = 1;
			this.Stop = .5m;
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

			this.D("market:"+market.ToString("N2")+" MA:"+MA.ToString("N2"));
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
