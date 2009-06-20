using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using System.Data;
using TradeLink.API;

namespace Responses
{
    /// <summary>
    /// This response saves the last 10 biggest trades, and lets user
    /// send buy and sell market orders
    /// </summary>
    public class BigTrades : ResponseTemplate
    {
        BigTradeUI ui = new BigTradeUI();

        string sym = "";
        public BigTrades()
        {
            Name = "BigTrades";
            // also handle events from buy/sell buttons on grid
            ui.GotMarket += new BigTradeMarket(ui_GotMarket);
        }

        void ui_GotMarket(bool side)
        {
            sendorder(new MarketOrder(sym, side, 100));
        }
        public override void GotTick(Tick tick)
        {
            // we want to see if this trade is one of the top 10 biggest

            // get symbol from first tick if we haven't gotten one yet
            if (sym == "") sym = tick.symbol;
            else if (tick.symbol != sym) return;

            // ignore it if it's not a trade
            if (!tick.isTrade) return;

            ui.GotTick(tick);

        }

    }
}
