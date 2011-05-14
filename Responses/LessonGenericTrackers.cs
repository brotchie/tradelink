using System;
using TradeLink.API;
using TradeLink.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Responses
{
    public class LessonGenericTrackers : ResponseTemplate
    {
        GenericTracker<bool> indicatorcross1 = new GenericTracker<bool>("indicatorcross1");
        GenericTracker<bool> indicatorcross2 = new GenericTracker<bool>("indicatorcross2");
        GenericTracker<bool> first1then2ok = new GenericTracker<bool>("first1then2");

        GenericTracker<string> PRIMARY = new GenericTracker<string>("SYMBOL");
        GenericTracker<int> expectedsize = new GenericTracker<int>("expect");
        GenericTracker<bool> entryok = new GenericTracker<bool>("entryok");

        PositionTracker pt = new PositionTracker();
        BarListTracker blt = new BarListTracker(new BarInterval[] { BarInterval.FiveMin, BarInterval.Minute });

        public LessonGenericTrackers()
        {
            blt.GotNewBar += new SymBarIntervalDelegate(GotNewBar);
            PRIMARY.NewTxt += new TextIdxDelegate(PRIMARY_NewTxt);
            Indicators = gt.GetIndicatorNames(gens());
        }

        GenericTrackerI[] gens()
        {
            return gt.geninds(PRIMARY, indicatorcross1, indicatorcross2, expectedsize, entryok, first1then2ok,pt);
        }


        void GotNewBar(string symbol, int interval)
        {
            // get current barlist for this symbol+interval
            BarList bl = blt[symbol, interval];
            // get index for symbol
            int idx = PRIMARY.getindex(symbol);
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
            // send order if everything looks good
            if (first1then2ok[symbol] && indicatorcross2[symbol] && indicatorcross1[symbol])
                sendorder(new BuyMarket(symbol, 100));
            // notify of current tracker values
            sendindicators(gt.GetIndicatorValues(idx, gens()));
        }

        public override void GotTick(Tick k)
        {
            // get index from symbol, add if it doesn't exist
            int idx = PRIMARY.addindex(k.symbol,k.symbol);
            // build bars
            blt.newTick(k);
            // update whether entries are allowed
            entryok[idx] = expectedsize[idx] == pt[idx].Size;
        }

        public override void sendorder(Order o)
        {
            // see if we're waiting for previous orders to fill
            if (expectedsize[o.symbol] != pt[o.symbol].Size)
                // we're still waiting for an order, so wait till next tick
                return;
            // track expected size before sending
            if (o.isMarket)
                expectedsize[o.symbol] += o.size;
            // pass order along
            base.sendorder(o);
        }

        public override void GotFill(Trade f)
        {
            pt.Adjust(f);
        }

        public override void GotPosition(Position p)
        {
            pt.Adjust(p);
        }


        void PRIMARY_NewTxt(string txt, int idx)
        {
            // index all the trackers we're using
            pt.addindex(txt);
            expectedsize.addindex(txt);
            entryok.addindex(txt);
            pt.addindex(txt);
            indicatorcross1.addindex(txt);
            indicatorcross2.addindex(txt);
            first1then2ok.addindex(txt);
        }



    }
}
