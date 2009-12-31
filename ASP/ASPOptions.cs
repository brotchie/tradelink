using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace ASP
{
    public partial class ASPOptions : Form
    {
        public ASPOptions()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(ASPOptions_FormClosing);
        }

        void ASPOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
        public event Int32Delegate TimeoutChanged;
        private void _brokertimeout_ValueChanged(object sender, EventArgs e)
        {
            if (TimeoutChanged != null)
                TimeoutChanged((int)_brokertimeout.Value);
        }


    }
}
