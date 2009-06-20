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
    public class BigTrades : Response
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
            if ((SendOrder != null) && (sym != ""))
                SendOrder(new MarketOrder(sym, side, 100));
        }
        public void GotTick(Tick tick)
        {
            // we want to see if this trade is one of the top 10 biggest

            // get symbol from first tick if we haven't gotten one yet
            if (sym == "") sym = tick.symbol;
            else if (tick.symbol != sym) return;

            // ignore it if it's not a trade
            if (!tick.isTrade) return;

            ui.GotTick(tick);

        }

        public void GotOrder(Order order) 
        { 
        }
        public void GotFill(Trade fill) 
        { 
        }
        public void GotOrderCancel(uint cancelid) 
        { 
        }

        public void Reset()
        {
        }

        public void GotMessage(MessageTypes t, uint id, string data) { }

        public void GotPosition(Position p) { }

        string[] _inds = new string[0];
        string _name = "";
        string _full = "";


        public bool isValid { get { return true; } set { } }
        public string[] Indicators { get { return _inds; } set { _inds = value; } }
        public string Name { get { return _name; } set { _name = value; }  }
        public string FullName { get { return _full; } set { _full = value; } }
        public event DebugFullDelegate SendDebug;
        public event OrderDelegate SendOrder;
        public event UIntDelegate SendCancel;
        public event StringParamDelegate SendIndicators;
        public event MessageDelegate SendMessage;
    }
}
