using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel;

namespace Responses
{
    public class PairsResponse : ResponseTemplate
    {
        PairsTracker pairs;
        PositionTracker pt = new PositionTracker();
        bool _auto = false;
        public PairsResponse() : this(false) { }
        public PairsResponse(bool auto)
        {
            _auto = auto;
            Name = "PairsResponse";
            isValid = true;

        }

        public override void Reset()
        {
            // get parameters from user if we're not running in auto
            ParamPrompt.Popup(this, !_auto,_auto);
            // create pairs tracker if not already
            if (pairs == null)
            {
                pairs = new PairsTracker(Asym, Bsym, RatioA2B, BasisPointEntry);
                pairs.SpreadOutsideBounds += new DecimalDelegate(pairs_SpreadOutsideBounds);
            }
            // subscribe to symbols
            sendbasket(new string[] { Asym, Bsym });
            D("Subscribed to pair: [" + Asym + "," + Bsym+"]");
        }

        void pairs_SpreadOutsideBounds(decimal difference)
        {
            if (!entrysignal)
            {
                // b is relatively expensive here
                if (difference > 0)
                {
                    sendorder(new BuyMarket(pairs.Asym, 100));
                    sendorder(new SellMarket(pairs.Bsym, 100));
                    entrysignal = true;
                }
                else // b is cheap here
                {
                    sendorder(new BuyMarket(pairs.Bsym, 100));
                    sendorder(new SellMarket(pairs.Asym, 100));
                    entrysignal = true;
                }
            }
        }

        // defaults
        string _syma = "WAG";
        string _symb = "FRX";
        int _spread = 10;
        decimal _ratio = .78m;
        int _loss = -100;
        int _profit = 200;
        // user parameters that are displayed/fetched when ParamPrompt is called
        [Description("first symbol")]
        public string Asym { get { return _syma; } set { _syma = value; } }
        [Description("second symbol")]
        public string Bsym { get { return _symb; } set { _symb = value; } }
        [Description("when delta between a&b exceeds this amount in basis points, enter")]
        public int BasisPointEntry { get { return _spread; } set { _spread = value; } }
        [Description("multiply this value times a to get b-relative price")]
        public decimal RatioA2B { get { return _ratio; } set { _ratio = value; } }
        [Description("pl loss target")]
        public int PlLoss { get { return _loss; } set { _loss = value; } }
        [Description("pl profit target")]
        public int PlProfit { get { return _profit; } set { _profit = value; } }


        bool entrysignal = false;
        bool exitsignal = false;
        public override void GotTick(Tick tick)
        {
            // check for spread break
            pairs.GotTick(tick);
            // check for return
            if (entrysignal && !pt[Asym].isFlat && !exitsignal)
            {
                if (pairs.Spread == 0)
                {
                    exitsignal = true;
                    D("pairs hit zero again, exiting trade.");
                    shutdown();
                }
                
            }
        }
        public override void GotOrder(Order order)
        {
            senddebug("order: "+order.ToString());
        }


        public override void GotFill(Trade fill)
        {
            D("fill: "+fill.ToString());
            pt.Adjust(fill);
            decimal openpl = Calc.OpenPL(pairs.Aprice, pt[pairs.Asym]) + Calc.OpenPL(pairs.Bprice, pt[pairs.Bsym]);
            if ((openpl > _profit) || (openpl< _loss))
                shutdown();
            if (pt[Asym].isFlat && pt[Bsym].isFlat)
            {
                entrysignal = false;
                exitsignal = false;
            }
        }
        void shutdown()
        {
            if (!isValid) return;
            sendorder(new MarketOrderFlat(pt[pairs.Asym]));
            sendorder(new MarketOrderFlat(pt[pairs.Bsym]));
            isValid= false;
        }
        public override void GotOrderCancel(long cancelid)
        {
            senddebug("canceled: " + cancelid);
        }
        public override void GotPosition(Position p) 
        {
            pt.Adjust(p);
        }

    }

    public class PairsResponseAuto : PairsResponse
    {
        public PairsResponseAuto() : base(true) { }
    }
}
