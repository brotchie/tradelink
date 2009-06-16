using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using IESignal;

namespace ServerEsignal
{
    public partial class EsignalMain : Form
    {

        EsignalServer tl = new EsignalServer();

        
        public EsignalMain()
        {
            InitializeComponent();
            // attempt to connect to esignal
            _ok_Click(null, null);
            // handle connector exits
            FormClosing += new FormClosingEventHandler(EsignalMain_FormClosing);
        }

        void EsignalMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tl.Stop();
        }


        // attempt to connect to esignal
        private void _ok_Click(object sender, EventArgs e)
        {
            if (_acctapp.Text == string.Empty) return;
            tl.Start(_acctapp.Text,null,null,0);
            if (tl.isValid) { BackColor = Color.Green; Invalidate(true); }
        }
    }
}
