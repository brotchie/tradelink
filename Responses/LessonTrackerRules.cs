using System;
using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Responses
{
    public class LessonTrackerRules : ResponseTemplate
    {
        GenericTracker<bool> indicatorcross1 = new GenericTracker<bool>("indicatorcross2");
        GenericTracker<bool> indicatorcross2 = new GenericTracker<bool>("indicatorcross1");
        GenericTracker<bool> first1then2ok = new GenericTracker<bool>("first1then2");
        GenericTracker<string> symbols = new GenericTracker<string>("symbol");

        BarListTracker blt = new BarListTracker( new BarInterval[] { BarInterval.FiveMin, BarInterval.Minute});

        public LessonTrackerRules()
        {
            blt.GotNewBar += new SymBarIntervalDelegate(GotNewBar);
            symbols.NewTxt += new TextIdxDelegate(indicatorcross1_NewTxt);

        }

        

        void GotNewBar(string symbol, int interval)
        {
            // get index for symbol
            int idx = symbols.addindex(symbol,symbol);
            // get current barlist for this symbol+interval
            BarList bl = blt[symbol, interval];
            // check for first cross on first interval
            if (interval == (int)BarInterval.Minute)
                // update the cross state
                indicatorcross1[symbol] = bl.RecentBar.Close > Calc.Avg(bl.Close());
            // check second cross 
            if (interval == (int)BarInterval.FiveMin)
                // update the cross state
                indicatorcross2[symbol] = bl.RecentBar.Close > Calc.Avg(bl.Close());
            // update first1then2
            if (first1then2ok[symbol] && indicatorcross2[symbol] && !indicatorcross1[symbol])
                first1then2ok[symbol] = false;
            // send order if everything looks good (also showed failed rules)
            if (gt.rulepasses(idx, "sequential buy entry", senddebug, true, first1then2ok,indicatorcross2, indicatorcross1))
                sendorder(new BuyMarket(symbol, 100));
        }

        public override void GotTick(Tick k)
        {
            blt.newTick(k);
        }

        void indicatorcross1_NewTxt(string txt, int idx)
        {
            indicatorcross1.addindex(txt);
            indicatorcross2.addindex(txt);
            first1then2ok.addindex(txt);
        }
    }
}
