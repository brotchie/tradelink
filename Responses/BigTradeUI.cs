using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;
using TradeLink.Common;
using TradeLink.API;

namespace Responses
{



    /// <summary>
    /// this is the user interface for the BigTrades response (found below)
    /// </summary>
    public partial class BigTradeUI : Form
    {
        // hold data for our UI using these objects
        DataTable _dt = new DataTable();
        SafeBindingSource _bs = new SafeBindingSource(false);

        BigTrades _engine = null;

        public BigTradeUI(BigTrades engine)
        {
            // save our response engine
            _engine = engine;
            // setup grid
            InitializeComponent();
            // here our the columns we'll save for 10ten trades
            _dt.Columns.Add("Time", typeof(int));
            _dt.Columns.Add("Sym");
            _dt.Columns.Add("Price", typeof(decimal));
            _dt.Columns.Add("TradeSize", typeof(int));
            _dt.Columns.Add("Exch");
            _dg.MultiSelect = false;
            // we'll bid the above columns to our grid
            _bs.DataSource = _dt;
            _dg.DataSource = _bs;
            // setup right click
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("Buy 100 Market", new EventHandler(butbuy_Click));
            ContextMenu.MenuItems.Add("Sell 100 Market", new EventHandler(butsell_Click));

            // then the grid to the user
            Show();
            // make sure to invalidate and unsubscribe on form closing
            // otherwise, it will continue to process in background
            FormClosing += new FormClosingEventHandler(BigTradeUI_FormClosing);
        }

        void BigTradeUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_engine != null)
            {
                _engine.sendbasket(new string[0]);
                _engine.isValid = false;
            }
        }

        const int ROWS = 10;

        public void GotTick(TradeLink.API.Tick tick)
        {

            // if we don't have 10 trades yet, add it
            if (_dt.Rows.Count < ROWS)
            {
                _dt.Rows.Add(tick.time, tick.symbol,tick.trade, tick.size, tick.ex);
                refresh();
                return;
            }

            // otherwise, go through list and check to see if it's bigger
            for (int i = 0; i < _dt.Rows.Count; i++)
                if ((int)_dt.Rows[i]["TradeSize"] < tick.size)
                {
                    _dt.Rows[i].ItemArray = new object[] { tick.time, tick.symbol,tick.trade, tick.size, tick.ex };
                    refresh();
                    return;
                }
        }


        delegate string StringRetDel();
        string getselectedsym()
        {
            if (InvokeRequired)
                Invoke(new StringRetDel(getselectedsym));
            else
            {
                if (_dg.SelectedRows.Count == 0)
                    return string.Empty;
                try
                {
                    string sym = _dt.Rows[_dg.SelectedRows[0].Index]["Sym"].ToString();
                }
                catch { }
            }
            return string.Empty;
        }



        private void butbuy_Click(object sender, EventArgs e)
        {
            string sym = getselectedsym();
            if ((_engine != null) && (sym!=string.Empty))
                _engine.sendorder(new BuyMarket(sym, 100));
                    

        }

        private void butsell_Click(object sender, EventArgs e)
        {
            string sym = getselectedsym();
            if ((_engine != null) && (sym != string.Empty))
                _engine.sendorder(new SellMarket(sym, 100));



        }


        delegate void VoidDelegate();
        void refresh()
        {
            if (_dg.InvokeRequired)
                _dg.Invoke(new VoidDelegate(refresh));
            else
            {
                SafeBindingSource.refreshgrid(_dg, _bs, true);
            }
        }



    }

    /// <summary>
    /// This response saves the last 10 biggest trades, and lets user
    /// send buy and sell market orders
    /// </summary>
    public class BigTrades : ResponseTemplate
    {
        BigTradeUI ui;

        public BigTrades()
        {
            // optional name
            Name = "BigTrades";
            // bind our gui to our response
            ui = new BigTradeUI(this);
        }

        public override void GotTick(Tick tick)
        {
            // ignore it if it's not a trade
            if (!tick.isTrade) return;
            // pass through to the UI for display
            ui.GotTick(tick);

        }

    }
}
