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
        public event BigTradeMarket GotMarket;
        public BigTradeUI()
        {
            InitializeComponent();
        }

        private void butbuy_Click(object sender, EventArgs e)
        {
            if (GotMarket != null)
                GotMarket(true);

        }

        private void butsell_Click(object sender, EventArgs e)
        {
            if (GotMarket != null)
                GotMarket(false);

        }
    }
}
