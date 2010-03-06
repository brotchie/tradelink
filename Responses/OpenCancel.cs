using System;
using TradeLink.Common;
using TradeLink.API;
using System.ComponentModel; // required for prompting user

namespace Responses
{
    public class OpenCancel : ResponseTemplate
    {
        /// <summary>
        /// This strategy is a grey box.  It will:
        ///   * send a profit-taking order at a fixed distance
        ///   * it will cancel and resend order if position changes
        ///   
        /// this particular response works with one symbol.
        /// to trade multiple symbols, you would have to apply this
        /// response to each symbol you're trading.
        /// (or you can modify the code to keep track of orders for many symbols)
        /// </summary>
        /// 

        // enable position tracking
        PositionTracker pt = new PositionTracker();

        // allow user to specify profit distance
        [Description("Distance of profit taking orders, in dollars")]
        public decimal ProfitDistance { get { return _profit; } set { _profit = value; } }

        public OpenCancel() 
        {
            // enable feature to prompt user
            ParamPrompt.Popup(this);
        }

        public override void GotFill(Trade fill)
        {
            // make sure fills are tracked as positions
            pt.Adjust(fill);
            // send profit order for this new position
            sendoffset(fill.symbol);
        }

        public override void GotPosition(Position p)
        {
            // make sure pre-existing positions are tracked
            pt.Adjust(p);
            // send profit offset for this position
            sendoffset(p.Symbol);
        }

        // here is our function that actually sends the offset
        void sendoffset(string symbol)
        {
            // if we have an existing profit order, cancel it
            if (profitid != 0) sendcancel(profitid);

            // just for simplicity, lets restate our position information
            // this is the size that will make our position flat
            int size = pt[symbol].FlatSize;
            // this is price of our position
            decimal posprice = pt[symbol].AvgPrice;
            

            // if we are long, send order above position price
            if (pt[symbol].isLong)
            {
                // send order above position price using next order id available
                sendorder(new BuyLimit(symbol, size, posprice + _profit, nextid++));
                // remember order id just sent, 
                // this way we can cancel it if the position changes
                profitid = nextid;
            }
            else if (pt[symbol].isShort)
            {
                // send order below position price using next order id available
                sendorder(new SellLimit(symbol, size, posprice - _profit, nextid++));
                // remember the order we just sent so we can cancel it later
                profitid = nextid;
            }
        }

        public override void GotOrderCancel(long cancelid)
        {
            // once our order has been canceled, we don't need to track it anymore
            if (cancelid == profitid)
                profitid = 0;
        }

        // working variable that holds pending profit order
        long profitid = 0;

        // working variable that keeps track of next order id
        long nextid = OrderImpl.Unique;

        // default profit target before user sets it
        decimal _profit = .3m;



    }
}
