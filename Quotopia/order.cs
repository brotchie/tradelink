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
    public partial class order : Form
    {

        public order(string symbol, int initialsize, bool side)
        {
            InitializeComponent();
            isize = Math.Abs(initialsize);
            size = Math.Abs(initialsize);
            Symbol = symbol;
            Text = symbol;

            osize.Text = size.ToString();
            oprice.Text = "0";
            if (side) { obuybut.Checked = true; osellbut.Checked = false; }
            else { osellbut.Checked = true; obuybut.Checked = false; }
            oprice.MouseWheel += new MouseEventHandler(order_MouseWheel);
            
            osize.MouseWheel += new MouseEventHandler(osize_MouseWheel);
        }

        delegate void SetTickCallBack(Tick t);
        public void newTick(Tick tick)
        {
            if ((tick == null) || (tick.sym != sym)) return;
            if (touched) return;

            decimal changedval = obuybut.Checked ? (limitbut.Checked ? tick.ask : tick.bid ) : (limitbut.Checked ? tick.bid : tick.ask);
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

        public void orderStatus(string symbol, int error)
        {
            if (symbol != Symbol) return; // not for us
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

        int size = 0;
        int isize = 0;
        bool touched = false;
        string sym = null;
        public string Symbol { get { return sym; } set { sym = value; } }
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
            return (sym != null) && (s > 0) && !castexcep && ((p > 0) || marketbut.Checked);
        }

        private void limitbut_Click(object sender, EventArgs e)
        {
            if (!isValid()) return;
            bool side = obuybut.Checked;
            int size = Convert.ToInt32(osize.Text);
            Order o;
            if (marketbut.Checked) o = new Order(sym, side, size, 0, 0, "", 0, 0);
            else
            {
                bool islimit = limitbut.Checked;
                decimal limit = islimit ? Convert.ToDecimal(oprice.Value) : 0;
                decimal stop = !islimit ? Convert.ToDecimal(oprice.Value) : 0;
                o = new Order(sym, side, size, limit, stop, "", 0, 0);
            }
            if (neworder!=null) neworder(o);
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