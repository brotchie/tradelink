using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Responses
{
    public delegate void BigTradeMarket(bool side);
    public partial class BigTradeUI : Form
    {
        DataTable dt = new DataTable();

        public event BigTradeMarket GotMarket;
        public BigTradeUI()
        {
            InitializeComponent();
            // here our the columns we'll save for 10ten trades
            dt.Columns.Add("Time", typeof(int));
            dt.Columns.Add("Price", typeof(decimal));
            dt.Columns.Add("TradeSize", typeof(int));
            dt.Columns.Add("Exch");
            // we'll bid the above columns to our grid
            dg.DataSource = dt;
            // then the grid to the user
            Show();
        }

        private void butbuy_Click(object sender, EventArgs e)
        {
            if (GotMarket != null)
                GotMarket(true);

        }

        delegate void VoidDelegate();
        void refresh()
        {
            if (dg.InvokeRequired)
                dg.Invoke(new VoidDelegate(refresh));
            else
            {
                dg.Invalidate(true);
            }
        }

        public void GotTick(TradeLink.API.Tick tick)
        {

            // if we don't have 10 trades yet, add it
            if (dt.Rows.Count < 10)
            {
                dt.Rows.Add(tick.time, tick.trade, tick.size, tick.ex);
                refresh();
                return;
            }

            // otherwise, go through list and check to see if it's bigger
            for (int i = 0; i < dt.Rows.Count; i++)
                if ((int)dt.Rows[i]["TradeSize"] < tick.size)
                {
                    dt.Rows[i].ItemArray = new object[] { tick.time, tick.trade, tick.size, tick.ex };
                    refresh();
                    return;
                }
        }

        private void butsell_Click(object sender, EventArgs e)
        {
            if (GotMarket != null)
                GotMarket(false);

        }
    }
}
