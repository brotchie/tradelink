using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;

namespace Record
{
    public partial class RecordMain : Form
    {
        TickArchiver ta = new TickArchiver();
        TickWatcher tw = new TickWatcher();
        TLClient_WM tl = new TLClient_WM();
        MarketBasket mb = new MarketBasket();

        public RecordMain()
        {
            InitializeComponent();
            if ((tl.LinkType == TLTypes.LIVEBROKER) || (tl.LinkType == TLTypes.SIMBROKER))
            {
                tl.gotTick += new TickDelegate(tl_gotTick);
            }
        }


        void tl_gotTick(Tick t)
        {
            ta.Save(t);
        }


        private void symbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                recordbut_Click(null, null);
        }

        void refreshlist(int size)
        {
            if (size != (mb.Count))
            {
                if (mb.Count>0) tl.Subscribe(mb);

                stockslist.Items.Clear();
                for (int i = 0; i < mb.Count; i++)
                    stockslist.Items.Add(mb[i].Name);
            }
        }

        private void recordbut_Click(object sender, EventArgs e)
        {
            int size = mb.Count;
            Security sec = Security.Parse(symbox.Text);
            if (sec.isValid)
                mb.Add(sec);
            refreshlist(size);

        }

    }
}