using System;
using System.Collections.Generic;
using System.Text;
using TradeLib;

namespace Responses
{
    public class OpenCancel : DayTradeResponse
    {
        /// <summary>
        /// Demo of response order-canceling features
        /// </summary>
        public OpenCancel() 
        { 
            Name = "OpenCancel";

            // we'll use full orders, not just market-based adjustments


            // prompt for parameters so user can change minsize if they want
            ParamPrompt p = new ParamPrompt(this);
            p.Show();
        }

        protected override Order ReadOrder(Tick tick, BarList bl)
        {
            if (tick.sec % 5 != 0) return new Order();
            if (_buyids.Count > 0)
                CancelOrders(true);
            else
                return new BuyLimit(Symbol, MinSize, 1);
            return new Order();
        }



    }
}
