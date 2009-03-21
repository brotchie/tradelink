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
        DataTable dt = new DataTable();
        string sym = "";
        public BigTrades()
        {
            Name = "BigTrades";
            // here our the columns we'll save for 10ten trades
            dt.Columns.Add("Time", typeof(int));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("TradeSize", typeof(int));
            dt.Columns.Add("Exch");
            // we'll bid the above columns to our grid
            ui.dg.DataSource = dt;
            // then the grid to the user
            ui.Show();
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

            // if we don't have 10 trades yet, add it
            if (dt.Rows.Count < 10)
            {
                dt.Rows.Add(tick.time, tick.trade, tick.size, tick.ex);
                ui.dg.Invalidate(true); // update the grid
                return;
            }

            // otherwise, go through list and check to see if it's bigger
            for (int i = 0; i < dt.Rows.Count; i++)
                if ((int)dt.Rows[i]["TradeSize"] < tick.size)
                {
                    dt.Rows[i].ItemArray = new object[] { tick.time, tick.trade, tick.size, tick.ex };
                    ui.dg.InvalidateRow(i); // update the grid
                    return;
                }
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
    }
}
