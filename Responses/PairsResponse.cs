using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    public class PairsResponse : ResponseTemplate
    {
        PairsTracker pairs;
        PositionTracker pt = new PositionTracker();
        public PairsResponse()
        {
            Name = "PairsResponse";
            isValid = true;
            pairs = new PairsTracker("WAG", "FRX", .78m, 10);
            pairs.SpreadOutsideBounds += new DecimalDelegate(pairs_SpreadOutsideBounds);
        }

        void pairs_SpreadOutsideBounds(decimal spread)
        {
            // b is relatively expensive here
            if (spread>0)
            {
              sendorder(new BuyMarket(pairs.Asym, 100));
              sendorder(new SellMarket(pairs.Bsym, 100));
            }
            else // b is cheap here
            {
                sendorder(new BuyMarket(pairs.Bsym, 100));
                sendorder(new SellMarket(pairs.Asym, 100));
            }
        }
        public void GotTick(Tick tick)
        {
            pairs.GotTick(tick);
        }
        public void GotOrder(Order order)
        {
            senddebug("order: "+order.ToString());
        }


        public void GotFill(Trade fill)
        {
            D("fill: "+fill.ToString());
            pt.Adjust(fill);
            decimal openpl = Calc.OpenPL(pairs.Aprice, pt[pairs.Asym]) + Calc.OpenPL(pairs.Bprice, pt[pairs.Bsym]);
            if ((openpl > 200) || (openpl< -100))
                shutdown();
        }
        void shutdown()
        {
            if (!isValid) return;
            sendorder(new MarketOrderFlat(pt[pairs.Asym]));
            sendorder(new MarketOrderFlat(pt[pairs.Bsym]));
            isValid= false;
        }
        public void GotOrderCancel(uint cancelid)
        {
            senddebug("canceled: " + cancelid);
        }
        public void GotPosition(Position p) 
        {
            pt.Adjust(p);
        }

    }
}
