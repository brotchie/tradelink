using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{
    public class PairsResponse : Response
    {
        PairsTracker pairs;
        PositionTracker pt = new PositionTracker();
        public PairsResponse()
        {
            pairs = new PairsTracker("WAG", "FRX", .78m, 10);
            pairs.SpreadOutsideBounds += new DecimalDelegate(pairs_SpreadOutsideBounds);
        }

        void pairs_SpreadOutsideBounds(decimal spread)
        {
            // b is relatively expensive here
            if (spread>0)
            { 
              SendOrder(new BuyMarket(pairs.Asym,100));
              SendOrder(new SellMarket(pairs.Bsym,100));
            }
            else // b is cheap here
            {
              SendOrder(new BuyMarket(pairs.Bsym,100));
              SendOrder(new SellMarket(pairs.Asym, 100));
            }
        }
        public void GotTick(Tick tick)
        {
            pairs.GotTick(tick);
        }
        public void GotOrder(Order order)
        {
            D("order: "+order.ToString());
        }

        void D(string msg) { SendDebug(DebugImpl.Create(msg)); }
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
            if (!_valid) return;
            SendOrder(new MarketOrderFlat(pt[pairs.Asym]));
            SendOrder(new MarketOrderFlat(pt[pairs.Bsym]));
            _valid = false;
        }
        public void GotOrderCancel(uint cancelid)
        {
            D("canceled: " + cancelid);
        }
        public void Reset() { }
        public void GotPosition(Position p) 
        {
            pt.Adjust(p);
        }
        bool _valid = true;
        public bool isValid { get { return _valid; } set {  } }
        public string[] Indicators { get { return new string[0]; } set { } }
        public string Name { get { return ""; } set { } }
        public string FullName { get { return ""; } set { } }
        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event ObjectArrayDelegate SendIndicators;
    }
}
