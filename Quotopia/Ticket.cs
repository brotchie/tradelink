using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;

namespace Quotopia
{
    public delegate void QuotopiaOrderDel(Order sendOrder);
    public partial class Ticket : Form
    {
        Order work = new Order();
        public Order WorkingOrder { get { return work; } set { work = value; } }
        public Ticket(Order working)
        {
            InitializeComponent();
            work = working;
            if (work.Security == SecurityType.FUT)
            {
                osize.Increment = 1;
                osize.Value = 1;
            }


            isize = work.UnSignedSize;
            Text = work.symbol;

            osize.Text = work.ToString();
            oprice.Text = work.Price.ToString();
            if (work.Side) { obuybut.Checked = true; osellbut.Checked = false; }
            else { osellbut.Checked = true; obuybut.Checked = false; }
            oprice.MouseWheel += new MouseEventHandler(order_MouseWheel);
            
            osize.MouseWheel += new MouseEventHandler(osize_MouseWheel);
        }

        delegate void SetTickCallBack(Tick t);
        public void newTick(Tick tick)
        {
            if (this.InvokeRequired)
                this.Invoke(new TickDelegate(newTick), new object[] { tick });
            else
            {
                if ((tick == null) || (tick.sym != work.symbol)) return;
                if (touched) return;

                decimal changedval = obuybut.Checked ? (limitbut.Checked ? tick.ask : tick.bid) : (limitbut.Checked ? tick.bid : tick.ask);
                if (changedval != 0)
                {
                    if (this.oprice.InvokeRequired)
                    {
                        SetTickCallBack d = new SetTickCallBack(newTick);
                        this.Invoke(d, new object[] { tick });
                    }
                    else oprice.Value = (decimal)changedval;
                }
            }
        }

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

        public event QuotopiaOrderDel neworder;

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
                p = Convert.ToDecimal(oprice.Value);
                s = Convert.ToInt32(osize.Value);
            }
            catch (InvalidCastException) { castexcep = true; }
            if (marketbut.Checked) p = 0;
            return (s > 0) && !castexcep && ((p > 0) || marketbut.Checked);
        }

        private void limitbut_Click(object sender, EventArgs e)
        {
            if (!isValid()) return;
            work.side = obuybut.Checked;
            work.size = Math.Abs(Convert.ToInt32(osize.Text));
            if (marketbut.Checked)
            {
                work.price = 0;
                work.stopp = 0;
            }
            else
            {
                bool islimit = limitbut.Checked;
                decimal limit = islimit ? Convert.ToDecimal(oprice.Value) : 0;
                decimal stop = !islimit ? Convert.ToDecimal(oprice.Value) : 0;
                work.price = limit;
                work.stopp = stop;
            }
            if (neworder!=null) neworder(work);
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