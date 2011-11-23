using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.AppKit
{
    /// <summary>
    /// create a order ticket to prompt user for sending an order.
    /// returns an order that can be easily sent with SendOrder fuctions.
    /// </summary>
    public partial class Ticket : Form
    {
        bool _assumenoordermod = true;
        public bool AssumeNewOrder { get { return _assumenoordermod; } set { _assumenoordermod = value; } }
        Order work = new OrderImpl();
        /// <summary>
        /// gets the current value of the working order for the ticket
        /// </summary>
        public Order WorkingOrder { get { return work; } set { work = value; } }
        /// <summary>
        /// creates ticket with default order
        /// </summary>
        /// <param name="working"></param>
        public Ticket(Order working)
        {
            InitializeComponent();
            work = working;
            if (work.Security == SecurityType.FUT)
            {
                osize.Increment = 1;
                osize.Value = 1;
            }


            isize = work.UnsignedSize;
            Text = work.symbol;

            osize.Text = work.ToString();
            oprice.Text = work.price.ToString();
            if (work.side) { obuybut.Checked = true; osellbut.Checked = false; }
            else { osellbut.Checked = true; obuybut.Checked = false; }
            oprice.MouseWheel += new MouseEventHandler(order_MouseWheel);
            osize.MouseWheel += new MouseEventHandler(osize_MouseWheel);
        }
        /// <summary>
        /// if new ticks are passed to this ticket, ticket will automatically update the price of limit and stops orders for opposing side.
        /// </summary>
        /// <param name="tick"></param>
        public void newTick(Tick tick)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    Invoke(new TickDelegate(newTick), new object[] { tick });
                }
                catch (ObjectDisposedException) { return; }
            }
            else
            {
                if ((tick == null) || (tick.symbol != work.symbol)) return;
                if (touched) return;

                decimal changedval = obuybut.Checked ? (limitbut.Checked ? tick.ask : tick.bid) : (limitbut.Checked ? tick.bid : tick.ask);
                if (changedval != 0)
                {
                    if (this.oprice.InvokeRequired)
                    {
                        this.Invoke(new TickDelegate(newTick), new object[] { tick });
                    }
                    else oprice.Value = (decimal)changedval;
                }
            }
        }
        /// <summary>
        /// called by external programs to report status of an order back to the ticket
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="error"></param>
        public void orderStatus(string symbol, int error)
        {
            if (symbol != work.symbol) return; // not for us
            sendbut.BackColor = (error == 0) ? sendbut.BackColor = Color.White : Color.Yellow;
            return;
        }

        void osize_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) osize.Value = isize; 
        }
        /// <summary>
        /// called when the Send button is pressed.   Working Order is automatically sent to the handler of this event
        /// </summary>
        public event OrderDelegate SendOrder;

        void order_MouseWheel(object sender, MouseEventArgs e)
        {
            touched = true;
        }

        int isize = 0;
        bool touched = false;
        bool isValid()
        {
            bool castexcep = false;
            decimal p = 0;
            int s = 0;
            try {
                p = oprice.Value;
                s = (int)osize.Value;
            }
            catch (InvalidCastException) { castexcep = true; }
            if (marketbut.Checked) p = 0;
            return (s > 0) && !castexcep && ((p > 0) || marketbut.Checked);
        }

        private void limitbut_Click(object sender, EventArgs e)
        {
            if (!isValid()) return;
            work.side = obuybut.Checked;
            work.size = Math.Abs((int)osize.Value);
            if (marketbut.Checked)
            {
                work.price = 0;
                work.stopp = 0;
            }
            else
            {
                bool islimit = limitbut.Checked;
                decimal limit = islimit ? oprice.Value : 0;
                decimal stop = !islimit ? oprice.Value : 0;
                work.price = limit;
                work.stopp = stop;
            }
            if (AssumeNewOrder)
                work.id = 0;
            if (SendOrder!=null) SendOrder(work);
        }


        private void obuybut_CheckedChanged(object sender, EventArgs e)
        {
            this.BackColor = obuybut.Checked ? Color.Green : Color.Red;
        }

        private void osellbut_CheckedChanged(object sender, EventArgs e)
        {
            this.BackColor = obuybut.Checked ? Color.Green : Color.Red;
        }

        private void oprice_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) touched = false;
            else touched = true;
        }

        private void oprice_KeyUp(object sender, KeyEventArgs e)
        {
            touched = true;
        }

    }
}